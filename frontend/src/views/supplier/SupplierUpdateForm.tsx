import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { type SubmitHandler, useForm } from "react-hook-form";
import { toast } from "sonner";
import { Loader2 } from "lucide-react";

import type { Supplier, UpdateSupplierForm } from "@/types/supplier.types";

import { updateSupplier } from "@/api/supplier";

import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

interface SupplierUpdateFormProps {
  supplier: Supplier;
  token: string | null;
}

export default function SupplierUpdateForm({
  supplier,
  token,
}: SupplierUpdateFormProps) {
  const navigate = useNavigate();

  const [savingSupplier, setSavingSupplier] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<UpdateSupplierForm>({
    defaultValues: {
      document: supplier.document,
      businessName: supplier.businessName,
      email: supplier.email,
      address: supplier.address,
      cellphone: supplier.cellphone || "",
      telephone: supplier.telephone || "",
    },
  });

  const onSubmit: SubmitHandler<UpdateSupplierForm> = async (data) => {
    try {
      setSavingSupplier(true);

      const response = await updateSupplier(supplier.id, data, token!);

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
          {...register("cellphone")}
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
          {...register("telephone")}
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
