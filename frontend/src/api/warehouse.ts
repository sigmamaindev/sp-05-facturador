import { isAxiosError } from "axios";

import api from "@/utils/axios";
import type { ApiResponseList, ApiResponseSingle } from "@/types/api.types";
import type {
  CreateWarehouseForm,
  UpdateWarehouseForm,
  Warehouse,
} from "@/types/warehouse.types";

export async function getWarehouses(
  keyword: string,
  page = 1,
  limit = 10,
  token: string
): Promise<ApiResponseList<Warehouse>> {
  try {
    const search = `keyword=${keyword}`;
    const pagination = `page=${page}&limit=${limit}`;

    const url = `/warehouse?${search}&${pagination}`;

    const { data } = await api.get<ApiResponseList<Warehouse>>(url, {
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

export async function getWarehouseById(
  id: number,
  token: string
): Promise<ApiResponseSingle<Warehouse>> {
  try {
    const url = `/warehouse/${id}`;

    const { data } = await api.get<ApiResponseSingle<Warehouse>>(url, {
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

export async function createWarehouse(
  body: CreateWarehouseForm,
  token: string
): Promise<ApiResponseSingle<Warehouse>> {
  try {
    const url = `/warehouse`;

    const { data } = await api.post<ApiResponseSingle<Warehouse>>(url, body, {
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

export async function updateWarehouse(
  id: number,
  body: UpdateWarehouseForm,
  token: string
): Promise<ApiResponseSingle<Warehouse>> {
  try {
    const url = `/warehouse/${id}`;

    const { data } = await api.put<ApiResponseSingle<Warehouse>>(url, body, {
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
