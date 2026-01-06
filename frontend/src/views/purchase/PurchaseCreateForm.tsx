import React from "react";
import { Trash2Icon } from "lucide-react";

import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Button } from "@/components/ui/button";

import PurchaseSupplierModal from "./PurchaseSupplierModal";
import PurchaseProductModal from "./PurchaseProductModal";
import PurchaseProductPresentationModal from "./PurchaseProductPresentationModal";

import type {
  PurchaseProduct,
  PurchaseSupplier,
  PurchaseTotals,
} from "@/types/purchase.type";
import type { Product } from "@/types/product.types";
import type { ProductPresentation } from "@/types/product.types";
import { Input } from "@/components/ui/input";

interface PurchaseCreateFormProps {
  supplier: PurchaseSupplier | null;
  products: PurchaseProduct[];
  totals: PurchaseTotals;
  openSupplierModal: boolean;
  setOpenSupplierModal: React.Dispatch<React.SetStateAction<boolean>>;
  openProductModal: boolean;
  setOpenProductModal: React.Dispatch<React.SetStateAction<boolean>>;
  handleSelectSupplier: (supplier: PurchaseSupplier) => void;
  handleSelectProduct: (product: Product) => void;
  handleUnitCostChange: (productId: number, unitCost: number) => void;
  handleDiscountChange: (productId: number, discount: number) => void;
  handleWeightChange: (
    productId: number,
    field: "netWeight" | "grossWeight",
    value: number
  ) => void;
  handleRemoveProduct: (productId: number) => void;
  openPresentationModal: boolean;
  presentationProduct: PurchaseProduct | null;
  onOpenPresentationModal: (productId: number) => void;
  onClosePresentationModal: () => void;
  handleSelectPresentation: (presentation: ProductPresentation) => void;
  onSave: () => void;
  onCancel: () => void;
  saving: boolean;
  canProceed: boolean;
}

export default function PurchaseCreateForm({
  supplier,
  products,
  totals,
  openSupplierModal,
  setOpenSupplierModal,
  openProductModal,
  setOpenProductModal,
  handleSelectSupplier,
  handleSelectProduct,
  handleUnitCostChange,
  handleDiscountChange,
  handleWeightChange,
  handleRemoveProduct,
  openPresentationModal,
  presentationProduct,
  onOpenPresentationModal,
  onClosePresentationModal,
  handleSelectPresentation,
  onSave,
  onCancel,
  saving,
  canProceed,
}: PurchaseCreateFormProps) {
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
              <div className="min-w-[500px]">
                <table className="w-full border-collapse">
                  <thead className="sticky top-0 z-10 bg-background">
                    <tr className="border-b">
                      <th className="text-left py-2 px-2 font-semibold">
                        Producto
                      </th>
                      <th className="text-right py-2 px-2 font-semibold">
                        Costo Unit.
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
                      <th className="text-right py-2 px-2 font-semibold">
                        Total
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
                          colSpan={10}
                          className="text-center py-4 text-muted-foreground"
                        >
                          No hay productos agregados
                        </td>
                      </tr>
                    ) : (
                      products.map((p) => (
                        <tr key={p.id} className="border-b">
                          <td className="py-2 px-2">
                            <div className="max-w-[220px] leading-tight">
                              <div className="text-xs text-muted-foreground whitespace-nowrap">
                                {p.sku}
                              </div>
                              <div className="truncate">{p.name}</div>
                            </div>
                          </td>

                          <td className="py-2 px-2 text-right whitespace-nowrap">
                            <Input
                              type="number"
                              step="0.01"
                              min={0}
                              value={p.unitCost}
                              onChange={(e) =>
                                handleUnitCostChange(
                                  p.id,
                                  Number(e.target.value)
                                )
                              }
                              className="h-8 w-24 px-1 text-right text-sm"
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

                          <td className="py-2 px-2">
                            <div className="flex items-center gap-2 whitespace-nowrap">
                              <Input
                                type="number"
                                step="0.01"
                                min={0}
                                value={p.quantity}
                                readOnly
                                className="h-8 w-16 px-1 text-center text-sm"
                              />
                              <Button
                                type="button"
                                variant="outline"
                                size="sm"
                                className="h-8 px-2 text-xs"
                                onClick={() => onOpenPresentationModal(p.id)}
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
                            <Input
                              type="number"
                              step="0.01"
                              min={0}
                              value={p.discount}
                              onChange={(e) =>
                                handleDiscountChange(
                                  p.id,
                                  Number(e.target.value)
                                )
                              }
                              className="h-8 w-20 px-1 text-right text-sm"
                            />
                          </td>

                          <td className="py-2 px-2 text-right whitespace-nowrap">
                            ${p.subtotal.toFixed(2)}
                          </td>
                          <td className="py-2 px-2 text-right whitespace-nowrap">
                            ${p.taxValue.toFixed(2)}
                          </td>
                          <td className="py-2 px-2 text-right whitespace-nowrap">
                            ${(p.subtotal + p.taxValue).toFixed(2)}
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
            <CardTitle>PROVEEDOR</CardTitle>
          </CardHeader>
          <CardContent>
            {supplier ? (
              <div className="space-y-2">
                <p className="text-sm font-medium">{supplier.name}</p>
                <p className="text-xs text-muted-foreground">
                  {supplier.document}
                </p>
                <p className="text-xs text-muted-foreground">{supplier.email}</p>
                <Button
                  variant="outline"
                  type="button"
                  className="w-full mt-4"
                  onClick={() => setOpenSupplierModal(true)}
                >
                  Cambiar proveedor
                </Button>
              </div>
            ) : (
              <Button
                type="button"
                className="w-full"
                onClick={() => setOpenSupplierModal(true)}
              >
                Agregar Proveedor
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
              <Button type="button" variant="outline" onClick={onCancel}>
                Cancelar
              </Button>
              <Button
                type="button"
                onClick={onSave}
                disabled={saving || !canProceed}
              >
                {saving ? "Guardando..." : "Guardar compra"}
              </Button>
            </div>
          </CardContent>
        </Card>
      </div>

      <PurchaseSupplierModal
        open={openSupplierModal}
        onClose={() => setOpenSupplierModal(false)}
        onSelect={handleSelectSupplier}
      />

      <PurchaseProductModal
        open={openProductModal}
        onClose={() => setOpenProductModal(false)}
        onSelect={handleSelectProduct}
      />

      <PurchaseProductPresentationModal
        open={openPresentationModal}
        onClose={onClosePresentationModal}
        product={presentationProduct}
        onSelect={handleSelectPresentation}
      />
    </div>
  );
}
