import z from "zod";

export type Inventory = {
  id: number;
  warehouseId: number;
  warehouseCode: string;
  warehouseName: string;
  stock: number;
  minStock: number;
  maxStock: number;
  productId?: number;
};

export const inventoryItemSchema = z.object({
  warehouseId: z.number().int().positive("ID de bodega inválido"),
  stock: z
    .number()
    .positive("La cantidad debe ser mayor a 0")
    .max(1000000, "La cantidad no debe pasar los 1.000.000"),
});

export const createInventorySchema = z.object({
  inventories: z
    .array(inventoryItemSchema)
    .nonempty("Debe ingresar al menos una bodega con su cantidad"),
});

export type CreateInventoryForm = z.infer<typeof createInventorySchema>;

export const updateInventorySchema = z.object({
  warehouseId: z.number().int().positive("ID de bodega inválido"),
  stock: z
    .number()
    .min(0, "La cantidad debe ser mayor o igual a 0")
    .max(1000000, "La cantidad no debe pasar los 1.000.000"),
  minStock: z
    .number()
    .min(0, "El mínimo debe ser mayor o igual a 0")
    .max(1000000, "El mínimo no debe pasar los 1.000.000")
    .default(0),
  maxStock: z
    .number()
    .min(0, "El máximo debe ser mayor o igual a 0")
    .max(1000000, "El máximo no debe pasar los 1.000.000")
    .default(0),
});

export type UpdateInventoryForm = z.infer<typeof updateInventorySchema>;
