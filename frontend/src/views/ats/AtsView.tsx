import { useEffect, useMemo, useState } from "react";

import { useAuth } from "@/contexts/AuthContext";

import {
  downloadAtsPurchasesXml,
  downloadAtsSalesXml,
  getAtsPurchases,
  getAtsSales,
} from "@/api/ats";

import type { AtsPurchase, AtsSale } from "@/types/ats.types";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

import AlertMessage from "@/components/shared/AlertMessage";
import DataTable from "@/components/shared/DataTable";

import { atsPurchasesColumns } from "./AtsPurchasesColumns";
import { atsSalesColumns } from "./AtsSalesColumns";

function clampYear(year: number) {
  return Math.min(2100, Math.max(2000, year));
}

function clampMonth(month: number) {
  return Math.min(12, Math.max(1, month));
}

function downloadBlob(blob: Blob, fileName: string) {
  const url = window.URL.createObjectURL(blob);
  const link = document.createElement("a");

  link.href = url;
  link.download = fileName;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);

  window.URL.revokeObjectURL(url);
}

function getErrorMessage(error: unknown, fallback: string) {
  if (error instanceof Error && error.message) return error.message;
  return fallback;
}

export default function AtsView() {
  const { token, user } = useAuth();

  const hasPermission =
    user?.roles?.includes("SuperAdmin") || user?.roles?.includes("Admin");

  const now = new Date();
  const [year, setYear] = useState(now.getFullYear());
  const [month, setMonth] = useState(now.getMonth() + 1);

  const [purchases, setPurchases] = useState<AtsPurchase[]>([]);
  const [sales, setSales] = useState<AtsSale[]>([]);

  const [loadingPurchases, setLoadingPurchases] = useState(false);
  const [loadingSales, setLoadingSales] = useState(false);
  const [purchaseError, setPurchaseError] = useState<string | null>(null);
  const [salesError, setSalesError] = useState<string | null>(null);

  const [purchasePage, setPurchasePage] = useState(1);
  const [purchasePageSize, setPurchasePageSize] = useState(10);
  const [salesPage, setSalesPage] = useState(1);
  const [salesPageSize, setSalesPageSize] = useState(10);

  const purchaseTotalPages = Math.max(
    1,
    Math.ceil(purchases.length / purchasePageSize),
  );
  const salesTotalPages = Math.max(1, Math.ceil(sales.length / salesPageSize));

  const purchasePagedData = useMemo(() => {
    const safePage = Math.min(purchaseTotalPages, Math.max(1, purchasePage));
    const start = (safePage - 1) * purchasePageSize;
    return purchases.slice(start, start + purchasePageSize);
  }, [purchases, purchasePage, purchasePageSize, purchaseTotalPages]);

  const salesPagedData = useMemo(() => {
    const safePage = Math.min(salesTotalPages, Math.max(1, salesPage));
    const start = (safePage - 1) * salesPageSize;
    return sales.slice(start, start + salesPageSize);
  }, [sales, salesPage, salesPageSize, salesTotalPages]);

  useEffect(() => {
    if (purchasePage > purchaseTotalPages) setPurchasePage(purchaseTotalPages);
  }, [purchasePage, purchaseTotalPages]);

  useEffect(() => {
    if (salesPage > salesTotalPages) setSalesPage(salesTotalPages);
  }, [salesPage, salesTotalPages]);

  const fetchData = async () => {
    if (!token) return;

    setLoadingPurchases(true);
    setLoadingSales(true);
    setPurchaseError(null);
    setSalesError(null);

    const [purchasesResult, salesResult] = await Promise.allSettled([
      getAtsPurchases(year, month, token),
      getAtsSales(year, month, token),
    ]);

    if (purchasesResult.status === "fulfilled") {
      const response = purchasesResult.value;
      if (!response.success) {
        setPurchaseError(response.error || response.message || "Error en la API");
        setPurchases([]);
      } else {
        setPurchases(response.data ?? []);
        setPurchasePage(1);
      }
    } else {
      setPurchaseError(
        getErrorMessage(
          purchasesResult.reason,
          "No se pudo cargar la información del ATS (compras)",
        ),
      );
      setPurchases([]);
    }

    if (salesResult.status === "fulfilled") {
      const response = salesResult.value;
      if (!response.success) {
        setSalesError(response.error || response.message || "Error en la API");
        setSales([]);
      } else {
        setSales(response.data ?? []);
        setSalesPage(1);
      }
    } else {
      setSalesError(
        getErrorMessage(
          salesResult.reason,
          "No se pudo cargar la información del ATS (ventas)",
        ),
      );
      setSales([]);
    }

    setLoadingPurchases(false);
    setLoadingSales(false);
  };

  const handleDownloadPurchasesXml = async () => {
    if (!token) return;

    try {
      const file = await downloadAtsPurchasesXml(year, month, token);
      downloadBlob(
        file,
        `ATS_Compras_${year}_${String(month).padStart(2, "0")}.xml`,
      );
    } catch (err: unknown) {
      alert(
        getErrorMessage(err, "No se pudo descargar el XML del ATS (compras)"),
      );
    }
  };

  const handleDownloadSalesXml = async () => {
    if (!token) return;

    try {
      const file = await downloadAtsSalesXml(year, month, token);
      downloadBlob(
        file,
        `ATS_Ventas_${year}_${String(month).padStart(2, "0")}.xml`,
      );
    } catch (err: unknown) {
      alert(getErrorMessage(err, "No se pudo descargar el XML del ATS (ventas)"));
    }
  };

  return (
    <Card>
      <CardContent>
        <div className="flex flex-col gap-6">
          <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-3 pb-2">
            <div className="flex flex-col gap-1">
              <h1 className="text-lg font-semibold">ATS</h1>
              <div className="text-sm text-muted-foreground">
                Anexo Transaccional Simplificado (Compras / Ventas)
              </div>
            </div>

            <div className="flex flex-col sm:flex-row sm:items-center gap-2">
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
              </div>
              <Button
                variant="outline"
                onClick={fetchData}
                disabled={!token || !hasPermission}
              >
                Actualizar
              </Button>
            </div>
          </div>

          {!hasPermission ? (
            <AlertMessage
              variant="destructive"
              message="No tienes permisos para acceder al ATS."
            />
          ) : (
            <div className="grid grid-cols-1 xl:grid-cols-2 gap-6">
              <Card>
                <CardHeader className="flex flex-row items-center justify-between">
                  <CardTitle>ATS - Compras</CardTitle>
                  <Button variant="outline" onClick={handleDownloadPurchasesXml}>
                    Descargar XML
                  </Button>
                </CardHeader>
                <CardContent>
                  {purchaseError ? (
                    <AlertMessage message={purchaseError} variant="destructive" />
                  ) : (
                    <DataTable
                      columns={atsPurchasesColumns}
                      data={purchasePagedData}
                      page={purchasePage}
                      pageSize={purchasePageSize}
                      totalPages={purchaseTotalPages}
                      onPageChange={setPurchasePage}
                      onPageSizeChange={(size) => {
                        setPurchasePage(1);
                        setPurchasePageSize(size);
                      }}
                      loading={loadingPurchases}
                    />
                  )}
                </CardContent>
              </Card>

              <Card>
                <CardHeader className="flex flex-row items-center justify-between">
                  <CardTitle>ATS - Ventas</CardTitle>
                  <Button variant="outline" onClick={handleDownloadSalesXml}>
                    Descargar XML
                  </Button>
                </CardHeader>
                <CardContent>
                  {salesError ? (
                    <AlertMessage message={salesError} variant="destructive" />
                  ) : (
                    <DataTable
                      columns={atsSalesColumns}
                      data={salesPagedData}
                      page={salesPage}
                      pageSize={salesPageSize}
                      totalPages={salesTotalPages}
                      onPageChange={setSalesPage}
                      onPageSizeChange={(size) => {
                        setSalesPage(1);
                        setSalesPageSize(size);
                      }}
                      loading={loadingSales}
                    />
                  )}
                </CardContent>
              </Card>
            </div>
          )}
        </div>
      </CardContent>
    </Card>
  );
}
