import type { ProductPresentation } from "@/types/product.types";
import type { InvoiceProduct } from "@/types/invoice.type";

import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";

function getPresentations(product: InvoiceProduct | null): ProductPresentation[] {
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

interface InvoiceProductPresentationModalProps {
  open: boolean;
  onClose: () => void;
  product: InvoiceProduct | null;
  onSelect: (presentation: ProductPresentation, priceTier: 1 | 2 | 3 | 4) => void;
}

export default function InvoiceProductPresentationModal({
  open,
  onClose,
  product,
  onSelect,
}: InvoiceProductPresentationModalProps) {
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
                      <th className="p-3 font-medium text-right">Precios</th>
                      <th className="p-3 font-medium text-center">Default</th>
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
                        <td className="p-3">
                          <div className="flex justify-end gap-2 whitespace-nowrap">
                            <Button
                              size="sm"
                              variant="outline"
                              disabled={!p.isActive}
                              onClick={() => onSelect(p, 1)}
                            >
                              {`P1 $${Number(p.price01 ?? 0).toFixed(2)}`}
                            </Button>
                            <Button
                              size="sm"
                              variant="outline"
                              disabled={!p.isActive}
                              onClick={() => onSelect(p, 2)}
                            >
                              {`P2 $${Number(p.price02 ?? 0).toFixed(2)}`}
                            </Button>
                            <Button
                              size="sm"
                              variant="outline"
                              disabled={!p.isActive}
                              onClick={() => onSelect(p, 3)}
                            >
                              {`P3 $${Number(p.price03 ?? 0).toFixed(2)}`}
                            </Button>
                            <Button
                              size="sm"
                              variant="outline"
                              disabled={!p.isActive}
                              onClick={() => onSelect(p, 4)}
                            >
                              {`P4 $${Number(p.price04 ?? 0).toFixed(2)}`}
                            </Button>
                          </div>
                        </td>
                        <td className="p-3 text-center">
                          {p.isDefault ? "Sí" : "—"}
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
