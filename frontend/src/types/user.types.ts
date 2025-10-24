import z from "zod";

import type { Business } from "./business.types";
import type { Establishment } from "./establishment.types";
import type { EmissionPoint } from "./emissionPoint.types";

export type User = {
  id: number;
  document: string;
  username: string;
  fullName: string;
  email: string;
  address: string;
  cellphone: string;
  telephone: string;
  imageUrl: string;
  companyName: string;
  isActive: boolean;
  createdAt: Date;
  roles: string[];
  business: Business;
  establishment: Establishment[];
  emissionPoint: EmissionPoint[];
};

const userSchema = z.object({
  document: z.string().min(1, "El documento es obligatorio"),
  username: z.string().min(1, "El usuario es obligatorio"),
  fullName: z.string().min(1, "El nombre completo es obligatorio"),
  email: z.email("Correo inválidos"),
  address: z.string().min(1, "La dirección es obligatoria"),
  cellphone: z.string().min(1, "El celular es obligatorio"),
  password: z.string().min(6, "La contraseña debe tener al menos 6 caracteres"),
  telephone: z.string().optional(),
  companyName: z.string().optional(),
  imageUrl: z.string().optional(),
  sequence: z.string().optional(),
  rolIds: z.array(z.number()).nonempty("Debe seleccionar al menos un rol"),
  establishmentId: z.number().int(),
  emissionPointId: z.number().int(),
});

export type CreateUserForm = z.infer<typeof userSchema>;
