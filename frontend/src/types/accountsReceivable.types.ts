import type { Customer } from "./customer.types";
import type { PaymentTypeValue } from "@/constants/paymentMethods";

export type AccountsReceivableTransaction = {
  id: number;
  arTransactionType: string;
  amount: number;
  paymentMethod: string | null;
  reference: string | null;
  notes: string | null;
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
  status: string;
  transactions: AccountsReceivableTransaction[];
};

export type AccountsReceivableDetail = AccountsReceivable;

export type AccountsReceivableCustomerSummary = {
  customer: Customer;
  totalBalance: number;
};

export type AccountsReceivableBulkPaymentItem = {
  accountsReceivableId: number;
  amount: number;
};

export type AccountsReceivableBulkPaymentCreate = {
  paymentMethod: PaymentTypeValue;
  notes?: string | null;
  items: AccountsReceivableBulkPaymentItem[];
};

export type CreateAccountsReceivableTransaction = {
  arTransactionType: string;
  amount: number;
  paymentMethod?: string | null;
  reference?: string | null;
  notes?: string | null;
  paymentDetails?: string | null;
};
