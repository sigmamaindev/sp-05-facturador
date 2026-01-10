import { useCallback, useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import { useAuth } from "@/contexts/AuthContext";

import { getAccountsPayableById } from "@/api/accountsPayable";

import type { AccountsPayable } from "@/types/accountsPayable.types";

import { Card, CardContent } from "@/components/ui/card";

import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";

import AccountsPayableDetailHeader from "./AccountsPayableDetailHeader";
import AccountsPayableDetailInfo from "./AccountsPayableDetailInfo";

export default function AccountsPayableDetailView() {
  const { id } = useParams<{ id: string }>();
  const { token } = useAuth();

  const [accountsPayable, setAccountsPayable] = useState<AccountsPayable | null>(
    null
  );
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchData = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);

      const response = await getAccountsPayableById(Number(id), token!);

      setAccountsPayable(response.data);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : "Error desconocido");
    } finally {
      setLoading(false);
    }
  }, [id, token]);

  useEffect(() => {
    if (id && token) fetchData();
  }, [id, token, fetchData]);

  return (
    <Card>
      <CardContent>
        <AccountsPayableDetailHeader />
        {loading ? (
          <Loading />
        ) : error ? (
          <AlertMessage message={error} variant="destructive" />
        ) : !accountsPayable ? (
          <AlertMessage
            message="Los datos de la cuenta por pagar no se han cargado completamente"
            variant="destructive"
          />
        ) : (
          <AccountsPayableDetailInfo accountsPayable={accountsPayable} />
        )}
      </CardContent>
    </Card>
  );
}
