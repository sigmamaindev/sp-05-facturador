import type { Supplier } from "./supplier.types";

export type AccountsPayableTransaction = {
  id: number;
  apTransactionType: string;
  amount: number;
  paymentMethod: string | null;
  reference: string | null;
  notes: string | null;
  paymentDetails: unknown | null;
  accountsPayableId: number;
  createdAt: string;
};

export type AccountsPayablePurchase = {
  id: number;
  establishmentCode?: string;
  emissionPointCode?: string;
  sequential: string;
  accessKey: string;
  authorizationNumber?: string | null;
  environment?: string;
  receiptType: string;
  status: string;
  isElectronic: boolean;
  issueDate: string;
  authorizationDate?: string | null;
  totalPurchase: number;
  paymentMethod?: string;
  paymentTermDays?: number;
  dueDate?: string;
};

export type AccountsPayable = {
  id: number;
  purchase: AccountsPayablePurchase;
  supplier: Supplier;
  issueDate: string;
  dueDate: string;
  expectedPaymentDate: string | null;
  total: number;
  balance: number;
  status: string;
  transactions: AccountsPayableTransaction[];
};

export type AccountsPayableDetail = AccountsPayable;

export type CreateAccountsPayableTransaction = {
  apTransactionType: string;
  amount: number;
  paymentMethod?: string | null;
  reference?: string | null;
  notes?: string | null;
  paymentDetails?: string | null;
};
