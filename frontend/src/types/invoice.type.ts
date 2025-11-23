import z from "zod";

import type { Customer } from "./customer.types";
import type { Product } from "./product.types";

export type Invoice = {
  id: number;
  sequential: string;
  accessKey: string;
  environment: string;
  documentType: string;
  status: string;
  isElectronic: boolean;
  invoiceDate: Date;
  authorizationDate: Date;
  customer: Customer;
  businessId: number;
  businessDocument: string;
  businessName: string;
  establishmentId: number;
  establishmentCode: string;
  establishmentName: string;
  emissionPointId: number;
  emissionPointCode: string;
  emissionPointDescription: string;
  userId: number;
  userDocument: string;
  userFullName: string;
  subtotalWithoutTaxes: number;
  subtotalWithTaxes: number;
  discountTotal: number;
  taxTotal: number;
  totalInvoice: number;
  paymentMethod: number;
  paymentTermDays: number;
  dueDate: Date;
  description: string;
  additionalInformation: string;
  xmlSigned: string;
  authorizationNumber: string | null;
  sriMessage: string | null;
  details: InvoiceDetail[];
};

export type InvoiceDetail = {
  id: number;
  invoiceId: number;
  productId: number;
  productCode: string;
  productName: string;
  unitMeasureId: number;
  unitMeasureCode: string;
  unitMeasureName: string;
  warehouseId: number;
  warehouseCode: string;
  warehouseName: string;
  taxId: number;
  taxCode: string;
  taxName: string;
  taxRate: number;
  taxValue: number;
  quantity: number;
  unitPrice: number;
  discount: number;
  subtotal: number;
  total: number;
  additionalDetail: string;
};

export interface InvoiceProduct extends Product {
  quantity: number;
  discount: number;
  subtotal: number;
  taxValue: number;
}

export interface InvoiceTotals {
  subtotal: number;
  discount: number;
  tax: number;
  total: number;
}

export const invoiceDetailSchema = z.object({
  productId: z
    .number()
    .int("El ID del producto debe ser un entero")
    .positive("El ID del producto debe ser positivo"),

  quantity: z
    .number()
    .positive("La cantidad debe ser mayor a 0")
    .max(1000000, "La cantidad no puede ser mayor a 1,000,000"),

  unitPrice: z.number().positive("El precio unitario debe ser mayor a 0"),

  discount: z
    .number()
    .min(0, "El descuento no puede ser negativo")
    .max(9999999, "Descuento demasiado grande"),

  taxId: z
    .number()
    .min(0, "El impuesto no puede ser negativo")
    .max(9999999, "Impuesto demasiado grande"),
});

export const createInvoiceSchema = z.object({
  documentType: z.string().min(1, "Debe seleccionar un tipo de documento"),
  isElectronic: z.boolean(),
  environment: z.string().min(1, "Debe seleccionar un ambiente"),
  invoiceDate: z.date(),
  dueDate: z.date(),
  customerId: z
    .number()
    .int("El ID del cliente debe ser un entero")
    .positive("Debe seleccionar un cliente válido"),
  subtotalWithoutTaxes: z
    .number()
    .min(0, "El subtotal sin impuestos no puede ser negativo"),
  subtotalWithTaxes: z
    .number()
    .min(0, "El subtotal con impuestos no puede ser negativo"),
  discountTotal: z
    .number()
    .min(0, "El total de descuentos no puede ser negativo"),
  taxTotal: z.number().min(0, "El total de impuestos no puede ser negativo"),
  totalInvoice: z
    .number()
    .min(0, "El total de la factura no puede ser negativo"),
  paymentMethod: z.string().min(1, "Debe seleccionar un método de pago"),
  paymentTermDays: z
    .number()
    .int("Los días de plazo deben ser un número entero")
    .min(0, "Los días de plazo no pueden ser negativos")
    .max(365, "El plazo no puede ser mayor a 365 días"),
  description: z
    .string()
    .max(1000, "La descripción no puede tener más de 1000 caracteres")
    .optional(),
  additionalInformation: z
    .string()
    .max(1000, "La información adicional no puede tener más de 1000 caracteres")
    .optional(),
  details: z
    .array(invoiceDetailSchema)
    .min(1, "Debe agregar al menos un detalle en la factura"),
});

export type CreateInvoiceForm = z.infer<typeof createInvoiceSchema>;
