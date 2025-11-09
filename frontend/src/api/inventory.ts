import { isAxiosError } from "axios";

import api from "@/utils/axios";

import type { ApiResponseList } from "@/types/api.types";
import type { CreateInventoryForm, Inventory } from "@/types/inventory.types";

export async function createInventory(
  id: number,
  body: CreateInventoryForm,
  token: string
): Promise<ApiResponseList<Inventory>> {
  try {
    const url = `/inventory/${id}`;

    const { data } = await api.post<ApiResponseList<Inventory>>(url, body, {
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
