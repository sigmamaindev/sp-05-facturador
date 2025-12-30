import { useEffect, useState } from "react";
import { toast } from "sonner";

import { useAuth } from "@/contexts/AuthContext";

import { getProductById } from "@/api/product";
import { getTaxes } from "@/api/tax";
import { getUnitMeasures } from "@/api/unitMeasure";
import { getWarehouses } from "@/api/warehouse";

import type { Product } from "@/types/product.types";
import type { Tax } from "@/types/tax.types";
import type { UnitMeasure } from "@/types/unitMeasure.types";
import type { Warehouse } from "@/types/warehouse.types";

import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";

import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";

import ProductCreateForm from "./ProductCreateForm";
import ProductCreateInventoryForm from "./ProductCreateInventoryForm";

interface ProductCreateModalProps {
  open: boolean;
  onClose: () => void;
  onCreated?: (product: Product) => void;
}

export default function ProductCreateModal({
  open,
  onClose,
  onCreated,
}: ProductCreateModalProps) {
  const { token } = useAuth();

  const [step, setStep] = useState<1 | 2>(1);
  const [product, setProduct] = useState<Product | null>(null);
  const [warehouses, setWarehouses] = useState<Warehouse[]>([]);
  const [unitMeasures, setUnitMeasures] = useState<UnitMeasure[]>([]);
  const [taxes, setTaxes] = useState<Tax[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!open) return;

    setStep(1);
    setProduct(null);
    setError(null);

    if (!token) {
      setError("No se ha iniciado sesión.");
      return;
    }

    let cancelled = false;

    const fetchAllData = async () => {
      try {
        setLoading(true);

        const [warRes, unitMeRes, taxRes] = await Promise.all([
          getWarehouses("", 1, 100, token),
          getUnitMeasures("", 1, 100, token),
          getTaxes("", 1, 100, token),
        ]);

        if (cancelled) return;

        setWarehouses(warRes.data);
        setUnitMeasures(unitMeRes.data);
        setTaxes(taxRes.data);
      } catch (err: any) {
        if (cancelled) return;
        setError(err.message);
      } finally {
        if (cancelled) return;
        setLoading(false);
      }
    };

    fetchAllData();

    return () => {
      cancelled = true;
    };
  }, [open, token]);

  const handleNext = (created: Product) => {
    setProduct(created);
    setStep(2);
  };

  const finalizeCreation = async () => {
    if (!token || !product) return;

    try {
      const refreshed = await getProductById(product.id, token);
      const created = refreshed.data ?? product;

      onCreated?.(created);
      onClose();
    } catch (err: any) {
      toast.error(err.message);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onClose}>
      <DialogContent className="!max-w-4xl">
        <DialogHeader>
          <DialogTitle>
            {step === 1 ? "Crear producto" : "Configurar inventario"}
          </DialogTitle>
        </DialogHeader>

        <Card>
          <CardContent>
            {loading ? (
              <Loading />
            ) : error ? (
              <AlertMessage message={error} variant="destructive" />
            ) : step === 1 ? (
              <>
                <ProductCreateForm
                  unitMeasures={unitMeasures}
                  taxes={taxes}
                  token={token}
                  onNext={handleNext}
                />
                <div className="flex justify-end mt-4">
                  <Button type="button" variant="outline" onClick={onClose}>
                    Cancelar
                  </Button>
                </div>
              </>
            ) : token && product ? (
              <>
                <ProductCreateInventoryForm
                  token={token}
                  warehouses={warehouses}
                  product={product}
                  onComplete={() => {
                    void finalizeCreation();
                  }}
                />
                <div className="flex justify-end mt-4">
                  <Button type="button" variant="outline" onClick={onClose}>
                    Cerrar
                  </Button>
                </div>
              </>
            ) : (
              <AlertMessage
                message="El producto no se ha creado correctamente."
                variant="destructive"
              />
            )}
          </CardContent>
        </Card>
      </DialogContent>
    </Dialog>
  );
}
