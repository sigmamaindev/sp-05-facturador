export const ACCOUNT_TRANSACTION_TYPES = [
  "CARGO",
  "PAGO",
  "NOTA CREDITO",
  "AJUSTE",
] as const;

export type AccountTransactionType = (typeof ACCOUNT_TRANSACTION_TYPES)[number];

