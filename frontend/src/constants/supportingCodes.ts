export const SUPPORTING_CODE_OPTIONS = [
  {
    value: "01",
    label:
      "01: Crédito Tributario para declaración de IVA (servicios y bienes distintos de inventarios y activos fijos)",
  },
  {
    value: "02",
    label:
      "02: Costo o Gasto para declaración de IR (servicios y bienes distintos de inventarios y activos fijos)",
  },
  {
    value: "03",
    label: "03: Activo Fijo - Crédito Tributario para declaración de IVA",
  },
  {
    value: "04",
    label: "04: Activo Fijo - Costo o Gasto para declaración de IR",
  },
  {
    value: "05",
    label:
      "05: Liquidación Gastos de Viaje, hospedaje y alimentación Gastos IR (a nombre de empleados y no de la empresa)",
  },
  {
    value: "06",
    label: "06: Inventario - Crédito Tributario para declaración de IVA",
  },
  {
    value: "07",
    label: "07: Inventario - Costo o Gasto para declaración de IR",
  },
  {
    value: "08",
    label:
      "08: Valor pagado para solicitar Reembolso de Gasto (intermediario)",
  },
  {
    value: "09",
    label: "09: Reembolso por Siniestros",
  },
  {
    value: "10",
    label: "10: Distribución de Dividendos, Beneficios o Utilidades",
  },
  {
    value: "11",
    label: "11: Convenios de débito o recaudación para IFI's",
  },
  {
    value: "12",
    label: "12: Impuestos y retenciones presuntivos",
  },
  {
    value: "13",
    label:
      "13: Valores reconocidos por entidades del sector público a favor de sujetos pasivos",
  },
  {
    value: "14",
    label:
      "14: Valores facturados por socios a operadoras de transporte (que no constituyen gasto de dicha operadora)",
  },
  {
    value: "15",
    label:
      "15: Pagos efectuados por consumos propios y de terceros de servicios digitales",
  },
  {
    value: "00",
    label:
      "00: Casos especiales cuyo sustento no aplica en las opciones anteriores",
  },
] as const;

export type SupportingCodeValue =
  (typeof SUPPORTING_CODE_OPTIONS)[number]["value"];

