import { isAxiosError } from "axios";

import api from "@/utils/axios";

import type { ApiResponseList } from "@/types/api.types";
import type { UnitMeasure } from "@/types/unitMeasure.types";

export async function getUnitMeasures(
  keyword: string,
  page = 1,
  limit = 10,
  token: string
): Promise<ApiResponseList<UnitMeasure>> {
  try {
    const search = `keyword=${keyword}`;
    const pagination = `page=${page}&limit=${limit}`;

    const url = `/unitMeasure?${search}&${pagination}`;

    const { data } = await api.get<ApiResponseList<UnitMeasure>>(url, {
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
