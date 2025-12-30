import z from "zod";

import type { Role } from "./role.types";
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
  roles: Role[];
  business: Business;
  establishment: Establishment[];
  emissionPoint: EmissionPoint[];
};

const createUserSchema = z.object({
  document: z.string().min(1, "El documento es obligatorio"),
  username: z.string().min(1, "El usuario es obligatorio"),
  fullName: z
    .string()
    .min(1, "El nombre completo es obligatorio")
    .transform((v) => v.toUpperCase()),
  email: z.email("Correo inválido"),
  address: z
    .string()
    .min(1, "La dirección es obligatoria")
    .transform((v) => v.toUpperCase()),
  cellphone: z
    .string()
    .min(1, "El celular es obligatorio")
    .min(10, "El celular debe tener al menos 10 caracteres")
    .regex(/^[0-9]+$/, "El celular debe contener solo números"),
  password: z.string().min(6, "La contraseña debe tener al menos 6 caracteres"),
  telephone: z
    .string()
    .optional()
    .refine((val) => !val || /^[0-9]+$/.test(val), {
      message: "El teléfono debe contener solo números",
    })
    .refine((val) => !val || val.length >= 10, {
      message: "El teléfono debe tener al menos 10 caracteres",
    }),
  companyName: z.string().optional(),
  imageUrl: z.string().optional(),
  sequence: z.string().optional(),
  rolIds: z.array(z.number()).nonempty("Debe seleccionar al menos un rol"),
  establishmentId: z.number().int(),
  emissionPointId: z.number().int(),
});

export type CreateUserForm = z.infer<typeof createUserSchema>;

const updateUserSchema = z.object({
  document: z.string().min(1, "El documento es obligatorio"),
  username: z.string().min(1, "El usuario es obligatorio"),
  fullName: z
    .string()
    .min(1, "El nombre completo es obligatorio")
    .transform((v) => v.toUpperCase()),
  email: z.email("Correo inválido"),
  address: z
    .string()
    .min(1, "La dirección es obligatoria")
    .transform((v) => v.toUpperCase()),
  cellphone: z
    .string()
    .min(1, "El celular es obligatorio")
    .min(10, "El celular debe tener al menos 10 caracteres")
    .regex(/^[0-9]+$/, "El celular debe contener solo números"),
  telephone: z
    .string()
    .optional()
    .refine((val) => !val || /^[0-9]+$/.test(val), {
      message: "El teléfono debe contener solo números",
    })
    .refine((val) => !val || val.length >= 10, {
      message: "El teléfono debe tener al menos 10 caracteres",
    }),
  companyName: z.string().optional(),
  imageUrl: z.string().optional(),
  sequence: z.string().optional(),
  rolIds: z.array(z.number()).nonempty("Debe seleccionar al menos un rol"),
  establishmentId: z.number().int(),
  emissionPointId: z.number().int(),
});

export type UpdateUserForm = z.infer<typeof updateUserSchema>;
