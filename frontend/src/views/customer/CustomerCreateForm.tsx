import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Controller, useForm } from "react-hook-form";
import { toast } from "sonner";
import { Loader2 } from "lucide-react";

import type { CreateCustomerForm } from "@/types/customer.types";

import { createCustomer } from "@/api/customer";

import { documentTypes } from "@/constants/documentTypes";

import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

interface CustomerCreateFormProps {
  token: string | null;
}

export default function CustomerCreateForm({ token }: CustomerCreateFormProps) {
  const navigate = useNavigate();

  const [savingCustomer, setSavingCustomer] = useState(false);

  const {
    control,
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<CreateCustomerForm>();

  const onSubmit = async (data: CreateCustomerForm) => {
    try {
      setSavingCustomer(true);

      const response = await createCustomer(data, token!);

      toast.success(response.message);

      navigate("/clientes");
    } catch (err: any) {
      toast.error(err.message);
    } finally {
      setSavingCustomer(false);
    }
  };

  return (
    <form
      className="grid grid-cols-1 md:grid-cols-2 gap-4"
      onSubmit={handleSubmit(onSubmit)}
    >
      <Controller
        name="documentType"
        control={control}
        rules={{ required: "Debe escoger un tipo de documento" }}
        render={({ field }) => (
          <div className="grid gap-2">
            <Label>Tipo de documento</Label>
            <Select
              onValueChange={(val) => field.onChange(val)}
              value={field.value ?? ""}
            >
              <SelectTrigger className="w-full">
                <SelectValue placeholder="Seleccionar tipo documento" />
              </SelectTrigger>
              <SelectContent>
                {documentTypes.map((dt) => (
                  <SelectItem key={dt.value} value={dt.value}>
                    {dt.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            {errors.documentType && (
              <p className="text-red-500 text-sm">
                {errors.documentType.message}
              </p>
            )}
          </div>
        )}
      />
      <div className="grid gap-2">
        <Label htmlFor="document">Documento</Label>
        <Input
          id="document"
          type="text"
          placeholder="Documento de identidad"
          {...register("document", { required: "El documento es obligatorio" })}
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
          transform="uppercase"
          {...register("name", { required: "El nombre es obligatorio" })}
        />
        {errors.name && (
          <p className="text-red-500 text-sm">{errors.name.message}</p>
        )}
      </div>
      <div className="grid gap-2">
        <Label htmlFor="email">Correo</Label>
        <Input
          id="email"
          type="text"
          placeholder="Correo"
          {...register("email", { required: "El correo es obligatorio" })}
        />
        {errors.email && (
          <p className="text-red-500 text-sm">{errors.email.message}</p>
        )}
      </div>
      <div className="grid gap-2">
        <Label htmlFor="address">Dirección</Label>
        <Input
          id="address"
          type="text"
          placeholder="Dirección"
          transform="uppercase"
          {...register("address", { required: "La dirección es obligatoria" })}
        />
        {errors.address && (
          <p className="text-red-500 text-sm">{errors.address.message}</p>
        )}
      </div>
      <div className="grid gap-2">
        <Label htmlFor="cellphone">Celular</Label>
        <Input
          id="cellphone"
          type="text"
          placeholder="Celular"
          transform="digits"
          inputMode="numeric"
          minLength={10}
          {...register("cellphone", {
            validate: {
              digitsOnly: (val) =>
                !val ||
                /^[0-9]+$/.test(val) ||
                "El celular debe contener solo números",
              minLength: (val) =>
                !val ||
                val.length >= 10 ||
                "El celular debe tener al menos 10 caracteres",
            },
          })}
        />
        {errors.cellphone && (
          <p className="text-red-500 text-sm">{errors.cellphone.message}</p>
        )}
      </div>
      <div className="grid gap-2">
        <Label htmlFor="telephone">Teléfono</Label>
        <Input
          id="telephone"
          type="text"
          placeholder="Teléfono"
          transform="digits"
          inputMode="numeric"
          minLength={10}
          {...register("telephone", {
            validate: {
              digitsOnly: (val) =>
                !val ||
                /^[0-9]+$/.test(val) ||
                "El teléfono debe contener solo números",
              minLength: (val) =>
                !val ||
                val.length >= 10 ||
                "El teléfono debe tener al menos 10 caracteres",
            },
          })}
        />
        {errors.telephone && (
          <p className="text-red-500 text-sm">{errors.telephone.message}</p>
        )}
      </div>
      <div className="md:col-span-2 flex justify-end mt-4">
        <Button type="submit" disabled={savingCustomer}>
          {savingCustomer ? (
            <>
              <Loader2 className="h-4 w-4 mr-2 animate-spin" />
              Guardando...
            </>
          ) : (
            "Guardar"
          )}
        </Button>
      </div>
    </form>
  );
}
