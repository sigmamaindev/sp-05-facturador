import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import { useAuth } from "@/contexts/AuthContext";

import { getPurchaseById } from "@/api/purchase";

import AlertMessage from "@/components/shared/AlertMessage";
// import PageLoader from "@/components/shared/PageLoa";

import type { Purchase } from "@/types/purchase.type";

import PurchaseDetailHeader from "./PurchaseDetailHeader";
import PurchaseDetailInfo from "./PurchaseDetailInfo";

export default function PurchaseDetailView() {
  const { id } = useParams();
  const { token } = useAuth();

  const [purchase, setPurchase] = useState<Purchase | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      if (!token || !id) return;

      try {
        const response = await getPurchaseById(Number(id), token);

        if (response.data) {
          setPurchase(response.data);
        } else {
          setError("No se pudo cargar la compra");
        }
      } catch (err: any) {
        setError(err.message ?? "No se pudo obtener la compra");
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [id, token]);

  // if (loading) return <PageLoader />;

  if (error || !purchase)
    return <AlertMessage message={error ?? "Compra no encontrada"} variant="destructive" />;

  return (
    <div className="space-y-4">
      <PurchaseDetailHeader documentNumber={purchase.documentNumber} />
      <PurchaseDetailInfo purchase={purchase} />
    </div>
  );
}
