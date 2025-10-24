import { isAxiosError } from "axios";

import type { ApiResponseList, ApiResponseSingle } from "@/types/api.types";
import type { CreateUserForm, User } from "@/types/user.types";

import api from "@/utils/axios";

export async function getUsers(
  keyword: string,
  page = 1,
  limit = 10,
  token: string
): Promise<ApiResponseList<User>> {
  try {
    const search = `keyword=${keyword}`;
    const pagination = `page=${page}&limit=${limit}`;

    const url = `/user?${search}&${pagination}`;

    const { data } = await api.get<ApiResponseList<User>>(url, {
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

export async function createUser(
  body: CreateUserForm,
  token: string
): Promise<ApiResponseSingle<CreateUserForm>> {
  try {
    const url = `/user`;

    const { data } = await api.post<ApiResponseSingle<CreateUserForm>>(
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
