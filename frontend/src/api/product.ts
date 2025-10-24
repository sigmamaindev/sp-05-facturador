import { isAxiosError } from "axios";

import api from "@/utils/axios";

import type { ApiResponseList } from "@/types/api.types";
import type { Product } from "@/types/product.types";

export async function getProducts(
  keyword: string,
  page = 1,
  limit = 10,
  token: string
): Promise<ApiResponseList<Product>> {
  try {
    const search = `keyword=${keyword}`;
    const pagination = `page=${page}&limit=${limit}`;

    const url = `/product?${search}&${pagination}`;

    const { data } = await api.get<ApiResponseList<Product>>(url, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    return data;
  } catch (error) {
    if (isAxiosError(error) && error.response) {
      throw new Error(error.response.data.error ?? "Error en la API");
    }
    throw new Error("Error desconocido");
  }
}
