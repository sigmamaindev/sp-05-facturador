import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { toast } from "sonner";

import { useAuth } from "@/contexts/AuthContext";

import type { Customer } from "@/types/customer.types";
import type { Product } from "@/types/product.types";
import type { UnitMeasure } from "@/types/unitMeasure.types";

import { createInvoice, updateInvoicePayment } from "@/api/invoice";

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

export default function InvoiceCreateView() {
  const navigate = useNavigate();

  const { token, user } = useAuth();

  const [savingDraft, setSavingDraft] = useState(false);
  const [savingAndContinue, setSavingAndContinue] = useState(false);
  const [savingPayment, setSavingPayment] = useState(false);
  const [openCustomerModal, setOpenCustomerModal] = useState(false);
  const [customer, setCustomer] = useState<Customer | null>(null);
  const [openProductModal, setOpenProductModal] = useState(false);
  const [products, setProducts] = useState<InvoiceProduct[]>([]);
  const [openUnitMeasureModal, setOpenUnitMeasureModal] = useState(false);
  const [productIdForUnitMeasure, setProductIdForUnitMeasure] = useState<
    number | null
  >(null);
  const [currentStep, setCurrentStep] = useState<1 | 2>(1);
  const [draftInvoice, setDraftInvoice] = useState<Invoice | null>(null);
  const [paymentMethod, setPaymentMethod] = useState("01");
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

  const handleOpenUnitMeasureModal = (productId: number) => {
    setProductIdForUnitMeasure(productId);
    setOpenUnitMeasureModal(true);
  };

  const handleCloseUnitMeasureModal = () => {
    setOpenUnitMeasureModal(false);
    setProductIdForUnitMeasure(null);
  };

  const handleSelectUnitMeasure = (unitMeasure: UnitMeasure) => {
    if (productIdForUnitMeasure == null) return;

    setProducts((prev) =>
      prev.map((p) =>
        p.id === productIdForUnitMeasure ? { ...p, unitMeasure } : p
      )
    );

    handleCloseUnitMeasureModal();
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
      taxId: p.tax.id,
      unitMeasureId: p.unitMeasure?.id,
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
        setPaymentMethod(response.data.paymentMethod ?? payload.paymentMethod);
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
            openProductModal={openProductModal}
            setOpenProductModal={setOpenProductModal}
            handleSelectCustomer={handleSelectCustomer}
            handleSelectProduct={handleSelectProduct}
            openUnitMeasureModal={openUnitMeasureModal}
            onOpenUnitMeasureModal={handleOpenUnitMeasureModal}
            onCloseUnitMeasureModal={handleCloseUnitMeasureModal}
            handleSelectUnitMeasure={handleSelectUnitMeasure}
            handleQuantityChange={handleQuantityChange}
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
            paymentMethod={paymentMethod}
            paymentTermDays={paymentTermDays}
            onPaymentMethodChange={setPaymentMethod}
            onPaymentTermChange={setPaymentTermDays}
            onConfirmPayment={handleConfirmPayment}
            loading={savingPayment}
            sequential={draftInvoice?.sequential}
          />
        )}
      </CardContent>
    </Card>
  );
}
