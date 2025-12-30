import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Controller,
  type SubmitHandler,
  useFieldArray,
  useForm,
  useWatch,
} from "react-hook-form";
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

function asBoolean(value: unknown, defaultValue: boolean): boolean {
  if (typeof value === "boolean") return value;
  if (typeof value === "number") {
    if (value === 1) return true;
    if (value === 0) return false;
    return defaultValue;
  }
  if (typeof value === "string") {
    const normalized = value.trim().toLowerCase();
    if (!normalized) return defaultValue;
    if (normalized === "true" || normalized === "on" || normalized === "1") {
      return true;
    }
    if (normalized === "false" || normalized === "0") return false;
    return defaultValue;
  }
  return defaultValue;
}

function toNumberOrUndefined(value: unknown): number | undefined {
  if (value === "" || value === null || value === undefined) return undefined;
  const num = typeof value === "number" ? value : Number(value);
  return Number.isFinite(num) ? num : undefined;
}

function toNumberOrZero(value: unknown): number {
  return toNumberOrUndefined(value) ?? 0;
}

export default function ProductUpdateForm({
  product,
  unitMeasures,
  taxes,
  token,
}: ProductUpdateFormProps) {
  const navigate = useNavigate();

  const [savingProduct, setSavingProduct] = useState(false);

  const defaultTaxId = taxes.find((t) => t.code === "4")?.id ?? taxes[0]?.id ?? 0;
  const defaultUnitMeasureId = unitMeasures[0]?.id ?? 0;

  const currentDefaultPresentation =
    product.defaultPresentation ??
    product.presentations?.find((p) => p.isDefault) ??
    null;

  const {
    control,
    setValue,
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<UpdateProductForm>({
    defaultValues: {
      name: product.name.toUpperCase(),
      description: product.description ?? "",
      type: product.type ?? "BIEN",
      taxId: product.tax?.id ?? defaultTaxId,
      defaultPresentation: {
        unitMeasureId:
          currentDefaultPresentation?.unitMeasureId ??
          product.unitMeasure?.id ??
          defaultUnitMeasureId,
        price01:
          currentDefaultPresentation?.price01 ??
          product.price ??
          0,
        price02: currentDefaultPresentation?.price02 ?? 0,
        price03: currentDefaultPresentation?.price03 ?? 0,
        price04: currentDefaultPresentation?.price04 ?? 0,
        netWeight: currentDefaultPresentation?.netWeight ?? 0,
        grossWeight: currentDefaultPresentation?.grossWeight ?? 0,
        isActive: currentDefaultPresentation?.isActive ?? true,
      },
      presentations:
        product.presentations
          ?.filter((p) => !p.isDefault)
          .map((p) => ({
            id: p.id,
            unitMeasureId: p.unitMeasureId,
            price01: p.price01,
            price02: p.price02,
            price03: p.price03,
            price04: p.price04,
            netWeight: p.netWeight,
            grossWeight: p.grossWeight,
            isActive: p.isActive,
            isDefault: false,
          })) ?? [],
    },
  });

  const productType = useWatch({ control, name: "type" });

  useEffect(() => {
    if (!taxes.length) return;
    setValue("taxId", product.tax?.id ?? defaultTaxId);
  }, [defaultTaxId, product.tax?.id, taxes.length, setValue]);

  useEffect(() => {
    if (productType === "SERVICIO") {
      const defaultUnit = unitMeasures.find((u) => u.code === "UND");
      if (defaultUnit) {
        setValue("defaultPresentation.unitMeasureId", defaultUnit.id);
      }
    }
  }, [productType, unitMeasures, setValue]);

  const { fields, append, remove } = useFieldArray({
    control,
    name: "presentations",
    keyName: "fieldKey",
  });

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
          value={product.sku}
          disabled
        />
      </div>
      <div className="grid gap-2">
        <Label htmlFor="name">Nombre</Label>
        <Input
          id="name"
          type="text"
          placeholder="Nombre"
          transform="uppercase"
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
          transform="uppercase"
          {...register("description")}
        />
        {errors.description && (
          <p className="text-red-500 text-sm">{errors.description.message}</p>
        )}
      </div>
      <Controller
        name="taxId"
        control={control}
        rules={{
          required: "Debe seleccionar un impuesto",
        }}
        render={({ field }) => (
          <div className="grid gap-2">
            <Label>Impuesto</Label>
            <Select
              onValueChange={(val) => field.onChange(Number(val))}
              value={field.value ? String(field.value) : ""}
            >
              <SelectTrigger className="w-full">
                <SelectValue placeholder="Seleccionar impuesto" />
              </SelectTrigger>
              <SelectContent>
                {taxes.map((t) => (
                  <SelectItem key={t.id} value={String(t.id)}>
                    {t.code} - {t.name}
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
        name="defaultPresentation.unitMeasureId"
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
            {errors.defaultPresentation?.unitMeasureId && (
              <p className="text-red-500 text-sm">
                {errors.defaultPresentation?.unitMeasureId.message}
              </p>
            )}
          </div>
        )}
      />
      <div className="grid gap-2">
        <Label htmlFor="defaultPrice01">Precio 1</Label>
        <Input
          id="defaultPrice01"
          type="number"
          step="0.01"
          inputMode="decimal"
          placeholder="Precio 1"
          {...register("defaultPresentation.price01", {
            required: "El precio es obligatorio",
            setValueAs: (v) => toNumberOrUndefined(v),
          })}
        />
        {errors.defaultPresentation?.price01 && (
          <p className="text-red-500 text-sm">
            {errors.defaultPresentation.price01.message}
          </p>
        )}
      </div>

      <div className="grid gap-2">
        <Label htmlFor="defaultPrice02">Precio 2</Label>
        <Input
          id="defaultPrice02"
          type="number"
          step="0.01"
          inputMode="decimal"
          placeholder="Precio 2"
          {...register("defaultPresentation.price02", {
            setValueAs: (v) => toNumberOrZero(v),
          })}
        />
      </div>
      <div className="grid gap-2">
        <Label htmlFor="defaultPrice03">Precio 3</Label>
        <Input
          id="defaultPrice03"
          type="number"
          step="0.01"
          inputMode="decimal"
          placeholder="Precio 3"
          {...register("defaultPresentation.price03", {
            setValueAs: (v) => toNumberOrZero(v),
          })}
        />
      </div>
      <div className="grid gap-2">
        <Label htmlFor="defaultPrice04">Precio 4</Label>
        <Input
          id="defaultPrice04"
          type="number"
          step="0.01"
          inputMode="decimal"
          placeholder="Precio 4"
          {...register("defaultPresentation.price04", {
            setValueAs: (v) => toNumberOrZero(v),
          })}
        />
      </div>

      <input
        type="hidden"
        {...register("defaultPresentation.netWeight", { valueAsNumber: true })}
        defaultValue={currentDefaultPresentation?.netWeight ?? 0}
      />
      <input
        type="hidden"
        {...register("defaultPresentation.grossWeight", { valueAsNumber: true })}
        defaultValue={currentDefaultPresentation?.grossWeight ?? 0}
      />
      <input
        type="hidden"
        {...register("defaultPresentation.isActive", {
          setValueAs: (v) => asBoolean(v, true),
        })}
        defaultValue={String(currentDefaultPresentation?.isActive ?? true)}
      />

      <div className="md:col-span-2 mt-4 space-y-3">
        <div className="flex items-center justify-between">
          <h3 className="text-sm font-medium">Presentaciones adicionales</h3>
          <Button
            type="button"
            variant="outline"
            onClick={() =>
              append({
                unitMeasureId: defaultUnitMeasureId,
                price01: 0,
                price02: 0,
                price03: 0,
                price04: 0,
                netWeight: 0,
                grossWeight: 0,
                isActive: true,
                isDefault: false,
              })
            }
          >
            Agregar
          </Button>
        </div>

        {fields.length ? (
          <div className="space-y-4">
            {fields.map((field, index) => (
              <div
                key={field.fieldKey}
                className="grid grid-cols-1 md:grid-cols-2 gap-4 rounded-md border p-4"
              >
                <input
                  type="hidden"
                  {...register(`presentations.${index}.id`, {
                    valueAsNumber: true,
                  })}
                  defaultValue={field.id ?? ""}
                />

                <Controller
                  name={`presentations.${index}.unitMeasureId`}
                  control={control}
                  rules={{ required: "Seleccione una unidad de medida" }}
                  render={({ field }) => (
                    <div className="grid gap-2">
                      <Label>Unidad de medida</Label>
                      <Select
                        onValueChange={(val) => field.onChange(Number(val))}
                        value={field.value ? String(field.value) : ""}
                      >
                        <SelectTrigger className="w-full">
                          <SelectValue placeholder="Seleccionar unidad" />
                        </SelectTrigger>
                        <SelectContent>
                          {unitMeasures.map((um) => (
                            <SelectItem key={um.id} value={String(um.id)}>
                              {um.code} - {um.name}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    </div>
                  )}
                />

                <div className="grid gap-2">
                  <Label>Precio 1</Label>
                  <Input
                    type="number"
                    step="0.01"
                    inputMode="decimal"
                    {...register(`presentations.${index}.price01`, {
                      required: "El precio es obligatorio",
                      setValueAs: (v) => toNumberOrUndefined(v),
                    })}
                  />
                </div>

                <div className="grid gap-2">
                  <Label>Precio 2</Label>
                  <Input
                    type="number"
                    step="0.01"
                    inputMode="decimal"
                    defaultValue={field.price02 ?? 0}
                    {...register(`presentations.${index}.price02`, {
                      setValueAs: (v) => toNumberOrZero(v),
                    })}
                  />
                </div>
                <div className="grid gap-2">
                  <Label>Precio 3</Label>
                  <Input
                    type="number"
                    step="0.01"
                    inputMode="decimal"
                    defaultValue={field.price03 ?? 0}
                    {...register(`presentations.${index}.price03`, {
                      setValueAs: (v) => toNumberOrZero(v),
                    })}
                  />
                </div>
                <div className="grid gap-2">
                  <Label>Precio 4</Label>
                  <Input
                    type="number"
                    step="0.01"
                    inputMode="decimal"
                    defaultValue={field.price04 ?? 0}
                    {...register(`presentations.${index}.price04`, {
                      setValueAs: (v) => toNumberOrZero(v),
                    })}
                  />
                </div>
                <input
                  type="hidden"
                  {...register(`presentations.${index}.netWeight`, {
                    valueAsNumber: true,
                  })}
                  defaultValue={field.netWeight ?? 0}
                />
                <input
                  type="hidden"
                  {...register(`presentations.${index}.grossWeight`, {
                    valueAsNumber: true,
                  })}
                  defaultValue={field.grossWeight ?? 0}
                />
                <input
                  type="hidden"
                  {...register(`presentations.${index}.isActive`, {
                    setValueAs: (v) => asBoolean(v, true),
                  })}
                  defaultValue={String(field.isActive ?? true)}
                />
                <input
                  type="hidden"
                  {...register(`presentations.${index}.isDefault`, {
                    setValueAs: (v) => asBoolean(v, false),
                  })}
                  defaultValue={String(field.isDefault ?? false)}
                />

                <div className="md:col-span-2 flex justify-end">
                  <Button
                    type="button"
                    variant="destructive"
                    onClick={() => remove(index)}
                  >
                    Quitar
                  </Button>
                </div>
              </div>
            ))}
          </div>
        ) : (
          <p className="text-sm text-muted-foreground">
            No hay presentaciones adicionales.
          </p>
        )}
      </div>
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
