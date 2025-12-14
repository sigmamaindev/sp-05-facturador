import { isAxiosError } from "axios";

import api from "@/utils/axios";

import type { ApiResponseList, ApiResponseSingle } from "@/types/api.types";
import type { CreateSupplierForm, Supplier } from "@/types/supplier.types";

export async function getSuppliers(
  keyword: string,
  page = 1,
  limit = 10,
  token: string
): Promise<ApiResponseList<Supplier>> {
  try {
    const search = `keyword=${keyword}`;
    const pagination = `page=${page}&limit=${limit}`;

    const url = `/supplier?${search}&${pagination}`;

    const { data } = await api.get<ApiResponseList<Supplier>>(url, {
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

export async function createSupplier(
  body: CreateSupplierForm,
  token: string
): Promise<ApiResponseSingle<Supplier>> {
  try {
    const url = `/supplier`;

    const { data } = await api.post<ApiResponseSingle<Supplier>>(url, body, {
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
