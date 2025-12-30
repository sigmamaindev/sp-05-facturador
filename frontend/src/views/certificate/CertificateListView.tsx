import { useEffect, useMemo, useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { toast } from "sonner";
import { Loader2 } from "lucide-react";

import { useAuth } from "@/contexts/AuthContext";

import { getBusiness } from "@/api/business";
import { uploadCertificate } from "@/api/certificate";

import type { Business } from "@/types/business.types";

import AlertMessage from "@/components/shared/AlertMessage";

import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

type CertificateUploadForm = {
  businessId: string;
  certificate?: FileList;
  password: string;
};

export default function CertificateListView() {
  const { token, user } = useAuth();

  const [businesses, setBusinesses] = useState<Business[]>([]);
  const [loadingBusinesses, setLoadingBusinesses] = useState(false);
  const [businessError, setBusinessError] = useState<string | null>(null);
  const [uploading, setUploading] = useState(false);
  const [fileInputKey, setFileInputKey] = useState(0);

  const {
    register,
    control,
    handleSubmit,
    watch,
    formState: { errors },
    reset,
  } = useForm<CertificateUploadForm>({
    defaultValues: {
      businessId: user?.business?.id ? String(user.business.id) : "",
      password: "",
    },
  });

  const selectedFile = watch("certificate")?.[0] ?? null;

  const businessOptions = useMemo(
    () =>
      businesses.map((business) => ({
        value: String(business.id),
        label: `${business.document} - ${business.name}`,
      })),
    [businesses]
  );

  useEffect(() => {
    const fetchBusinesses = async () => {
      if (!token) return;

      setBusinessError(null);
      setLoadingBusinesses(true);

      try {
        const response = await getBusiness("", 1, 1000, token);
        setBusinesses(response.data);
      } catch (err: any) {
        setBusinessError(err.message);
      } finally {
        setLoadingBusinesses(false);
      }
    };

    fetchBusinesses();
  }, [token]);

  const onSubmit = async (data: CertificateUploadForm) => {
    if (!token) {
      toast.error("Sesión no válida");
      return;
    }

    const file = data.certificate?.[0];
    if (!file) {
      toast.error("Debe seleccionar un archivo .p12 o .pfx");
      return;
    }

    const businessId = Number(data.businessId);
    if (!Number.isFinite(businessId) || businessId <= 0) {
      toast.error("Debe seleccionar una empresa");
      return;
    }

    try {
      setUploading(true);

      const response = await uploadCertificate(
        businessId,
        file,
        data.password,
        token
      );

      toast.success(response.message || "Certificado guardado correctamente");

      setFileInputKey((prev) => prev + 1);
      reset({ businessId: data.businessId, password: "", certificate: undefined });
    } catch (err: any) {
      toast.error(err.message);
    } finally {
      setUploading(false);
    }
  };

  return (
    <Card>
      <CardHeader>
        <div>
          <div className="leading-none font-semibold">Certificados</div>
          <CardDescription>
            Carga el certificado para firmar facturas (archivo .p12 o .pfx).
          </CardDescription>
        </div>
      </CardHeader>

      <CardContent className="space-y-4">
        {businessError ? (
          <AlertMessage
            variant="destructive"
            message={businessError}
            description="No se pudieron cargar las empresas."
          />
        ) : null}

        <form onSubmit={handleSubmit(onSubmit)} className="grid gap-4 max-w-xl">
          <Controller
            name="businessId"
            control={control}
            rules={{ required: "La empresa es obligatoria" }}
            render={({ field }) => (
              <div className="grid gap-2">
                <Label>Empresa</Label>
                <Select
                  value={field.value ?? ""}
                  onValueChange={field.onChange}
                  disabled={loadingBusinesses || businessOptions.length === 0}
                >
                  <SelectTrigger className="w-full">
                    <SelectValue
                      placeholder={
                        loadingBusinesses
                          ? "Cargando empresas..."
                          : "Seleccionar empresa"
                      }
                    />
                  </SelectTrigger>
                  <SelectContent>
                    {businessOptions.map((option) => (
                      <SelectItem key={option.value} value={option.value}>
                        {option.label}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                {errors.businessId && (
                  <p className="text-red-500 text-sm">
                    {errors.businessId.message}
                  </p>
                )}
              </div>
            )}
          />

          <div className="grid gap-2">
            <Label htmlFor="certificate">Certificado (.p12 o .pfx)</Label>
            <Input
              key={fileInputKey}
              id="certificate"
              type="file"
              accept=".p12,.pfx,application/x-pkcs12,application/octet-stream"
              {...register("certificate", {
                required: "Debe seleccionar un archivo .p12 o .pfx",
                validate: (files) => {
                  const file = files?.[0];
                  if (!file) return "Debe seleccionar un archivo .p12 o .pfx";
                  const lower = file.name.toLowerCase();
                  if (lower.endsWith(".p12") || lower.endsWith(".pfx")) return true;
                  return "Debe seleccionar un archivo .p12 o .pfx";
                },
              })}
            />
            {selectedFile ? (
              <p className="text-xs text-muted-foreground">{selectedFile.name}</p>
            ) : null}
            {errors.certificate && (
              <p className="text-red-500 text-sm">
                {errors.certificate.message}
              </p>
            )}
          </div>

          <div className="grid gap-2">
            <Label htmlFor="password">Contraseña</Label>
            <Input
              id="password"
              type="password"
              placeholder="Contraseña del certificado"
              autoComplete="new-password"
              {...register("password", {
                required: "La contraseña es obligatoria",
              })}
            />
            {errors.password && (
              <p className="text-red-500 text-sm">{errors.password.message}</p>
            )}
          </div>

          <div className="flex justify-end pt-2">
            <Button
              type="submit"
              disabled={
                uploading ||
                loadingBusinesses ||
                businessOptions.length === 0 ||
                !!businessError
              }
            >
              {uploading ? (
                <>
                  <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                  Subiendo...
                </>
              ) : (
                "Guardar certificado"
              )}
            </Button>
          </div>
        </form>
      </CardContent>
    </Card>
  );
}
