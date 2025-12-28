import { useEffect, useState } from "react";

import { useAuth } from "@/contexts/AuthContext";

import { getAccountsReceivable } from "@/api/accountsReceivable";

import type { AccountsReceivable } from "@/types/accountsReceivable.types";

import { Card, CardContent } from "@/components/ui/card";

import AlertMessage from "@/components/shared/AlertMessage";
import DataTable from "@/components/shared/DataTable";

import AccountsReceivableListHeader from "./AccountsReceivableListHeader";
import { columns } from "./AccountsReceivableListColumns";

export default function AccountsReceivableListView() {
  const { token } = useAuth();

  const [data, setData] = useState<AccountsReceivable[]>([]);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(1);
  const [keyword, setKeyword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchData = async () => {
    if (!token) return;

    setLoading(true);

    try {
      const accountsReceivables = await getAccountsReceivable(
        keyword,
        page,
        pageSize,
        token
      );

      if (!accountsReceivables) return;

      setData(accountsReceivables.data);
      setPage(accountsReceivables.pagination.page);
      setPageSize(accountsReceivables.pagination.limit);
      setTotalPages(accountsReceivables.pagination.totalPages);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, [keyword, page, pageSize, token]);

  return (
    <Card>
      <CardContent>
        <AccountsReceivableListHeader
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
