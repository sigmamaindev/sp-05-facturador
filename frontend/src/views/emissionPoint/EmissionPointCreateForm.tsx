import { useState } from "react";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";
import { Loader2 } from "lucide-react";

import type { CreateEmissionPointForm } from "@/types/emissionPoint.types";

import { createEmissionPoint } from "@/api/emissionPoint";

import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

interface EmissionPointCreateFormProps {
  token: string | null;
  establishmentId: string;
}

export default function EmissionPointCreateForm({
  token,
  establishmentId,
}: EmissionPointCreateFormProps) {
  const navigate = useNavigate();

  const [savingEmissionPoint, setSavingEmissionPoint] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<CreateEmissionPointForm>();

  const onSubmit = async (data: CreateEmissionPointForm) => {
    try {
      setSavingEmissionPoint(true);

      const response = await createEmissionPoint(
        Number(establishmentId),
        data,
        token!
      );

      toast.success(response.message);

      navigate("/puntos-emision");
    } catch (err: any) {
      toast.error(err.message);
    } finally {
      setSavingEmissionPoint(false);
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="grid grid-cols-1">
      <div className="grid gap-2">
        <Label htmlFor="description">Descripción</Label>
        <Input
          id="description"
          type="text"
          placeholder="Descripción"
          transform="uppercase"
          {...register("description", {
            required: "La descripción es obligatorio",
          })}
        />
        {errors.description && (
          <p className="text-red-500 text-sm">{errors.description.message}</p>
        )}
      </div>
      <div className="md:col-span-2 flex justify-end mt-4">
        <Button type="submit" disabled={savingEmissionPoint}>
          {savingEmissionPoint ? (
            <>
              <Loader2 className="h-4 w-4 mr-2 animate-spin" />
              Creando...
            </>
          ) : (
            "Crear"
          )}
        </Button>
      </div>
    </form>
  );
}
