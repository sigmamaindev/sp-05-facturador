import type { Business } from "./business.types";
import type { ProductType } from "./productType.types";
import type { ProductWarehouses } from "./productWarehouses.types";
import type { Tax } from "./tax.types";

export type Product = {
  id: number;
  sku: string;
  name: string;
  description: string;
  price: number;
  iva: boolean;
  isActive: boolean;
  business: Business;
  tax: Tax;
  productType: ProductType;
  productWarehouses: ProductWarehouses;
};
