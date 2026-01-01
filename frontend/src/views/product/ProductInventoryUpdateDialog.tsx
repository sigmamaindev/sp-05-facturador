import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { toast } from "sonner";
import { Loader2, Pencil } from "lucide-react";

import { updateInventory } from "@/api/inventory";

import type { Inventory, UpdateInventoryForm } from "@/types/inventory.types";

import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

interface ProductInventoryUpdateDialogProps {
  productId: number;
  inventory: Inventory;
  token: string;
  onUpdated: (inventory: Inventory) => void;
}

export default function ProductInventoryUpdateDialog({
  productId,
  inventory,
  token,
  onUpdated,
}: ProductInventoryUpdateDialogProps) {
  const [open, setOpen] = useState(false);
  const [saving, setSaving] = useState(false);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<UpdateInventoryForm>({
    defaultValues: {
      warehouseId: inventory.warehouseId,
      stock: inventory.stock ?? 0,
      minStock: inventory.minStock ?? 0,
      maxStock: inventory.maxStock ?? 0,
    },
  });

  useEffect(() => {
    if (!open) return;
    reset({
      warehouseId: inventory.warehouseId,
      stock: inventory.stock ?? 0,
      minStock: inventory.minStock ?? 0,
      maxStock: inventory.maxStock ?? 0,
    });
  }, [inventory.maxStock, inventory.minStock, inventory.stock, inventory.warehouseId, open, reset]);

  const onSubmit = async (data: UpdateInventoryForm) => {
    try {
      setSaving(true);

      const response = await updateInventory(productId, data, token);

      if (response.data) {
        onUpdated(response.data);
      }

      toast.success(response.message);
      setOpen(false);
    } catch (err: unknown) {
      toast.error(err instanceof Error ? err.message : "Error desconocido");
    } finally {
      setSaving(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <Button
        type="button"
        variant="outline"
        size="icon-sm"
        onClick={() => setOpen(true)}
        aria-label="Editar inventario"
      >
        <Pencil className="size-4" />
      </Button>

      <DialogContent>
        <DialogHeader>
          <DialogTitle>{`Editar inventario - ${inventory.warehouseCode}`}</DialogTitle>
        </DialogHeader>

        <form className="grid gap-4" onSubmit={handleSubmit(onSubmit)}>
          <div className="grid gap-2">
            <Label htmlFor={`warehouse-${inventory.id}`}>Bodega</Label>
            <Input
              id={`warehouse-${inventory.id}`}
              value={`${inventory.warehouseCode} - ${inventory.warehouseName}`}
              disabled
            />
            <input type="hidden" {...register("warehouseId", { valueAsNumber: true })} />
          </div>

          <div className="grid gap-2">
            <Label htmlFor={`stock-${inventory.id}`}>Stock</Label>
            <Input
              id={`stock-${inventory.id}`}
              type="number"
              step="0.01"
              min="0"
              {...register("stock", {
                required: "Ingrese una cantidad",
                min: { value: 0, message: "Debe ser mayor o igual a 0" },
                valueAsNumber: true,
              })}
            />
            {errors.stock && (
              <p className="text-red-500 text-sm">{errors.stock.message}</p>
            )}
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="grid gap-2">
              <Label htmlFor={`minStock-${inventory.id}`}>Mínimo</Label>
              <Input
                id={`minStock-${inventory.id}`}
                type="number"
                step="0.01"
                min="0"
                {...register("minStock", {
                  min: { value: 0, message: "Debe ser mayor o igual a 0" },
                  valueAsNumber: true,
                })}
              />
              {errors.minStock && (
                <p className="text-red-500 text-sm">{errors.minStock.message}</p>
              )}
            </div>

            <div className="grid gap-2">
              <Label htmlFor={`maxStock-${inventory.id}`}>Máximo</Label>
              <Input
                id={`maxStock-${inventory.id}`}
                type="number"
                step="0.01"
                min="0"
                {...register("maxStock", {
                  min: { value: 0, message: "Debe ser mayor o igual a 0" },
                  valueAsNumber: true,
                })}
              />
              {errors.maxStock && (
                <p className="text-red-500 text-sm">{errors.maxStock.message}</p>
              )}
            </div>
          </div>

          <div className="flex justify-end gap-2 pt-2">
            <Button
              type="button"
              variant="outline"
              onClick={() => setOpen(false)}
              disabled={saving}
            >
              Cancelar
            </Button>
            <Button type="submit" disabled={saving}>
              {saving ? (
                <>
                  <Loader2 className="h-4 w-4 animate-spin" />
                  Guardando...
                </>
              ) : (
                "Guardar"
              )}
            </Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
}
