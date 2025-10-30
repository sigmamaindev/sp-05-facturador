import { isAxiosError } from "axios";

import type { ApiResponseList, ApiResponseSingle } from "@/types/api.types";
import type { EmissionPoint } from "@/types/emissionPoint.types";

import api from "@/utils/axios";

export async function getEmissionPoints(
  keyword: string,
  page = 1,
  limit = 10,
  token: string
): Promise<ApiResponseList<EmissionPoint>> {
  try {
    const search = `keyword=${keyword}`;
    const pagination = `page=${page}&limit=${limit}`;

    const url = `/emissionPoint?${search}&${pagination}`;

    const { data } = await api.get<ApiResponseList<EmissionPoint>>(url, {
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

export async function getEmissionPointById(
  id: number,
  token: string
): Promise<ApiResponseSingle<EmissionPoint>> {
  try {
    const url = `/emissionPoint/${id}`;

    const { data } = await api.get<ApiResponseSingle<EmissionPoint>>(url, {
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
