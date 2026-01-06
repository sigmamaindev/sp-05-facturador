import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";

import type { Purchase } from "@/types/purchase.type";

interface PurchaseDetailInfoProps {
  purchase: Purchase;
}

export default function PurchaseDetailInfo({
  purchase,
}: PurchaseDetailInfoProps) {
  const issueDate = new Date(purchase.issueDate);
  const authorizationDate = purchase.authorizationDate
    ? new Date(purchase.authorizationDate)
    : null;

  const documentCode = `${purchase.establishmentCode}-${purchase.emissionPointCode}-${purchase.sequential}`;

  const normalizedStatus = (purchase.status ?? "").trim().toUpperCase();
  const statusVariant = normalizedStatus.includes("ISSUED")
    ? "default"
    : normalizedStatus.includes("DRAFT")
    ? "secondary"
    : normalizedStatus.includes("RECHAZADO") || normalizedStatus.includes("CANCELED")
    ? "destructive"
    : "outline";

  const details = purchase.details ?? [];

  return (
    <div className="flex flex-col md:flex-row gap-4">
      <div className="md:w-3/4">
        <Card className="h-full">
          <CardHeader>
            <CardTitle>PRODUCTOS AGREGADOS</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="max-h-[400px] overflow-auto rounded-md border">
              <div className="min-w-[650px]">
                <table className="w-full">
                  <thead className="sticky top-0 bg-background z-10">
                    <tr className="border-b">
                      <th className="text-left py-2 px-2 font-semibold">
                        Producto
                      </th>
                      <th className="text-right py-2 px-2 font-semibold">
                        C. Unitario
                      </th>
                      <th className="text-right py-2 px-2 font-semibold">
                        P. Bruto
                      </th>
                      <th className="text-right py-2 px-2 font-semibold">
                        Tara
                      </th>
                      <th className="text-left py-2 px-2 font-semibold">
                        Cant. / U.M.
                      </th>
                      <th className="text-right py-2 px-2 font-semibold">
                        Desc.
                      </th>
                      <th className="text-right py-2 px-2 font-semibold">
                        Base IVA
                      </th>
                      <th className="text-right py-2 px-2 font-semibold">
                        IVA
                      </th>
                    </tr>
                  </thead>

                  <tbody>
                    {details.length === 0 ? (
                      <tr>
                        <td
                          colSpan={8}
                          className="text-center py-4 text-muted-foreground"
                        >
                          No hay productos
                        </td>
                      </tr>
                    ) : (
                      details.map((d) => {
                        const netWeight = Number(d.netWeight ?? 0);
                        const grossWeight = Number(d.grossWeight ?? 0);
                        const calculatedQuantity = Number(
                          (grossWeight - netWeight).toFixed(2)
                        );
                        const shouldUseCalculated =
                          netWeight !== 0 || grossWeight !== 0;
                        const displayQuantity = shouldUseCalculated
                          ? calculatedQuantity
                          : d.quantity;

                        return (
                          <tr
                            key={d.id}
                            className="border-b last:border-b-0"
                          >
                            <td className="py-2 px-2">
                              <div className="max-w-[220px] leading-tight">
                                <div className="text-xs text-muted-foreground whitespace-nowrap">
                                  {d.productCode}
                                </div>
                                <div className="truncate">{d.productName}</div>
                              </div>
                            </td>
                            <td className="py-2 px-2 text-right whitespace-nowrap">
                              {d.unitCost.toFixed(2)}
                            </td>
                            <td className="py-2 px-2 text-right whitespace-nowrap">
                              {grossWeight.toFixed(2)}
                            </td>
                            <td className="py-2 px-2 text-right whitespace-nowrap">
                              {netWeight.toFixed(2)}
                            </td>
                            <td className="py-2 px-2">
                              <div className="flex flex-col items-end whitespace-nowrap">
                                <span>{displayQuantity.toFixed(2)}</span>
                                <span className="text-xs text-muted-foreground">
                                  {d.unitMeasureCode} {d.unitMeasureName}
                                </span>
                              </div>
                            </td>
                            <td className="py-2 px-2 text-right whitespace-nowrap">
                              {d.discount.toFixed(2)}
                            </td>
                            <td className="py-2 px-2 text-right whitespace-nowrap">
                              {d.subtotal.toFixed(2)}
                            </td>
                            <td className="py-2 px-2 text-right whitespace-nowrap">
                              {d.taxValue.toFixed(2)}
                            </td>
                          </tr>
                        );
                      })
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
            <div className="flex justify-between">
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
              <span>Código:</span>
              <span className="text-right whitespace-nowrap">{documentCode}</span>
            </div>

            <div className="flex justify-between items-center">
              <span>Estado:</span>
              <Badge variant={statusVariant}>{purchase.status}</Badge>
            </div>

            <div className="flex justify-between gap-2">
              <span>Ambiente:</span>
              <span className="text-right">{purchase.environment}</span>
            </div>

            <div className="flex justify-between gap-2">
              <span>Tipo emisión:</span>
              <span className="text-right">{purchase.emissionTypeCode}</span>
            </div>

            <div className="flex justify-between gap-2">
              <span>Comprobante:</span>
              <span className="text-right">{purchase.receiptType}</span>
            </div>

            <div className="flex justify-between gap-2">
              <span>Periodo fiscal:</span>
              <span className="text-right">{purchase.fiscalPeriod}</span>
            </div>

            <div className="flex justify-between items-center gap-2">
              <span>Electrónico:</span>
              <Badge variant="secondary">
                {purchase.isElectronic ? "Sí" : "No"}
              </Badge>
            </div>

            {purchase.isElectronic ? (
              <div className="space-y-2 border-t pt-3">
                <div className="flex justify-between gap-2">
                  <span>Autorización:</span>
                  <span className="text-right break-all">
                    {purchase.authorizationNumber ?? "—"}
                  </span>
                </div>

                <div className="flex justify-between gap-2">
                  <span>Fecha aut.:</span>
                  <span className="text-right">
                    {authorizationDate
                      ? authorizationDate.toLocaleDateString("es-EC", {
                          year: "numeric",
                          month: "2-digit",
                          day: "2-digit",
                        })
                      : "—"}
                  </span>
                </div>

                <div className="flex flex-col gap-1">
                  <span>Clave acceso:</span>
                  <span className="text-right break-all text-xs text-muted-foreground">
                    {purchase.accessKey}
                  </span>
                </div>
              </div>
            ) : null}
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>PROVEEDOR</CardTitle>
          </CardHeader>
          <CardContent>
            {purchase.supplier ? (
              <div className="space-y-2">
                <p className="text-xs font-medium">
                  {purchase.supplier.businessName}
                </p>
                <p className="text-xs text-muted-foreground">
                  {purchase.supplier.document}
                </p>
                <p className="text-xs text-muted-foreground">
                  {purchase.supplier.email}
                </p>
                <p className="text-xs text-muted-foreground">
                  {purchase.supplier.address}
                </p>
                {purchase.supplier.cellphone ? (
                  <p className="text-xs text-muted-foreground">
                    {purchase.supplier.cellphone}
                  </p>
                ) : null}
                {purchase.supplier.telephone ? (
                  <p className="text-xs text-muted-foreground">
                    {purchase.supplier.telephone}
                  </p>
                ) : null}
              </div>
            ) : (
              <div className="text-xs text-muted-foreground">
                Proveedor #{purchase.supplierId}
              </div>
            )}
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>DIRECCIONES</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2 text-xs text-muted-foreground">
            <div className="flex justify-between gap-2">
              <span>Matriz:</span>
              <span className="text-right">{purchase.mainAddress}</span>
            </div>
            <div className="flex justify-between gap-2">
              <span>Establecimiento:</span>
              <span className="text-right">
                {purchase.establishmentAddress ?? "—"}
              </span>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>RESUMEN</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2">
            <div className="flex justify-between text-sm">
              <span>Subtotal:</span>
              <span>${purchase.subtotalWithoutTaxes.toFixed(2)}</span>
            </div>
            <div className="flex justify-between text-sm">
              <span>Descuento:</span>
              <span>${purchase.discountTotal.toFixed(2)}</span>
            </div>
            <div className="flex justify-between text-sm">
              <span>IVA:</span>
              <span>${purchase.taxTotal.toFixed(2)}</span>
            </div>
            <div className="flex justify-between font-semibold text-base border-t pt-2">
              <span>Total:</span>
              <span>${purchase.totalPurchase.toFixed(2)}</span>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
