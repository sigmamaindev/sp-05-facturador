export interface SalesReport {
  id: number;
  invoiceDate: string;
  sequential: string;
  paymentTermDays: number;
  userFullName: string;
  customerName: string;
  customerDocument: string;
  totalInvoice: number;
  status: string;
}

export interface SalesReportDetailItem {
  id: number;
  productCode: string;
  productName: string;
  unitMeasureName: string;
  quantity: number;
  unitPrice: number;
  discount: number;
  taxRate: number;
  taxValue: number;
  subtotal: number;
  total: number;
}

export interface SalesReportDetail {
  id: number;
  invoiceDate: string;
  sequential: string;
  paymentTermDays: number;
  userFullName: string;
  customerName: string;
  customerDocument: string;
  subtotalWithoutTaxes: number;
  subtotalWithTaxes: number;
  discountTotal: number;
  taxTotal: number;
  totalInvoice: number;
  status: string;
  items: SalesReportDetailItem[];
}

// ─── Purchases Report ────────────────────────────────────────────────────────

export interface PurchasesReport {
  id: number;
  issueDate: string;
  sequential: string;
  userFullName: string;
  supplierName: string;
  supplierDocument: string;
  totalPurchase: number;
  status: string;
}

export interface PurchasesReportDetailItem {
  id: number;
  productCode: string;
  productName: string;
  unitMeasureName: string;
  warehouseName: string;
  quantity: number;
  unitCost: number;
  discount: number;
  taxRate: number;
  taxValue: number;
  subtotal: number;
  total: number;
}

export interface PurchasesReportDetail {
  id: number;
  issueDate: string;
  sequential: string;
  userFullName: string;
  supplierName: string;
  supplierDocument: string;
  subtotalWithoutTaxes: number;
  subtotalWithTaxes: number;
  discountTotal: number;
  taxTotal: number;
  totalPurchase: number;
  status: string;
  items: PurchasesReportDetailItem[];
}
