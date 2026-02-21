import { isAxiosError } from "axios";

import api from "@/utils/axios";

import type { ApiResponseList, ApiResponseSingle } from "@/types/api.types";
import type { SalesReport, SalesReportDetail } from "@/types/report.types";

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

export async function getSalesReportDetail(
  id: number,
  token: string
): Promise<ApiResponseSingle<SalesReportDetail>> {
  try {
    const { data } = await api.get<ApiResponseSingle<SalesReportDetail>>(
      `/report/sales/${id}`,
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
