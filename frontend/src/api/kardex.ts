import { isAxiosError } from "axios";

import api from "@/utils/axios";

import type { ApiResponseList } from "@/types/api.types";
import type { KardexMovement } from "@/types/kardex.types";

export async function getKardexMovements(
  keyword: string,
  page = 1,
  limit = 10,
  token: string
): Promise<ApiResponseList<KardexMovement>> {
  try {
    const search = `keyword=${keyword}`;
    const pagination = `page=${page}&limit=${limit}`;

    const url = `/kardex?${search}&${pagination}`;

    const { data } = await api.get<ApiResponseList<KardexMovement>>(url, {
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

