import { useCallback, useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import { useAuth } from "@/contexts/AuthContext";

import { getAccountsReceivableById } from "@/api/accountsReceivable";

import type { AccountsReceivableDetail } from "@/types/accountsReceivable.types";

import { Card, CardContent } from "@/components/ui/card";

import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";

import AccountsReceivableDetailHeader from "./AccountsReceivableDetailHeader";
import AccountsReceivableDetailInfo from "./AccountsReceivableDetailInfo";

export default function AccountsReceivableDetailView() {
  const { id } = useParams<{ id: string }>();
  const { token } = useAuth();

  const [accountsReceivable, setAccountsReceivable] =
    useState<AccountsReceivableDetail | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchData = useCallback(async () => {
    if (!id || !token) return;

    const numericId = Number(id);
    if (Number.isNaN(numericId)) {
      setError("ID inválido");
      setAccountsReceivable(null);
      setLoading(false);
      return;
    }

    try {
      setLoading(true);
      setError(null);

      const response = await getAccountsReceivableById(numericId, token);

      setAccountsReceivable(response.data);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : "Error desconocido");
    } finally {
      setLoading(false);
    }
  }, [id, token]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  return (
    <Card>
      <CardContent>
        <AccountsReceivableDetailHeader />
        {loading ? (
          <Loading />
        ) : error ? (
          <AlertMessage message={error} variant="destructive" />
        ) : !accountsReceivable ? (
          <AlertMessage
            message="Los datos de la cuenta por cobrar no se han cargado completamente"
            variant="destructive"
          />
        ) : (
          <AccountsReceivableDetailInfo
            accountsReceivable={accountsReceivable}
          />
        )}
      </CardContent>
    </Card>
  );
}
