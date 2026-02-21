import z from "zod";

import type { Product } from "./product.types";
import type { Supplier } from "./supplier.types";

export type Purchase = {
  id: number;
  environment: string;
  emissionTypeCode: string;
  businessName: string;
  name: string;
  document: string;
  accessKey: string;
  receiptType: string;
  supportingCode?: string | null;
  supportingDocumentCode?: string | null;
  establishmentCode: string;
  emissionPointCode: string;
  sequential: string;
  mainAddress: string;
  issueDate: string | Date;
  establishmentAddress: string | null;
  specialTaxpayer: string | null;
  mandatoryAccounting: string;
  typeDocumentSubjectDetained: string;
  typeSubjectDetained: string;
  relatedParty: string;
  businessNameSubjectDetained: string;
  documentSubjectDetained: string;
  fiscalPeriod: string;
  supplierId: number;
  supplier?: Supplier | null;
  subtotalWithoutTaxes: number;
  subtotalWithTaxes: number;
  discountTotal: number;
  taxTotal: number;
  totalPurchase: number;
  paymentMethod?: string | null;
  paymentType?: string | null;
  paymentTermDays?: number | null;
  dueDate?: string | Date | null;
  status: string;
  isElectronic: boolean;
  authorizationNumber: string | null;
  authorizationDate: string | Date | null;
  details?: PurchaseDetail[];
};

export type PurchaseDetail = {
  id: number;
  purchaseId: number;
  productId: number;
  productCode: string;
  productName: string;
  warehouseId: number;
  warehouseCode: string;
  warehouseName: string;
  unitMeasureId: number;
  unitMeasureCode: string;
  unitMeasureName: string;
  taxId: number;
  taxCode: string;
  taxName: string;
  quantity: number;
  netWeight: number;
  grossWeight: number;
  unitCost: number;
  discount: number;
  subtotal: number;
  taxRate: number;
  taxValue: number;
  total: number;
};

export interface PurchaseProduct extends Product {
  quantity: number;
  unitCost: number;
  discount: number;
  netWeight: number;
  grossWeight: number;
  subtotal: number;
  taxValue: number;
}

export interface PurchaseTotals {
  subtotal: number;
  discount: number;
  tax: number;
  total: number;
}

export type PurchaseSupplier = {
  id: number;
  name: string;
  document: string;
  email: string;
};

export const purchaseDetailSchema = z.object({
  productId: z
    .number()
    .int("El ID del producto debe ser un entero")
    .positive("El ID del producto debe ser positivo"),
  quantity: z
    .number()
    .positive("La cantidad debe ser mayor a 0")
    .max(1000000, "La cantidad no puede ser mayor a 1,000,000"),
  unitCost: z.number().positive("El costo unitario debe ser mayor a 0"),
  discount: z
    .number()
    .min(0, "El descuento no puede ser negativo")
    .max(9999999, "Descuento demasiado grande")
    .optional()
    .default(0),
  warehouseId: z.number().int().positive(),
  unitMeasureId: z.number().int().positive().optional(),
  taxId: z.number().int().min(0).nullable(),
});

export const createPurchaseSchema = z.object({
  receiptType: z.string().min(1, "Debe seleccionar un tipo de documento"),
  supportingDocumentCode: z
    .string()
    .min(2, "Debe seleccionar un documento de sustento")
    .max(3, "El documento de sustento es inválido"),
  supportingCode: z
    .string()
    .min(2, "Debe seleccionar un código de sustento")
    .max(2, "El código de sustento debe tener 2 dígitos"),
  relatedParty: z.enum(["SI", "NO"]),
  isElectronic: z.boolean(),
  environment: z.string().min(1, "Debe seleccionar un ambiente"),
  emissionTypeCode: z.string().min(1, "Debe seleccionar un tipo de emisión"),
  supplierId: z.number().int().positive("Seleccione un proveedor"),
  purchaseDate: z.date(),
  establishmentCode: z
    .string()
    .min(3, "El establecimiento debe tener 3 dígitos")
    .max(3, "El establecimiento debe tener 3 dígitos"),
  emissionPointCode: z
    .string()
    .min(3, "El punto de emisión debe tener 3 dígitos")
    .max(3, "El punto de emisión debe tener 3 dígitos"),
  sequential: z
    .string()
    .min(1, "Ingrese el secuencial")
    .max(20, "El secuencial es demasiado largo"),
  documentNumber: z.string().min(1).max(50),
  accessKey: z.string().max(49, "La clave de acceso es demasiado larga").optional(),
  authorizationNumber: z
    .string()
    .max(100, "El número de autorización es demasiado largo")
    .optional()
    .default(""),
  authorizationDate: z.date().optional(),
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
  totalPurchase: z
    .number()
    .min(0, "El total de la compra no puede ser negativo"),
  paymentMethod: z.string().min(1, "Debe seleccionar un método de pago"),
  paymentTermDays: z
    .number()
    .int("Los días de plazo deben ser un número entero")
    .min(0, "Los días de plazo no pueden ser negativos")
    .max(365, "El plazo no puede ser mayor a 365 días"),
  reference: z
    .string()
    .max(100, "La referencia es demasiado larga")
    .optional()
    .default(""),
  details: z
    .array(purchaseDetailSchema)
    .min(1, "Debe agregar al menos un detalle en la compra"),
});

export type CreatePurchaseForm = z.infer<typeof createPurchaseSchema>;

export type UpdatePurchasePayload = {
  supplierId: number;
  accessKey: string;
  receiptType: string;
  supportingCode?: string | null;
  supportingDocumentCode?: string | null;
  establishmentCode: string;
  emissionPointCode: string;
  sequential: string;
  mainAddress: string;
  issueDate: Date;
  establishmentAddress?: string | null;
  specialTaxpayer?: string | null;
  mandatoryAccounting?: string | null;
  typeDocumentSubjectDetained: string;
  typeSubjectDetained: string;
  relatedParty: string;
  businessNameSubjectDetained: string;
  documentSubjectDetained: string;
  fiscalPeriod: string;
  isElectronic: boolean;
  authorizationNumber?: string | null;
  authorizationDate?: Date | null;
  paymentTermDays: number;
  initialPaymentMethodCode?: string | null;
  reference?: string | null;
  notes?: string | null;
  details: Array<{
    productId: number;
    warehouseId: number;
    unitMeasureId: number;
    taxId: number;
    taxRate: number;
    taxValue: number;
    quantity: number;
    netWeight: number;
    grossWeight: number;
    unitCost: number;
    discount: number;
    subtotal: number;
    total: number;
  }>;
};

export type CreatePurchasePayload = {
  businessId: number;
  userId: number;
  environment: string;
  emissionTypeCode: string;
  businessName: string;
  name: string;
  document: string;
  accessKey: string;
  receiptType: string;
  supportingDocumentCode: string;
  supportingCode: string;
  establishmentCode: string;
  emissionPointCode: string;
  sequential: string;
  mainAddress: string;
  issueDate: Date;
  establishmentAddress: string | null;
  specialTaxpayer: string | null;
  mandatoryAccounting: string;
  typeDocumentSubjectDetained: string;
  typeSubjectDetained: string;
  relatedParty: string;
  businessNameSubjectDetained: string;
  documentSubjectDetained: string;
  fiscalPeriod: string;
  supplierId: number;
  status: string;
  isElectronic: boolean;
  authorizationNumber: string;
  authorizationDate: Date | null;
  paymentMethod: string;
  paymentTermDays: number;
  subtotalWithoutTaxes?: number;
  subtotalWithTaxes?: number;
  discountTotal?: number;
  taxTotal?: number;
  totalPurchase?: number;
  details: Array<{
    productId: number;
    warehouseId: number;
    unitMeasureId: number;
    taxId: number;
    taxRate: number;
    taxValue: number;
    quantity: number;
    netWeight: number;
    grossWeight: number;
    unitCost: number;
    discount: number;
    subtotal: number;
    total: number;
  }>;
};
