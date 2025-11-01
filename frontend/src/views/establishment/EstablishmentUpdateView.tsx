import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import { useAuth } from "@/contexts/AuthContext";

import type { Establishment } from "@/types/establishment.types";

import { getEstablishmentById } from "@/api/establishment";

import { Card, CardContent } from "@/components/ui/card";

import EstablishmentUpdateHeader from "./EstablishmentUpdateHeader";
import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";
import EstablishmentUpdateForm from "./EstablishmentUpdateForm";

export default function EstablishmentUpdateView() {
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
          <>
            <EstablishmentUpdateHeader code={establishment.code} />
            <EstablishmentUpdateForm
              establishment={establishment}
              token={token}
            />
          </>
        )}
      </CardContent>
    </Card>
  );
}
