import { isAxiosError } from "axios";

import type { ApiResponseSingle } from "@/types/api.types";
import type { AuthData, UserLoginForm } from "@/types/auth.types";

import api from "@/utils/axios";

export async function loginUser(
  formData: UserLoginForm
): Promise<ApiResponseSingle<AuthData>> {
  try {
    const url = "/user/login";
    const { data } = await api.post<ApiResponseSingle<AuthData>>(url, formData);
    return data;
  } catch (error) {
    if (isAxiosError(error) && error.response) {
      throw new Error(error.response.data.error ?? "Error en la API");
    }
    throw new Error("Error desconocido");
  }
}
