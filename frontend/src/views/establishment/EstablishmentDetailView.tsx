import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import { useAuth } from "@/contexts/AuthContext";

import { getEstablishmentById } from "@/api/establishment";

import type { Establishment } from "@/types/establishment.types";

import { Card, CardContent } from "@/components/ui/card";

import EstablishmentDetailHeader from "./EstablishmentDetailHeader";
import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";
import EstablishmentDetailInfo from "./EstablishmentDetailInfo";

export default function EstablishmentDetailView() {
  const { id } = useParams<{ id: string }>();
  const { token } = useAuth();

  const [establishment, setEstablishment] = useState<Establishment | null>(
    null
  );
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchData = async () => {
    try {
      setLoading(true);

      const response = await getEstablishmentById(Number(id), token!);

      setEstablishment(response.data);
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
        <EstablishmentDetailHeader />
        {loading ? (
          <Loading />
        ) : error ? (
          <AlertMessage message={error} variant="destructive" />
        ) : !establishment ? (
          <AlertMessage
            message="Los datos del establecimiento no se han cargado completamente"
            variant="destructive"
          />
        ) : (
          <EstablishmentDetailInfo establishment={establishment} />
        )}
      </CardContent>
    </Card>
  );
}
