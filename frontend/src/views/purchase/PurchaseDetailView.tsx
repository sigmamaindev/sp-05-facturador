import { useCallback, useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import { useAuth } from "@/contexts/AuthContext";

import { getPurchaseById } from "@/api/purchase";

import type { Purchase } from "@/types/purchase.type";

import { Card, CardContent } from "@/components/ui/card";

import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";

import PurchaseDetailHeader from "./PurchaseDetailHeader";
import PurchaseDetailInfo from "./PurchaseDetailInfo";

export default function PurchaseDetailView() {
  const { id } = useParams<{ id: string }>();
  const { token } = useAuth();

  const [purchase, setPurchase] = useState<Purchase | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchData = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);

      const response = await getPurchaseById(Number(id), token!);

      setPurchase(response.data);
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
        <PurchaseDetailHeader />
        {loading ? (
          <Loading />
        ) : error ? (
          <AlertMessage message={error} variant="destructive" />
        ) : !purchase ? (
          <AlertMessage
            message="Los datos de la compra no se han cargado completamente"
            variant="destructive"
          />
        ) : (
          <PurchaseDetailInfo purchase={purchase} />
        )}
      </CardContent>
    </Card>
  );
}
