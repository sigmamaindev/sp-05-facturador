import { isAxiosError } from "axios";

import api from "@/utils/axios";

import type { ApiResponseList, ApiResponseSingle } from "@/types/api.types";
import type {
  CreatePurchasePayload,
  Purchase,
  UpdatePurchasePayload,
} from "@/types/purchase.type";

export async function getPurchases(
  keyword: string,
  page = 1,
  limit = 10,
  token: string
): Promise<ApiResponseList<Purchase>> {
  try {
    const search = `keyword=${keyword}`;
    const pagination = `page=${page}&limit=${limit}`;

    const url = `/purchase?${search}&${pagination}`;

    const { data } = await api.get<ApiResponseList<Purchase>>(url, {
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

export async function getPurchaseById(
  id: number,
  token: string
): Promise<ApiResponseSingle<Purchase>> {
  try {
    const url = `/purchase/${id}`;

    const { data } = await api.get<ApiResponseSingle<Purchase>>(url, {
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

export async function createPurchase(
  body: CreatePurchasePayload,
  token: string
): Promise<ApiResponseSingle<Purchase>> {
  try {
    const url = `/purchase`;

    const { data } = await api.post<ApiResponseSingle<Purchase>>(url, body, {
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

export async function updatePurchase(
  id: number,
  body: UpdatePurchasePayload,
  token: string
): Promise<ApiResponseSingle<Purchase>> {
  try {
    const url = `/purchase/${id}`;

    const { data } = await api.put<ApiResponseSingle<Purchase>>(url, body, {
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
