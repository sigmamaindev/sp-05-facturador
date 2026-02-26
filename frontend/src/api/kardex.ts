import { isAxiosError } from "axios";

import api from "@/utils/axios";

import type { ApiResponseList, ApiResponseSingle } from "@/types/api.types";
import type { KardexMovement, KardexReportWrapper } from "@/types/kardex.types";

export async function getKardexMovements(
  keyword: string,
  page = 1,
  limit = 10,
  token: string
): Promise<ApiResponseList<KardexMovement>> {
  try {
    const search = `keyword=${keyword}`;
    const pagination = `page=${page}&limit=${limit}`;

    const url = `/kardex?${search}&${pagination}`;

    const { data } = await api.get<ApiResponseList<KardexMovement>>(url, {
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

export async function getKardexReport(
  productId: number,
  dateFrom: string,
  dateTo: string,
  token: string
): Promise<ApiResponseSingle<KardexReportWrapper>> {
  try {
    const params = new URLSearchParams();
    params.append("productId", String(productId));
    params.append("dateFrom", dateFrom);
    params.append("dateTo", dateTo);

    const { data } = await api.get<ApiResponseSingle<KardexReportWrapper>>(
      `/kardex/report?${params.toString()}`,
      {
        headers: {
          Authorization: `Bearer ${token}`,
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

export async function downloadKardexReportPdf(
  productId: number,
  dateFrom: string,
  dateTo: string,
  token: string
): Promise<Blob> {
  try {
    const params = new URLSearchParams();
    params.append("productId", String(productId));
    params.append("dateFrom", dateFrom);
    params.append("dateTo", dateTo);

    const { data } = await api.get<Blob>(
      `/kardex/report/pdf?${params.toString()}`,
      {
        headers: {
          Authorization: `Bearer ${token}`,
        },
        responseType: "blob",
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

export async function downloadKardexReportExcel(
  productId: number,
  dateFrom: string,
  dateTo: string,
  token: string
): Promise<Blob> {
  try {
    const params = new URLSearchParams();
    params.append("productId", String(productId));
    params.append("dateFrom", dateFrom);
    params.append("dateTo", dateTo);

    const { data } = await api.get<Blob>(
      `/kardex/report/excel?${params.toString()}`,
      {
        headers: {
          Authorization: `Bearer ${token}`,
        },
        responseType: "blob",
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

