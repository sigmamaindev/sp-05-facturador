import React from "react";
import { Trash2Icon } from "lucide-react";

import type { InvoiceProduct, InvoiceTotals } from "@/types/invoice.type";
import type { Customer } from "@/types/customer.types";
import type { Product } from "@/types/product.types";
import type { UnitMeasure } from "@/types/unitMeasure.types";

import {
  Card,
  CardAction,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

import InvoiceCustomerModal from "./InvoiceCustomerModal";
import InvoiceProductModal from "./InvoiceProductModal";
import InvoiceUnitMeasureModal from "./InvoiceUnitMeasureModal";

interface InvoiceCreateFormProps {
  customer: Customer | null;
  products: InvoiceProduct[];
  totals: InvoiceTotals;
  openCustomerModal: boolean;
  setOpenCustomerModal: React.Dispatch<React.SetStateAction<boolean>>;
  openProductModal: boolean;
  setOpenProductModal: React.Dispatch<React.SetStateAction<boolean>>;
  handleSelectCustomer: (customer: Customer) => void;
  handleSelectProduct: (product: Product) => void;
  openUnitMeasureModal: boolean;
  onOpenUnitMeasureModal: (productId: number) => void;
  onCloseUnitMeasureModal: () => void;
  handleSelectUnitMeasure: (unitMeasure: UnitMeasure) => void;
  handleQuantityChange: (productId: number, qty: number) => void;
  handleRemoveProduct: (productId: number) => void;
  onSaveDraft: () => void;
  onContinue: () => void;
  savingDraft: boolean;
  savingAndContinuing: boolean;
}

export default function InvoiceCreateForm({
  customer,
  products,
  totals,
  openCustomerModal,
  setOpenCustomerModal,
  openProductModal,
  setOpenProductModal,
  handleSelectCustomer,
  handleSelectProduct,
  openUnitMeasureModal,
  onOpenUnitMeasureModal,
  onCloseUnitMeasureModal,
  handleSelectUnitMeasure,
  handleQuantityChange,
  handleRemoveProduct,
  onSaveDraft,
  onContinue,
  savingDraft,
  savingAndContinuing,
}: InvoiceCreateFormProps) {
  const canProceed = customer !== null && products.length > 0;

  return (
    <div className="flex flex-col md:flex-row gap-4">
      <div className="md:w-3/4">
        <Card className="h-full">
          <CardHeader>
            <CardTitle>PRODUCTOS AGREGADOS</CardTitle>
            <CardAction>
              <Button type="button" onClick={() => setOpenProductModal(true)}>
                Agregar Producto
              </Button>
            </CardAction>
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
                    <th className="text-right py-2 px-2 font-semibold">IVA</th>
                    <th className="text-center py-2 px-2 font-semibold">
                      Acciones
                    </th>
                  </tr>
                </thead>
                <tbody>
                  {products.length === 0 ? (
                    <tr>
                      <td
                        colSpan={9}
                        className="text-center py-4 text-muted-foreground"
                      >
                        No hay productos agregados
                      </td>
                    </tr>
                  ) : (
                    products.map((p) => (
                      <tr key={p.id} className="border-b">
                        <td className="py-2 px-2">{p.sku}</td>
                        <td className="py-2 px-2">{p.name}</td>
                        <td className="py-2 px-2">
                          <div className="flex items-center gap-2">
                            <Input
                              type="number"
                              step="0.01"
                              min={1}
                              value={p.quantity}
                              onChange={(e) =>
                                handleQuantityChange(
                                  p.id,
                                  Number(e.target.value)
                                )
                              }
                              className="w-20 text-center"
                            />
                            <Button
                              type="button"
                              variant="outline"
                              size="sm"
                              onClick={() => onOpenUnitMeasureModal(p.id)}
                            >
                              {p.unitMeasure?.code ?? "UND"}
                            </Button>
                          </div>
                          {p.unitMeasure?.name && (
                            <p className="text-xs text-muted-foreground mt-1">
                              {p.unitMeasure.name}
                            </p>
                          )}
                        </td>
                        <td className="py-2 px-2 text-right">
                          ${p.price.toFixed(2)}
                        </td>
                        <td className="py-2 px-2 text-right">
                          ${p.discount.toFixed(2)}
                        </td>
                        <td className="py-2 px-2 text-right">
                          ${p.subtotal.toFixed(2)}
                        </td>
                        <td className="py-2 px-2 text-right">
                          ${p.taxValue.toFixed(2)}
                        </td>
                        <td className="py-2 px-2 text-center">
                          <Button
                            variant="ghost"
                            size="icon"
                            onClick={() => handleRemoveProduct(p.id)}
                          >
                            <Trash2Icon className="h-4 w-4 text-red-500" />
                          </Button>
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
            {customer ? (
              <div className="space-y-2">
                <p className="text-sm font-medium">{customer.name}</p>
                <p className="text-xs text-muted-foreground">
                  {customer.document}
                </p>
                <p className="text-xs text-muted-foreground">
                  {customer.email}
                </p>
                <Button
                  variant="outline"
                  type="button"
                  className="w-full mt-4"
                  onClick={() => setOpenCustomerModal(true)}
                >
                  Cambiar cliente
                </Button>
              </div>
            ) : (
              <Button
                type="button"
                className="w-full"
                onClick={() => setOpenCustomerModal(true)}
              >
                Agregar Cliente
              </Button>
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
              <span>${totals.subtotal.toFixed(2)}</span>
            </div>
            <div className="flex justify-between text-sm">
              <span>Descuento:</span>
              <span>${totals.discount.toFixed(2)}</span>
            </div>
            <div className="flex justify-between text-sm">
              <span>IVA:</span>
              <span>${totals.tax.toFixed(2)}</span>
            </div>
            <div className="flex justify-between font-semibold text-base border-t pt-2">
              <span>Total:</span>
              <span>${totals.total.toFixed(2)}</span>
            </div>

            <div className="flex flex-col gap-2 mt-4">
              <Button
                type="button"
                variant="outline"
                onClick={onSaveDraft}
                disabled={savingDraft || !canProceed}
              >
                {savingDraft ? "Guardando borrador..." : "Guardar borrador"}
              </Button>
              <Button
                type="button"
                onClick={onContinue}
                disabled={savingAndContinuing || !canProceed}
              >
                {savingAndContinuing ? "Cargando pago..." : "Método de pago"}
              </Button>
            </div>
          </CardContent>
        </Card>
        <InvoiceCustomerModal
          open={openCustomerModal}
          onClose={() => setOpenCustomerModal(false)}
          onSelect={handleSelectCustomer}
        />
        <InvoiceProductModal
          open={openProductModal}
          onClose={() => setOpenProductModal(false)}
          onSelect={handleSelectProduct}
        />
        <InvoiceUnitMeasureModal
          open={openUnitMeasureModal}
          onClose={onCloseUnitMeasureModal}
          onSelect={handleSelectUnitMeasure}
        />
      </div>
    </div>
  );
}
