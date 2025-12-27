import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import { useAuth } from "@/contexts/AuthContext";

import { getSupplierById } from "@/api/supplier";

import type { Supplier } from "@/types/supplier.types";

import { Card, CardContent } from "@/components/ui/card";

import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";

import SupplierDetailHeader from "./SupplierDetailHeader";
import SupplierDetailInfo from "./SupplierDetailInfo";

export default function SupplierDetailView() {
  const { id } = useParams<{ id: string }>();
  const { token } = useAuth();

  const [supplier, setSupplier] = useState<Supplier | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchData = async () => {
    try {
      setLoading(true);
      setError(null);

      const response = await getSupplierById(Number(id), token!);

      setSupplier(response.data);
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
        <SupplierDetailHeader />
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
          <SupplierDetailInfo supplier={supplier} />
        )}
      </CardContent>
    </Card>
  );
}

