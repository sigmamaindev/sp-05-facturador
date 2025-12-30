import { isAxiosError } from "axios";

import api from "@/utils/axios";

import type { ApiResponseSingle } from "@/types/api.types";
import type { Certificate } from "@/types/certificate.types";

export async function uploadCertificate(
  businessId: number,
  certificate: File,
  password: string,
  token: string
): Promise<ApiResponseSingle<Certificate>> {
  try {
    const url = "/certificate/upload";

    const body = new FormData();
    body.append("businessId", String(businessId));
    body.append("certificate", certificate);
    body.append("password", password);

    const { data } = await api.post<ApiResponseSingle<Certificate>>(url, body, {
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

