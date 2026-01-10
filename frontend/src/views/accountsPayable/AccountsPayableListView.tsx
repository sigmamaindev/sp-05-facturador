import { useCallback, useEffect, useState } from "react";

import { useAuth } from "@/contexts/AuthContext";

import { getAccountsPayable } from "@/api/accountsPayable";

import type { AccountsPayable } from "@/types/accountsPayable.types";

import { Card, CardContent } from "@/components/ui/card";

import AlertMessage from "@/components/shared/AlertMessage";
import DataTable from "@/components/shared/DataTable";

import AccountsPayableListHeader from "./AccountsPayableListHeader";
import { columns } from "./AccountsPayableListColumns";

export default function AccountsPayableListView() {
  const { token } = useAuth();

  const [data, setData] = useState<AccountsPayable[]>([]);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(1);
  const [keyword, setKeyword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchData = useCallback(async () => {
    if (!token) return;

    setLoading(true);

    try {
      const accountsPayables = await getAccountsPayable(
        keyword,
        page,
        pageSize,
        token
      );

      if (!accountsPayables) return;

      setData(accountsPayables.data);
      setPage(accountsPayables.pagination.page);
      setPageSize(accountsPayables.pagination.limit);
      setTotalPages(accountsPayables.pagination.totalPages);
    } catch (err: unknown) {
      setError(
        err instanceof Error
          ? err.message
          : "Aún no hay un endpoint público para listar cuentas por pagar, pero la vista ya está disponible"
      );
    } finally {
      setLoading(false);
    }
  }, [keyword, page, pageSize, token]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  return (
    <Card>
      <CardContent>
        <AccountsPayableListHeader
          keyword={keyword}
          setKeyword={setKeyword}
          setPage={setPage}
        />
        {error ? (
          <AlertMessage message={error} variant="destructive" />
        ) : (
          <DataTable
            columns={columns}
            data={data}
            page={page}
            pageSize={pageSize}
            totalPages={totalPages}
            onPageChange={setPage}
            onPageSizeChange={setPageSize}
            loading={loading}
          />
        )}
      </CardContent>
    </Card>
  );
}
