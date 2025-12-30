import z from "zod";

import type { Inventory } from "./inventory.types";
import type { Tax } from "./tax.types";
import type { UnitMeasure } from "./unitMeasure.types";

export type ProductPresentation = {
  id: number;
  unitMeasureId: number;
  unitMeasure: UnitMeasure;
  price01: number;
  price02: number;
  price03: number;
  price04: number;
  netWeight: number;
  grossWeight: number;
  isDefault: boolean;
  isActive: boolean;
};

export type Product = {
  id: number;
  sku: string;
  name: string;
  description?: string;
  price?: number;
  iva?: boolean;
  type?: "BIEN" | "SERVICIO" | "OTRO";
  isActive: boolean;
  tax?: Tax;
  unitMeasure?: UnitMeasure;
  defaultPresentation?: ProductPresentation | null;
  presentations?: ProductPresentation[];
  inventory: Inventory[];
  createdAt?: Date;
};

const productPresentationInputSchema = z.object({
  id: z.number().int().positive().optional(),
  unitMeasureId: z
    .number()
    .int("El ID de unidad de medida debe ser un número entero")
    .positive("El ID de unidad de medida debe ser positivo"),
  price01: z
    .number()
    .min(0, "El precio debe ser mayor o igual a 0")
    .max(9999999, "El precio no puede superar los 9,999,999"),
  price02: z
    .number()
    .min(0, "El precio debe ser mayor o igual a 0")
    .max(9999999, "El precio no puede superar los 9,999,999")
    .default(0),
  price03: z
    .number()
    .min(0, "El precio debe ser mayor o igual a 0")
    .max(9999999, "El precio no puede superar los 9,999,999")
    .default(0),
  price04: z
    .number()
    .min(0, "El precio debe ser mayor o igual a 0")
    .max(9999999, "El precio no puede superar los 9,999,999")
    .default(0),
  netWeight: z
    .number()
    .min(0, "El peso neto debe ser mayor o igual a 0")
    .max(9999999, "El peso neto no puede superar los 9,999,999")
    .default(0),
  grossWeight: z
    .number()
    .min(0, "El peso bruto debe ser mayor o igual a 0")
    .max(9999999, "El peso bruto no puede superar los 9,999,999")
    .default(0),
  isActive: z.boolean().default(true),
  isDefault: z.boolean().optional(),
});

export type ProductPresentationInput = z.infer<
  typeof productPresentationInputSchema
>;

export const createProductSchema = z.object({
  sku: z
    .string()
    .min(1, "El Código no puede estar vacío")
    .max(50, "El Código no puede tener más de 50 caracteres")
    .transform((v) => v.toUpperCase()),
  name: z
    .string()
    .min(2, "El nombre debe tener al menos 2 caracteres")
    .max(100, "El nombre no puede tener más de 100 caracteres")
    .transform((v) => v.toUpperCase()),
  description: z
    .string()
    .max(500, "La descripción no puede tener más de 500 caracteres")
    .optional(),
  taxId: z
    .number()
    .int("El ID del impuesto debe ser un número entero")
    .positive("El ID del impuesto debe ser positivo"),
  type: z.enum(["BIEN", "SERVICIO", "OTRO"], {
    error: () => ({
      message: "Debe escoger un tipo de producto",
    }),
  }),
  defaultPresentation: productPresentationInputSchema.omit({
    id: true,
    isDefault: true,
  }),
  presentations: z
    .array(
      productPresentationInputSchema.omit({
        id: true,
      })
    )
    .optional()
    .default([]),
});

export type CreateProductForm = z.infer<typeof createProductSchema>;

export const updateProductSchema = createProductSchema
  .omit({ sku: true })
  .extend({
    presentations: z
      .array(productPresentationInputSchema)
      .optional()
      .default([]),
  });

export type UpdateProductForm = z.infer<typeof updateProductSchema>;
