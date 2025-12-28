import { isAxiosError } from "axios";

import api from "@/utils/axios";

import type { ApiResponseList, ApiResponseSingle } from "@/types/api.types";
import type {
  AccountsReceivable,
  AccountsReceivableDetail,
} from "@/types/accountsReceivable.types";

export async function getAccountsReceivable(
  keyword: string,
  page = 1,
  limit = 10,
  token: string
): Promise<ApiResponseList<AccountsReceivable>> {
  try {
    const search = `keyword=${keyword}`;
    const pagination = `page=${page}&limit=${limit}`;
    const url = `/accountsReceivable?${search}&${pagination}`;

    const { data } = await api.get<ApiResponseList<AccountsReceivable>>(url, {
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

export async function getAccountsReceivableById(
  id: number,
  token: string
): Promise<ApiResponseSingle<AccountsReceivableDetail>> {
  try {
    const url = `/accountsReceivable/${id}`;

    const { data } = await api.get<ApiResponseSingle<AccountsReceivableDetail>>(
      url,
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
