import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

import type { Invoice } from "@/types/invoice.type";

interface InvoiceDetailInfoProps {
  invoice: Invoice;
}

export default function InvoiceDetailInfo({ invoice }: InvoiceDetailInfoProps) {
  return (
    <div className="flex flex-col md:flex-row gap-4">
      <div className="md:w-3/4">
        <Card className="h-full">
          <CardHeader>
            <CardTitle>PRODUCTOS AGREGADOS</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="max-h-[400px] overflow-auto rounded-md border">
              <div className="min-w-[750px]">
                <table className="w-full">
                  <thead className="sticky top-0 bg-background z-10">
                    <tr className="border-b">
                      <th className="text-left py-2 px-2 font-semibold">
                        Código
                      </th>
                      <th className="text-left py-2 px-2 font-semibold">
                        Nombre
                      </th>
                      <th className="text-left py-2 px-2 font-semibold">
                        Cant. / U.M.
                      </th>
                      <th className="text-right py-2 px-2 font-semibold">
                        P. Unitario
                      </th>
                      <th className="text-right py-2 px-2 font-semibold">
                        P. Neto
                      </th>
                      <th className="text-right py-2 px-2 font-semibold">
                        P. Bruto
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
                    {invoice.details.length === 0 ? (
                      <tr>
                        <td
                          colSpan={9}
                          className="text-center py-4 text-muted-foreground"
                        >
                          No hay productos
                        </td>
                      </tr>
                    ) : (
                      invoice.details.map((d) => (
                        <tr key={d.id} className="border-b last:border-b-0">
                          <td className="py-2 px-2 whitespace-nowrap">
                            {d.productCode}
                          </td>
                          <td className="py-2 px-2">{d.productName}</td>
                          <td className="py-2 px-2">
                            <div className="flex flex-col items-end whitespace-nowrap">
                              <span>{d.quantity.toFixed(2)}</span>
                              <span className="text-xs text-muted-foreground">
                                {d.unitMeasureCode} {d.unitMeasureName}
                              </span>
                            </div>
                          </td>
                          <td className="py-2 px-2 text-right whitespace-nowrap">
                            {d.unitPrice.toFixed(2)}
                          </td>
                          <td className="py-2 px-2 text-right whitespace-nowrap">
                            {Number(d.netWeight ?? 0).toFixed(2)}
                          </td>
                          <td className="py-2 px-2 text-right whitespace-nowrap">
                            {Number(d.grossWeight ?? 0).toFixed(2)}
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
            <CardTitle>CLIENTE</CardTitle>
          </CardHeader>
          <CardContent>
            {invoice.customer && (
              <div className="space-y-2">
                <p className="text-xs font-medium">{invoice.customer.name}</p>
                <p className="text-xs text-muted-foreground">
                  {invoice.customer.document}
                </p>
                <p className="text-xs text-muted-foreground">
                  {invoice.customer.email}
                </p>
              </div>
            )}
          </CardContent>
        </Card>
        <Card>
          <CardHeader>
            <CardTitle>RESUMEN</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2">
            <div className="flex justify-between text-sm">
              <span>Subtotal:</span>
              <span>${invoice.subtotalWithoutTaxes.toFixed(2)}</span>
            </div>
            <div className="flex justify-between text-sm">
              <span>Descuento:</span>
              <span>${invoice.discountTotal.toFixed(2)}</span>
            </div>
            <div className="flex justify-between text-sm">
              <span>IVA:</span>
              <span>${invoice.taxTotal.toFixed(2)}</span>
            </div>
            <div className="flex justify-between font-semibold text-base border-t pt-2">
              <span>Total:</span>
              <span>${invoice.totalInvoice.toFixed(2)}</span>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
