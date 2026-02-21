import { useCallback, useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import { useAuth } from "@/contexts/AuthContext";

import { getPurchasesReportDetail } from "@/api/report";

import type { PurchasesReportDetail } from "@/types/report.types";

import { Card, CardContent } from "@/components/ui/card";

import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";

import PurchasesReportDetailHeader from "./PurchasesReportDetailHeader";
import PurchasesReportDetailInfo from "./PurchasesReportDetailInfo";

export default function PurchasesReportDetailView() {
  const { id } = useParams<{ id: string }>();
  const { token } = useAuth();

  const [detail, setDetail] = useState<PurchasesReportDetail | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchData = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);

      const response = await getPurchasesReportDetail(Number(id), token!);

      setDetail(response.data);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : "Error desconocido");
    } finally {
      setLoading(false);
    }
  }, [id, token]);

  useEffect(() => {
    if (!id) {
      setError("Compra no encontrada");
      setLoading(false);
      return;
    }

    if (!token) {
      setError("No se encontró una sesión activa");
      setLoading(false);
      return;
    }

    fetchData();
  }, [id, token, fetchData]);

  return (
    <Card>
      <CardContent>
        <PurchasesReportDetailHeader
          id={detail?.id}
          sequential={detail?.sequential}
          token={token ?? undefined}
        />
        {loading ? (
          <Loading />
        ) : error ? (
          <AlertMessage message={error} variant="destructive" />
        ) : !detail ? (
          <AlertMessage
            message="Los datos de la compra no se han cargado completamente"
            variant="destructive"
          />
        ) : (
          <PurchasesReportDetailInfo detail={detail} />
        )}
      </CardContent>
    </Card>
  );
}
