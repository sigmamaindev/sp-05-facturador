import { isAxiosError } from "axios";

import api from "@/utils/axios";

import type { ApiResponseSingle } from "@/types/api.types";
import type { PersonLookupResult } from "@/types/personLookup.types";

export async function lookupPersonByDocument(
  document: string,
  token: string
): Promise<ApiResponseSingle<PersonLookupResult>> {
  try {
    const url = `/personlookup/${document}`;

    const { data } = await api.get<ApiResponseSingle<PersonLookupResult>>(url, {
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
