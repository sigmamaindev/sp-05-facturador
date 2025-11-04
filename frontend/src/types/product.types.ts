import type { Tax } from "./tax.types";

export type Product = {
  id: number;
  sku: string;
  name: string;
  description: string;
  price: number;
  iva: boolean;
  isActive: boolean;
  tax: Tax;
};
