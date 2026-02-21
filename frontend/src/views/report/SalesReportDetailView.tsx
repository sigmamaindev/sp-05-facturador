import { useCallback, useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import { useAuth } from "@/contexts/AuthContext";

import { getSalesReportDetail } from "@/api/report";

import type { SalesReportDetail } from "@/types/report.types";

import { Card, CardContent } from "@/components/ui/card";

import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";

import SalesReportDetailHeader from "./SalesReportDetailHeader";
import SalesReportDetailInfo from "./SalesReportDetailInfo";

export default function SalesReportDetailView() {
  const { id } = useParams<{ id: string }>();
  const { token } = useAuth();

  const [detail, setDetail] = useState<SalesReportDetail | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchData = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);

      const response = await getSalesReportDetail(Number(id), token!);

      setDetail(response.data);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : "Error desconocido");
    } finally {
      setLoading(false);
    }
  }, [id, token]);

  useEffect(() => {
    if (!id) {
      setError("Venta no encontrada");
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
        <SalesReportDetailHeader />
        {loading ? (
          <Loading />
        ) : error ? (
          <AlertMessage message={error} variant="destructive" />
        ) : !detail ? (
          <AlertMessage
            message="Los datos de la venta no se han cargado completamente"
            variant="destructive"
          />
        ) : (
          <SalesReportDetailInfo detail={detail} />
        )}
      </CardContent>
    </Card>
  );
}
