import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import { useAuth } from "@/contexts/AuthContext";

import { getCustomerById } from "@/api/customer";

import type { Customer } from "@/types/customer.types";

import { Card, CardContent } from "@/components/ui/card";

import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";

import CustomerDetailHeader from "./CustomerDetailHeader";
import CustomerDetailInfo from "./CustomerDetailInfo";

export default function CustomerDetailView() {
  const { id } = useParams<{ id: string }>();
  const { token } = useAuth();

  const [customer, setCustomer] = useState<Customer | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchData = async () => {
    try {
      setLoading(true);
      setError(null);

      const response = await getCustomerById(Number(id), token!);

      setCustomer(response.data);
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
        <CustomerDetailHeader />
        {loading ? (
          <Loading />
        ) : error ? (
          <AlertMessage message={error} variant="destructive" />
        ) : !customer ? (
          <AlertMessage
            message="Los datos del cliente no se han cargado completamente"
            variant="destructive"
          />
        ) : (
          <CustomerDetailInfo customer={customer} />
        )}
      </CardContent>
    </Card>
  );
}

