import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Controller, type SubmitHandler, useForm, useWatch } from "react-hook-form";
import { toast } from "sonner";
import { Loader2 } from "lucide-react";

import type { UnitMeasure } from "@/types/unitMeasure.types";
import type { Tax } from "@/types/tax.types";
import type { Product, UpdateProductForm } from "@/types/product.types";

import { updateProduct } from "@/api/product";

import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Button } from "@/components/ui/button";

interface ProductUpdateFormProps {
  product: Product;
  unitMeasures: UnitMeasure[];
  taxes: Tax[];
  token: string | null;
}

export default function ProductUpdateForm({
  product,
  unitMeasures,
  taxes,
  token,
}: ProductUpdateFormProps) {
  const navigate = useNavigate();

  const [savingProduct, setSavingProduct] = useState(false);

  const {
    control,
    setValue,
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<UpdateProductForm>({
    defaultValues: {
      sku: product.sku,
      name: product.name,
      description: product.description ?? "",
      price: product.price,
      iva: product.iva,
      taxId: product.tax?.id,
      unitMeasureId: product.unitMeasure?.id,
      type: product.type ?? "BIEN",
    },
  });

  const hasIva = useWatch({ control, name: "iva" });
  const selectedTaxId = useWatch({ control, name: "taxId" });
  const productType = useWatch({ control, name: "type" });

  useEffect(() => {
    if (!taxes.length) return;

    if (!hasIva) {
      const ivaZero = taxes.find((t) => t.code === "0");
      if (ivaZero) {
        setValue("taxId", ivaZero.id);
      }
      return;
    }

    const selectedTax = taxes.find((t) => t.id === selectedTaxId);
    if (!selectedTax || selectedTax.code === "0") {
      const defaultIva = taxes.find((t) => t.code === "4");
      if (defaultIva) {
        setValue("taxId", defaultIva.id);
      }
    }
  }, [hasIva, selectedTaxId, taxes, setValue]);

  useEffect(() => {
    if (productType === "SERVICIO") {
      const defaultUnit = unitMeasures.find((u) => u.code === "UND");
      if (defaultUnit) {
        setValue("unitMeasureId", defaultUnit.id);
      }
    }
  }, [productType, unitMeasures, setValue]);

  const onSubmit: SubmitHandler<UpdateProductForm> = async (data) => {
    try {
      setSavingProduct(true);

      const response = await updateProduct(product.id, data, token!);

      toast.success(response.message);

      navigate("/productos");
    } catch (err: any) {
      toast.error(err.message);
    } finally {
      setSavingProduct(false);
    }
  };

  return (
    <form
      className="grid grid-cols-1 md:grid-cols-2 gap-4"
      onSubmit={handleSubmit(onSubmit)}
    >
      <div className="grid gap-2">
        <Label htmlFor="sku">Código</Label>
        <Input
          id="sku"
          type="text"
          placeholder="Código"
          {...register("sku", { required: "El código es obligatorio" })}
        />
        {errors.sku && (
          <p className="text-red-500 text-sm">{errors.sku.message}</p>
        )}
      </div>
      <div className="grid gap-2">
        <Label htmlFor="name">Nombre</Label>
        <Input
          id="name"
          type="text"
          placeholder="Nombre"
          {...register("name", { required: "El nombre es obligatorio" })}
        />
        {errors.name && (
          <p className="text-red-500 text-sm">{errors.name.message}</p>
        )}
      </div>
      <div className="grid gap-2">
        <Label htmlFor="description">Descripción</Label>
        <Input
          id="description"
          type="text"
          placeholder="Descripción"
          {...register("description")}
        />
        {errors.description && (
          <p className="text-red-500 text-sm">{errors.description.message}</p>
        )}
      </div>
      <div className="grid gap-2">
        <Label htmlFor="price">Precio</Label>
        <Input
          id="price"
          type="number"
          step="0.01"
          inputMode="decimal"
          placeholder="Precio"
          {...register("price", {
            required: "El precio es obligatorio",
            valueAsNumber: true,
          })}
        />
        {errors.price && (
          <p className="text-red-500 text-sm">{errors.price.message}</p>
        )}
      </div>
      <Controller
        name="iva"
        control={control}
        rules={{ required: "Debe escoger una opción" }}
        render={({ field }) => (
          <div className="grid gap-2">
            <Label>¿Tiene IVA?</Label>
            <Select
              onValueChange={(val) => field.onChange(val === "true")}
              value={field.value?.toString() ?? ""}
            >
              <SelectTrigger className="w-full">
                <SelectValue placeholder="Seleccionar una opción" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="true">Sí</SelectItem>
                <SelectItem value="false">No</SelectItem>
              </SelectContent>
            </Select>
            {errors.iva && (
              <p className="text-red-500 text-sm">{errors.iva.message}</p>
            )}
          </div>
        )}
      />
      <Controller
        name="taxId"
        control={control}
        rules={{
          required: hasIva ? "Debe seleccionar un tipo de IVA" : undefined,
        }}
        render={({ field }) => (
          <div className="grid gap-2">
            <Label>Tipo de IVA</Label>
            <Select
              disabled={!hasIva}
              onValueChange={(val) => field.onChange(Number(val))}
              value={field.value ? String(field.value) : ""}
            >
              <SelectTrigger className="w-full">
                <SelectValue
                  placeholder={
                    hasIva ? "Seleccionar tipo de IVA" : "IVA 0% automático"
                  }
                />
              </SelectTrigger>
              <SelectContent>
                {taxes
                  .filter((t) => t.code !== "0")
                  .map((t) => (
                    <SelectItem key={t.id} value={String(t.id)}>
                      {t.name}
                    </SelectItem>
                  ))}
              </SelectContent>
            </Select>
            {errors.taxId && (
              <p className="text-red-500 text-sm">{errors.taxId.message}</p>
            )}
          </div>
        )}
      />
      <Controller
        name="type"
        control={control}
        rules={{ required: "Debe escoger una opción" }}
        render={({ field }) => (
          <div className="grid gap-2">
            <Label>Tipo de producto</Label>
            <Select
              onValueChange={(val) => field.onChange(val)}
              value={field.value?.toString() ?? ""}
            >
              <SelectTrigger className="w-full">
                <SelectValue placeholder="Seleccionar una opción" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="BIEN">Bien</SelectItem>
                <SelectItem value="SERVICIO">Servicio</SelectItem>
                <SelectItem value="OTRO">Otro</SelectItem>
              </SelectContent>
            </Select>
            {errors.type && (
              <p className="text-red-500 text-sm">{errors.type.message}</p>
            )}
          </div>
        )}
      />
      <Controller
        name="unitMeasureId"
        control={control}
        rules={{
          required: "Seleccione una unidad de medida",
        }}
        render={({ field }) => (
          <div className="grid gap-2">
            <Label>Unidad de medida</Label>
            <Select
              disabled={productType === "SERVICIO" || !productType}
              onValueChange={(val) => field.onChange(Number(val))}
              value={field.value ? String(field.value) : ""}
            >
              <SelectTrigger className="w-full">
                <SelectValue placeholder="Seleccionar unidad de medida" />
              </SelectTrigger>
              <SelectContent>
                {unitMeasures.map((um) => (
                  <SelectItem key={um.id} value={String(um.id)}>
                    {um.code} - {um.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            {errors.unitMeasureId && (
              <p className="text-red-500 text-sm">
                {errors.unitMeasureId.message}
              </p>
            )}
          </div>
        )}
      />
      <div className="md:col-span-2 flex justify-end mt-4">
        <Button type="submit" disabled={savingProduct}>
          {savingProduct ? (
            <>
              <Loader2 className="h-4 w-4 mr-2 animate-spin" />
              Actualizando...
            </>
          ) : (
            "Actualizar"
          )}
        </Button>
      </div>
    </form>
  );
}
