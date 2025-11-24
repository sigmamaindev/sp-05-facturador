import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { toast } from "sonner";

import { useAuth } from "@/contexts/AuthContext";

import type { Customer } from "@/types/customer.types";

import { createInvoice } from "@/api/invoice";

import { Card, CardContent } from "@/components/ui/card";

import type {
  CreateInvoiceForm,
  InvoiceProduct,
  InvoiceTotals,
} from "@/types/invoice.type";

import InvoiceCreateHeader from "./InvoiceCreateHeader";
import InvoiceCreateForm from "./InvoiceCreateForm";
import type { Product } from "@/types/product.types";

export default function InvoiceCreateView() {
  const navigate = useNavigate();

  const { token } = useAuth();

  const [savingInvoice, setSavingInvoice] = useState(false);
  const [openCustomerModal, setOpenCustomerModal] = useState(false);
  const [customer, setCustomer] = useState<Customer | null>(null);
  const [openProductModal, setOpenProductModal] = useState(false);
  const [products, setProducts] = useState<InvoiceProduct[]>([]);

  const { setValue, handleSubmit, watch } = useForm<CreateInvoiceForm>({
    defaultValues: {
      documentType: "",
      isElectronic: true,
      environment: "1",
      invoiceDate: new Date(),
      dueDate: new Date(),
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

  useEffect(() => {
    const now = new Date();
    setValue("invoiceDate", now);
    setValue("dueDate", now);
  }, [setValue]);

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
        taxValue: taxValue,
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
          const ivaValue = base * (ivaRate / 100);
          return {
            ...p,
            quantity: newQty,
            subtotal: base,
            ivaValue,
          };
        }
        return p;
      })
    );
  };

  const handleRemoveProduct = (id: number) => {
    setProducts((prev) => prev.filter((p) => p.id !== id));
  };

  const calculateTotals = (): InvoiceTotals => {
    const subtotal = products.reduce((sum, p) => sum + p.subtotal, 0);
    const discount = products.reduce((sum, p) => sum + p.discount, 0);
    const tax = products.reduce((sum, p) => sum + p.taxValue, 0);
    const total = subtotal + tax;

    return { subtotal, discount, tax, total };
  };

  const totals = calculateTotals();

  useEffect(() => {
    setValue("subtotalWithoutTaxes", totals.subtotal);
    setValue("subtotalWithTaxes", totals.subtotal);
    setValue("discountTotal", totals.discount);
    setValue("taxTotal", totals.tax);
    setValue("totalInvoice", totals.total);
  }, [totals]);

  const invoiceDate = watch("invoiceDate");
  const dueDate = watch("dueDate");

  const onSubmit = async (data: CreateInvoiceForm) => {
    const details = products.map((p) => ({
      productId: p.id,
      quantity: p.quantity,
      unitPrice: p.price,
      discount: p.discount,
      warehouseId: p.inventory[0].warehouseId,
      taxId: p.tax.id,
    }));

    const emissionDate = data.invoiceDate ?? new Date();
    const selectedDueDate = data.dueDate ?? emissionDate;

    const payload = {
      ...data,
      invoiceDate: emissionDate,
      dueDate: selectedDueDate,
      details,
    };

    try {
      setSavingInvoice(true);

      const response = await createInvoice(payload, token!);

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
        <InvoiceCreateHeader />
        <InvoiceCreateForm
          customer={customer}
          products={products}
          totals={totals}
          invoiceDate={invoiceDate}
          dueDate={dueDate}
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
      </CardContent>
    </Card>
  );
}
