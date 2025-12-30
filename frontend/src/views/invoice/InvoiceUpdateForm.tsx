import React from "react";
import { Trash2Icon } from "lucide-react";

import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Button } from "@/components/ui/button";

import InvoiceCustomerModal from "./InvoiceCustomerModal";
import InvoiceProductModal from "./InvoiceProductModal";
import InvoiceProductPresentationModal from "./InvoiceProductPresentationModal";

import type { InvoiceProduct, InvoiceTotals } from "@/types/invoice.type";
import type { Customer } from "@/types/customer.types";
import type { Product, ProductPresentation } from "@/types/product.types";
import { Input } from "@/components/ui/input";

interface InvoiceUpdateFormProps {
  customer: Customer | null;
  products: InvoiceProduct[];
  totals: InvoiceTotals;
  invoiceDate?: Date;
  dueDate?: Date;
  openCustomerModal: boolean;
  setOpenCustomerModal: React.Dispatch<React.SetStateAction<boolean>>;
  openProductModal: boolean;
  setOpenProductModal: React.Dispatch<React.SetStateAction<boolean>>;
  handleSelectCustomer: (customer: Customer) => void;
  handleSelectProduct: (product: Product) => void;
  openPresentationModal: boolean;
  presentationProduct: InvoiceProduct | null;
  onOpenPresentationModal: (productId: number) => void;
  onClosePresentationModal: () => void;
  handleSelectPresentation: (
    presentation: ProductPresentation,
    priceTier: 1 | 2 | 3 | 4
  ) => void;
  handleWeightChange: (
    productId: number,
    field: "netWeight" | "grossWeight",
    value: number
  ) => void;
  handleQuantityChange: (productId: number, qty: number) => void;
  handleRemoveProduct: (productId: number) => void;
  onSaveDraft: () => void;
  onContinue: () => void;
  savingDraft: boolean;
  savingAndContinuing: boolean;
}

export default function InvoiceUpdateForm({
  customer,
  products,
  totals,
  openCustomerModal,
  setOpenCustomerModal,
  openProductModal,
  setOpenProductModal,
  handleSelectCustomer,
  handleSelectProduct,
  openPresentationModal,
  presentationProduct,
  onOpenPresentationModal,
  onClosePresentationModal,
  handleSelectPresentation,
  handleWeightChange,
  handleQuantityChange,
  handleRemoveProduct,
  onSaveDraft,
  onContinue,
  savingDraft,
  savingAndContinuing,
}: InvoiceUpdateFormProps) {
  const canProceed = customer !== null && products.length > 0;

  return (
    <div className="flex flex-col md:flex-row gap-4">
      <div className="md:w-3/4">
        <Card className="h-full">
          <CardHeader className="gap-2">
            <div className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
              <CardTitle>PRODUCTOS AGREGADOS</CardTitle>

              <Button
                type="button"
                onClick={() => setOpenProductModal(true)}
                size="sm"
                className="h-8 px-3 text-xs sm:text-sm w-full sm:w-auto"
              >
                Agregar Producto
              </Button>
            </div>
          </CardHeader>

          <CardContent>
            <div className="max-h-[400px] overflow-auto rounded-md border">
              <div className="min-w-[600px]">
                <table className="w-full border-collapse">
                  <thead className="sticky top-0 z-10 bg-background">
                    <tr className="border-b">
                      <th className="text-left py-2 px-2 font-semibold">
                        Producto
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
                        <tr key={p.id} className="border-b last:border-b-0">
                          <td className="py-2 px-2">
                            <div className="max-w-[220px] leading-tight">
                              <div className="text-xs text-muted-foreground whitespace-nowrap">
                                {p.sku}
                              </div>
                              <div className="truncate">{p.name}</div>
                            </div>
                          </td>

                          <td className="py-2 px-2">
                            <div className="flex items-center gap-2 whitespace-nowrap">
                              <Input
                                type="number"
                                min={1}
                                value={p.quantity}
                                onChange={(e) =>
                                  handleQuantityChange(
                                    p.id,
                                    Number(e.target.value)
                                  )
                                }
                                className="h-8 w-16 px-1 text-center text-sm"
                              />

                              <Button
                                type="button"
                                variant="outline"
                                size="sm"
                                onClick={() => onOpenPresentationModal(p.id)}
                                className="h-8 px-2 text-xs"
                              >
                                {p.unitMeasure?.code ?? "UND"}
                              </Button>
                            </div>

                            {p.unitMeasure?.name && (
                              <p className="mt-1 text-xs text-muted-foreground">
                                {p.unitMeasure.name}
                              </p>
                            )}
                          </td>

	                          <td className="py-2 px-2 text-right whitespace-nowrap">
	                            ${p.price.toFixed(2)}
	                          </td>
		                          <td className="py-2 px-2 text-right whitespace-nowrap">
		                            <Input
		                              type="number"
		                              step="0.01"
		                              min={0}
		                              value={p.netWeight ?? 0}
	                              onChange={(e) =>
	                                handleWeightChange(
	                                  p.id,
	                                  "netWeight",
		                                  Number(e.target.value)
		                                )
		                              }
		                              className="h-8 w-20 px-1 text-right text-sm"
		                            />
		                          </td>
		                          <td className="py-2 px-2 text-right whitespace-nowrap">
		                            <Input
		                              type="number"
		                              step="0.01"
		                              min={0}
		                              value={p.grossWeight ?? 0}
	                              onChange={(e) =>
	                                handleWeightChange(
	                                  p.id,
	                                  "grossWeight",
		                                  Number(e.target.value)
		                                )
		                              }
		                              className="h-8 w-20 px-1 text-right text-sm"
		                            />
		                          </td>
	                          <td className="py-2 px-2 text-right whitespace-nowrap">
	                            ${p.discount.toFixed(2)}
	                          </td>
                          <td className="py-2 px-2 text-right whitespace-nowrap">
                            ${p.subtotal.toFixed(2)}
                          </td>
                          <td className="py-2 px-2 text-right whitespace-nowrap">
                            ${p.taxValue.toFixed(2)}
                          </td>

                          <td className="py-2 px-2 text-center">
                            <Button
                              variant="ghost"
                              size="icon"
                              onClick={() => handleRemoveProduct(p.id)}
                              className="h-8 w-8"
                            >
                              <Trash2Icon className="h-4 w-4 text-red-500" />
                            </Button>
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
                {savingDraft
                  ? "Actualizando borrador..."
                  : "Actualizar borrador"}
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
        <InvoiceProductPresentationModal
          open={openPresentationModal}
          onClose={onClosePresentationModal}
          product={presentationProduct}
          onSelect={handleSelectPresentation}
        />
      </div>
    </div>
  );
}
