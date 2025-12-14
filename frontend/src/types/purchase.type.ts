import z from "zod";

import type { Product } from "./product.types";

export type Purchase = {
  id: number;
  businessId: number;
  establishmentId: number;
  warehouseId: number;
  supplierId: number;
  purchaseDate: Date;
   documentNumber: string;
   reference: string;
  subtotalWithoutTaxes: number;
  subtotalWithTaxes: number;
  discountTotal: number;
  taxTotal: number;
  totalPurchase: number;
  status: string;
  details: PurchaseDetail[];
};

export type PurchaseDetail = {
  productId: number;
  warehouseId: number;
  taxId: number | null;
  quantity: number;
  unitCost: number;
  subtotal: number;
  taxRate: number;
  taxValue: number;
  total: number;
};

export interface PurchaseProduct extends Product {
  quantity: number;
  unitCost: number;
  subtotal: number;
  taxValue: number;
}

export interface PurchaseTotals {
  subtotal: number;
  tax: number;
  total: number;
}

export type PurchaseSupplier = {
  id: number;
  name: string;
  document: string;
  email: string;
};

export const purchaseDetailSchema = z.object({
  productId: z
    .number()
    .int("El ID del producto debe ser un entero")
    .positive("El ID del producto debe ser positivo"),
  quantity: z
    .number()
    .positive("La cantidad debe ser mayor a 0")
    .max(1000000, "La cantidad no puede ser mayor a 1,000,000"),
  unitCost: z.number().positive("El costo unitario debe ser mayor a 0"),
  warehouseId: z.number().int().positive(),
  taxId: z.number().int().min(0).nullable(),
});

export const createPurchaseSchema = z.object({
  supplierId: z.number().int().positive("Seleccione un proveedor"),
  purchaseDate: z.date(),
  documentNumber: z
    .string()
    .min(1, "Ingrese un número de documento")
    .max(50, "El número de documento es demasiado largo"),
  reference: z
    .string()
    .max(100, "La referencia es demasiado larga")
    .optional()
    .default(""),
  details: z
    .array(purchaseDetailSchema)
    .min(1, "Debe agregar al menos un detalle en la compra"),
});

export type CreatePurchaseForm = z.infer<typeof createPurchaseSchema>;
