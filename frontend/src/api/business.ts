import { isAxiosError } from "axios";

import api from "@/utils/axios";

import type { ApiResponseList, ApiResponseSingle } from "@/types/api.types";
import type { Business } from "@/types/business.types";

export async function getBusiness(
  keyword: string,
  page = 1,
  limit = 10,
  token: string
): Promise<ApiResponseList<Business>> {
  try {
    const search = `keyword=${keyword}`;
    const pagination = `page=${page}&limit=${limit}`;

    const url = `/business?${search}&${pagination}`;

    const { data } = await api.get<ApiResponseList<Business>>(url, {
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

export async function getBusinessById(
  id: number,
  token: string
): Promise<ApiResponseSingle<Business>> {
  try {
    const url = `/business/${id}`;

    const { data } = await api.get<ApiResponseSingle<Business>>(url, {
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
