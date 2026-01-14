import { useEffect, useMemo, useState } from "react";

import { useAuth } from "@/contexts/AuthContext";

import { downloadAtsPurchasesXml, getAtsPurchases } from "@/api/ats";

import type { AtsPurchase } from "@/types/ats.types";

import { Card, CardContent } from "@/components/ui/card";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

import AlertMessage from "@/components/shared/AlertMessage";
import DataTable from "@/components/shared/DataTable";

import { atsColumns } from "./PurchaseAtsColumns";

interface PurchaseAtsModalProps {
  open: boolean;
  onClose: () => void;
  year: number;
  month: number;
  setYear: React.Dispatch<React.SetStateAction<number>>;
  setMonth: React.Dispatch<React.SetStateAction<number>>;
}

function clampYear(year: number) {
  return Math.min(2100, Math.max(2000, year));
}

function clampMonth(month: number) {
  return Math.min(12, Math.max(1, month));
}

export default function PurchaseAtsModal({
  open,
  onClose,
  year,
  month,
  setYear,
  setMonth,
}: PurchaseAtsModalProps) {
  const { token } = useAuth();

  const [data, setData] = useState<AtsPurchase[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);

  const totalPages = Math.max(1, Math.ceil(data.length / pageSize));

  const pagedData = useMemo(() => {
    const start = (page - 1) * pageSize;
    return data.slice(start, start + pageSize);
  }, [data, page, pageSize]);

  const fetchData = async () => {
    if (!token) return;

    setLoading(true);
    setError(null);

    try {
      const response = await getAtsPurchases(year, month, token);

      if (!response.success) {
        throw new Error(response.error || response.message || "Error en la API");
      }

      setData(response.data ?? []);
      setPage(1);
    } catch (err: any) {
      setError(err.message ?? "No se pudo cargar la información del ATS");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!open || !token) return;

    let cancelled = false;

    (async () => {
      setLoading(true);
      setError(null);

      try {
        const response = await getAtsPurchases(year, month, token);

        if (!response.success) {
          throw new Error(
            response.error || response.message || "Error en la API"
          );
        }

        if (!cancelled) {
          setData(response.data ?? []);
          setPage(1);
        }
      } catch (err: any) {
        if (!cancelled) {
          setError(err.message ?? "No se pudo cargar la información del ATS");
        }
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();

    return () => {
      cancelled = true;
    };
  }, [open, year, month, token]);

  useEffect(() => {
    if (page > totalPages) setPage(totalPages);
  }, [page, totalPages]);

  const handleDownloadXml = async () => {
    if (!token) return;

    try {
      const file = await downloadAtsPurchasesXml(year, month, token);
      const url = window.URL.createObjectURL(file);
      const link = document.createElement("a");

      link.href = url;
      link.download = `ATS_Compras_${year}_${String(month).padStart(2, "0")}.xml`;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);

      window.URL.revokeObjectURL(url);
    } catch (err: any) {
      alert(err.message ?? "No se pudo descargar el XML del ATS");
    }
  };

  return (
    <Dialog open={open} onOpenChange={onClose}>
      <DialogContent className="!max-w-5xl">
        <DialogHeader>
          <DialogTitle>ATS - Compras</DialogTitle>
        </DialogHeader>

        <Card>
          <CardContent>
            <div className="flex flex-col gap-4">
              <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-2">
                <div className="flex flex-col sm:flex-row sm:items-center gap-2">
                  <div className="text-sm text-muted-foreground">
                    Periodo (año / mes)
                  </div>
                  <div className="flex items-center gap-2">
                    <Input
                      type="number"
                      inputMode="numeric"
                      min={2000}
                      max={2100}
                      value={year}
                      onChange={(e) => {
                        const next = Number(e.target.value);
                        if (!Number.isFinite(next)) return;
                        setYear(clampYear(next));
                      }}
                      className="w-28"
                      aria-label="Año ATS"
                    />
                    <Input
                      type="number"
                      inputMode="numeric"
                      min={1}
                      max={12}
                      value={month}
                      onChange={(e) => {
                        const next = Number(e.target.value);
                        if (!Number.isFinite(next)) return;
                        setMonth(clampMonth(next));
                      }}
                      className="w-24"
                      aria-label="Mes ATS"
                    />
                    <Button variant="outline" onClick={fetchData} disabled={loading}>
                      Actualizar
                    </Button>
                  </div>
                </div>

                <div className="flex gap-2">
                  <Button variant="outline" onClick={handleDownloadXml}>
                    Descargar XML
                  </Button>
                </div>
              </div>

              {error ? (
                <AlertMessage message={error} variant="destructive" />
              ) : (
                <DataTable
                  columns={atsColumns}
                  data={pagedData}
                  page={page}
                  pageSize={pageSize}
                  totalPages={totalPages}
                  onPageChange={setPage}
                  onPageSizeChange={(size) => {
                    setPage(1);
                    setPageSize(size);
                  }}
                  loading={loading}
                />
              )}
            </div>
          </CardContent>
        </Card>
      </DialogContent>
    </Dialog>
  );
}
