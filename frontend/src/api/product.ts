import { isAxiosError } from "axios";

import api from "@/utils/axios";

import type { ApiResponseList, ApiResponseSingle } from "@/types/api.types";
import type {
  CreateProductForm,
  Product,
  ProductPresentationInput,
  UpdateProductForm,
} from "@/types/product.types";

type ProductPresentationPayload = Omit<ProductPresentationInput, "isDefault"> & {
  isDefault?: boolean;
};

type CreateProductPayload = Omit<CreateProductForm, "presentations"> & {
  presentations?: Array<Omit<ProductPresentationPayload, "id" | "isDefault">>;
};

type UpdateProductPayload = Omit<UpdateProductForm, "presentations"> & {
  presentations?: ProductPresentationPayload[];
};

function asBoolean(value: unknown, defaultValue: boolean): boolean {
  if (typeof value === "boolean") return value;
  if (typeof value === "string") return value === "true";
  return defaultValue;
}

function normalizeProduct(product: Product): Product {
  const inferredDefaultPresentation =
    product.defaultPresentation ??
    product.presentations?.find((p) => p.isDefault) ??
    null;

  const unitMeasure =
    product.unitMeasure ?? inferredDefaultPresentation?.unitMeasure;
  const price = product.price ?? inferredDefaultPresentation?.price01 ?? 0;

  return {
    ...product,
    defaultPresentation: inferredDefaultPresentation,
    unitMeasure,
    price,
  };
}

function toCreateProductPayload(body: CreateProductForm): CreateProductPayload {
  return {
    ...body,
    defaultPresentation: {
      ...body.defaultPresentation,
      price01: body.defaultPresentation.price01 ?? 0,
      price02: body.defaultPresentation.price02 ?? 0,
      price03: body.defaultPresentation.price03 ?? 0,
      price04: body.defaultPresentation.price04 ?? 0,
      netWeight: body.defaultPresentation.netWeight ?? 0,
      grossWeight: body.defaultPresentation.grossWeight ?? 0,
      isActive: asBoolean(body.defaultPresentation.isActive as unknown, true),
    },
    presentations: body.presentations?.map((p) => {
      const {
        unitMeasureId,
        price01,
        price02,
        price03,
        price04,
        netWeight,
        grossWeight,
        isActive,
      } = p;
      return {
        unitMeasureId,
        price01: price01 ?? 0,
        price02: price02 ?? 0,
        price03: price03 ?? 0,
        price04: price04 ?? 0,
        netWeight: netWeight ?? 0,
        grossWeight: grossWeight ?? 0,
        isActive: asBoolean(isActive as unknown, true),
      };
    }),
  };
}

function toUpdateProductPayload(body: UpdateProductForm): UpdateProductPayload {
  return {
    ...body,
    defaultPresentation: {
      ...body.defaultPresentation,
      price01: body.defaultPresentation.price01 ?? 0,
      price02: body.defaultPresentation.price02 ?? 0,
      price03: body.defaultPresentation.price03 ?? 0,
      price04: body.defaultPresentation.price04 ?? 0,
      netWeight: body.defaultPresentation.netWeight ?? 0,
      grossWeight: body.defaultPresentation.grossWeight ?? 0,
      isActive: asBoolean(body.defaultPresentation.isActive as unknown, true),
    },
    presentations: body.presentations?.map((p) => ({
      ...p,
      price01: p.price01 ?? 0,
      price02: p.price02 ?? 0,
      price03: p.price03 ?? 0,
      price04: p.price04 ?? 0,
      netWeight: p.netWeight ?? 0,
      grossWeight: p.grossWeight ?? 0,
      isActive: asBoolean(p.isActive as unknown, true),
      isDefault: false,
    })),
  };
}

export async function getProducts(
  keyword: string,
  page = 1,
  limit = 10,
  token: string
): Promise<ApiResponseList<Product>> {
  try {
    const search = `keyword=${keyword}`;
    const pagination = `page=${page}&limit=${limit}`;

    const url = `/product?${search}&${pagination}`;

    const { data } = await api.get<ApiResponseList<Product>>(url, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    return {
      ...data,
      data: data.data.map(normalizeProduct),
    };
  } catch (error) {
    if (isAxiosError(error) && error.response) {
      throw new Error(error.response.data.error ?? "Error en la API");
    }
    throw new Error("Error desconocido");
  }
}

export async function createProduct(
  body: CreateProductForm,
  token: string
): Promise<ApiResponseSingle<Product>> {
  try {
    const url = `/product`;

    const payload = toCreateProductPayload(body);

    const { data } = await api.post<ApiResponseSingle<Product>>(url, payload, {
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
    });

    return {
      ...data,
      data: data.data ? normalizeProduct(data.data) : null,
    };
  } catch (error) {
    if (isAxiosError(error) && error.response) {
      throw new Error(error.response.data.error ?? "Error en la API");
    }
    throw new Error("Error desconocido");
  }
}

export async function getProductById(
  id: number,
  token: string
): Promise<ApiResponseSingle<Product>> {
  try {
    const url = `/product/${id}`;

    const { data } = await api.get<ApiResponseSingle<Product>>(url, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });

    return {
      ...data,
      data: data.data ? normalizeProduct(data.data) : null,
    };
  } catch (error) {
    if (isAxiosError(error) && error.response) {
      throw new Error(error.response.data.error ?? "Error en la API");
    }
    throw new Error("Error desconocido");
  }
}

export async function updateProduct(
  id: number,
  body: UpdateProductForm,
  token: string
): Promise<ApiResponseSingle<Product>> {
  try {
    const url = `/product/${id}`;

    const payload = toUpdateProductPayload(body);

    const { data } = await api.put<ApiResponseSingle<Product>>(url, payload, {
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
    });

    return {
      ...data,
      data: data.data ? normalizeProduct(data.data) : null,
    };
  } catch (error) {
    if (isAxiosError(error) && error.response) {
      throw new Error(error.response.data.error ?? "Error en la API");
    }
    throw new Error("Error desconocido");
  }
}
