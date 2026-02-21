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
  sku: string;
  productName: string;
  unitMeasure: string;
  quantity: number;
  unitPrice: number;
  discount: number;
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
  subtotal0: number;
  subtotal12: number;
  discountTotal: number;
  taxValue: number;
  totalInvoice: number;
  status: string;
  items: SalesReportDetailItem[];
}
