import { useState } from "react";
import { useForm } from "react-hook-form";
import { toast } from "sonner";
import { Loader2, Search } from "lucide-react";

import { useAuth } from "@/contexts/AuthContext";
import { useDocumentLookup } from "@/hooks/useDocumentLookup";

import { createSupplier } from "@/api/supplier";

import type { CreateSupplierForm, Supplier } from "@/types/supplier.types";

import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";

type PurchaseSupplierCreateModalProps = {
  open: boolean;
  onClose: () => void;
  onCreated: (supplier: Supplier) => void;
};

export default function PurchaseSupplierCreateModal({
  open,
  onClose,
  onCreated,
}: PurchaseSupplierCreateModalProps) {
  const { token } = useAuth();
  const [savingSupplier, setSavingSupplier] = useState(false);

  const { lookup, isSearching } = useDocumentLookup(token);

  const {
    register,
    handleSubmit,
    reset,
    setValue,
    getValues,
    formState: { errors },
  } = useForm<CreateSupplierForm>({
    defaultValues: {
      document: "",
      businessName: "",
      email: "",
      address: "",
      cellphone: "",
      telephone: "",
    },
  });

  const onSubmit = async (data: CreateSupplierForm) => {
    try {
      setSavingSupplier(true);
      const response = await createSupplier(data, token!);

      if (response.data) {
        onCreated(response.data);
        toast.success("Proveedor creado correctamente");
        reset();
        onClose();
      }
    } catch (err: any) {
      toast.error(err.message);
    } finally {
      setSavingSupplier(false);
    }
  };

  return (
    <Dialog open={open} onOpenChange={onClose}>
      <DialogContent className="max-w-xl">
        <DialogHeader>
          <DialogTitle>Crear proveedor</DialogTitle>
        </DialogHeader>
        <form
          className="grid grid-cols-1 md:grid-cols-2 gap-3"
          onSubmit={handleSubmit(onSubmit)}
        >
          <div className="grid gap-2">
            <Label htmlFor="document">Documento</Label>
            <div className="flex gap-2">
              <Input
                id="document"
                type="text"
                placeholder="Documento de identidad"
                {...register("document", {
                  required: "El documento es obligatorio",
                  onBlur: async (e) => {
                    const doc = e.target.value;
                    if (doc && doc.length >= 10) {
                      const result = await lookup(doc);
                      if (result) {
                        setValue("businessName", result.name);
                        setValue("document", result.document);
                      }
                    }
                  },
                })}
              />
              <Button
                type="button"
                variant="outline"
                disabled={isSearching}
                onClick={async () => {
                  const result = await lookup(getValues("document"));
                  if (result) {
                    setValue("businessName", result.name);
                    setValue("document", result.document);
                  }
                }}
              >
                {isSearching ? (
                  <Loader2 className="h-4 w-4 animate-spin" />
                ) : (
                  <Search className="h-4 w-4" />
                )}
              </Button>
            </div>
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
              <p className="text-red-500 text-sm">
                {errors.businessName.message}
              </p>
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

          <div className="md:col-span-2 flex justify-end gap-3 pt-2">
            <Button
              type="button"
              variant="outline"
              onClick={onClose}
              disabled={savingSupplier}
            >
              Cancelar
            </Button>
            <Button type="submit" disabled={savingSupplier}>
              {savingSupplier ? "Guardando..." : "Guardar proveedor"}
            </Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
}

