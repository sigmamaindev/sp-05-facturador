import { useEffect, useState } from "react";
import { useParams, useSearchParams } from "react-router-dom";

import { useAuth } from "@/contexts/AuthContext";

import { getEmissionPointById } from "@/api/emissionPoint";

import type { EmissionPoint } from "@/types/emissionPoint.types";

import { Card, CardContent } from "@/components/ui/card";

import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";
import EmissionPointUpdateHeader from "./EmissionPointUpdateHeader";
import EmissionPointUpdateForm from "./EmissionPointUpdateForm";

export default function EmissionPointUpdateView() {
  const { id } = useParams<{ id: string }>();
  const [searchParams] = useSearchParams();
  const establishmentId = searchParams.get("establishmentId");
  const { token } = useAuth();

  const [emissionPoint, setEmissionPoint] = useState<EmissionPoint | null>(
    null
  );
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchData = async () => {
    try {
      setLoading(true);

      const response = await getEmissionPointById(
        Number(id),
        Number(establishmentId),
        token!
      );

      setEmissionPoint(response.data);
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
        ) : !emissionPoint ? (
          <AlertMessage
            message="Los datos del punto de emisión no se han cargado completamente"
            variant="destructive"
          />
        ) : (
          <>
            <EmissionPointUpdateHeader code={emissionPoint.code} />
            <EmissionPointUpdateForm
              emissionPoint={emissionPoint}
              token={token}
            />
          </>
        )}
      </CardContent>
    </Card>
  );
}
