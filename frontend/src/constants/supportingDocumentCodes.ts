export const SUPPORTING_DOCUMENT_CODE_OPTIONS = [
  { value: "01", label: "01: Factura" },
  { value: "02", label: "02: Nota o boleta de venta" },
  {
    value: "03",
    label: "03: Liquidación de compra de Bienes o Prestación de servicios",
  },
  { value: "04", label: "04: Nota de crédito" },
  { value: "05", label: "05: Nota de débito" },
  { value: "06", label: "06: Guias de Remisión" },
  { value: "07", label: "07: Comprobante de Retención" },
  { value: "08", label: "08: Boletos o entradas a espectáculos públicos" },
  {
    value: "09",
    label: "09: Tiquetes o vales emitidos por máquinas registradoras",
  },
  { value: "11", label: "11: Pasajes expedidos por empresas de aviación" },
  {
    value: "12",
    label: "12: Documentos emitidos por instituciones financieras",
  },
  { value: "15", label: "15: Comprobante de venta emitido en el Exterior" },
  {
    value: "16",
    label:
      "16: Formulario Único de Exportación (FUE) o Declaración Aduanera Única (DAU) o Declaración Andina de Valor (DAV)",
  },
  {
    value: "18",
    label: "18: Documentos autorizados utilizados en ventas excepto N/C N/D",
  },
  {
    value: "19",
    label: "19: Comprobantes de Pago de Cuotas o Aportes",
  },
  {
    value: "20",
    label:
      "20: Documentos por Servicios Administrativos emitidos por Inst. del Estado",
  },
  { value: "21", label: "21: Carta de Porte Aéreo" },
  { value: "22", label: "22: RECAP" },
  { value: "23", label: "23: Nota de Crédito TC" },
  { value: "24", label: "24: Nota de Débito TC" },
  { value: "41", label: "41: Comprobante de venta emitido por reembolso" },
  {
    value: "42",
    label:
      "42: Documento retención presuntiva y retención emitida por propio vendedor o por intermediario",
  },
  {
    value: "43",
    label:
      "43: Liquidación para Explotación y Exploración de Hidrocarburos",
  },
  { value: "44", label: "44: Comprobante de Contribuciones y Aportes" },
  { value: "45", label: "45: Liquidación por reclamos de aseguradoras" },
  {
    value: "47",
    label: "47: Nota de Crédito por Reembolso Emitida por Intermediario",
  },
  {
    value: "48",
    label: "48: Nota de Débito por Reembolso Emitida por Intermediario",
  },
  {
    value: "49",
    label:
      "49: Proveedor Directo de Exportador Bajo Régimen Especial",
  },
  {
    value: "50",
    label:
      "50: A Inst. Estado y Empr. Públicas que percibe ingreso exento de Imp. Renta",
  },
  {
    value: "51",
    label:
      "51: N/C a Inst. Estado y Empr. Públicas que percibe ingreso exento de Imp. Renta",
  },
  {
    value: "52",
    label:
      "52: N/D a Inst. Estado y Empr. Públicas que percibe ingreso exento de Imp. Renta",
  },
  { value: "294", label: "294: Liquidación de compra de Bienes Muebles Usados" },
  { value: "344", label: "344: Liquidación de compra de vehículos usados" },
  { value: "364", label: "364: Acta Entrega-Recepción PET" },
  { value: "370", label: "370: Factura operadora transporte / socio" },
  { value: "371", label: "371: Comprobante socio a operadora de transporte" },
  { value: "372", label: "372: Nota de crédito operadora transporte / socio" },
  { value: "373", label: "373: Nota de débito operadora transporte / socio" },
  { value: "374", label: "374: Nota de débito operadora transporte / socio" },
  {
    value: "375",
    label:
      "375: Liquidación de compra RISE de bienes o prestación de servicios",
  },
] as const;

export type SupportingDocumentCodeValue =
  (typeof SUPPORTING_DOCUMENT_CODE_OPTIONS)[number]["value"];

