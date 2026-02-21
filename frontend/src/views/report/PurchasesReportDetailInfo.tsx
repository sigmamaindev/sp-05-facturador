import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";

import type { PurchasesReportDetail } from "@/types/report.types";

interface PurchasesReportDetailInfoProps {
  detail: PurchasesReportDetail;
}

export default function PurchasesReportDetailInfo({
  detail,
}: PurchasesReportDetailInfoProps) {
  const issueDate = new Date(detail.issueDate);

  const normalized = (detail.status ?? "").toUpperCase();
  const statusVariant = normalized.includes("ISSUED")
    ? "default"
    : normalized.includes("DRAFT")
    ? "secondary"
    : normalized.includes("RECHAZADO") || normalized.includes("CANCELED")
    ? "destructive"
    : "outline";

  return (
    <div className="flex flex-col md:flex-row gap-4">
      <div className="md:w-3/4">
        <Card className="h-full">
          <CardHeader>
            <CardTitle>PRODUCTOS</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="max-h-[400px] overflow-auto rounded-md border">
              <div className="min-w-[700px]">
                <table className="w-full">
                  <thead className="sticky top-0 bg-background z-10">
                    <tr className="border-b">
                      <th className="text-left py-2 px-2 font-semibold">
                        Producto
                      </th>
                      <th className="text-left py-2 px-2 font-semibold">
                        U.M.
                      </th>
                      <th className="text-left py-2 px-2 font-semibold">
                        Bodega
                      </th>
                      <th className="text-right py-2 px-2 font-semibold">
                        Cantidad
                      </th>
                      <th className="text-right py-2 px-2 font-semibold">
                        C. Unitario
                      </th>
                      <th className="text-right py-2 px-2 font-semibold">
                        Desc.
                      </th>
                      <th className="text-right py-2 px-2 font-semibold">
                        IVA
                      </th>
                      <th className="text-right py-2 px-2 font-semibold">
                        Subtotal
                      </th>
                      <th className="text-right py-2 px-2 font-semibold">
                        Total
                      </th>
                    </tr>
                  </thead>

                  <tbody>
                    {detail.items.length === 0 ? (
                      <tr>
                        <td
                          colSpan={9}
                          className="text-center py-4 text-muted-foreground"
                        >
                          No hay productos
                        </td>
                      </tr>
                    ) : (
                      detail.items.map((item) => (
                        <tr key={item.id} className="border-b last:border-b-0">
                          <td className="py-2 px-2">
                            <div className="max-w-[200px] leading-tight">
                              <div className="text-xs text-muted-foreground whitespace-nowrap">
                                {item.productCode}
                              </div>
                              <div className="truncate">{item.productName}</div>
                            </div>
                          </td>
                          <td className="py-2 px-2 text-sm text-muted-foreground whitespace-nowrap">
                            {item.unitMeasureName}
                          </td>
                          <td className="py-2 px-2 text-sm text-muted-foreground whitespace-nowrap">
                            {item.warehouseName}
                          </td>
                          <td className="py-2 px-2 text-right whitespace-nowrap">
                            {Number(item.quantity).toFixed(2)}
                          </td>
                          <td className="py-2 px-2 text-right whitespace-nowrap">
                            {Number(item.unitCost).toFixed(2)}
                          </td>
                          <td className="py-2 px-2 text-right whitespace-nowrap">
                            {Number(item.discount).toFixed(2)}
                          </td>
                          <td className="py-2 px-2 text-right whitespace-nowrap">
                            {Number(item.taxValue).toFixed(2)}
                          </td>
                          <td className="py-2 px-2 text-right whitespace-nowrap">
                            {Number(item.subtotal).toFixed(2)}
                          </td>
                          <td className="py-2 px-2 text-right whitespace-nowrap">
                            {Number(item.total).toFixed(2)}
                          </td>
                        </tr>
                      ))
                    )}
                  </tbody>
                </table>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      <div className="md:w-1/4 flex flex-col gap-4">
        <Card>
          <CardHeader>
            <CardTitle>DOCUMENTO</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2 text-sm">
            <div className="flex justify-between gap-2">
              <span>Fecha:</span>
              <span>
                {issueDate.toLocaleDateString("es-EC", {
                  year: "numeric",
                  month: "2-digit",
                  day: "2-digit",
                })}
              </span>
            </div>

            <div className="flex justify-between gap-2">
              <span>Comprobante:</span>
              <span className="font-semibold">{detail.sequential}</span>
            </div>

            <div className="flex justify-between items-center gap-2">
              <span>Estado:</span>
              <Badge variant={statusVariant}>{detail.status}</Badge>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>PROVEEDOR</CardTitle>
          </CardHeader>
          <CardContent className="space-y-1 text-sm">
            <p className="font-medium">{detail.supplierName}</p>
            <p className="text-muted-foreground">{detail.supplierDocument}</p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>COMPRADOR</CardTitle>
          </CardHeader>
          <CardContent className="text-sm">
            <p className="text-muted-foreground">{detail.userFullName}</p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>RESUMEN</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2">
            <div className="flex justify-between text-sm">
              <span>Subtotal 0%:</span>
              <span>${Number(detail.subtotalWithoutTaxes).toFixed(2)}</span>
            </div>
            <div className="flex justify-between text-sm">
              <span>Subtotal 12%:</span>
              <span>${Number(detail.subtotalWithTaxes).toFixed(2)}</span>
            </div>
            <div className="flex justify-between text-sm">
              <span>Descuento:</span>
              <span>${Number(detail.discountTotal).toFixed(2)}</span>
            </div>
            <div className="flex justify-between text-sm">
              <span>IVA:</span>
              <span>${Number(detail.taxTotal).toFixed(2)}</span>
            </div>
            <div className="flex justify-between font-semibold text-base border-t pt-2">
              <span>Total:</span>
              <span>${Number(detail.totalPurchase).toFixed(2)}</span>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
