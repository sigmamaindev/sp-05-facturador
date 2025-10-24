import { useState } from "react";
import { useForm, Controller } from "react-hook-form";

import { Loader2 } from "lucide-react";

import type { Role } from "@/types/role.types";
import type { Establishment } from "@/types/establishment.types";
import type { EmissionPoint } from "@/types/emissionPoint.types";
import type { CreateUserForm } from "@/types/user.types";

import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import { createUser } from "@/api/user";

interface UserCreateFormProps {
  roles: Role[];
  establishments: Establishment[];
  emissionPoints: EmissionPoint[];
  token: string | null;
}

export default function UserCreateForm({
  roles,
  establishments,
  emissionPoints,
  token,
}: UserCreateFormProps) {
  const {
    control,
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<CreateUserForm>();

  const [savingUser, setSavingUser] = useState(false);

  const onSubmit = async (data: CreateUserForm) => {
    try {
      setSavingUser(true);

      await createUser(data, token!);

      reset();
    } catch (err: any) {
      console.log(err.message);
    } finally {
      setSavingUser(false);
    }
  };

  return (
    <form
      onSubmit={handleSubmit(onSubmit)}
      className="grid grid-cols-1 md:grid-cols-2 gap-4"
    >
      <div className="grid gap-2">
        <Label htmlFor="document">Documento</Label>
        <Input
          id="document"
          type="text"
          placeholder="Cédula, Ruc, Pasaporte, etc."
          {...register("document", {
            required: "El documento es obligatorio",
          })}
        />
        {errors.document && (
          <p className="text-red-500 text-sm">{errors.document.message}</p>
        )}
      </div>
      <div className="grid gap-2">
        <Label htmlFor="fullName">Nombre Completo</Label>
        <Input
          id="fullName"
          type="text"
          placeholder="Nombre Completo"
          {...register("fullName", {
            required: "El nombre completo es obligatorio",
          })}
        />
        {errors.fullName && (
          <p className="text-red-500 text-sm">{errors.fullName.message}</p>
        )}
      </div>
      <div className="grid gap-2">
        <Label htmlFor="username">Nombre de usuario</Label>
        <Input
          id="username"
          type="text"
          placeholder="Usuario"
          {...register("username", {
            required: "El usuario es obligatorio",
          })}
        />
        {errors.username && (
          <p className="text-red-500 text-sm">{errors.username.message}</p>
        )}
      </div>
      <div className="grid gap-2">
        <Label htmlFor="password">Contraseña</Label>
        <Input
          id="password"
          type="password"
          placeholder="Contraseña"
          {...register("password", {
            required: "La contraseña es obligatoria",
          })}
        />
        {errors.password && (
          <p className="text-red-500 text-sm">{errors.password.message}</p>
        )}
      </div>
      <div className="grid gap-2">
        <Label htmlFor="email">Correo electrónico</Label>
        <Input
          id="email"
          type="text"
          placeholder="Correo electrónico"
          {...register("email", {
            required: "El correo electrónico es obligatorio",
          })}
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
          {...register("address", {
            required: "La dirección es obligatorio",
          })}
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
          {...register("cellphone", {
            required: "El celular es obligatorio",
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
          {...register("telephone", {
            required: "El teléfono es obligatorio",
          })}
        />
        {errors.telephone && (
          <p className="text-red-500 text-sm">{errors.telephone.message}</p>
        )}
      </div>
      <Controller
        name="rolIds"
        control={control}
        rules={{ required: "El rol es obligatorio" }}
        render={({ field }) => (
          <div className="grid gap-2">
            <Label>Rol</Label>
            <Select
              onValueChange={(val) => field.onChange([Number(val)])}
              value={field.value?.[0] ? String(field.value[0]) : ""}
            >
              <SelectTrigger className="w-full">
                <SelectValue placeholder="Seleccionar rol" />
              </SelectTrigger>
              <SelectContent>
                {roles.map((r) => (
                  <SelectItem key={r.id} value={r.id.toString()}>
                    {r.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            {errors.rolIds && (
              <p className="text-red-500 text-sm">{errors.rolIds.message}</p>
            )}
          </div>
        )}
      />
      <Controller
        name="establishmentId"
        control={control}
        rules={{ required: "El establecimiento es obligatorio" }}
        render={({ field }) => (
          <div className="grid gap-2">
            <Label>Establecimiento</Label>
            <Select
              onValueChange={(val) => field.onChange(Number(val))}
              value={field.value ? String(field.value) : ""}
            >
              <SelectTrigger className="w-full">
                <SelectValue placeholder="Seleccionar establecimiento" />
              </SelectTrigger>
              <SelectContent>
                {establishments.map((e) => (
                  <SelectItem key={e.id} value={e.id.toString()}>
                    {e.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            {errors.establishmentId && (
              <p className="text-red-500 text-sm">
                {errors.establishmentId.message}
              </p>
            )}
          </div>
        )}
      />
      <Controller
        name="emissionPointId"
        control={control}
        rules={{ required: "El punto de emision es obligatorio" }}
        render={({ field }) => (
          <div className="grid gap-2">
            <Label>Punto de Emisión</Label>
            <Select
              onValueChange={(val) => field.onChange(Number(val))}
              value={field.value ? String(field.value) : ""}
            >
              <SelectTrigger className="w-full">
                <SelectValue placeholder="Seleccionar punto de emisión" />
              </SelectTrigger>
              <SelectContent>
                {emissionPoints.map((p) => (
                  <SelectItem key={p.id} value={p.id.toString()}>
                    {p.code}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            {errors.emissionPointId && (
              <p className="text-red-500 text-sm">
                {errors.emissionPointId.message}
              </p>
            )}
          </div>
        )}
      />

      <div className="md:col-span-2 flex justify-end mt-4">
        <Button type="submit" disabled={savingUser}>
          {savingUser ? (
            <>
              <Loader2 className="h-4 w-4 mr-2 animate-spin" />
              Guardando...
            </>
          ) : (
            "Crear Usuario"
          )}
        </Button>
      </div>
    </form>
  );
}
