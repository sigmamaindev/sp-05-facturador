import type { ProductPresentation } from "@/types/product.types";
import type { PurchaseProduct } from "@/types/purchase.type";

import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";

function getPresentations(
  product: PurchaseProduct | null
): ProductPresentation[] {
  if (!product) return [];
  const presentations = [
    ...(product.defaultPresentation ? [product.defaultPresentation] : []),
    ...(product.presentations ?? []),
  ];

  const byUnitMeasure = new Map<number, ProductPresentation>();
  for (const p of presentations) {
    if (!p?.unitMeasureId) continue;
    if (!byUnitMeasure.has(p.unitMeasureId)) byUnitMeasure.set(p.unitMeasureId, p);
  }

  return Array.from(byUnitMeasure.values()).sort((a, b) => {
    if (a.isDefault && !b.isDefault) return -1;
    if (!a.isDefault && b.isDefault) return 1;
    return a.unitMeasure?.code?.localeCompare(b.unitMeasure?.code ?? "") ?? 0;
  });
}

interface PurchaseProductPresentationModalProps {
  open: boolean;
  onClose: () => void;
  product: PurchaseProduct | null;
  onSelect: (presentation: ProductPresentation) => void;
}

export default function PurchaseProductPresentationModal({
  open,
  onClose,
  product,
  onSelect,
}: PurchaseProductPresentationModalProps) {
  const presentations = getPresentations(product);
  const activePresentations = presentations.filter((p) => p.isActive);
  const rows = activePresentations.length ? activePresentations : presentations;
  const selectedUnitMeasureId = product?.unitMeasure?.id ?? null;

  return (
    <Dialog open={open} onOpenChange={onClose}>
      <DialogContent className="!max-w-2xl">
        <DialogHeader>
          <DialogTitle>
            {product
              ? `Presentaciones: ${product.sku} - ${product.name}`
              : "Presentaciones"}
          </DialogTitle>
        </DialogHeader>

        <Card>
          <CardContent className="pt-6">
            {!product ? (
              <p className="text-sm text-muted-foreground">
                Selecciona un producto primero.
              </p>
            ) : rows.length === 0 ? (
              <p className="text-sm text-muted-foreground">
                Este producto no tiene presentaciones configuradas.
              </p>
            ) : (
              <div className="w-full overflow-auto rounded-md border">
                <table className="w-full text-sm">
                  <thead className="bg-muted/50">
                    <tr className="text-left">
                      <th className="p-3 font-medium">U. medida</th>
                      <th className="p-3 font-medium text-right">Pesos</th>
                      <th className="p-3 font-medium text-center">Default</th>
                      <th className="p-3 font-medium text-right">Acción</th>
                    </tr>
                  </thead>
                  <tbody>
                    {rows.map((p) => (
                      <tr
                        key={`${p.id}-${p.unitMeasureId}`}
                        className={[
                          "border-t",
                          selectedUnitMeasureId === p.unitMeasureId
                            ? "bg-muted/30"
                            : "",
                        ].join(" ")}
                      >
                        <td className="p-3">
                          {p.unitMeasure
                            ? `${p.unitMeasure.code} - ${p.unitMeasure.name}`
                            : `ID ${p.unitMeasureId}`}
                        </td>
                        <td className="p-3 text-right whitespace-nowrap">
                          {`N ${Number(p.netWeight ?? 0).toFixed(2)} / B ${Number(
                            p.grossWeight ?? 0
                          ).toFixed(2)}`}
                        </td>
                        <td className="p-3 text-center">
                          {p.isDefault ? "Sí" : "—"}
                        </td>
                        <td className="p-3 text-right">
                          <Button
                            size="sm"
                            variant="outline"
                            disabled={!p.isActive}
                            onClick={() => onSelect(p)}
                          >
                            Seleccionar
                          </Button>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}
          </CardContent>
        </Card>
      </DialogContent>
    </Dialog>
  );
}

