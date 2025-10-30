import z from "zod";

export interface Establishment {
  id: number;
  name: string;
  code: string;
  isActive: boolean;
  createdAt: Date;
}

const createEstablishmentSchema = z.object({
  name: z.string().min(4, "El nombre debe tener al menos 4 caracteres"),
});

export type CreateEstablishmentForm = z.infer<typeof createEstablishmentSchema>;
