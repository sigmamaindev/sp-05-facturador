export interface KardexMovement {
  id: number;
  productId: number;
  productSku: string;
  productName: string;
  warehouseId: number;
  warehouseCode: string;
  warehouseName: string;
  movementDate: string;
  movementType: string;
  reference: string;
  quantityIn: number;
  quantityOut: number;
  unitCost: number;
  totalCost: number;
}

export interface KardexReportRow {
  movementDate: string;
  movementType: string;
  warehouseCode: string;
  reference: string;
  entryQuantity: number;
  entryCost: number;
  entryTotal: number;
  exitQuantity: number;
  exitCost: number;
  exitTotal: number;
  runningStock: number;
  runningValue: number;
}

export interface KardexReportWrapper {
  businessName: string;
  businessAddress: string;
  businessRuc: string;
  productSku: string;
  productName: string;
  dateFrom: string;
  dateTo: string;
  reportDate: string;
  movements: KardexReportRow[];
}

