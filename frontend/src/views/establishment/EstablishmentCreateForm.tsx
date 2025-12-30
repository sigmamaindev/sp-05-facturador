import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { toast } from "sonner";
import { Loader2 } from "lucide-react";

import type { CreateEstablishmentForm } from "@/types/establishment.types";

import { createEstablishment } from "@/api/establishment";

import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

interface EstablishmentCreateFormProps {
  token: string | null;
}

export default function EstablishmentCreateForm({
  token,
}: EstablishmentCreateFormProps) {
  const navigate = useNavigate();

  const [savingEstablishment, setSavingEstablishment] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<CreateEstablishmentForm>();

  const onSubmit = async (data: CreateEstablishmentForm) => {
    try {
      setSavingEstablishment(true);

      const response = await createEstablishment(data, token!);

      toast.success(response.message);

      navigate("/establecimientos");
    } catch (err: any) {
      toast.error(err.message);
    } finally {
      setSavingEstablishment(false);
    }
  };

  return (
    <form className="grid grid-cols-1" onSubmit={handleSubmit(onSubmit)}>
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
      <div className="md:col-span-2 flex justify-end mt-4">
        <Button type="submit" disabled={savingEstablishment}>
          {savingEstablishment ? (
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
