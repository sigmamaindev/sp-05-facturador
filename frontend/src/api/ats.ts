import { isAxiosError } from "axios";

import api from "@/utils/axios";

import type { ApiResponse } from "@/types/api.types";
import type { AtsPurchase } from "@/types/ats.types";

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

export async function downloadAtsPurchasesXml(
  year: number,
  month: number,
  token: string
): Promise<Blob> {
  try {
    const url = `/ats/purchases/xml?year=${year}&month=${month}`;

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

