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
  name: z.string().min(4, "El nombre debe tener al menos 4 caracteres"),
  address: z.string().min(4, "La direccion debe tener al menos 4 caracteres"),
});

export type createWarehouseForm = z.infer<typeof createWarehouseSchema>;

const updateWarehouseSchema = z.object({
  name: z.string().min(4, "El nombre debe tener al menos 4 caracteres"),
  address: z.string().min(4, "La direccion debe tener al menos 4 caracteres"),
});

export type updateWarehouseForm = z.infer<typeof updateWarehouseSchema>;
