import z from "zod";

export interface Warehouse {
  id: number;
  code: string;
  name: string;
  address: string;
  isActive: boolean;
  isMain: boolean;
}

const createWarehouseSchema = z.object({
  name: z
    .string()
    .min(4, "El nombre debe tener al menos 4 caracteres")
    .transform((v) => v.toUpperCase()),
  address: z
    .string()
    .min(4, "La direccion debe tener al menos 4 caracteres")
    .transform((v) => v.toUpperCase()),
});

export type CreateWarehouseForm = z.infer<typeof createWarehouseSchema>;

const updateWarehouseSchema = z.object({
  name: z
    .string()
    .min(4, "El nombre debe tener al menos 4 caracteres")
    .transform((v) => v.toUpperCase()),
  address: z
    .string()
    .min(4, "La direccion debe tener al menos 4 caracteres")
    .transform((v) => v.toUpperCase()),
});

export type UpdateWarehouseForm = z.infer<typeof updateWarehouseSchema>;
