import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { toast } from "sonner";
import { Loader2 } from "lucide-react";

import type { CreateWarehouseForm } from "@/types/warehouse.types";

import { createWarehouse } from "@/api/warehouse";

import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

interface WarehouseCreateFormProps {
  token: string | null;
}

export default function WarehouseCreateForm({
  token,
}: WarehouseCreateFormProps) {
  const navigate = useNavigate();

  const [savingWarehouse, setSavingWarehouse] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<CreateWarehouseForm>();

  const onSubmit = async (data: CreateWarehouseForm) => {
    try {
      setSavingWarehouse(true);

      const response = await createWarehouse(data, token!);

      toast.success(response.message);

      navigate("/bodegas");
    } catch (err: any) {
      toast.error(err.message);
    } finally {
      setSavingWarehouse(false);
    }
  };

  return (
    <form
      className="grid grid-cols-1 md:grid-cols-2 gap-4"
      onSubmit={handleSubmit(onSubmit)}
    >
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
          {...register("address", { required: "La dirección es obligatoria" })}
        />
        {errors.address && (
          <p className="text-red-500 text-sm">{errors.address.message}</p>
        )}
      </div>

      <div className="md:col-span-2 flex justify-end mt-2">
        <Button type="submit" disabled={savingWarehouse}>
          {savingWarehouse ? (
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
