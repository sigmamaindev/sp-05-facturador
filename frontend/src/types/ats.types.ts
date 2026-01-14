export interface AtsPurchase {
  purchaseId: number;
  codSustento: string;
  tpIdProv: string;
  idProv: string;
  tipoComprobante: string;
  parteRel: string;
  fechaRegistro: string;
  establecimiento: string;
  puntoEmision: string;
  secuencial: string;
  fechaEmision: string;
  autorizacion: string;
  baseNoGraIva: number;
  baseImponible: number;
  baseImpGrav: number;
  baseImpExe: number;
  montoIce: number;
  montoIva: number;
  total: number;
  proveedorRazonSocial: string;
}

