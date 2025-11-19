import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import { useAuth } from "@/contexts/AuthContext";

import { getInvoiceById } from "@/api/invoice";

import type { Invoice } from "@/types/invoice.type";

import { Card, CardContent } from "@/components/ui/card";

import InvoiceDetailHeader from "./InvoiceDetailHeader";
import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";
import InvoiceDetailInfo from "./InvoiceDetailInfo";

export default function InvoiceDetailView() {
  const { id } = useParams<{ id: string }>();
  const { token } = useAuth();

  const [invoice, setInvoice] = useState<Invoice | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchData = async () => {
    try {
      setLoading(true);

      const response = await getInvoiceById(Number(id), token!);

      setInvoice(response.data);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (id && token) fetchData();
  }, [id, token]);

  return (
    <Card>
      <CardContent>
        <InvoiceDetailHeader />
        {loading ? (
          <Loading />
        ) : error ? (
          <AlertMessage message={error} variant="destructive" />
        ) : !invoice ? (
          <AlertMessage
            message="Los datos de la factura no se han cargado completamente"
            variant="destructive"
          />
        ) : (
          <InvoiceDetailInfo invoice={invoice} />
        )}
      </CardContent>
    </Card>
  );
}
