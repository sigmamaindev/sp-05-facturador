import { useCallback, useEffect, useMemo, useState } from "react";
import { Controller, useForm } from "react-hook-form";
import { toast } from "sonner";
import { BadgeCheck, Loader2 } from "lucide-react";

import { useAuth } from "@/contexts/AuthContext";

import { getBusiness } from "@/api/business";
import { getCertificateByBusiness, uploadCertificate } from "@/api/certificate";

import type { Business } from "@/types/business.types";
import type { Certificate } from "@/types/certificate.types";

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

  const [existingCertificate, setExistingCertificate] = useState<Certificate | null>(null);
  const [loadingCertificate, setLoadingCertificate] = useState(false);

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
  const selectedBusinessId = watch("businessId");

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

  const fetchCertificate = useCallback(async (businessId: string) => {
    if (!token || !businessId) {
      setExistingCertificate(null);
      return;
    }

    setLoadingCertificate(true);

    try {
      const response = await getCertificateByBusiness(Number(businessId), token);
      setExistingCertificate(response.data ?? null);
    } catch {
      setExistingCertificate(null);
    } finally {
      setLoadingCertificate(false);
    }
  }, [token]);

  useEffect(() => {
    fetchCertificate(selectedBusinessId);
  }, [selectedBusinessId, fetchCertificate]);

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

      fetchCertificate(data.businessId);
    } catch (err: any) {
      toast.error(err.message);
    } finally {
      setUploading(false);
    }
  };

  const hasExistingCertificate = existingCertificate !== null;

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

        {hasExistingCertificate && (
          <div className="flex items-center gap-2 rounded-md border p-3 bg-muted/50 max-w-xl">
            <BadgeCheck className="h-5 w-5 text-green-600 shrink-0" />
            <div className="text-sm">
              <span className="font-medium">Certificado actual:</span>{" "}
              <span className="text-muted-foreground">
                {existingCertificate.fileName ?? "Certificado cargado"}
              </span>
            </div>
          </div>
        )}

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
                loadingCertificate ||
                businessOptions.length === 0 ||
                !!businessError
              }
            >
              {uploading ? (
                <>
                  <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                  Subiendo...
                </>
              ) : hasExistingCertificate ? (
                "Actualizar certificado"
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
