import { useState } from "react";
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
import { Input } from "@/components/ui/input";

import type { Customer } from "@/types/customer.types";
import type { Product } from "@/types/product.types";

import InvoiceCreateHeader from "./InvoiceCreateHeader";
import InvoiceCustomerModal from "./InvoiceCustomerModal";
import InvoiceProductModal from "./InvoiceProductModal";

export default function InvoiceCreateView() {
  const [openCustomerModal, setOpenCustomerModal] = useState(false);
  const [selectedCustomer, setSelectedCustomer] = useState<Customer | null>(
    null
  );
  const [openProductModal, setOpenProductModal] = useState(false);
  const [products, setProducts] = useState<
    (Product & {
      quantity: number;
      discount: number;
      subtotal: number;
      ivaValue: number;
    })[]
  >([]);

  const handleSelectCustomer = (customer: Customer) => {
    setSelectedCustomer(customer);
    setOpenCustomerModal(false);
  };

  const handleSelectProduct = (product: Product) => {
    setOpenProductModal(false);

    setProducts((prev) => {
      const exists = prev.find((p) => p.id === product.id);
      if (exists) return prev;

      const price = product.price ?? 0;
      const discount = 0;
      const base = price - discount;
      const ivaRate = product.tax.rate ?? 12;
      const ivaValue = base * (ivaRate / 100);

      const newProduct = {
        ...product,
        quantity: 1,
        discount,
        subtotal: base,
        ivaValue,
      };

      return [...prev, newProduct];
    });
  };

  const handleQuantityChange = (productId: number, newQty: number) => {
    setProducts((prev) =>
      prev.map((p) => {
        if (p.id === productId) {
          const base = (p.price - p.discount) * newQty;
          const ivaRate = p.tax.rate ?? 12;
          const ivaValue = base * (ivaRate / 100);
          return {
            ...p,
            quantity: newQty,
            subtotal: base,
            ivaValue,
          };
        }
        return p;
      })
    );
  };

  const handleRemoveProduct = (id: number) => {
    setProducts((prev) => prev.filter((p) => p.id !== id));
  };

  const calculateTotals = () => {
    const subtotal = products.reduce((sum, p) => sum + p.subtotal, 0);
    const discount = products.reduce((sum, p) => sum + p.discount, 0);
    const iva = products.reduce((sum, p) => sum + p.ivaValue, 0);
    const total = subtotal + iva;
    return { subtotal, discount, iva, total };
  };

  const totals = calculateTotals();

  return (
    <Card>
      <CardContent>
        <InvoiceCreateHeader />
        <div className="flex flex-col md:flex-row gap-4">
          <div className="md:w-3/4">
            <Card className="h-full">
              <CardHeader>
                <CardTitle>PRODUCTOS AGREGADOS</CardTitle>
                <CardAction>
                  <Button onClick={() => setOpenProductModal(true)}>
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
                          Cant.
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
                                min={1}
                                value={p.quantity}
                                onChange={(e) =>
                                  handleQuantityChange(
                                    p.id,
                                    Number(e.target.value)
                                  )
                                }
                                className="w-16 text-center"
                              />
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
                              ${p.ivaValue.toFixed(2)}
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
                <CardTitle>Cliente</CardTitle>
              </CardHeader>
              <CardContent>
                {selectedCustomer ? (
                  <div className="space-y-2">
                    <p className="text-sm font-medium">
                      {selectedCustomer.name}
                    </p>
                    <p className="text-xs text-muted-foreground">
                      {selectedCustomer.document}
                    </p>
                    <p className="text-xs text-muted-foreground">
                      {selectedCustomer.email}
                    </p>
                    <Button
                      variant="outline"
                      className="w-full mt-4"
                      onClick={() => setOpenCustomerModal(true)}
                    >
                      Cambiar cliente
                    </Button>
                  </div>
                ) : (
                  <Button
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
                <CardTitle>Resumen</CardTitle>
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
                  <span>IVA (15%):</span>
                  <span>${totals.iva.toFixed(2)}</span>
                </div>
                <div className="flex justify-between font-semibold text-base border-t pt-2">
                  <span>Total:</span>
                  <span>${totals.total.toFixed(2)}</span>
                </div>

                <div className="flex flex-col gap-2 mt-4">
                  <Button variant="secondary">Guardar</Button>
                  <Button>Facturar</Button>
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
          </div>
        </div>
      </CardContent>
    </Card>
  );
}
