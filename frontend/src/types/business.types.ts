import z from "zod";

export interface Business {
  id: number;
  document: string;
  name: string;
  address: string;
  city: string;
  province: string;
  isActive: boolean;
  sriEnvironment: string;
  createdAt: Date;
}

export const updateBusinessSchema = z.object({
  document: z
    .string()
    .min(10, "El documento debe tener al menos 10 caracteres")
    .max(13, "El documento no debe exceder 13 caracteres"),
  name: z.string().min(3, "El nombre debe tener al menos 3 caracteres"),
  address: z.string().min(3, "La dirección debe tener al menos 3 caracteres"),
  city: z.string().min(2, "La ciudad debe tener al menos 2 caracteres"),
  province: z.string().min(2, "La provincia debe tener al menos 2 caracteres"),
  sriEnvironment: z.enum(["1", "2"], {
    error: () => ({ message: "Debe escoger un ambiente SRI válido" }),
  }),
});

export type UpdateBusinessForm = z.infer<typeof updateBusinessSchema>;
