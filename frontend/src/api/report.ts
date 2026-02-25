import { isAxiosError } from "axios";

import api from "@/utils/axios";

import type { ApiResponseList } from "@/types/api.types";
import type {
  SalesReport,
  PurchasesReport,
} from "@/types/report.types";

export async function getSalesReport(
  keyword: string,
  creditDays: number | null,
  dateFrom: string,
  dateTo: string,
  page = 1,
  limit = 10,
  token: string
): Promise<ApiResponseList<SalesReport>> {
  try {
    const params = new URLSearchParams();

    if (keyword) params.append("keyword", keyword);
    if (creditDays !== null) params.append("creditDays", String(creditDays));
    if (dateFrom) params.append("dateFrom", dateFrom);
    if (dateTo) params.append("dateTo", dateTo);
    params.append("page", String(page));
    params.append("limit", String(limit));

    const { data } = await api.get<ApiResponseList<SalesReport>>(
      `/report/sales?${params.toString()}`,
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

export async function downloadSalesReportPdf(
  keyword: string,
  creditDays: number | null,
  dateFrom: string,
  dateTo: string,
  token: string
): Promise<Blob> {
  try {
    const params = new URLSearchParams();

    if (keyword) params.append("keyword", keyword);
    if (creditDays !== null) params.append("creditDays", String(creditDays));
    if (dateFrom) params.append("dateFrom", dateFrom);
    if (dateTo) params.append("dateTo", dateTo);

    const { data } = await api.get<Blob>(
      `/report/sales/pdf?${params.toString()}`,
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

export async function downloadSalesReportExcel(
  keyword: string,
  creditDays: number | null,
  dateFrom: string,
  dateTo: string,
  token: string
): Promise<Blob> {
  try {
    const params = new URLSearchParams();

    if (keyword) params.append("keyword", keyword);
    if (creditDays !== null) params.append("creditDays", String(creditDays));
    if (dateFrom) params.append("dateFrom", dateFrom);
    if (dateTo) params.append("dateTo", dateTo);

    const { data } = await api.get<Blob>(
      `/report/sales/excel?${params.toString()}`,
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

// ─── Purchases Report ────────────────────────────────────────────────────────

export async function getPurchasesReport(
  keyword: string,
  dateFrom: string,
  dateTo: string,
  page = 1,
  limit = 10,
  token: string
): Promise<ApiResponseList<PurchasesReport>> {
  try {
    const params = new URLSearchParams();

    if (keyword) params.append("keyword", keyword);
    if (dateFrom) params.append("dateFrom", dateFrom);
    if (dateTo) params.append("dateTo", dateTo);
    params.append("page", String(page));
    params.append("limit", String(limit));

    const { data } = await api.get<ApiResponseList<PurchasesReport>>(
      `/report/purchases?${params.toString()}`,
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

export async function downloadPurchasesReportPdf(
  keyword: string,
  dateFrom: string,
  dateTo: string,
  token: string
): Promise<Blob> {
  try {
    const params = new URLSearchParams();

    if (keyword) params.append("keyword", keyword);
    if (dateFrom) params.append("dateFrom", dateFrom);
    if (dateTo) params.append("dateTo", dateTo);

    const { data } = await api.get<Blob>(
      `/report/purchases/pdf?${params.toString()}`,
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

export async function downloadPurchasesReportExcel(
  keyword: string,
  dateFrom: string,
  dateTo: string,
  token: string
): Promise<Blob> {
  try {
    const params = new URLSearchParams();

    if (keyword) params.append("keyword", keyword);
    if (dateFrom) params.append("dateFrom", dateFrom);
    if (dateTo) params.append("dateTo", dateTo);

    const { data } = await api.get<Blob>(
      `/report/purchases/excel?${params.toString()}`,
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
