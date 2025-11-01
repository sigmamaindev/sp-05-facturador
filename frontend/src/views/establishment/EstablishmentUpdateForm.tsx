import { useState } from "react";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";
import { Loader2 } from "lucide-react";

import type {
  Establishment,
  UpdateEstablishmentForm,
} from "@/types/establishment.types";

import { updateEstablishment } from "@/api/establishment";

import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

interface EstablishmentUpdateFormProps {
  establishment: Establishment;
  token: string | null;
}

export default function EstablishmentUpdateForm({
  establishment,
  token,
}: EstablishmentUpdateFormProps) {
  const navigate = useNavigate();

  const [savingEstablishment, setSavingEstablishment] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<UpdateEstablishmentForm>({
    defaultValues: {
      name: establishment.name,
    },
  });

  const onSubmit = async (data: UpdateEstablishmentForm) => {
    try {
      setSavingEstablishment(true);

      await updateEstablishment(establishment.id, data, token!);

      navigate("/establecimientos");
    } catch (err: any) {
      toast.error(err.message);
    } finally {
      setSavingEstablishment(false);
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="grid grid-cols-1">
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
      <div className="md:col-span-2 flex justify-end mt-4">
        <Button type="submit" disabled={savingEstablishment}>
          {savingEstablishment ? (
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
