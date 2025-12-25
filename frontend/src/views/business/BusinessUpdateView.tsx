import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import { useAuth } from "@/contexts/AuthContext";

import type { Business } from "@/types/business.types";

import { getBusinessById } from "@/api/business";

import { Card, CardContent } from "@/components/ui/card";

import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";

import BusinessUpdateHeader from "./BusinessUpdateHeader";
import BusinessUpdateForm from "./BusinessUpdateForm";

export default function BusinessUpdateView() {
  const { id } = useParams<{ id: string }>();
  const { token } = useAuth();

  const [business, setBusiness] = useState<Business | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchData = async () => {
    try {
      setLoading(true);

      const response = await getBusinessById(Number(id), token!);

      setBusiness(response.data);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : "Error desconocido");
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
        {loading ? (
          <Loading />
        ) : error ? (
          <AlertMessage message={error} variant="destructive" />
        ) : !business ? (
          <AlertMessage
            message="Los datos de la empresa no se han cargado completamente"
            variant="destructive"
          />
        ) : (
          <>
            <BusinessUpdateHeader document={business.document} />
            <BusinessUpdateForm business={business} token={token} />
          </>
        )}
      </CardContent>
    </Card>
  );
}
