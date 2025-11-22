import { isAxiosError } from "axios";

import api from "@/utils/axios";

import type { ApiResponseList, ApiResponseSingle } from "@/types/api.types";
import type { CreateInvoiceForm, Invoice } from "@/types/invoice.type";

export async function getInvoices(
  keyword: string,
  page = 1,
  limit = 10,
  token: string
): Promise<ApiResponseList<Invoice>> {
  try {
    const search = `keyword=${keyword}`;
    const pagination = `page=${page}&limit=${limit}`;

    const url = `/invoice?${search}&${pagination}`;

    const { data } = await api.get<ApiResponseList<Invoice>>(url, {
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

export async function getInvoiceById(
  id: number,
  token: string
): Promise<ApiResponseSingle<Invoice>> {
  try {
    const url = `/invoice/${id}`;

    const { data } = await api.get<ApiResponseSingle<Invoice>>(url, {
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

export async function createInvoice(
  body: CreateInvoiceForm,
  token: string
): Promise<ApiResponseSingle<Invoice>> {
  try {
    const url = `/invoice`;

    const { data } = await api.post<ApiResponseSingle<Invoice>>(url, body, {
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
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

export async function updateInvoice(
  id: number,
  body: CreateInvoiceForm,
  token: string
): Promise<ApiResponseSingle<Invoice>> {
  try {
    const url = `/invoice/${id}`;

    const { data } = await api.put<ApiResponseSingle<Invoice>>(url, body, {
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
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
