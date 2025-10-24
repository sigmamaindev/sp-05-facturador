export interface ApiResponseSingle<T> {
  success: boolean;
  message: string;
  data: T | null;
  error: string;
}

export interface ApiResponseList<T> {
  success: boolean;
  message: string;
  data: T[];
  pagination: {
    total: number;
    page: number;
    limit: number;
    totalPages: number;
  };
  error: string;
}
