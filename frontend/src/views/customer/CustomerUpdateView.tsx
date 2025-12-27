import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import { useAuth } from "@/contexts/AuthContext";

import type { Customer } from "@/types/customer.types";

import { getCustomerById } from "@/api/customer";

import { Card, CardContent } from "@/components/ui/card";

import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";

import CustomerUpdateHeader from "./CustomerUpdateHeader";
import CustomerUpdateForm from "./CustomerUpdateForm";

export default function CustomerUpdateView() {
  const { id } = useParams<{ id: string }>();
  const { token } = useAuth();

  const [customer, setCustomer] = useState<Customer | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchData = async () => {
    try {
      setLoading(true);

      const response = await getCustomerById(Number(id), token!);

      setCustomer(response.data);
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
        ) : !customer ? (
          <AlertMessage
            message="Los datos del cliente no se han cargado completamente"
            variant="destructive"
          />
        ) : (
          <>
            <CustomerUpdateHeader document={customer.document} />
            <CustomerUpdateForm customer={customer} token={token} />
          </>
        )}
      </CardContent>
    </Card>
  );
}

