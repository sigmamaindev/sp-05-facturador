import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { toast } from "sonner";

import { useAuth } from "@/contexts/AuthContext";

import { createPurchase } from "@/api/purchase";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import type {
  CreatePurchaseForm,
  CreatePurchasePayload,
  PurchaseProduct,
  PurchaseSupplier,
  PurchaseTotals,
} from "@/types/purchase.type";
import type { Product } from "@/types/product.types";

import PurchaseCreateHeader from "./PurchaseCreateHeader";
import PurchaseCreateForm from "./PurchaseCreateForm";

export default function PurchaseCreateView() {
  const navigate = useNavigate();

  const { token, user } = useAuth();

  const [saving, setSaving] = useState(false);
  const [openSupplierModal, setOpenSupplierModal] = useState(false);
  const [supplier, setSupplier] = useState<PurchaseSupplier | null>(null);
  const [openProductModal, setOpenProductModal] = useState(false);
  const [products, setProducts] = useState<PurchaseProduct[]>([]);

  const { setValue, handleSubmit, watch } = useForm<CreatePurchaseForm>({
    defaultValues: {
      receiptType: "01",
      isElectronic: true,
      environment: user?.business?.sriEnvironment ?? "1",
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
      details: [],
    },
  });

  useEffect(() => {
    setValue("purchaseDate", new Date());
  }, [setValue]);

  useEffect(() => {
    if (user?.business?.sriEnvironment) {
      setValue("environment", user.business.sriEnvironment);
    }

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
      const base = unitCost - discount;
      const ivaRate = product.tax?.rate ?? 12;
      const taxValue = base * (ivaRate / 100);

      const newProduct: PurchaseProduct = {
        ...product,
        quantity: 1,
        unitCost,
        discount,
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
          const base = p.unitCost * newQty - p.discount;
          const ivaRate = p.tax?.rate ?? 12;
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

  const purchaseDate = watch("purchaseDate");
  const environment = watch("environment");
  const receiptType = watch("receiptType");
  const emissionTypeCode = watch("emissionTypeCode");
  const isElectronic = watch("isElectronic");
  const establishmentCode = watch("establishmentCode");
  const emissionPointCode = watch("emissionPointCode");
  const sequential = watch("sequential");
  const documentNumber = watch("documentNumber");
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
        netWeight: 0,
        grossWeight: 0,
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
      emissionTypeCode: data.emissionTypeCode,
      businessName: user?.business?.name ?? "",
      name: `Compra #${data.sequential}`,
      document: user?.business?.document ?? "",
      accessKey: data.accessKey ?? "",
      receiptType: data.receiptType,
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
      relatedParty: "NO",
      businessNameSubjectDetained: "",
      documentSubjectDetained: "",
      fiscalPeriod,
      supplierId: data.supplierId,
      status: "ISSUED",
      isElectronic: data.isElectronic,
      authorizationNumber: data.authorizationNumber ?? "",
      authorizationDate: data.authorizationDate ?? null,
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

  return (
    <div className="space-y-4">
      <PurchaseCreateHeader onSave={handleSubmit(savePurchase)} saving={saving} />

      <Card>
        <CardHeader>
          <CardTitle>Datos generales</CardTitle>
        </CardHeader>
        <CardContent className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <div className="space-y-2">
            <label className="text-sm font-medium">Documento electrónico</label>
            <Select
              value={isElectronic ? "true" : "false"}
              onValueChange={(v) => setValue("isElectronic", v === "true")}
            >
              <SelectTrigger className="w-full">
                <SelectValue placeholder="Seleccione" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="true">Sí</SelectItem>
                <SelectItem value="false">No</SelectItem>
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-2">
            <label className="text-sm font-medium">Ambiente SRI</label>
            <Select
              value={environment}
              onValueChange={(v) => setValue("environment", v)}
            >
              <SelectTrigger className="w-full">
                <SelectValue placeholder="Seleccione" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="1">Pruebas (1)</SelectItem>
                <SelectItem value="2">Producción (2)</SelectItem>
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-2">
            <label className="text-sm font-medium">Tipo de documento</label>
            <Select
              value={receiptType}
              onValueChange={(v) => setValue("receiptType", v)}
            >
              <SelectTrigger className="w-full">
                <SelectValue placeholder="Seleccione" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="01">Factura (01)</SelectItem>
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-2">
            <label className="text-sm font-medium">Tipo de emisión</label>
            <Select
              value={emissionTypeCode}
              onValueChange={(v) => setValue("emissionTypeCode", v)}
            >
              <SelectTrigger className="w-full">
                <SelectValue placeholder="Seleccione" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="1">Normal (1)</SelectItem>
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

          <div className="space-y-2 md:col-span-3">
            <label className="text-sm font-medium">Número de documento</label>
            <Input value={documentNumber} disabled />
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
                <label className="text-sm font-medium">Fecha autorización</label>
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
        purchaseDate={purchaseDate}
        openSupplierModal={openSupplierModal}
        setOpenSupplierModal={setOpenSupplierModal}
        openProductModal={openProductModal}
        setOpenProductModal={setOpenProductModal}
        handleSelectSupplier={handleSelectSupplier}
        handleSelectProduct={handleSelectProduct}
        handleQuantityChange={handleQuantityChange}
        handleUnitCostChange={handleUnitCostChange}
        handleDiscountChange={handleDiscountChange}
        handleRemoveProduct={handleRemoveProduct}
      />
    </div>
  );
}
