import type { Customer } from "./customer.types";

export type AccountsReceivableStatus = "ABIERTO" | "CERRADO" | "ANULADO" | string;

export type AccountsReceivableTransactionType =
  | "CARGO"
  | "ABONO"
  | "PAGO"
  | "ANULACION"
  | string;

export type AccountsReceivableTransaction = {
  id: number;
  arTransactionType: AccountsReceivableTransactionType;
  amount: number;
  paymentMethod: string | null;
  reference: string;
  notes: string;
  paymentDetails: unknown | null;
  accountReceivableId: number;
  createdAt: string;
};

export type AccountsReceivableInvoice = {
  id: number;
  establishmentCode: string;
  emissionPointCode: string;
  sequential: string;
  accessKey: string;
  authorizationNumber: string | null;
  environment: string;
  receiptType: string;
  status: string;
  isElectronic: boolean;
  invoiceDate: string;
  authorizationDate: string | null;
  totalInvoice: number;
  paymentMethod: string;
  paymentTermDays: number;
  dueDate: string;
};

export type AccountsReceivable = {
  id: number;
  invoice: AccountsReceivableInvoice;
  customer: Customer;
  issueDate: string;
  dueDate: string;
  expectedPaymentDate: string | null;
  paymentMethod: string;
  originalAmount: number;
  balance: number;
  status: AccountsReceivableStatus;
};

export type AccountsReceivableDetail = {
  id: number;
  establishmentCode: string;
  emissionPointCode: string;
  sequential: string;
  accessKey: string;
  authorizationNumber: string | null;
  environment: string;
  receiptType: string;
  status: AccountsReceivableStatus;
  isElectronic: boolean;
  invoiceDate: string;
  authorizationDate: string | null;
  originalAmount: number;
  paymentMethod: string;
  paymentTermDays: number;
  dueDate: string;
  transactions: AccountsReceivableTransaction[];
};
