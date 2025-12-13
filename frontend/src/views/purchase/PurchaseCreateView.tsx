import { useEffect, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { toast } from "sonner";

import { useAuth } from "@/contexts/AuthContext";

import { createPurchase } from "@/api/purchase";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";

import type {
  CreatePurchaseForm,
  PurchaseProduct,
  PurchaseSupplier,
  PurchaseTotals,
} from "@/types/purchase.type";
import type { Product } from "@/types/product.types";

import PurchaseCreateHeader from "./PurchaseCreateHeader";
import PurchaseCreateForm from "./PurchaseCreateForm";

export default function PurchaseCreateView() {
  const navigate = useNavigate();

  const { token } = useAuth();

  const [saving, setSaving] = useState(false);
  const [openSupplierModal, setOpenSupplierModal] = useState(false);
  const [supplier, setSupplier] = useState<PurchaseSupplier | null>(null);
  const [openProductModal, setOpenProductModal] = useState(false);
  const [products, setProducts] = useState<PurchaseProduct[]>([]);

  const { setValue, handleSubmit, watch } = useForm<CreatePurchaseForm>({
    defaultValues: {
      businessId: 1,
      establishmentId: 1,
      warehouseId: 1,
      supplierId: 0,
      purchaseDate: new Date(),
      documentNumber: "",
      reference: "",
      subtotal: 0,
      totalTax: 0,
      total: 0,
      details: [],
    },
  });

  useEffect(() => {
    setValue("purchaseDate", new Date());
  }, [setValue]);

  const handleSelectSupplier = (selectedSupplier: PurchaseSupplier) => {
    setSupplier(selectedSupplier);
    setValue("supplierId", selectedSupplier.id);
    setOpenSupplierModal(false);
  };

  const handleSelectProduct = (product: Product) => {
    setOpenProductModal(false);

    setProducts((prev) => {
      const exists = prev.find((p) => p.id === product.id);

      if (exists) return prev;

      const unitCost = product.price ?? 0;
      const base = unitCost;
      const ivaRate = product.tax.rate ?? 12;
      const taxValue = base * (ivaRate / 100);

      const newProduct: PurchaseProduct = {
        ...product,
        quantity: 1,
        unitCost,
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
          const base = (p.unitCost ?? 0) * newQty;
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

  const totals = useMemo((): PurchaseTotals => {
    const subtotal = products.reduce((sum, p) => sum + p.subtotal, 0);
    const tax = products.reduce((sum, p) => sum + p.taxValue, 0);
    const total = subtotal + tax;

    return { subtotal, tax, total };
  }, [products]);

  useEffect(() => {
    setValue("subtotal", totals.subtotal);
    setValue("totalTax", totals.tax);
    setValue("total", totals.total);
  }, [totals, setValue]);

  const purchaseDate = watch("purchaseDate");
  const documentNumber = watch("documentNumber");
  const reference = watch("reference");

  const buildPayload = (data: CreatePurchaseForm) => {
    const details = products.map((p) => ({
      productId: p.id,
      quantity: p.quantity,
      unitCost: p.unitCost,
      warehouseId: p.inventory[0]?.warehouseId ?? data.warehouseId,
      taxId: p.tax.id,
    }));

    return {
      ...data,
      purchaseDate: data.purchaseDate ?? new Date(),
      details,
    };
  };

  const savePurchase = async (data: CreatePurchaseForm) => {
    if (!supplier) {
      toast.error("Debe seleccionar un proveedor");
      return;
    }

    if (!products.length) {
      toast.error("Debe agregar al menos un producto");
      return;
    }

    if (!documentNumber) {
      toast.error("Ingrese un número de documento");
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
    } catch (error: any) {
      toast.error(error.message ?? "No se pudo crear la compra");
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
        <CardContent className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div className="space-y-2">
            <label className="text-sm font-medium">Número de documento</label>
            <Input
              placeholder="Serie o factura del proveedor"
              value={documentNumber}
              onChange={(e) => setValue("documentNumber", e.target.value)}
            />
          </div>
          <div className="space-y-2">
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
        handleRemoveProduct={handleRemoveProduct}
      />
    </div>
  );
}
