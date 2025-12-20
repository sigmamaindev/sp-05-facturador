export interface Business {
  id: number;
  document: string;
  name: string;
  address: string;
  city: string;
  province: string;
  isActive: boolean;
  sriEnvironment: string;
  createdAt: Date;
}
