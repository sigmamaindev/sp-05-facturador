import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { toast } from "sonner";
import { Loader2 } from "lucide-react";

import type { CreateSupplierForm } from "@/types/supplier.types";

import { createSupplier } from "@/api/supplier";

import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

interface SupplierCreateFormProps {
  token: string | null;
}

export default function SupplierCreateForm({ token }: SupplierCreateFormProps) {
  const navigate = useNavigate();

  const [savingSupplier, setSavingSupplier] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<CreateSupplierForm>();

  const onSubmit = async (data: CreateSupplierForm) => {
    try {
      setSavingSupplier(true);

      const response = await createSupplier(data, token!);

      toast.success(response.message);

      navigate("/proveedores");
    } catch (err: any) {
      toast.error(err.message);
    } finally {
      setSavingSupplier(false);
    }
  };

  return (
    <form
      className="grid grid-cols-1 md:grid-cols-2 gap-4"
      onSubmit={handleSubmit(onSubmit)}
    >
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
        <Label htmlFor="businessName">Nombre</Label>
        <Input
          id="businessName"
          type="text"
          placeholder="Nombre"
          transform="uppercase"
          {...register("businessName", {
            required: "El nombre es obligatorio",
          })}
        />
        {errors.businessName && (
          <p className="text-red-500 text-sm">{errors.businessName.message}</p>
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
        <Button type="submit" disabled={savingSupplier}>
          {savingSupplier ? (
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
