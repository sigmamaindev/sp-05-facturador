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

export const PaymentType = {
  CASH: "EFECTIVO",
  CHECK: "CHEQUE",
  CREDIT_CARD: "TARJETA CREDITO",
  DEBIT_CARD: "TARJETA DEBITO",
} as const;

export type PaymentTypeValue = (typeof PaymentType)[keyof typeof PaymentType];

const PAYMENT_TYPE_VALUES = Object.values(PaymentType) as PaymentTypeValue[];

export function isPaymentType(value: unknown): value is PaymentTypeValue {
  return (
    typeof value === "string" &&
    PAYMENT_TYPE_VALUES.includes(value as PaymentTypeValue)
  );
}

export const PAYMENT_TYPE_OPTIONS = PAYMENT_TYPE_VALUES.map((value) => ({
  value,
  label: value,
}));

export function paymentMethodCodeFromPaymentType(
  paymentType: PaymentTypeValue
): PaymentMethodCode {
  switch (paymentType) {
    case PaymentType.CASH:
      return PaymentMethodCode.NFS;
    case PaymentType.CHECK:
      return PaymentMethodCode.FS;
    case PaymentType.CREDIT_CARD:
      return PaymentMethodCode.CREDIT_CARD;
    case PaymentType.DEBIT_CARD:
      return PaymentMethodCode.DEBIT_CARD;
  }
}

export function paymentTypeFromPaymentMethodCode(
  paymentMethod: string | null | undefined
): PaymentTypeValue {
  switch (paymentMethod) {
    case PaymentMethodCode.FS:
      return PaymentType.CHECK;
    case PaymentMethodCode.CREDIT_CARD:
      return PaymentType.CREDIT_CARD;
    case PaymentMethodCode.DEBIT_CARD:
      return PaymentType.DEBIT_CARD;
    default:
      return PaymentType.CASH;
  }
}
