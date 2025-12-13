import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Badge } from "@/components/ui/badge";

import type { Purchase } from "@/types/purchase.type";

interface PurchaseDetailInfoProps {
  purchase: Purchase;
}

export default function PurchaseDetailInfo({ purchase }: PurchaseDetailInfoProps) {
  return (
    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
      <Card>
        <CardHeader>
          <CardTitle>Resumen</CardTitle>
        </CardHeader>
        <CardContent className="space-y-2 text-sm">
          <div className="flex justify-between">
            <span>Fecha</span>
            <span>
              {new Date(purchase.purchaseDate).toLocaleString("es-EC", {
                year: "numeric",
                month: "2-digit",
                day: "2-digit",
              })}
            </span>
          </div>
          <div className="flex justify-between">
            <span>Documento</span>
            <span>{purchase.documentNumber}</span>
          </div>
          <div className="flex justify-between items-center">
            <span>Estado</span>
            <Badge variant="secondary">{purchase.status ?? "Pendiente"}</Badge>
          </div>
          <div className="flex justify-between">
            <span>Subtotal</span>
            <span>${purchase.subtotal.toFixed(2)}</span>
          </div>
          <div className="flex justify-between">
            <span>IVA</span>
            <span>${purchase.totalTax.toFixed(2)}</span>
          </div>
          <div className="flex justify-between font-semibold">
            <span>Total</span>
            <span>${purchase.total.toFixed(2)}</span>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Detalle de productos</CardTitle>
        </CardHeader>
        <CardContent>
          <ScrollArea className="max-h-[260px]">
            <table className="w-full border-collapse text-sm">
              <thead>
                <tr className="border-b">
                  <th className="text-left py-2 px-2">Producto</th>
                  <th className="text-right py-2 px-2">Cant.</th>
                  <th className="text-right py-2 px-2">Costo</th>
                  <th className="text-right py-2 px-2">Subtotal</th>
                </tr>
              </thead>
              <tbody>
                {purchase.details.map((detail, index) => (
                  <tr key={`${detail.productId}-${index}`} className="border-b">
                    <td className="py-2 px-2">{detail.productId}</td>
                    <td className="py-2 px-2 text-right">{detail.quantity}</td>
                    <td className="py-2 px-2 text-right">${detail.unitCost.toFixed(2)}</td>
                    <td className="py-2 px-2 text-right">${detail.subtotal.toFixed(2)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </ScrollArea>
        </CardContent>
      </Card>
    </div>
  );
}
