import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { toast } from "sonner";

import { useAuth } from "@/contexts/AuthContext";

import {
  PaymentType,
  type PaymentTypeValue,
  paymentMethodCodeFromPaymentType,
} from "@/constants/paymentMethods";
import type { PaymentMethodCode as PaymentMethodCodeType } from "@/constants/paymentMethods";

import { createPurchase } from "@/api/purchase";
import { getProductById } from "@/api/product";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import { SUPPORTING_CODE_OPTIONS } from "@/constants/supportingCodes";
import { SUPPORTING_DOCUMENT_CODE_OPTIONS } from "@/constants/supportingDocumentCodes";

import type {
  CreatePurchaseForm,
  CreatePurchasePayload,
  PurchaseProduct,
  PurchaseSupplier,
  PurchaseTotals,
} from "@/types/purchase.type";
import type { Product } from "@/types/product.types";
import type { ProductPresentation } from "@/types/product.types";

import PurchaseCreateHeader from "./PurchaseCreateHeader";
import PurchaseCreateForm from "./PurchaseCreateForm";

export default function PurchaseCreateView() {
  const navigate = useNavigate();

  const { token, user } = useAuth();

  const isAdmin =
    user?.roles?.includes("SuperAdmin") || user?.roles?.includes("Admin");

  const [saving, setSaving] = useState(false);
  const [openSupplierModal, setOpenSupplierModal] = useState(false);
  const [supplier, setSupplier] = useState<PurchaseSupplier | null>(null);
  const [openProductModal, setOpenProductModal] = useState(false);
  const [products, setProducts] = useState<PurchaseProduct[]>([]);
  const [openPresentationModal, setOpenPresentationModal] = useState(false);
  const [productIdForPresentation, setProductIdForPresentation] = useState<
    number | null
  >(null);
  const [paymentCondition, setPaymentCondition] = useState<"CASH" | "CREDIT">(
    "CASH"
  );
  const [paymentType, setPaymentType] = useState<PaymentTypeValue>(
    PaymentType.CASH
  );
  const [paymentMethod, setPaymentMethod] = useState<PaymentMethodCodeType>(
    paymentMethodCodeFromPaymentType(PaymentType.CASH)
  );
  const [paymentTermDays, setPaymentTermDays] = useState(0);

  const { setValue, handleSubmit, watch } = useForm<CreatePurchaseForm>({
    defaultValues: {
      receiptType: "01",
      supportingDocumentCode: "01",
      supportingCode: "01",
      relatedParty: "NO",
      isElectronic: true,
      environment: "2",
      emissionTypeCode: "1",
      supplierId: 0,
      purchaseDate: new Date(),
      establishmentCode: user?.establishment?.[0]?.code ?? "001",
      emissionPointCode: user?.emissionPoint?.[0]?.code ?? "001",
      sequential: "",
      documentNumber: "001-001-",
      accessKey: "",
      authorizationNumber: "",
      authorizationDate: undefined,
      reference: "",
      subtotalWithoutTaxes: 0,
      subtotalWithTaxes: 0,
      discountTotal: 0,
      taxTotal: 0,
      totalPurchase: 0,
      paymentMethod: paymentMethodCodeFromPaymentType(PaymentType.CASH),
      paymentTermDays: 0,
      details: [],
    },
  });

  useEffect(() => {
    setValue("purchaseDate", new Date());
  }, [setValue]);

  useEffect(() => {
    setValue("environment", "2");
    setValue("isElectronic", true);

    if (user?.establishment?.[0]?.code) {
      setValue("establishmentCode", user.establishment[0].code);
    }

    if (user?.emissionPoint?.[0]?.code) {
      setValue("emissionPointCode", user.emissionPoint[0].code);
    }
  }, [setValue, user]);

  const handleSelectSupplier = (selectedSupplier: PurchaseSupplier) => {
    setSupplier(selectedSupplier);
    setValue("supplierId", selectedSupplier.id);
    setOpenSupplierModal(false);
  };

  const formatDateTimeLocalValue = (date?: Date) => {
    if (!date) return "";
    const pad = (n: number) => String(n).padStart(2, "0");
    const yyyy = date.getFullYear();
    const mm = pad(date.getMonth() + 1);
    const dd = pad(date.getDate());
    const hh = pad(date.getHours());
    const min = pad(date.getMinutes());
    return `${yyyy}-${mm}-${dd}T${hh}:${min}`;
  };

  const buildDocumentNumber = (
    establishmentCode: string,
    emissionPointCode: string,
    sequential: string
  ) => `${establishmentCode}-${emissionPointCode}-${sequential}`;

  const fiscalPeriodFromDate = (date: Date) => {
    const mm = String(date.getMonth() + 1).padStart(2, "0");
    const yyyy = date.getFullYear();
    return `${mm}/${yyyy}`;
  };

  const handleSelectProduct = (product: Product) => {
    setOpenProductModal(false);

    setProducts((prev) => {
      const exists = prev.find((p) => p.id === product.id);

      if (exists) return prev;

      const unitCost = product.price ?? 0;
      const discount = 0;
      const ivaRate = product.tax?.rate ?? 12;
      const netWeight = product.defaultPresentation?.netWeight ?? 0;
      const grossWeight = product.defaultPresentation?.grossWeight ?? 0;
      const unitMeasure =
        product.defaultPresentation?.unitMeasure ?? product.unitMeasure;
      const computedQuantity = Number((grossWeight - netWeight).toFixed(2));
      const quantity = Number.isFinite(computedQuantity)
        ? Math.max(0, computedQuantity)
        : 0;

      const newProduct: PurchaseProduct = {
        ...product,
        unitMeasure,
        quantity,
        unitCost,
        discount,
        netWeight,
        grossWeight,
        subtotal: unitCost * quantity - discount,
        taxValue: (unitCost * quantity - discount) * (ivaRate / 100),
      };

      return [...prev, newProduct];
    });
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
    } catch (error: unknown) {
      const message =
        error instanceof Error ? error.message : "No se pudo cargar el producto";
      toast.error(message);
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

  const handleSelectPresentation = (presentation: ProductPresentation) => {
    if (productIdForPresentation == null) return;

    setProducts((prev) =>
      prev.map((p) => {
        if (p.id !== productIdForPresentation) return p;

        const unitMeasure = presentation.unitMeasure ?? p.unitMeasure;
        const netWeight = Number(presentation.netWeight ?? p.netWeight ?? 0);
        const grossWeight = Number(
          presentation.grossWeight ?? p.grossWeight ?? 0
        );
        const computedQuantity = Number((grossWeight - netWeight).toFixed(2));
        const quantity = Number.isFinite(computedQuantity)
          ? Math.max(0, computedQuantity)
          : 0;
        const base = p.unitCost * quantity - p.discount;
        const ivaRate = p.tax?.rate ?? 12;
        const taxValue = base * (ivaRate / 100);

        return {
          ...p,
          unitMeasure,
          netWeight,
          grossWeight,
          quantity,
          subtotal: base,
          taxValue,
        };
      })
    );

    handleClosePresentationModal();
  };

  const handleUnitCostChange = (productId: number, newUnitCost: number) => {
    setProducts((prev) =>
      prev.map((p) => {
        if (p.id !== productId) return p;
        const base = newUnitCost * p.quantity - p.discount;
        const ivaRate = p.tax?.rate ?? 12;
        const taxValue = base * (ivaRate / 100);
        return { ...p, unitCost: newUnitCost, subtotal: base, taxValue };
      })
    );
  };

  const handleDiscountChange = (productId: number, newDiscount: number) => {
    setProducts((prev) =>
      prev.map((p) => {
        if (p.id !== productId) return p;
        const base = p.unitCost * p.quantity - newDiscount;
        const ivaRate = p.tax?.rate ?? 12;
        const taxValue = base * (ivaRate / 100);
        return { ...p, discount: newDiscount, subtotal: base, taxValue };
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
        const quantity = Number.isFinite(computedQuantity)
          ? Math.max(0, computedQuantity)
          : 0;
        const base = p.unitCost * quantity - p.discount;
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

  const totals = useMemo((): PurchaseTotals => {
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
    setValue("totalPurchase", totals.total);
  }, [totals, setValue]);

  useEffect(() => {
    setValue("paymentMethod", paymentMethod);
  }, [paymentMethod, setValue]);

  useEffect(() => {
    setValue("paymentTermDays", paymentTermDays);
  }, [paymentTermDays, setValue]);

  useEffect(() => {
    if (paymentTermDays > 0) setPaymentCondition("CREDIT");
  }, [paymentTermDays]);

  const purchaseDate = watch("purchaseDate");
  const receiptType = watch("receiptType");
  const supportingCode = watch("supportingCode");
  const relatedParty = watch("relatedParty");
  const isElectronic = watch("isElectronic");
  const establishmentCode = watch("establishmentCode");
  const emissionPointCode = watch("emissionPointCode");
  const sequential = watch("sequential");
  const accessKey = watch("accessKey");
  const authorizationNumber = watch("authorizationNumber");
  const authorizationDate = watch("authorizationDate");
  const reference = watch("reference");

  useEffect(() => {
    if (!isElectronic) {
      setValue("accessKey", "");
      setValue("authorizationNumber", "");
      setValue("authorizationDate", undefined);
    }
  }, [isElectronic, setValue]);

  useEffect(() => {
    setValue(
      "documentNumber",
      buildDocumentNumber(
        establishmentCode ?? "001",
        emissionPointCode ?? "001",
        sequential ?? ""
      )
    );
  }, [emissionPointCode, establishmentCode, sequential, setValue]);

  const buildPayload = (data: CreatePurchaseForm): CreatePurchasePayload => {
    const issueDate = data.purchaseDate ?? new Date();
    const fiscalPeriod = fiscalPeriodFromDate(issueDate);

    const details = products.map((p) => {
      const taxRate = p.tax?.rate ?? 0;
      const subtotal = p.unitCost * p.quantity - p.discount;
      const taxValue = subtotal * (taxRate / 100);
      const total = subtotal + taxValue;
      return {
        productId: p.id,
        warehouseId: p.inventory[0]?.warehouseId ?? 1,
        unitMeasureId: p.unitMeasure?.id ?? 1,
        taxId: p.tax?.id ?? 0,
        taxRate,
        taxValue,
        quantity: p.quantity,
        netWeight: p.netWeight ?? 0,
        grossWeight: p.grossWeight ?? 0,
        unitCost: p.unitCost,
        discount: p.discount,
        subtotal,
        total,
      };
    });

    return {
      businessId: user?.business?.id ?? 0,
      userId: user?.id ?? 0,
      environment: data.environment,
      emissionTypeCode: "1",
      businessName: user?.business?.name ?? "",
      name: `Compra #${data.sequential}`,
      document: user?.business?.document ?? "",
      accessKey: data.accessKey ?? "",
      receiptType: data.receiptType,
      supportingDocumentCode: data.supportingDocumentCode,
      supportingCode: data.supportingCode,
      establishmentCode: data.establishmentCode,
      emissionPointCode: data.emissionPointCode,
      sequential: data.sequential,
      mainAddress: user?.business?.address ?? "",
      issueDate,
      establishmentAddress: null,
      specialTaxpayer: null,
      mandatoryAccounting: "NO",
      typeDocumentSubjectDetained: "04",
      typeSubjectDetained: "",
      relatedParty: data.relatedParty,
      businessNameSubjectDetained: "",
      documentSubjectDetained: "",
      fiscalPeriod,
      supplierId: data.supplierId,
      status: "EMITIDO",
      isElectronic: data.isElectronic,
      authorizationNumber: data.authorizationNumber ?? "",
      authorizationDate: data.authorizationDate ?? null,
      paymentMethod: data.paymentMethod,
      paymentTermDays: data.paymentTermDays,
      subtotalWithoutTaxes: data.subtotalWithoutTaxes,
      subtotalWithTaxes: data.subtotalWithTaxes,
      discountTotal: data.discountTotal,
      taxTotal: data.taxTotal,
      totalPurchase: data.totalPurchase,
      details,
    };
  };

  const savePurchase = async (data: CreatePurchaseForm) => {
    if (!user?.business?.id || !user?.id) {
      toast.error("No se pudo identificar el usuario/empresa");
      return;
    }

    if (!supplier) {
      toast.error("Debe seleccionar un proveedor");
      return;
    }

    if (!products.length) {
      toast.error("Debe agregar al menos un producto");
      return;
    }

    if (!sequential) {
      toast.error("Ingrese el secuencial del documento");
      return;
    }

    if (isElectronic && !accessKey) {
      toast.error("Ingrese la clave de acceso");
      return;
    }

    if (isElectronic && accessKey && accessKey.length !== 49) {
      toast.error("La clave de acceso debe tener 49 dígitos");
      return;
    }

    const payload = buildPayload(data);

    try {
      setSaving(true);
      const response = await createPurchase(payload, token!);

      if (response.data) {
        toast.success("Compra creada correctamente");
        navigate(`/compras/${response.data.id}`);
      }
    } catch (error: unknown) {
      const message =
        error instanceof Error ? error.message : "No se pudo crear la compra";
      toast.error(message);
    } finally {
      setSaving(false);
    }
  };

  const canProceed =
    supplier !== null &&
    products.length > 0 &&
    Boolean(String(sequential ?? "").trim()) &&
    (!isElectronic || String(accessKey ?? "").trim().length === 49);

  return (
    <Card>
      <CardContent className="space-y-4">
        <PurchaseCreateHeader />

        <Card>
          <CardHeader>
            <CardTitle>Datos generales</CardTitle>
          </CardHeader>
          <CardContent className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="space-y-2">
              <label className="text-sm font-medium">Tipo de documento</label>
              <Select
                value={receiptType}
                onValueChange={(v) => {
                  setValue("receiptType", v);
                  setValue("supportingDocumentCode", v);
                }}
              >
                <SelectTrigger className="w-full">
                  <SelectValue placeholder="Seleccione" />
                </SelectTrigger>
                <SelectContent>
                  {SUPPORTING_DOCUMENT_CODE_OPTIONS.map((option) => (
                    <SelectItem key={option.value} value={option.value}>
                      {option.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2">
              <label className="text-sm font-medium">Código de sustento</label>
              <Select
                value={supportingCode}
                onValueChange={(v) => setValue("supportingCode", v)}
              >
                <SelectTrigger className="w-full">
                  <SelectValue placeholder="Seleccione" />
                </SelectTrigger>
                <SelectContent>
                  {SUPPORTING_CODE_OPTIONS.map((option) => (
                    <SelectItem key={option.value} value={option.value}>
                      {option.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2">
              <label className="text-sm font-medium">Fecha de emisión</label>
              <Input
                type="datetime-local"
                value={formatDateTimeLocalValue(purchaseDate)}
                onChange={(e) =>
                  setValue(
                    "purchaseDate",
                    e.target.value ? new Date(e.target.value) : new Date()
                  )
                }
              />
            </div>

            <div className="space-y-2">
              <label className="text-sm font-medium">Parte relacionada</label>
              <Select
                value={relatedParty}
                onValueChange={(v) => setValue("relatedParty", v as "SI" | "NO")}
              >
                <SelectTrigger className="w-full">
                  <SelectValue placeholder="Seleccione" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="NO">NO</SelectItem>
                  <SelectItem value="SI">SI</SelectItem>
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2">
              <label className="text-sm font-medium">Establecimiento</label>
              <Input
                placeholder="001"
                value={establishmentCode}
                onChange={(e) => setValue("establishmentCode", e.target.value)}
              />
            </div>
            <div className="space-y-2">
              <label className="text-sm font-medium">Punto de emisión</label>
              <Input
                placeholder="001"
                value={emissionPointCode}
                onChange={(e) => setValue("emissionPointCode", e.target.value)}
              />
            </div>

            <div className="space-y-2">
              <label className="text-sm font-medium">Secuencial</label>
              <Input
                placeholder="000000001"
                value={sequential}
                onChange={(e) => setValue("sequential", e.target.value)}
              />
            </div>

            {isElectronic && (
              <>
                <div className="space-y-2 md:col-span-3">
                  <label className="text-sm font-medium">Clave de acceso</label>
                  <Input
                    placeholder="49 dígitos"
                    value={accessKey ?? ""}
                    onChange={(e) => setValue("accessKey", e.target.value)}
                  />
                </div>

                <div className="space-y-2 md:col-span-2">
                  <label className="text-sm font-medium">
                    Número de autorización
                  </label>
                  <Input
                    placeholder="Opcional"
                    value={authorizationNumber ?? ""}
                    onChange={(e) =>
                      setValue("authorizationNumber", e.target.value)
                    }
                  />
                </div>

                <div className="space-y-2">
                  <label className="text-sm font-medium">
                    Fecha autorización
                  </label>
                  <Input
                    type="datetime-local"
                    value={formatDateTimeLocalValue(authorizationDate)}
                    onChange={(e) =>
                      setValue(
                        "authorizationDate",
                        e.target.value ? new Date(e.target.value) : undefined
                      )
                    }
                  />
                </div>
              </>
            )}

            <div className="space-y-2 md:col-span-3">
              <label className="text-sm font-medium">Referencia</label>
              <Input
                placeholder="Nota interna opcional"
                value={reference ?? ""}
                onChange={(e) => setValue("reference", e.target.value)}
              />
            </div>
          </CardContent>
        </Card>

        <PurchaseCreateForm
          supplier={supplier}
          products={products}
          totals={totals}
          paymentCondition={paymentCondition}
          paymentType={paymentType}
          paymentTermDays={paymentTermDays}
          onPaymentConditionChange={(value) => {
            setPaymentCondition(value);
            if (value === "CASH") setPaymentTermDays(0);
          }}
          onPaymentTypeChange={(value) => {
            setPaymentType(value);
            setPaymentMethod(paymentMethodCodeFromPaymentType(value));
          }}
          onPaymentTermDaysChange={(value) => {
            const normalized = Number.isFinite(value)
              ? Math.min(365, Math.max(0, Math.trunc(value)))
              : 0;
            setPaymentTermDays(normalized);
          }}
          openSupplierModal={openSupplierModal}
          setOpenSupplierModal={setOpenSupplierModal}
          openProductModal={openProductModal}
          setOpenProductModal={setOpenProductModal}
          handleSelectSupplier={handleSelectSupplier}
          handleSelectProduct={handleSelectProduct}
          handleUnitCostChange={handleUnitCostChange}
          handleDiscountChange={handleDiscountChange}
          handleWeightChange={handleWeightChange}
          handleRemoveProduct={handleRemoveProduct}
          openPresentationModal={openPresentationModal}
          presentationProduct={productForPresentation}
          onOpenPresentationModal={handleOpenPresentationModal}
          onClosePresentationModal={handleClosePresentationModal}
          handleSelectPresentation={handleSelectPresentation}
          onSave={handleSubmit(savePurchase)}
          onCancel={() => navigate("/compras")}
          saving={saving}
          canProceed={canProceed}
          canConfirmPayment={!!isAdmin}
        />
      </CardContent>
    </Card>
  );
}
