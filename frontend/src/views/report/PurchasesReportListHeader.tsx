import React, { useState } from "react";
import { FileDown, FileSpreadsheet } from "lucide-react";

import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

import {
  downloadPurchasesReportPdf,
  downloadPurchasesReportExcel,
} from "@/api/report";

interface PurchasesReportListHeaderProps {
  keyword: string;
  dateFrom: string;
  dateTo: string;
  token: string;
  setPage: React.Dispatch<React.SetStateAction<number>>;
  setKeyword: React.Dispatch<React.SetStateAction<string>>;
  setDateFrom: React.Dispatch<React.SetStateAction<string>>;
  setDateTo: React.Dispatch<React.SetStateAction<string>>;
}

function triggerDownload(blob: Blob, fileName: string) {
  const url = window.URL.createObjectURL(blob);
  const link = document.createElement("a");

  link.href = url;
  link.download = fileName;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);

  window.URL.revokeObjectURL(url);
}

export default function PurchasesReportListHeader({
  keyword,
  dateFrom,
  dateTo,
  token,
  setPage,
  setKeyword,
  setDateFrom,
  setDateTo,
}: PurchasesReportListHeaderProps) {
  const [loadingPdf, setLoadingPdf] = useState(false);
  const [loadingExcel, setLoadingExcel] = useState(false);

  function handleChange(setter: React.Dispatch<React.SetStateAction<string>>) {
    return (e: React.ChangeEvent<HTMLInputElement>) => {
      setPage(1);
      setter(e.target.value);
    };
  }

  const hasFilters = keyword || dateFrom || dateTo;

  function clearFilters() {
    setPage(1);
    setKeyword("");
    setDateFrom("");
    setDateTo("");
  }

  async function handlePdf() {
    setLoadingPdf(true);
    try {
      const blob = await downloadPurchasesReportPdf(keyword, dateFrom, dateTo, token);
      triggerDownload(blob, "ReporteCompras.pdf");
    } catch (err: unknown) {
      alert(err instanceof Error ? err.message : "No se pudo descargar el PDF");
    } finally {
      setLoadingPdf(false);
    }
  }

  async function handleExcel() {
    setLoadingExcel(true);
    try {
      const blob = await downloadPurchasesReportExcel(keyword, dateFrom, dateTo, token);
      triggerDownload(blob, "ReporteCompras.xlsx");
    } catch (err: unknown) {
      alert(err instanceof Error ? err.message : "No se pudo descargar el Excel");
    } finally {
      setLoadingExcel(false);
    }
  }

  return (
    <div className="flex flex-col gap-4 pb-4">
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-3">
        <h1 className="text-lg font-semibold">REPORTE DE COMPRAS</h1>
        <div className="flex items-center gap-2">
          <Button
            variant="outline"
            size="sm"
            onClick={handlePdf}
            disabled={loadingPdf}
            className="flex items-center gap-2"
          >
            <FileDown size={16} />
            {loadingPdf ? "Generando..." : "PDF"}
          </Button>
          <Button
            variant="outline"
            size="sm"
            onClick={handleExcel}
            disabled={loadingExcel}
            className="flex items-center gap-2"
          >
            <FileSpreadsheet size={16} />
            {loadingExcel ? "Generando..." : "Excel"}
          </Button>
        </div>
      </div>

      <div className="flex flex-col md:flex-row md:flex-wrap md:items-center gap-3">
        <Input
          placeholder="Buscar por proveedor o documento..."
          value={keyword}
          onChange={handleChange(setKeyword)}
          className="max-w-sm"
        />
        <div className="flex items-center gap-2">
          <Input
            type="date"
            value={dateFrom}
            onChange={handleChange(setDateFrom)}
            className="w-44"
            aria-label="Desde"
          />
          <span className="text-muted-foreground text-sm">—</span>
          <Input
            type="date"
            value={dateTo}
            onChange={handleChange(setDateTo)}
            className="w-44"
            aria-label="Hasta"
          />
        </div>
        {hasFilters && (
          <Button type="button" variant="outline" size="sm" onClick={clearFilters}>
            Limpiar filtros
          </Button>
        )}
      </div>
    </div>
  );
}
