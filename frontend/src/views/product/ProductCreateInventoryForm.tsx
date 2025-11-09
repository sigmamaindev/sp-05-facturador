import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { toast } from "sonner";
import { Loader2 } from "lucide-react";

import { createInventory } from "@/api/inventory";

import type { Product } from "@/types/product.types";
import type { Warehouse } from "@/types/warehouse.types";
import type { CreateInventoryForm } from "@/types/inventory.types";

import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

interface ProductCreateInventoryFormProps {
  token: string;
  warehouses: Warehouse[];
  product: Product;
}

export default function ProductCreateInventoryForm({
  token,
  warehouses,
  product,
}: ProductCreateInventoryFormProps) {
  const navigate = useNavigate();

  const [savingInventory, setSavingInventory] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<CreateInventoryForm>({
    defaultValues: {
      inventories: warehouses.map((warehouse) => ({
        warehouseId: warehouse.id,
        stock: 0,
      })),
    },
  });

  const onSubmit = async (data: CreateInventoryForm) => {
    try {
      setSavingInventory(true);

      const response = await createInventory(product.id, data, token);

      toast.success(response.message);

      navigate("/productos");
    } catch (err: any) {
      toast.error(err.message);
    } finally {
      setSavingInventory(false);
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <>
        {warehouses.map((warehouse, index) => (
          <div
            key={warehouse.id}
            className="grid grid-cols-1 md:grid-cols-2 gap-4"
          >
            <div className="grid gap-2">
              <Label htmlFor={`warehouse-${warehouse.id}`}>Bodega</Label>
              <Input
                id={`warehouse-${warehouse.id}`}
                value={`${warehouse.code} - ${warehouse.name}`}
                disabled
              />
            </div>

            <div className="grid gap-2">
              <Label htmlFor={`stock-${warehouse.id}`}>Cantidad</Label>
              <Input
                id={`stock-${warehouse.id}`}
                type="number"
                step="1"
                min="0"
                placeholder="0"
                {...register(`inventories.${index}.stock`, {
                  required: "Ingrese una cantidad",
                  min: { value: 0, message: "Debe ser mayor o igual a 0" },
                  valueAsNumber: true,
                })}
              />
              <input
                type="hidden"
                {...register(`inventories.${index}.warehouseId`)}
                value={warehouse.id}
              />
              {errors.inventories?.[index]?.stock && (
                <p className="text-red-500 text-sm mt-1">
                  {errors.inventories[index]?.stock?.message}
                </p>
              )}
            </div>
          </div>
        ))}
      </>
      <div className="md:col-span-2 flex justify-end mt-4">
        <Button type="submit" disabled={savingInventory}>
          {savingInventory ? (
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
