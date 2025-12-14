import React from "react";
import { Trash2Icon } from "lucide-react";

import {
  Card,
  CardAction,
  CardContent,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Button } from "@/components/ui/button";

import PurchaseSupplierModal from "./PurchaseSupplierModal";
import PurchaseProductModal from "./PurchaseProductModal";

import type {
  PurchaseProduct,
  PurchaseSupplier,
  PurchaseTotals,
} from "@/types/purchase.type";
import type { Product } from "@/types/product.types";
import { Input } from "@/components/ui/input";

interface PurchaseCreateFormProps {
  supplier: PurchaseSupplier | null;
  products: PurchaseProduct[];
  totals: PurchaseTotals;
  purchaseDate?: Date;
  openSupplierModal: boolean;
  setOpenSupplierModal: React.Dispatch<React.SetStateAction<boolean>>;
  openProductModal: boolean;
  setOpenProductModal: React.Dispatch<React.SetStateAction<boolean>>;
  handleSelectSupplier: (supplier: PurchaseSupplier) => void;
  handleSelectProduct: (product: Product) => void;
  handleQuantityChange: (productId: number, qty: number) => void;
  handleRemoveProduct: (productId: number) => void;
}

export default function PurchaseCreateForm({
  supplier,
  products,
  totals,
  purchaseDate,
  openSupplierModal,
  setOpenSupplierModal,
  openProductModal,
  setOpenProductModal,
  handleSelectSupplier,
  handleSelectProduct,
  handleQuantityChange,
  handleRemoveProduct,
}: PurchaseCreateFormProps) {
  const formatDateTimeLocal = (date?: Date) => {
    if (!date) return "No disponible";

    return date.toLocaleString("es-EC");
  };

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
                    <th className="text-left py-2 px-2 font-semibold">Cant.</th>
                    <th className="text-right py-2 px-2 font-semibold">
                      Costo Unitario
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
                          <Input
                            type="number"
                            step="0.01"
                            min={1}
                            value={p.quantity}
                            onChange={(e) =>
                              handleQuantityChange(p.id, Number(e.target.value))
                            }
                            className="w-16 text-center"
                          />
                        </td>
                        <td className="py-2 px-2 text-right">
                          ${p.unitCost.toFixed(2)}
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
          <CardContent className="space-y-4">
            <div className="flex justify-between text-sm">
              <span>Fecha de compra</span>
              <span className="font-medium">
                {formatDateTimeLocal(purchaseDate)}
              </span>
            </div>
            <div className="space-y-2 text-sm">
              <div className="flex justify-between">
                <span>Subtotal</span>
                <span>${totals.subtotal.toFixed(2)}</span>
              </div>
              <div className="flex justify-between">
                <span>IVA</span>
                <span>${totals.tax.toFixed(2)}</span>
              </div>
              <div className="flex justify-between font-semibold">
                <span>Total</span>
                <span>${totals.total.toFixed(2)}</span>
              </div>
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
    </div>
  );
}
