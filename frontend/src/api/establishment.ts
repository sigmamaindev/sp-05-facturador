import { isAxiosError } from "axios";

import api from "@/utils/axios";

import type { ApiResponseList, ApiResponseSingle } from "@/types/api.types";
import type {
  CreateEstablishmentForm,
  Establishment,
  UpdateEstablishmentForm,
} from "@/types/establishment.types";

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

export async function getEstablishmentById(
  id: number,
  token: string
): Promise<ApiResponseSingle<Establishment>> {
  try {
    const url = `/establishment/${id}`;

    const { data } = await api.get<ApiResponseSingle<Establishment>>(url, {
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

export async function createEstablishment(
  body: CreateEstablishmentForm,
  token: string
): Promise<ApiResponseSingle<Establishment>> {
  try {
    const url = `/establishment`;

    const { data } = await api.post<ApiResponseSingle<Establishment>>(
      url,
      body,
      {
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
      }
    );

    return data;
  } catch (error) {
    if (isAxiosError(error) && error.response) {
      throw new Error(error.response.data.error ?? "Error en la API");
    }
    throw new Error("Error desconocido");
  }
}

export async function updateEstablishment(
  id: number,
  body: UpdateEstablishmentForm,
  token: string
): Promise<ApiResponseSingle<Establishment>> {
  try {
    const url = `/establishment/${id}`;

    const { data } = await api.put<ApiResponseSingle<Establishment>>(
      url,
      body,
      {
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
      }
    );

    return data;
  } catch (error) {
    if (isAxiosError(error) && error.response) {
      throw new Error(error.response.data.error ?? "Error en la API");
    }
    throw new Error("Error desconocido");
  }
}
