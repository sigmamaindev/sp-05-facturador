import { isAxiosError } from "axios";

import api from "@/utils/axios";

import type { ApiResponseList, ApiResponseSingle } from "@/types/api.types";
import type {
  AccountsReceivable,
  AccountsReceivableBulkPaymentCreate,
  AccountsReceivableCustomerSummary,
  AccountsReceivableDetail,
  CreateAccountsReceivableTransaction,
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

export async function getAccountsReceivableByCustomer(
  keyword: string,
  page = 1,
  limit = 10,
  token: string
): Promise<ApiResponseList<AccountsReceivableCustomerSummary>> {
  try {
    const search = `keyword=${keyword}`;
    const pagination = `page=${page}&limit=${limit}`;
    const url = `/accountsReceivable/by-customer?${search}&${pagination}`;

    const { data } = await api.get<ApiResponseList<AccountsReceivableCustomerSummary>>(
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

export async function getAccountsReceivableByCustomerId(
  customerId: number,
  keyword: string,
  page = 1,
  limit = 10,
  token: string
): Promise<ApiResponseList<AccountsReceivable>> {
  try {
    const search = `keyword=${keyword}`;
    const pagination = `page=${page}&limit=${limit}`;
    const url = `/accountsReceivable/customer/${customerId}?${search}&${pagination}`;

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

export async function addAccountsReceivableBulkPayments(
  payload: AccountsReceivableBulkPaymentCreate,
  token: string
): Promise<ApiResponseList<AccountsReceivable>> {
  try {
    const url = `/accountsReceivable/payments`;

    const { data } = await api.post<ApiResponseList<AccountsReceivable>>(
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

export async function addAccountsReceivableTransaction(
  id: number,
  payload: CreateAccountsReceivableTransaction,
  token: string
): Promise<ApiResponseSingle<AccountsReceivableDetail>> {
  try {
    const url = `/accountsReceivable/${id}/transactions`;

    const { data } = await api.post<
      ApiResponseSingle<AccountsReceivableDetail>
    >(url, payload, {
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
