import { isAxiosError } from "axios";

import type { ApiResponseList } from "@/types/api.types";
import type { Establishment } from "@/types/establishment.types";

import api from "@/utils/axios";

export async function getEstablishments(
  keyword: string,
  page = 1,
  limit = 10,
  token: string
): Promise<ApiResponseList<Establishment>> {
  try {
    const search = `keyword=${keyword}`;
    const pagination = `page=${page}&limit=${limit}`;

    const url = `/establishment?${search}&${pagination}`;

    const { data } = await api.get<ApiResponseList<Establishment>>(url, {
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
