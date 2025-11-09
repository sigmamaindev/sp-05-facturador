import { isAxiosError } from "axios";

import api from "@/utils/axios";

import type { ApiResponseList, ApiResponseSingle } from "@/types/api.types";
import type { CreateProductForm, Product } from "@/types/product.types";

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

export async function createProduct(
  body: CreateProductForm,
  token: string
): Promise<ApiResponseSingle<Product>> {
  try {
    const url = `/product`;

    const { data } = await api.post<ApiResponseSingle<Product>>(url, body, {
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
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
