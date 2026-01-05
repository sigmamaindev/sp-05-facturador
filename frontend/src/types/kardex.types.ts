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

