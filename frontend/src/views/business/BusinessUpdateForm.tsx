import { useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";
import { Loader2 } from "lucide-react";

import type { Business, UpdateBusinessForm } from "@/types/business.types";

import { updateBusiness } from "@/api/business";

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

interface BusinessUpdateFormProps {
  business: Business;
  token: string | null;
}

export default function BusinessUpdateForm({
  business,
  token,
}: BusinessUpdateFormProps) {
  const navigate = useNavigate();

  const [savingBusiness, setSavingBusiness] = useState(false);

  const {
    register,
    control,
    handleSubmit,
    formState: { errors },
  } = useForm<UpdateBusinessForm>({
    defaultValues: {
      document: business.document,
      name: business.name,
      address: business.address,
      city: business.city,
      province: business.province,
      sriEnvironment: String(business.sriEnvironment) === "2" ? "2" : "1",
    },
  });

  const onSubmit = async (data: UpdateBusinessForm) => {
    try {
      setSavingBusiness(true);

      const response = await updateBusiness(business.id, data, token!);

      toast.success(response.message);

      navigate("/empresas");
    } catch (err: any) {
      toast.error(err.message);
    } finally {
      setSavingBusiness(false);
    }
  };

  return (
    <form
      onSubmit={handleSubmit(onSubmit)}
      className="grid grid-cols-1 md:grid-cols-2 gap-4"
    >
      <div className="grid gap-2">
        <Label htmlFor="document">Identificación</Label>
        <Input
          id="document"
          type="text"
          placeholder="RUC o identificación"
          {...register("document", {
            required: "La identificación es obligatoria",
          })}
        />
        {errors.document && (
          <p className="text-red-500 text-sm">{errors.document.message}</p>
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
        <Label htmlFor="address">Dirección</Label>
        <Input
          id="address"
          type="text"
          placeholder="Dirección"
          {...register("address", {
            required: "La dirección es obligatoria",
          })}
        />
        {errors.address && (
          <p className="text-red-500 text-sm">{errors.address.message}</p>
        )}
      </div>

      <div className="grid gap-2">
        <Label htmlFor="city">Ciudad</Label>
        <Input
          id="city"
          type="text"
          placeholder="Ciudad"
          {...register("city", { required: "La ciudad es obligatoria" })}
        />
        {errors.city && (
          <p className="text-red-500 text-sm">{errors.city.message}</p>
        )}
      </div>

      <div className="grid gap-2">
        <Label htmlFor="province">Provincia</Label>
        <Input
          id="province"
          type="text"
          placeholder="Provincia"
          {...register("province", {
            required: "La provincia es obligatoria",
          })}
        />
        {errors.province && (
          <p className="text-red-500 text-sm">{errors.province.message}</p>
        )}
      </div>
      <Controller
        name="sriEnvironment"
        control={control}
        rules={{ required: "El ambiente SRI es obligatorio" }}
        render={({ field }) => (
          <div className="grid gap-2">
            <Label>Ambiente SRI</Label>
            <Select
              onValueChange={(val) => field.onChange(val)}
              value={field.value ?? ""}
            >
              <SelectTrigger className="w-full">
                <SelectValue placeholder="Seleccionar ambiente" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="1">PRUEBAS</SelectItem>
                <SelectItem value="2">PRODUCCIÓN</SelectItem>
              </SelectContent>
            </Select>
            {errors.sriEnvironment && (
              <p className="text-red-500 text-sm">
                {errors.sriEnvironment.message}
              </p>
            )}
          </div>
        )}
      />
      <div className="md:col-span-2 flex justify-end mt-4">
        <Button type="submit" disabled={savingBusiness}>
          {savingBusiness ? (
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
