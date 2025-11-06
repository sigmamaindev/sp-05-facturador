import type { Inventory } from "./inventory.types";
import type { Tax } from "./tax.types";
import type { UnitMeasure } from "./unitMeasure.types";

export type Product = {
  id: number;
  sku: string;
  name: string;
  description: string;
  price: number;
  iva: boolean;
  isActive: boolean;
  tax: Tax;
  unitMeasure: UnitMeasure;
  inventory: Inventory[];
};
