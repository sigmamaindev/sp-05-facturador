import z from "zod";

export type Customer = {
  id: number;
  document: string;
  name: string;
  email: string;
  address: string;
  cellphone: string;
  telephone: string;
  isActive: boolean;
  documentType: "04" | "05" | "06" | "07" | "08" | "09";
  createdAt: Date;
};

const phoneRegex = /^[0-9]{7,15}$/;

export const createCustomerSchema = z.object({
  document: z
    .string()
    .min(10, "El documento debe tener al menos 10 caracteres")
    .max(13, "El documento no debe exceder 13 caracteres")
    .regex(/^[0-9]+$/, "El documento solo debe contener números"),
  name: z
    .string()
    .min(3, "El nombre debe tener al menos 3 caracteres")
    .max(100, "El nombre no puede superar los 100 caracteres"),
  email: z.email("El correo electrónico no es válido"),
  address: z
    .string()
    .min(3, "La dirección debe tener al menos 3 caracteres")
    .max(150, "La dirección no puede superar los 150 caracteres"),
  cellphone: z
    .string()
    .optional()
    .refine((val) => !val || phoneRegex.test(val), {
      message: "El celular debe contener solo números (7-15 dígitos)",
    }),
  telephone: z
    .string()
    .optional()
    .refine((val) => !val || phoneRegex.test(val), {
      message: "El teléfono debe contener solo números (7-15 dígitos)",
    }),
  documentType: z.enum(["04", "05", "06", "07", "08", "09"], {
    error: () => ({
      message: "Debe escoger un tipo de documento",
    }),
  }),
});

export type CreateCustomerForm = z.infer<typeof createCustomerSchema>;

export const updateCustomerSchema = z.object({
  document: z
    .string()
    .min(10, "El documento debe tener al menos 10 caracteres")
    .max(13, "El documento no debe exceder 13 caracteres")
    .regex(/^[0-9]+$/, "El documento solo debe contener números"),
  name: z
    .string()
    .min(3, "El nombre debe tener al menos 3 caracteres")
    .max(100, "El nombre no puede superar los 100 caracteres"),
  email: z.email("El correo electrónico no es válido"),
  address: z
    .string()
    .min(3, "La dirección debe tener al menos 3 caracteres")
    .max(150, "La dirección no puede superar los 150 caracteres"),
  cellphone: z
    .string()
    .optional()
    .refine((val) => !val || phoneRegex.test(val), {
      message: "El celular debe contener solo números (7-15 dígitos)",
    }),
  telephone: z
    .string()
    .optional()
    .refine((val) => !val || phoneRegex.test(val), {
      message: "El teléfono debe contener solo números (7-15 dígitos)",
    }),
  documentType: z.enum(["04", "05", "06", "07", "08", "09"], {
    error: () => ({
      message: "Debe escoger un tipo de documento",
    }),
  }),
});

export type UpdateCustomerForm = z.infer<typeof updateCustomerSchema>;
