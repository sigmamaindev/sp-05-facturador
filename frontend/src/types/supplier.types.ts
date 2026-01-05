import z from "zod";

export type Supplier = {
  id: number;
  document: string;
  businessName: string;
  email: string;
  address: string;
  cellphone: string | null;
  telephone: string | null;
  isActive: boolean;
  createdAt: Date;
};

const phoneRegex = /^[0-9]{10,}$/;

export const createSupplierSchema = z.object({
  document: z
    .string()
    .min(10, "El documento debe tener al menos 10 caracteres")
    .max(13, "El documento no debe exceder 13 caracteres")
    .regex(/^[0-9]+$/, "El documento solo debe contener números"),
  businessName: z
    .string()
    .min(3, "El nombre debe tener al menos 3 caracteres")
    .max(100, "El nombre no puede superar los 100 caracteres")
    .transform((v) => v.toUpperCase()),
  email: z.email("El correo electrónico no es válido"),
  address: z
    .string()
    .min(3, "La dirección debe tener al menos 3 caracteres")
    .max(150, "La dirección no puede superar los 150 caracteres")
    .transform((v) => v.toUpperCase()),
  cellphone: z
    .string()
    .optional()
    .refine((val) => !val || phoneRegex.test(val), {
      message: "El celular debe contener solo números (mínimo 10 dígitos)",
    }),
  telephone: z
    .string()
    .optional()
    .refine((val) => !val || phoneRegex.test(val), {
      message: "El teléfono debe contener solo números (mínimo 10 dígitos)",
    }),
});

export type CreateSupplierForm = z.infer<typeof createSupplierSchema>;

export const updateSupplierSchema = z.object({
  document: z
    .string()
    .min(10, "El documento debe tener al menos 10 caracteres")
    .max(13, "El documento no debe exceder 13 caracteres")
    .regex(/^[0-9]+$/, "El documento solo debe contener números"),
  businessName: z
    .string()
    .min(3, "El nombre debe tener al menos 3 caracteres")
    .max(100, "El nombre no puede superar los 100 caracteres")
    .transform((v) => v.toUpperCase()),
  email: z.email("El correo electrónico no es válido"),
  address: z
    .string()
    .min(3, "La dirección debe tener al menos 3 caracteres")
    .max(150, "La dirección no puede superar los 150 caracteres")
    .transform((v) => v.toUpperCase()),
  cellphone: z
    .string()
    .optional()
    .refine((val) => !val || phoneRegex.test(val), {
      message: "El celular debe contener solo números (mínimo 10 dígitos)",
    }),
  telephone: z
    .string()
    .optional()
    .refine((val) => !val || phoneRegex.test(val), {
      message: "El teléfono debe contener solo números (mínimo 10 dígitos)",
    }),
});

export type UpdateSupplierForm = z.infer<typeof updateSupplierSchema>;
