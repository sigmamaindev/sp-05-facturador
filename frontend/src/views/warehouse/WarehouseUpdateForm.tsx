import { useState } from "react";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";
import { Loader2 } from "lucide-react";

import type { UpdateWarehouseForm, Warehouse } from "@/types/warehouse.types";

import { updateWarehouse } from "@/api/warehouse";

import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

interface WarehouseUpdateFormProps {
  warehouse: Warehouse;
  token: string | null;
}

export default function WarehouseUpdateForm({
  warehouse,
  token,
}: WarehouseUpdateFormProps) {
  const navigate = useNavigate();

  const [savingWarehouse, setSavingWarehouse] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<UpdateWarehouseForm>({
    defaultValues: {
      name: warehouse.name,
      address: warehouse.address,
    },
  });

  const onSubmit = async (data: UpdateWarehouseForm) => {
    try {
      setSavingWarehouse(true);

      const response = await updateWarehouse(warehouse.id, data, token!);

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
      onSubmit={handleSubmit(onSubmit)}
      className="grid grid-cols-1 md:grid-cols-2 gap-4"
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
