export interface SalesReport {
  invoiceDate: string;
  sequential: string;
  customerName: string;
  paymentTermDays: number;
  userFullName: string;
  productName: string;
  quantity: number;
  grossWeight: number;
  netWeight: number;
  unitPrice: number;
  total: number;
}

// ─── Purchases Report ────────────────────────────────────────────────────────

export interface PurchasesReport {
  issueDate: string;
  sequential: string;
  supplierName: string;
  userFullName: string;
  productName: string;
  quantity: number;
  grossWeight: number;
  netWeight: number;
  unitCost: number;
  total: number;
}
