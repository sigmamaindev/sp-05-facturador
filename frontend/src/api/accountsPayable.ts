import { isAxiosError } from "axios";

import api from "@/utils/axios";

import type { ApiResponseList, ApiResponseSingle } from "@/types/api.types";
import type {
  AccountsPayable,
  AccountsPayableDetail,
  CreateAccountsPayableTransaction,
} from "@/types/accountsPayable.types";

export async function getAccountsPayable(
  keyword: string,
  page = 1,
  limit = 10,
  token: string
): Promise<ApiResponseList<AccountsPayable>> {
  try {
    const search = `keyword=${keyword}`;
    const pagination = `page=${page}&limit=${limit}`;
    const url = `/accountsPayable?${search}&${pagination}`;

    const { data } = await api.get<ApiResponseList<AccountsPayable>>(url, {
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

export async function getAccountsPayableById(
  id: number,
  token: string
): Promise<ApiResponseSingle<AccountsPayableDetail>> {
  try {
    const url = `/accountsPayable/${id}`;

    const { data } = await api.get<ApiResponseSingle<AccountsPayableDetail>>(
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

export async function addAccountsPayableTransaction(
  id: number,
  payload: CreateAccountsPayableTransaction,
  token: string
): Promise<ApiResponseSingle<AccountsPayableDetail>> {
  try {
    const url = `/accountsPayable/${id}/transactions`;

    const { data } = await api.post<ApiResponseSingle<AccountsPayableDetail>>(
      url,
      payload,
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
