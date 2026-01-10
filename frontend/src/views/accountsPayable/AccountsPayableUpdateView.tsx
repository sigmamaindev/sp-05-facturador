import { useCallback, useEffect, useMemo, useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { useNavigate, useParams } from "react-router-dom";
import { toast } from "sonner";
import { Loader2 } from "lucide-react";

import { ACCOUNT_TRANSACTION_TYPES } from "@/constants/accountTransactionTypes";
import { useAuth } from "@/contexts/AuthContext";
import {
  addAccountsPayableTransaction,
  getAccountsPayableById,
} from "@/api/accountsPayable";
import type {
  AccountsPayableDetail,
  CreateAccountsPayableTransaction,
} from "@/types/accountsPayable.types";

import { Card, CardContent } from "@/components/ui/card";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";
import AccountsPayableUpdateHeader from "./AccountsPayableUpdateHeader";

export default function AccountsPayableUpdateView() {
  const formId = "accounts-payable-transaction-form";
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { token } = useAuth();

  const [accountsPayable, setAccountsPayable] =
    useState<AccountsPayableDetail | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  const {
    register,
    handleSubmit,
    control,
    formState: { errors },
    watch,
    setValue,
  } = useForm<CreateAccountsPayableTransaction>({
    defaultValues: {
      apTransactionType: "PAGO",
      amount: 0,
      paymentMethod: "",
      reference: "",
      notes: "",
      paymentDetails: "",
    },
  });

  const transactionType = watch("apTransactionType");

  const backTo = useMemo(
    () => (id ? `/cuentas-por-pagar/${id}` : "/cuentas-por-pagar"),
    [id]
  );

  const fetchData = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);

      const response = await getAccountsPayableById(Number(id), token!);
      setAccountsPayable(response.data);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : "Error desconocido");
    } finally {
      setLoading(false);
    }
  }, [id, token]);

  useEffect(() => {
    if (id && token) fetchData();
  }, [id, token, fetchData]);

  useEffect(() => {
    if (transactionType !== "PAGO") {
      setValue("paymentMethod", "");
    }
  }, [transactionType, setValue]);

  const onSubmit = async (data: CreateAccountsPayableTransaction) => {
    try {
      setSaving(true);

      const payload: CreateAccountsPayableTransaction = {
        apTransactionType: data.apTransactionType,
        amount: data.amount,
        paymentMethod:
          data.apTransactionType === "PAGO"
            ? (data.paymentMethod ?? "").trim()
            : null,
        reference: data.reference?.trim() ? data.reference.trim() : null,
        notes: data.notes?.trim() ? data.notes.trim() : null,
        paymentDetails: data.paymentDetails?.trim()
          ? data.paymentDetails.trim()
          : null,
      };

      const response = await addAccountsPayableTransaction(
        Number(id),
        payload,
        token!
      );

      toast.success(response.message);
      navigate(backTo);
    } catch (err: unknown) {
      toast.error(err instanceof Error ? err.message : "Error desconocido");
    } finally {
      setSaving(false);
    }
  };

  return (
    <Card>
      <CardContent className="space-y-4">
        <AccountsPayableUpdateHeader
          formId={formId}
          backTo={backTo}
          saving={saving}
        />

        {loading ? (
          <Loading />
        ) : error ? (
          <AlertMessage message={error} variant="destructive" />
        ) : !accountsPayable ? (
          <AlertMessage
            message="Los datos de la cuenta por pagar no se han cargado completamente"
            variant="destructive"
          />
        ) : (
          <form
            id={formId}
            className="grid grid-cols-1 md:grid-cols-2 gap-4"
            onSubmit={handleSubmit(onSubmit)}
          >
            <div className="md:col-span-2 grid gap-2 rounded-md border p-3 text-sm">
              <div className="flex flex-wrap items-center justify-between gap-2">
                <span className="text-muted-foreground">Proveedor</span>
                <span className="font-medium">
                  {accountsPayable.supplier.businessName}
                </span>
              </div>
              <div className="flex flex-wrap items-center justify-between gap-2">
                <span className="text-muted-foreground">Saldo actual</span>
                <span className="font-medium">
                  ${accountsPayable.balance.toFixed(2)}
                </span>
              </div>
            </div>

            <Controller
              name="apTransactionType"
              control={control}
              rules={{ required: "Seleccione un tipo de transacción" }}
              render={({ field }) => (
                <div className="grid gap-2">
                  <Label>Tipo de transacción</Label>
                  <Select
                    onValueChange={(val) => field.onChange(val)}
                    value={field.value ?? ""}
                  >
                    <SelectTrigger className="w-full">
                      <SelectValue placeholder="Seleccione un tipo" />
                    </SelectTrigger>
                    <SelectContent>
                      {ACCOUNT_TRANSACTION_TYPES.map((t) => (
                        <SelectItem key={t} value={t}>
                          {t}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  {errors.apTransactionType && (
                    <p className="text-red-500 text-sm">
                      {errors.apTransactionType.message}
                    </p>
                  )}
                </div>
              )}
            />

            <div className="grid gap-2">
              <Label htmlFor="amount">Monto</Label>
              <Input
                id="amount"
                type="number"
                inputMode="decimal"
                step="0.01"
                {...register("amount", {
                  required: "El monto es obligatorio",
                  valueAsNumber: true,
                  validate: (value) => {
                    if (!Number.isFinite(value)) return "El monto es inválido";
                    if (transactionType === "AJUSTE") {
                      return value !== 0 || "El ajuste no puede ser 0";
                    }
                    return value > 0 || "El monto debe ser mayor a 0";
                  },
                })}
              />
              {errors.amount && (
                <p className="text-red-500 text-sm">{errors.amount.message}</p>
              )}
            </div>

            <div className="grid gap-2">
              <Label htmlFor="paymentMethod">Método de pago</Label>
              <Input
                id="paymentMethod"
                type="text"
                placeholder="Ej: EFECTIVO, TRANSFERENCIA, TARJETA..."
                disabled={transactionType !== "PAGO"}
                {...register("paymentMethod", {
                  validate: (value) => {
                    if (transactionType !== "PAGO") return true;
                    return value?.trim().length
                      ? true
                      : "El método de pago es obligatorio para PAGO";
                  },
                })}
              />
              {errors.paymentMethod && (
                <p className="text-red-500 text-sm">
                  {errors.paymentMethod.message}
                </p>
              )}
            </div>

            <div className="grid gap-2">
              <Label htmlFor="reference">Referencia</Label>
              <Input
                id="reference"
                type="text"
                placeholder="Referencia (opcional)"
                {...register("reference")}
              />
            </div>

            <div className="grid gap-2 md:col-span-2">
              <Label htmlFor="notes">Notas</Label>
              <Textarea
                id="notes"
                placeholder="Notas (opcional)"
                className="min-h-[96px]"
                {...register("notes")}
              />
            </div>

            <div className="grid gap-2 md:col-span-2">
              <Label htmlFor="paymentDetails">Detalles de pago</Label>
              <Textarea
                id="paymentDetails"
                placeholder="Detalles (opcional)"
                className="min-h-[96px]"
                {...register("paymentDetails")}
              />
            </div>

            <div className="md:col-span-2 flex justify-end mt-2">
              <button
                type="submit"
                form={formId}
                disabled={saving}
                className="hidden"
              />
              <p className="text-xs text-muted-foreground">
                {saving ? (
                  <span className="inline-flex items-center gap-2">
                    <Loader2 className="h-4 w-4 animate-spin" />
                    Registrando transacción...
                  </span>
                ) : (
                  "Complete los datos y presione Actualizar."
                )}
              </p>
            </div>
          </form>
        )}
      </CardContent>
    </Card>
  );
}
