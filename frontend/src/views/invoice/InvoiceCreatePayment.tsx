import { useState } from "react";

import { PAYMENT_METHOD_OPTIONS } from "@/constants/paymentMethods";
import type { PaymentMethodCode } from "@/constants/paymentMethods";

import type { Customer } from "@/types/customer.types";
import type { InvoiceProduct, InvoiceTotals } from "@/types/invoice.type";

import { Button } from "@/components/ui/button";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

interface InvoiceCreatePaymentProps {
  customer: Customer | null;
  products: InvoiceProduct[];
  totals: InvoiceTotals;
  paymentMethod: PaymentMethodCode;
  paymentTermDays: number;
  onPaymentMethodChange: (value: PaymentMethodCode) => void;
  onPaymentTermChange: (value: number) => void;
  onConfirmPayment: () => void;
  loading: boolean;
  sequential?: string;
}

export default function InvoiceCreatePayment({
  customer,
  products,
  totals,
  paymentMethod,
  paymentTermDays,
  onPaymentMethodChange,
  onPaymentTermChange,
  onConfirmPayment,
  loading,
  sequential,
}: InvoiceCreatePaymentProps) {
  const [confirmOpen, setConfirmOpen] = useState(false);

  return (
    <div className="flex flex-col md:flex-row gap-4">
      <div className="md:w-3/4">
        <Card className="md:col-span-2">
          <CardHeader>
            <CardTitle>Productos seleccionados</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="max-h-[400px] overflow-auto rounded-md border">
              <div className="min-w-[500px]">
                <table className="w-full border-collapse">
                  <thead className="sticky top-0 z-10 bg-background">
                    <tr className="border-b text-left">
                      <th className="py-2 px-2">Código</th>
                      <th className="py-2 px-2">Nombre</th>
                      <th className="py-2 px-2 text-right">P. Neto</th>
                      <th className="py-2 px-2 text-right">P. Bruto</th>
                      <th className="py-2 px-2">Cant.</th>
                      <th className="py-2 px-2 text-right">Base IVA</th>
                      <th className="py-2 px-2 text-right">IVA</th>
                      <th className="py-2 px-2 text-right">Total</th>
                    </tr>
                  </thead>

                  <tbody>
                    {products.length === 0 ? (
                      <tr>
                        <td
                          colSpan={8}
                          className="py-4 text-center text-muted-foreground"
                        >
                          No hay productos agregados
                        </td>
                      </tr>
                    ) : (
                      products.map((product) => (
                        <tr
                          key={product.id}
                          className="border-b last:border-b-0"
                        >
                          <td className="py-2 px-2 whitespace-nowrap">
                            {product.sku}
                          </td>
                          <td className="py-2 px-2">{product.name}</td>
                          <td className="py-2 px-2 text-right whitespace-nowrap">
                            {Number(product.netWeight ?? 0).toFixed(2)}
                          </td>
                          <td className="py-2 px-2 text-right whitespace-nowrap">
                            {Number(product.grossWeight ?? 0).toFixed(2)}
                          </td>
                          <td className="py-2 px-2 whitespace-nowrap">
                            {product.quantity}
                          </td>
                          <td className="py-2 px-2 text-right whitespace-nowrap">
                            ${product.subtotal.toFixed(2)}
                          </td>
                          <td className="py-2 px-2 text-right whitespace-nowrap">
                            ${product.taxValue.toFixed(2)}
                          </td>
                          <td className="py-2 px-2 text-right whitespace-nowrap">
                            ${(product.subtotal + product.taxValue).toFixed(2)}
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
          <CardContent className="space-y-1 text-sm">
            <p className="font-semibold">{customer?.name}</p>
            <p className="text-muted-foreground">{customer?.document}</p>
            <p className="text-muted-foreground">{customer?.email}</p>
            {sequential && (
              <p className="text-xs text-muted-foreground">
                Borrador #{sequential}
              </p>
            )}
          </CardContent>
        </Card>
        <Card>
          <CardHeader>
            <CardTitle>CONFIRMAR PAGO</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="space-y-2">
              <Label>Método de pago</Label>
              <Select
                value={paymentMethod}
                onValueChange={onPaymentMethodChange}
              >
                <SelectTrigger className="w-full">
                  <SelectValue placeholder="Seleccionar método" />
                </SelectTrigger>
                <SelectContent>
                  {PAYMENT_METHOD_OPTIONS.map((option) => (
                    <SelectItem key={option.value} value={option.value}>
                      {option.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            <div className="space-y-2">
              <Label>Plazo (días)</Label>
              <Input
                type="number"
                min={0}
                value={paymentTermDays}
                onChange={(e) => onPaymentTermChange(Number(e.target.value))}
              />
            </div>

            <div className="space-y-1 text-sm">
              <div className="flex justify-between">
                <span>Subtotal</span>
                <span>${totals.subtotal.toFixed(2)}</span>
              </div>
              <div className="flex justify-between">
                <span>Descuento</span>
                <span>${totals.discount.toFixed(2)}</span>
              </div>
              <div className="flex justify-between">
                <span>IVA</span>
                <span>${totals.tax.toFixed(2)}</span>
              </div>
              <div className="flex justify-between font-semibold border-t pt-2">
                <span>Total a pagar</span>
                <span>${totals.total.toFixed(2)}</span>
              </div>
            </div>

            <AlertDialog open={confirmOpen} onOpenChange={setConfirmOpen}>
              <AlertDialogTrigger asChild>
                <Button
                  className="w-full"
                  disabled={loading || totals.total <= 0}
                >
                  Confirmar pago
                </Button>
              </AlertDialogTrigger>
              <AlertDialogContent>
                <AlertDialogHeader>
                  <AlertDialogTitle>Confirmar pago</AlertDialogTitle>
                  <AlertDialogDescription>
                    Se actualizará el pago y la factura pasará de borrador a
                    pendiente para ser enviada al SRI. ¿Desea continuar?
                  </AlertDialogDescription>
                </AlertDialogHeader>
                <AlertDialogFooter>
                  <AlertDialogCancel>Cancelar</AlertDialogCancel>
                  <AlertDialogAction asChild>
                    <Button
                      onClick={() => {
                        setConfirmOpen(false);
                        onConfirmPayment();
                      }}
                      disabled={loading}
                    >
                      {loading ? "Guardando pago..." : "Pagar"}
                    </Button>
                  </AlertDialogAction>
                </AlertDialogFooter>
              </AlertDialogContent>
            </AlertDialog>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
