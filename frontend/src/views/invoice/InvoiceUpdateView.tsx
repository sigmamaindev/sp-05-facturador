import { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useForm } from "react-hook-form";
import { toast } from "sonner";

import { useAuth } from "@/contexts/AuthContext";

import type { Customer } from "@/types/customer.types";
import type { CreateInvoiceForm, InvoiceProduct } from "@/types/invoice.type";
import type { Product } from "@/types/product.types";
import type { Invoice } from "@/types/invoice.type";

import { getInvoiceById, updateInvoice } from "@/api/invoice";

import { Card, CardContent } from "@/components/ui/card";

import InvoiceUpdateHeader from "./InvoiceUpdateHeader";
import InvoiceUpdateForm from "./InvoiceUpdateForm";
import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";

export default function InvoiceUpdateView() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const { token } = useAuth();

  const [invoice, setInvoice] = useState<Invoice | null>(null);
  const [savingInvoice, setSavingInvoice] = useState(false);
  const [openCustomerModal, setOpenCustomerModal] = useState(false);
  const [customer, setCustomer] = useState<Customer | null>(null);
  const [openProductModal, setOpenProductModal] = useState(false);
  const [products, setProducts] = useState<InvoiceProduct[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const { setValue, handleSubmit } = useForm<CreateInvoiceForm>({
    defaultValues: {
      documentType: "05",
      isElectronic: true,
      environment: "1",
      invoiceDate: new Date(),
      customerId: 0,
      subtotalWithoutTaxes: 0,
      subtotalWithTaxes: 0,
      discountTotal: 0,
      taxTotal: 0,
      totalInvoice: 0,
      paymentMethod: "",
      paymentTermDays: 0,
      description: "",
      additionalInformation: "",
      details: [],
    },
  });

  const mapDetailsToProducts = (invoiceData: Invoice) =>
    invoiceData.details.map((detail) => ({
      id: detail.productId,
      sku: detail.productCode,
      name: detail.productName,
      description: detail.additionalDetail,
      price: detail.unitPrice,
      iva: detail.taxRate > 0,
      isActive: true,
      tax: {
        id: detail.taxId,
        code: detail.taxCode,
        codePercentage: detail.taxCode,
        name: detail.taxName,
        group: "",
        rate: detail.taxRate,
        isActive: true,
      },
      unitMeasure: {
        id: detail.unitMeasureId,
        code: detail.unitMeasureCode,
        name: detail.unitMeasureName,
        factorBase: 1,
        isActive: true,
      },
      inventory: [
        {
          id: detail.warehouseId,
          warehouseId: detail.warehouseId,
          warehouseCode: detail.warehouseCode,
          warehouseName: detail.warehouseName,
          stock: detail.quantity,
          minStock: 0,
          maxStock: 0,
        },
      ],
      quantity: detail.quantity,
      discount: detail.discount,
      subtotal: detail.subtotal,
      taxValue: detail.taxValue,
    }));

  const fetchData = async () => {
    try {
      setLoading(true);

      const response = await getInvoiceById(Number(id), token!);
      const invoiceData = response.data!;

      setInvoice(invoiceData);
      setCustomer(invoiceData.customer);
      setProducts(mapDetailsToProducts(invoiceData));
      setValue("documentType", "05");
      setValue("isElectronic", invoiceData.isElectronic);
      setValue("environment", invoiceData.environment);
      setValue("invoiceDate", new Date(invoiceData.invoiceDate));
      setValue("customerId", invoiceData.customer.id);
      setValue("paymentMethod", String(invoiceData.paymentMethod));
      setValue("paymentTermDays", invoiceData.paymentTermDays);
      setValue("description", invoiceData.description ?? "");
      setValue(
        "additionalInformation",
        invoiceData.additionalInformation ?? ""
      );
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (id && token) fetchData();
  }, [id, token]);

  const handleSelectCustomer = (customer: Customer) => {
    setCustomer(customer);
    setValue("customerId", customer.id);
    setOpenCustomerModal(false);
  };

  const handleSelectProduct = (product: Product) => {
    setOpenProductModal(false);

    setProducts((prev) => {
      const exists = prev.find((p) => p.id === product.id);

      if (exists) return prev;

      const price = product.price ?? 0;
      const discount = 0;
      const base = price - discount;
      const ivaRate = product.tax.rate ?? 12;
      const taxValue = base * (ivaRate / 100);

      const newProduct = {
        ...product,
        quantity: 1,
        discount: discount,
        subtotal: base,
        taxValue,
      };

      return [...prev, newProduct];
    });
  };

  const handleQuantityChange = (productId: number, newQty: number) => {
    setProducts((prev) =>
      prev.map((p) => {
        if (p.id === productId) {
          const base = (p.price - p.discount) * newQty;
          const ivaRate = p.tax.rate ?? 12;
          const taxValue = base * (ivaRate / 100);
          return {
            ...p,
            quantity: newQty,
            subtotal: base,
            taxValue,
          };
        }
        return p;
      })
    );
  };

  const handleRemoveProduct = (productId: number) => {
    setProducts((prev) => prev.filter((p) => p.id !== productId));
  };

  const calculateTotals = useMemo(() => {
    const subtotal = products.reduce((sum, p) => sum + p.subtotal, 0);
    const discount = products.reduce((sum, p) => sum + p.discount, 0);
    const tax = products.reduce((sum, p) => sum + p.taxValue, 0);
    const total = subtotal + tax;

    return { subtotal, discount, tax, total };
  }, [products]);

  useEffect(() => {
    setValue("subtotalWithoutTaxes", calculateTotals.subtotal);
    setValue("subtotalWithTaxes", calculateTotals.subtotal);
    setValue("discountTotal", calculateTotals.discount);
    setValue("taxTotal", calculateTotals.tax);
    setValue("totalInvoice", calculateTotals.total);
  }, [calculateTotals, setValue]);

  const onSubmit = async (data: CreateInvoiceForm) => {
    const details = products.map((p) => ({
      productId: p.id,
      quantity: p.quantity,
      unitPrice: p.price,
      discount: p.discount,
      warehouseId: p.inventory[0].warehouseId,
      taxId: p.tax.id,
    }));

    const payload = {
      ...data,
      details,
    };

    try {
      setSavingInvoice(true);

      const response = await updateInvoice(Number(id), payload, token!);

      navigate("/facturas");

      toast.success(response.message);
    } catch (err: any) {
      toast.error(err.message);
    } finally {
      setSavingInvoice(false);
    }
  };

  return (
    <Card>
      <CardContent>
        {loading ? (
          <Loading />
        ) : error ? (
          <AlertMessage message={error} variant="destructive" />
        ) : !invoice ? (
          <AlertMessage
            message="Los datos de la factura no se han cargado completamente"
            variant="destructive"
          />
        ) : (
          <>
            <InvoiceUpdateHeader sequential={invoice.sequential} />
            <InvoiceUpdateForm
              customer={customer}
              products={products}
              totals={calculateTotals}
              openCustomerModal={openCustomerModal}
              setOpenCustomerModal={setOpenCustomerModal}
              openProductModal={openProductModal}
              setOpenProductModal={setOpenProductModal}
              handleSelectCustomer={handleSelectCustomer}
              handleSelectProduct={handleSelectProduct}
              handleQuantityChange={handleQuantityChange}
              handleRemoveProduct={handleRemoveProduct}
              handleSubmit={handleSubmit(onSubmit)}
              savingInvoice={savingInvoice}
            />
          </>
        )}
      </CardContent>
    </Card>
  );
}
