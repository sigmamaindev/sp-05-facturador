import { isAxiosError } from "axios";

import api from "@/utils/axios";

import type { ApiResponse } from "@/types/api.types";
import type { AtsPurchase, AtsSale } from "@/types/ats.types";

export async function getAtsPurchases(
  year: number,
  month: number,
  token: string
): Promise<ApiResponse<AtsPurchase[]>> {
  try {
    const url = `/ats/purchases?year=${year}&month=${month}`;

    const { data } = await api.get<ApiResponse<AtsPurchase[]>>(url, {
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

export async function downloadAtsXml(
  year: number,
  month: number,
  token: string
): Promise<Blob> {
  try {
    const url = `/ats/xml?year=${year}&month=${month}`;

    const { data } = await api.get<Blob>(url, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
      responseType: "blob",
    });

    return data;
  } catch (error) {
    if (isAxiosError(error) && error.response) {
      throw new Error(error.response.data.error ?? "Error en la API");
    }
    throw new Error("Error desconocido");
  }
}

export async function getAtsSales(
  year: number,
  month: number,
  token: string
): Promise<ApiResponse<AtsSale[]>> {
  try {
    const url = `/ats/sales?year=${year}&month=${month}`;

    const { data } = await api.get<ApiResponse<AtsSale[]>>(url, {
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

