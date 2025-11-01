import z from "zod";

export interface EmissionPoint {
  id: number;
  code: string;
  description: string;
  isActive: boolean;
  createdAt: Date;
}

const createEmissioPointSchema = z.object({
  description: z.string().min(4, "La descripción debe tener al menos 4 caracteres"),
});

export type CreateEmissionPointForm = z.infer<typeof createEmissioPointSchema>;

const updateEmissioPointSchema = z.object({
  description: z.string().min(4, "La descripción debe tener al menos 4 caracteres"),
});

export type UpdateEmissionPointForm = z.infer<typeof updateEmissioPointSchema>;
