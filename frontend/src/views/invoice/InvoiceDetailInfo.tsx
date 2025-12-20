import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { ScrollArea } from "@/components/ui/scroll-area";

import type { Invoice } from "@/types/invoice.type";

interface InvoiceDetailInfoProps {
  invoice: Invoice;
}

export default function InvoiceDetailInfo({ invoice }: InvoiceDetailInfoProps) {
  return (
    <div className="flex flex-col md:flex-row gap-4">
      <>
        <div className="md:w-3/4">
          <Card className="h-full">
            <CardHeader>
              <CardTitle>PRODUCTOS AGREGADOS</CardTitle>
            </CardHeader>
            <CardContent>
              <ScrollArea className="max-h-[400px]">
                <table className="w-full border-collapse">
                  <thead>
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
                        <tr key={d.id}>
                          <td className="py-2 px-2">{d.productCode}</td>
                          <td className="py-2 px-2">{d.productName}</td>
                          <td className="py-2 px-2">
                            <div className="flex flex-col items-end">
                              <span>{d.quantity.toFixed(2)}</span>
                              <span className="text-xs text-muted-foreground">
                                {d.unitMeasureCode} {d.unitMeasureName}
                              </span>
                            </div>
                          </td>
                          <td className="py-2 px-2 text-right">
                            {d.unitPrice.toFixed(2)}
                          </td>
                          <td className="py-2 px-2 text-right">
                            {d.discount.toFixed(2)}
                          </td>
                          <td className="py-2 px-2 text-right">
                            {d.subtotal.toFixed(2)}
                          </td>
                          <td className="py-2 px-2 text-right">
                            {d.taxValue.toFixed(2)}
                          </td>
                        </tr>
                      ))
                    )}
                  </tbody>
                </table>
              </ScrollArea>
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
              {invoice && (
                <>
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
                </>
              )}
            </CardContent>
          </Card>
        </div>
      </>
    </div>
  );
}
