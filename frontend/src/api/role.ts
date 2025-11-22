import { isAxiosError } from "axios";

import type { ApiResponseList } from "@/types/api.types";
import type { Role } from "@/types/role.types";

import api from "@/utils/axios";

export async function getRoles(token: string): Promise<ApiResponseList<Role>> {
  try {
    const url = `/role`;

    const { data } = await api.get<ApiResponseList<Role>>(url, {
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
