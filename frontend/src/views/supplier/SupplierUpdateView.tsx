import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import { useAuth } from "@/contexts/AuthContext";

import type { Supplier } from "@/types/supplier.types";

import { getSupplierById } from "@/api/supplier";

import { Card, CardContent } from "@/components/ui/card";

import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";

import SupplierUpdateHeader from "./SupplierUpdateHeader";
import SupplierUpdateForm from "./SupplierUpdateForm";

export default function SupplierUpdateView() {
  const { id } = useParams<{ id: string }>();
  const { token } = useAuth();

  const [supplier, setSupplier] = useState<Supplier | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchData = async () => {
    try {
      setLoading(true);

      const response = await getSupplierById(Number(id), token!);

      setSupplier(response.data);
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
        ) : !supplier ? (
          <AlertMessage
            message="Los datos del proveedor no se han cargado completamente"
            variant="destructive"
          />
        ) : (
          <>
            <SupplierUpdateHeader document={supplier.document} />
            <SupplierUpdateForm supplier={supplier} token={token} />
          </>
        )}
      </CardContent>
    </Card>
  );
}

