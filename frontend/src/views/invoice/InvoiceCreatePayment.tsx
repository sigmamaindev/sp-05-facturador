import { useState } from "react";

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
import { ScrollArea } from "@/components/ui/scroll-area";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

const paymentOptions = [
  { value: "01", label: "Efectivo" },
  { value: "19", label: "Tarjeta de crédito" },
  { value: "20", label: "Tarjeta de débito" },
];

interface InvoiceCreatePaymentProps {
  customer: Customer | null;
  products: InvoiceProduct[];
  totals: InvoiceTotals;
  paymentMethod: string;
  paymentTermDays: number;
  onPaymentMethodChange: (value: string) => void;
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
    <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
      <Card className="md:col-span-2">
        <CardHeader>
          <CardTitle>Productos seleccionados</CardTitle>
        </CardHeader>
        <CardContent>
          <ScrollArea className="max-h-[420px]">
            <table className="w-full border-collapse">
              <thead>
                <tr className="border-b text-left">
                  <th className="py-2 px-2">Código</th>
                  <th className="py-2 px-2">Nombre</th>
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
                      colSpan={6}
                      className="py-4 text-center text-muted-foreground"
                    >
                      No hay productos agregados
                    </td>
                  </tr>
                ) : (
                  products.map((product) => (
                    <tr key={product.id} className="border-b">
                      <td className="py-2 px-2">{product.sku}</td>
                      <td className="py-2 px-2">{product.name}</td>
                      <td className="py-2 px-2">{product.quantity}</td>
                      <td className="py-2 px-2 text-right">
                        ${product.subtotal.toFixed(2)}
                      </td>
                      <td className="py-2 px-2 text-right">
                        ${product.taxValue.toFixed(2)}
                      </td>
                      <td className="py-2 px-2 text-right">
                        ${(product.subtotal + product.taxValue).toFixed(2)}
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </ScrollArea>
        </CardContent>
      </Card>

      <div className="flex flex-col gap-4">
        <Card>
          <CardHeader>
            <CardTitle>Datos del cliente</CardTitle>
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
            <CardTitle>Confirmar pago</CardTitle>
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
                  {paymentOptions.map((option) => (
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
                  Confirmar pago y marcar como pendiente
                </Button>
              </AlertDialogTrigger>
              <AlertDialogContent>
                <AlertDialogHeader>
                  <AlertDialogTitle>Confirmar envío</AlertDialogTitle>
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
                      {loading ? "Guardando pago..." : "Confirmar"}
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
