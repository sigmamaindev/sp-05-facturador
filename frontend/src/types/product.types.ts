import z from "zod";

import type { Inventory } from "./inventory.types";
import type { Tax } from "./tax.types";
import type { UnitMeasure } from "./unitMeasure.types";

export type Product = {
  id: number;
  sku: string;
  name: string;
  description?: string;
  price: number;
  iva: boolean;
  type?: "BIEN" | "SERVICIO" | "OTRO";
  isActive: boolean;
  tax: Tax;
  unitMeasure: UnitMeasure;
  inventory: Inventory[];
  createdAt?: Date;
};

export const createProductSchema = z.object({
  sku: z
    .string()
    .min(1, "El Código no puede estar vacío")
    .max(50, "El Código no puede tener más de 50 caracteres"),
  name: z
    .string()
    .min(2, "El nombre debe tener al menos 2 caracteres")
    .max(100, "El nombre no puede tener más de 100 caracteres"),
  description: z
    .string()
    .max(500, "La descripción no puede tener más de 500 caracteres")
    .optional(),
  price: z
    .number()
    .positive("El precio debe ser mayor a 0")
    .max(9999999, "El precio no puede superar los 9,999,999"),
  iva: z.boolean(),
  taxId: z
    .int()
    .int("El ID del impuesto debe ser un número entero")
    .positive("El ID del impuesto debe ser positivo"),
  unitMeasureId: z
    .int()
    .int("El ID de unidad de medida debe ser un número entero")
    .positive("El ID de unidad de medida debe ser positivo"),
  type: z.enum(["BIEN", "SERVICIO", "OTRO"], {
    error: () => ({
      message: "Debe escoger un tipo de producto",
    }),
  }),
});

export type CreateProductForm = z.infer<typeof createProductSchema>;

export const updateProductSchema = createProductSchema;

export type UpdateProductForm = z.infer<typeof updateProductSchema>;
