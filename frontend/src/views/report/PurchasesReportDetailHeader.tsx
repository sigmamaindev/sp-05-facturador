import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { ArrowLeftIcon, FileDown, FileSpreadsheet } from "lucide-react";

import { Button } from "@/components/ui/button";

import {
  downloadPurchasesReportPdf,
  downloadPurchasesReportExcel,
} from "@/api/report";

interface PurchasesReportDetailHeaderProps {
  id?: number;
  sequential?: string;
  token?: string;
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

export default function PurchasesReportDetailHeader({
  id,
  sequential,
  token,
}: PurchasesReportDetailHeaderProps) {
  const navigate = useNavigate();
  const [loadingPdf, setLoadingPdf] = useState(false);
  const [loadingExcel, setLoadingExcel] = useState(false);

  const canDownload = !!id && !!sequential && !!token;

  async function handlePdf() {
    if (!canDownload) return;

    setLoadingPdf(true);
    try {
      const blob = await downloadPurchasesReportPdf(id, token);
      triggerDownload(blob, `ReporteCompra_${sequential}.pdf`);
    } catch (err: unknown) {
      alert(err instanceof Error ? err.message : "No se pudo descargar el PDF");
    } finally {
      setLoadingPdf(false);
    }
  }

  async function handleExcel() {
    if (!canDownload) return;

    setLoadingExcel(true);
    try {
      const blob = await downloadPurchasesReportExcel(id, token);
      triggerDownload(blob, `ReporteCompra_${sequential}.xlsx`);
    } catch (err: unknown) {
      alert(
        err instanceof Error ? err.message : "No se pudo descargar el Excel"
      );
    } finally {
      setLoadingExcel(false);
    }
  }

  return (
    <div className="grid grid-cols-1 md:flex md:flex-row md:justify-between md:items-center items-center gap-4 pb-4">
      <div className="grid grid-cols-2 items-center gap-4 md:inline-flex w-auto">
        <h1 className="text-lg font-semibold">DETALLE COMPRA</h1>
      </div>
      <div className="inline-flex items-center gap-2 w-auto">
        {canDownload && (
          <>
            <Button
              variant="outline"
              onClick={handlePdf}
              disabled={loadingPdf}
              className="flex items-center gap-2"
            >
              <FileDown size={16} />
              {loadingPdf ? "Generando..." : "PDF"}
            </Button>
            <Button
              variant="outline"
              onClick={handleExcel}
              disabled={loadingExcel}
              className="flex items-center gap-2"
            >
              <FileSpreadsheet size={16} />
              {loadingExcel ? "Generando..." : "Excel"}
            </Button>
          </>
        )}
        <Button
          variant="outline"
          onClick={() => navigate("/reportes/compras")}
          className="flex items-center gap-2"
        >
          <ArrowLeftIcon size={16} />
          Volver al reporte
        </Button>
      </div>
    </div>
  );
}
