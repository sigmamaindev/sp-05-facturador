import type { Business } from "./business.types";
import type { DocumentType } from "./documentType.types";

export type Customer = {
  id: number;
  document: string;
  name: string;
  email: string;
  address: string;
  cellphone: string;
  telephone: string;
  isActive: boolean;
  documentType: DocumentType;
  business: Business;
};
