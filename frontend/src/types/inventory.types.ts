export type Inventory = {
  id: number;
  warehouseId: number;
  warehouseCode: string;
  warehouseName: string;
  stock: number;
  minStock: number;
  maxStock: number;
};
