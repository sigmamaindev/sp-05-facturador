export const PaymentMethodCode = {
  NFS: "01", // SIN UTILIZACION DEL SISTEMA FINANCIERO
  DEBIT_COMPESATION: "15", // COMPENSACIÓN DE DEUDAS
  DEBIT_CARD: "16", // TARJETA DE DÉBITO
  ELECTRONIC_MONEY: "17", // DINERO ELECTRÓNICO
  PREPAID_CARD: "18", // TARJETA PREPAGO
  CREDIT_CARD: "19", // TARJETA DE CRÉDITO
  FS: "20", // OTROS CON UTILIZACIÓN DEL SISTEMA FINANCIERO
  ENDORSEMENT_TITLES: "21", // ENDOSO DE TÍTULOS
} as const;

export type PaymentMethodCode =
  (typeof PaymentMethodCode)[keyof typeof PaymentMethodCode];

export const PAYMENT_METHOD_LABEL: Record<PaymentMethodCode, string> = {
  "01": "Sin utilización del sistema financiero",
  "15": "Compensación de deudas",
  "16": "Tarjeta de débito",
  "17": "Dinero electrónico",
  "18": "Tarjeta prepago",
  "19": "Tarjeta de crédito",
  "20": "Otros con utilización del sistema financiero",
  "21": "Endoso de títulos",
} as const;

export const PAYMENT_METHOD_OPTIONS = (
  Object.values(PaymentMethodCode) as PaymentMethodCode[]
).map((value) => ({
  value,
  label: PAYMENT_METHOD_LABEL[value],
}));
