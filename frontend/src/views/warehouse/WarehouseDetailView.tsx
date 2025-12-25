import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import { useAuth } from "@/contexts/AuthContext";

import { getWarehouseById } from "@/api/warehouse";

import type { Warehouse } from "@/types/warehouse.types";

import { Card, CardContent } from "@/components/ui/card";

import WarehouseDetailHeader from "./WarehouseDetailHeader";
import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";
import WarehouseDetailInfo from "./WarehouseDetailInfo";

export default function WarehouseDetailView() {
  const { id } = useParams<{ id: string }>();
  const { token } = useAuth();

  const [warehouse, setWarehouse] = useState<Warehouse | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchData = async () => {
    try {
      setLoading(true);

      const response = await getWarehouseById(Number(id), token!);

      setWarehouse(response.data);
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
        <WarehouseDetailHeader />
        {loading ? (
          <Loading />
        ) : error ? (
          <AlertMessage message={error} variant="destructive" />
        ) : !warehouse ? (
          <AlertMessage
            message="Los datos de la bodega no se han cargado completamente"
            variant="destructive"
          />
        ) : (
          <WarehouseDetailInfo warehouse={warehouse} />
        )}
      </CardContent>
    </Card>
  );
}

