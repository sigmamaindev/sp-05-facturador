import { isAxiosError } from "axios";

import api from "@/utils/axios";

import type { ApiResponseList, ApiResponseSingle } from "@/types/api.types";
import type { CreateCustomerForm, Customer } from "@/types/customer.types";

export async function getCustomers(
  keyword: string,
  page = 1,
  limit = 10,
  token: string
): Promise<ApiResponseList<Customer>> {
  try {
    const search = `keyword=${keyword}`;
    const pagination = `page=${page}&limit=${limit}`;

    const url = `/customer?${search}&${pagination}`;

    const { data } = await api.get<ApiResponseList<Customer>>(url, {
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

export async function createCustomer(
  body: CreateCustomerForm,
  token: string
): Promise<ApiResponseSingle<Customer>> {
  try {
    const url = `/customer`;

    const { data } = await api.post<ApiResponseSingle<Customer>>(url, body, {
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
