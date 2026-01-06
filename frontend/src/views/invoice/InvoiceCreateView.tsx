import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { toast } from "sonner";

import { useAuth } from "@/contexts/AuthContext";

import {
  PaymentMethodCode,
  PaymentType,
  type PaymentTypeValue,
  isPaymentType,
  paymentMethodCodeFromPaymentType,
  paymentTypeFromPaymentMethodCode,
  type PaymentMethodCode as PaymentMethodCodeType,
} from "@/constants/paymentMethods";

import type { Customer } from "@/types/customer.types";
import type { Product } from "@/types/product.types";
import type { ProductPresentation } from "@/types/product.types";

import { createInvoice, updateInvoicePayment } from "@/api/invoice";
import { getProductById } from "@/api/product";

import { Card, CardContent } from "@/components/ui/card";

import type {
  CreateInvoiceForm,
  Invoice,
  InvoicePaymentUpdate,
  InvoiceProduct,
  InvoiceTotals,
} from "@/types/invoice.type";

import InvoiceCreateHeader from "./InvoiceCreateHeader";
import InvoiceCreateForm from "./InvoiceCreateForm";
import InvoiceCreatePayment from "./InvoiceCreatePayment";
import InvoiceAdditionalInfoModal from "./InvoiceAdditionalInfoModal";

export default function InvoiceCreateView() {
  type PriceTier = 1 | 2 | 3 | 4;

  const navigate = useNavigate();

  const { token, user } = useAuth();

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
  const [currentStep, setCurrentStep] = useState<1 | 2>(1);
  const [draftInvoice, setDraftInvoice] = useState<Invoice | null>(null);
  const [paymentMethod, setPaymentMethod] = useState<PaymentMethodCodeType>(
    PaymentMethodCode.NFS
  );
  const [paymentType, setPaymentType] = useState<PaymentTypeValue>(
    PaymentType.CASH
  );
  const [paymentTermDays, setPaymentTermDays] = useState(0);
  const [openAdditionalInfoModal, setOpenAdditionalInfoModal] = useState(false);
  const [additionalInfoDraft, setAdditionalInfoDraft] = useState<{
    description: string;
    additionalInformation: string;
  }>({ description: "", additionalInformation: "" });

  const { setValue, handleSubmit, getValues } = useForm<CreateInvoiceForm>({
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
      paymentMethod: PaymentMethodCode.NFS,
      paymentTermDays: 0,
      description: "",
      additionalInformation: "",
      details: [],
    },
  });

  const handleOpenAdditionalInfoModal = () => {
    setAdditionalInfoDraft({
      description: getValues("description") ?? "",
      additionalInformation: getValues("additionalInformation") ?? "",
    });
    setOpenAdditionalInfoModal(true);
  };

  const handleSaveAdditionalInfo = (values: {
    description: string;
    additionalInformation: string;
  }) => {
    setValue("description", values.description ?? "");
    setValue("additionalInformation", values.additionalInformation ?? "");
    setAdditionalInfoDraft(values);
    setOpenAdditionalInfoModal(false);
  };

  useEffect(() => {
    const now = new Date();
    setValue("invoiceDate", now);
    setValue("dueDate", now);

    if (user?.business?.sriEnvironment) {
      setValue("environment", user.business.sriEnvironment);
    }
  }, [setValue, user]);

  const handleSelectCustomer = (selectedCustomer: Customer) => {
    setCustomer(selectedCustomer);
    setValue("customerId", selectedCustomer.id);
    setOpenCustomerModal(false);
  };

  const handleSelectProduct = (product: Product) => {
    setOpenProductModal(false);

    setProducts((prev) => {
      const exists = prev.find((p) => p.id === product.id);

      if (exists) return prev;

      const priceTier: PriceTier = 1;
      const price =
        product.defaultPresentation?.price01 ?? product.price ?? 0;
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
        priceMode: "manual",
        priceTier,
        netWeight,
        grossWeight,
        quantity,
        discount: discount,
        subtotal: base,
        taxValue: taxValue,
      };

      return [...prev, newProduct];
    });
  };

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

  const handleUnitPriceChange = (productId: number, value: number) => {
    const next = Number.isFinite(value) ? value : 0;

    setProducts((prev) =>
      prev.map((p) => {
        if (p.id !== productId) return p;

        const price = next;
        const base = (price - p.discount) * p.quantity;
        const ivaRate = p.tax?.rate ?? 12;
        const taxValue = base * (ivaRate / 100);

        return {
          ...p,
          priceMode: "manual",
          price,
          subtotal: base,
          taxValue,
        };
      })
    );
  };

  const handleQuantityChange = (productId: number, value: number) => {
    const next = Number.isFinite(value) ? value : 0;

    setProducts((prev) =>
      prev.map((p) => {
        if (p.id !== productId) return p;

        const quantity = next;
        const base = (p.price - p.discount) * quantity;
        const ivaRate = p.tax?.rate ?? 12;
        const taxValue = base * (ivaRate / 100);

        return {
          ...p,
          quantity,
          subtotal: base,
          taxValue,
        };
      })
    );
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

  const handleRemoveProduct = (id: number) => {
    setProducts((prev) => prev.filter((p) => p.id !== id));
  };

  const totals = useMemo((): InvoiceTotals => {
    const subtotal = products.reduce((sum, p) => sum + p.subtotal, 0);
    const discount = products.reduce((sum, p) => sum + p.discount, 0);
    const tax = products.reduce((sum, p) => sum + p.taxValue, 0);
    const total = subtotal + tax;

    return { subtotal, discount, tax, total };
  }, [products]);

  useEffect(() => {
    setValue("subtotalWithoutTaxes", totals.subtotal);
    setValue("subtotalWithTaxes", totals.subtotal);
    setValue("discountTotal", totals.discount);
    setValue("taxTotal", totals.tax);
    setValue("totalInvoice", totals.total);
  }, [totals, setValue]);

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

    return {
      ...data,
      details,
    };
  };

  const saveInvoiceDraft = async (
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

    if (totals.total <= 0) {
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

      const response = await createInvoice(payload, token!);

      if (response.data) {
        setDraftInvoice(response.data);
        
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

      toast.success(response.message || "Factura guardada como borrador.");

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

  const handleSaveDraftAndExit = () =>
    handleSubmit((data) => saveInvoiceDraft(data, true))();

  const handleContinueToPayment = () =>
    handleSubmit((data) => saveInvoiceDraft(data, false))();

  const handleConfirmPayment = async () => {
    if (!draftInvoice) return;

    if (totals.total <= 0) {
      toast.error("El total a pagar debe ser mayor a 0");
      return;
    }

    const body: InvoicePaymentUpdate = {
      paymentMethod,
      paymentType,
      paymentTermDays,
    };

    try {
      setSavingPayment(true);
      const response = await updateInvoicePayment(
        draftInvoice.id,
        body,
        token!
      );
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
      <CardContent className="space-y-4">
        <InvoiceCreateHeader currentStep={currentStep} />
        {currentStep === 1 ? (
          <InvoiceCreateForm
            customer={customer}
            products={products}
            totals={totals}
            openCustomerModal={openCustomerModal}
            setOpenCustomerModal={setOpenCustomerModal}
            hasAdditionalInfo={
              additionalInfoDraft.description.trim().length > 0 ||
              additionalInfoDraft.additionalInformation.trim().length > 0
            }
            onOpenAdditionalInfoModal={handleOpenAdditionalInfoModal}
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
            handleQuantityChange={handleQuantityChange}
            handleUnitPriceChange={handleUnitPriceChange}
            handleRemoveProduct={handleRemoveProduct}
            onSaveDraft={handleSaveDraftAndExit}
            onContinue={handleContinueToPayment}
            savingDraft={savingDraft}
            savingAndContinuing={savingAndContinue}
          />
        ) : (
          <InvoiceCreatePayment
            customer={customer}
            products={products}
            totals={totals}
            paymentType={paymentType}
            paymentTermDays={paymentTermDays}
            onPaymentTypeChange={setPaymentType}
            onPaymentMethodChange={setPaymentMethod}
            onPaymentTermChange={setPaymentTermDays}
            onConfirmPayment={handleConfirmPayment}
            loading={savingPayment}
            sequential={draftInvoice?.sequential}
          />
        )}
        <InvoiceAdditionalInfoModal
          open={openAdditionalInfoModal}
          onClose={() => setOpenAdditionalInfoModal(false)}
          initialDescription={additionalInfoDraft.description}
          initialAdditionalInformation={additionalInfoDraft.additionalInformation}
          onSave={handleSaveAdditionalInfo}
        />
      </CardContent>
    </Card>
  );
}
