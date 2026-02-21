import { useEffect, useMemo, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { useForm } from "react-hook-form";
import { toast } from "sonner";

import { useAuth } from "@/contexts/AuthContext";

import {
  PaymentMethodCode,
  PaymentType,
  type PaymentMethodCode as PaymentMethodCodeType,
  type PaymentTypeValue,
  isPaymentType,
  paymentMethodCodeFromPaymentType,
  paymentTypeFromPaymentMethodCode,
} from "@/constants/paymentMethods";

import type { Customer } from "@/types/customer.types";
import type { CreateInvoiceForm, InvoiceProduct } from "@/types/invoice.type";
import type { Product } from "@/types/product.types";
import type { Invoice } from "@/types/invoice.type";
import type { ProductPresentation } from "@/types/product.types";

import {
  getInvoiceById,
  updateInvoice,
  updateInvoicePayment,
} from "@/api/invoice";
import { getProductById } from "@/api/product";

import { Card, CardContent } from "@/components/ui/card";

import InvoiceUpdateHeader from "./InvoiceUpdateHeader";
import InvoiceUpdateForm from "./InvoiceUpdateForm";
import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";
import InvoicePaymentStep from "./InvoiceCreatePayment";

export default function InvoiceUpdateView() {
  type PriceTier = 1 | 2 | 3 | 4;

  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const { token, user } = useAuth();

  const isAdmin =
    user?.roles?.includes("SuperAdmin") || user?.roles?.includes("Admin");

  const [invoice, setInvoice] = useState<Invoice | null>(null);
  const [savingDraft, setSavingDraft] = useState(false);
  const [savingAndContinue, setSavingAndContinue] = useState(false);
  const [savingPayment, setSavingPayment] = useState(false);
  const [openCustomerModal, setOpenCustomerModal] = useState(false);
  const [customer, setCustomer] = useState<Customer | null>(null);
  const [openProductModal, setOpenProductModal] = useState(false);
  const [products, setProducts] = useState<InvoiceProduct[]>([]);
  const [openPresentationModal, setOpenPresentationModal] = useState(false);
  const [productIdForPresentation, setProductIdForPresentation] = useState<
    number | null
  >(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const [currentStep, setCurrentStep] = useState<1 | 2>(1);
  const [paymentMethod, setPaymentMethod] = useState<PaymentMethodCodeType>(
    PaymentMethodCode.NFS
  );
  const [paymentType, setPaymentType] = useState<PaymentTypeValue>(
    PaymentType.CASH
  );
  const [paymentTermDays, setPaymentTermDays] = useState(0);

  const { setValue, handleSubmit } = useForm<CreateInvoiceForm>({
    defaultValues: {
      receiptType: "01",
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
      paymentMethod: "01",
      paymentTermDays: 0,
      description: "",
      additionalInformation: "",
      details: [],
    },
  });

	  const mapDetailsToProducts = (invoiceData: Invoice) =>
	    invoiceData.details.map((detail) => {
	      const netWeight = detail.netWeight ?? 0;
	      const grossWeight = detail.grossWeight ?? 0;
	      const computedQuantity = Number((grossWeight - netWeight).toFixed(2));
	      const calculatedQuantity = Number.isFinite(computedQuantity)
	        ? Math.max(0, computedQuantity)
	        : 0;
	      const shouldUseCalculated = netWeight !== 0 || grossWeight !== 0;
	      const quantity = shouldUseCalculated ? calculatedQuantity : detail.quantity;

	      const base = (detail.unitPrice - detail.discount) * quantity;
	      const taxValue = base * (detail.taxRate / 100);

      return {
        id: detail.productId,
        sku: detail.productCode,
        name: detail.productName,
        description: detail.additionalDetail,
        price: detail.unitPrice,
        netWeight,
        grossWeight,
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
        quantity,
        discount: detail.discount,
        subtotal: base,
        taxValue,
      };
    });

  const fetchData = async () => {
    try {
      setLoading(true);

      const response = await getInvoiceById(Number(id), token!);
      const invoiceData = response.data!;

      setInvoice(invoiceData);
      setCustomer(invoiceData.customer);
      setProducts(mapDetailsToProducts(invoiceData));
      const nextPaymentType = isPaymentType(invoiceData.paymentType)
        ? invoiceData.paymentType
        : paymentTypeFromPaymentMethodCode(invoiceData.paymentMethod);
      setPaymentType(nextPaymentType);
      setPaymentMethod(paymentMethodCodeFromPaymentType(nextPaymentType));
      setPaymentTermDays(invoiceData.paymentTermDays ?? 0);
      setValue("receiptType", "01");
      setValue("isElectronic", invoiceData.isElectronic);
      setValue("environment", invoiceData.environment);
      setValue("invoiceDate", new Date(invoiceData.invoiceDate));
      setValue("dueDate", new Date(invoiceData.dueDate));
      setValue("customerId", invoiceData.customer.id);
      setValue("paymentMethod", invoiceData.paymentMethod);
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
	      const netWeight = 0;
	      const grossWeight = 0;
	      const quantity = 1;
	      const base = (price - discount) * quantity;
	      const ivaRate = product.tax?.rate ?? 12;
	      const taxValue = base * (ivaRate / 100);

      const newProduct: InvoiceProduct = {
        ...product,
        price,
        netWeight,
        grossWeight,
        quantity,
        discount: discount,
        subtotal: base,
        taxValue,
      };

      return [...prev, newProduct];
    });
  };

	  const handleWeightChange = (
	    productId: number,
	    field: "netWeight" | "grossWeight",
	    value: number
	  ) => {
	    const nextValue = Number.isFinite(value) ? value : 0;
	    setProducts((prev) =>
	      prev.map((p) => {
	        if (p.id !== productId) return p;

	        const netWeight = field === "netWeight" ? nextValue : p.netWeight ?? 0;
	        const grossWeight =
	          field === "grossWeight" ? nextValue : p.grossWeight ?? 0;
	        const computedQuantity = Number((grossWeight - netWeight).toFixed(2));
	        const calculatedQuantity = Number.isFinite(computedQuantity)
	          ? Math.max(0, computedQuantity)
	          : 0;
	        const shouldUseCalculated = netWeight !== 0 || grossWeight !== 0;
	        const quantity = shouldUseCalculated ? calculatedQuantity : p.quantity;

	        const base = (p.price - p.discount) * quantity;
	        const ivaRate = p.tax?.rate ?? 12;
	        const taxValue = base * (ivaRate / 100);

        return {
          ...p,
          netWeight,
          grossWeight,
          quantity,
          subtotal: base,
          taxValue,
        };
      })
    );
  };

  const handleRemoveProduct = (productId: number) => {
    setProducts((prev) => prev.filter((p) => p.id !== productId));
  };

  const handleOpenPresentationModal = async (productId: number) => {
    setProductIdForPresentation(productId);
    setOpenPresentationModal(true);

    const current = products.find((p) => p.id === productId);
    if (!token || !current) return;
    if (current.defaultPresentation || current.presentations?.length) return;

    try {
      const fetched = await getProductById(productId, token);
      const fetchedProduct = fetched.data;
      if (!fetchedProduct) return;

      setProducts((prev) =>
        prev.map((p) =>
          p.id === productId
            ? {
                ...fetchedProduct,
                ...p,
                defaultPresentation:
                  fetchedProduct.defaultPresentation ?? p.defaultPresentation,
                presentations: fetchedProduct.presentations ?? p.presentations,
              }
            : p
        )
      );
    } catch (err: any) {
      toast.error(err.message);
    }
  };

  const handleClosePresentationModal = () => {
    setOpenPresentationModal(false);
    setProductIdForPresentation(null);
  };

  const productForPresentation = useMemo(() => {
    if (productIdForPresentation == null) return null;
    return products.find((p) => p.id === productIdForPresentation) ?? null;
  }, [productIdForPresentation, products]);

  const getPriceForTier = (
    presentation: ProductPresentation,
    tier: PriceTier
  ) => {
    switch (tier) {
      case 1:
        return Number(presentation.price01 ?? 0);
      case 2:
        return Number(presentation.price02 ?? 0);
      case 3:
        return Number(presentation.price03 ?? 0);
      case 4:
        return Number(presentation.price04 ?? 0);
    }
  };

  const handleSelectPresentation = (
    presentation: ProductPresentation,
    priceTier: PriceTier
  ) => {
    if (productIdForPresentation == null) return;

    setProducts((prev) =>
      prev.map((p) => {
        if (p.id !== productIdForPresentation) return p;

        const price = getPriceForTier(presentation, priceTier);
        const unitMeasure = presentation.unitMeasure ?? p.unitMeasure;
        const base = (price - p.discount) * p.quantity;
        const ivaRate = p.tax?.rate ?? 12;
        const taxValue = base * (ivaRate / 100);

        return {
          ...p,
          unitMeasure,
          price,
          priceMode: "manual",
          priceTier,
          subtotal: base,
          taxValue,
        };
      })
    );

    handleClosePresentationModal();
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

  const buildInvoicePayload = (data: CreateInvoiceForm) => {
    const details = products.map((p) => ({
      productId: p.id,
      quantity: p.quantity,
      unitPrice: p.price,
      discount: p.discount,
      warehouseId: p.inventory[0]?.warehouseId ?? 0,
      taxId: p.tax?.id ?? 0,
      unitMeasureId: p.unitMeasure?.id ?? 0,
      netWeight: p.netWeight ?? 0,
      grossWeight: p.grossWeight ?? 0,
    }));

    const emissionDate = data.invoiceDate ?? new Date();
    const selectedDueDate = data.dueDate ?? emissionDate;

    return {
      ...data,
      invoiceDate: new Date(emissionDate),
      dueDate: new Date(selectedDueDate),
      details,
    };
  };

  const saveInvoice = async (
    data: CreateInvoiceForm,
    exitAfterSave: boolean
  ) => {
    if (!customer) {
      toast.error("Debe seleccionar un cliente");
      return;
    }

    if (!products.length) {
      toast.error("Debe agregar al menos un producto");
      return;
    }

    if (calculateTotals.total <= 0) {
      toast.error("El total de la factura debe ser mayor a 0");
      return;
    }

    const payload = buildInvoicePayload(data);

    try {
      if (exitAfterSave) {
        setSavingDraft(true);
      } else {
        setSavingAndContinue(true);
      }

      const response = await updateInvoice(Number(id), payload, token!);

      if (response.data) {
        setInvoice(response.data);
        const pm = response.data.paymentMethod ?? payload.paymentMethod;
        const nextPaymentType = isPaymentType(response.data.paymentType)
          ? response.data.paymentType
          : paymentTypeFromPaymentMethodCode(pm);
        setPaymentType(nextPaymentType);
        setPaymentMethod(paymentMethodCodeFromPaymentType(nextPaymentType));
        setPaymentTermDays(
          response.data.paymentTermDays ?? payload.paymentTermDays
        );
      }

      toast.success(response.message || "Factura actualizada correctamente.");

      if (exitAfterSave) {
        navigate("/facturas");
      } else {
        setCurrentStep(2);
      }
    } catch (err: any) {
      toast.error(err.message);
    } finally {
      setSavingDraft(false);
      setSavingAndContinue(false);
    }
  };

  const handleUpdateDraft = () =>
    handleSubmit((data) => saveInvoice(data, true))();

  const handleContinueToPayment = () =>
    handleSubmit((data) => saveInvoice(data, false))();

  const handleConfirmPayment = async () => {
    if (!invoice) return;

    if (calculateTotals.total <= 0) {
      toast.error("El total a pagar debe ser mayor a 0");
      return;
    }

    const body = {
      paymentMethod,
      paymentType,
      paymentTermDays,
    };

    try {
      setSavingPayment(true);
      const response = await updateInvoicePayment(invoice.id, body, token!);
      toast.success(response.message);
      navigate("/facturas");
    } catch (err: any) {
      toast.error(err.message);
    } finally {
      setSavingPayment(false);
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
            {currentStep === 1 ? (
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
              openPresentationModal={openPresentationModal}
              presentationProduct={productForPresentation}
              onOpenPresentationModal={handleOpenPresentationModal}
              onClosePresentationModal={handleClosePresentationModal}
              handleSelectPresentation={handleSelectPresentation}
              handleWeightChange={handleWeightChange}
              handleRemoveProduct={handleRemoveProduct}
              onSaveDraft={handleUpdateDraft}
              onContinue={handleContinueToPayment}
              savingDraft={savingDraft}
                savingAndContinuing={savingAndContinue}
                canConfirmPayment={!!isAdmin}
              />
            ) : (
              <InvoicePaymentStep
                customer={customer}
                products={products}
                totals={calculateTotals}
                paymentType={paymentType}
                paymentTermDays={paymentTermDays}
                onPaymentTypeChange={setPaymentType}
                onPaymentMethodChange={setPaymentMethod}
                onPaymentTermChange={setPaymentTermDays}
                onConfirmPayment={handleConfirmPayment}
                loading={savingPayment}
                sequential={invoice?.sequential}
              />
            )}
          </>
        )}
      </CardContent>
    </Card>
  );
}
