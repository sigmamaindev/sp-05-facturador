import { useCallback, useEffect, useState } from "react";

import { useAuth } from "@/contexts/AuthContext";

import { getAccountsPayable, getAccountsPayableBySupplier } from "@/api/accountsPayable";

import type {
  AccountsPayable,
  AccountsPayableSupplierSummary,
} from "@/types/accountsPayable.types";

import { Card, CardContent } from "@/components/ui/card";

import AlertMessage from "@/components/shared/AlertMessage";
import DataTable from "@/components/shared/DataTable";

import AccountsPayableListHeader from "./AccountsPayableListHeader";
import { columns } from "./AccountsPayableListColumns";
import { bySupplierColumns } from "./AccountsPayableBySupplierColumns";

export default function AccountsPayableListView() {
  const { token } = useAuth();

  const [viewMode, setViewMode] = useState<"purchase" | "supplier">("purchase");
  const [purchaseData, setPurchaseData] = useState<AccountsPayable[]>([]);
  const [supplierData, setSupplierData] = useState<
    AccountsPayableSupplierSummary[]
  >([]);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(1);
  const [keyword, setKeyword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchData = useCallback(async () => {
    if (!token) return;

    setLoading(true);
    setError(null);

    try {
      if (viewMode === "purchase") {
        const accountsPayables = await getAccountsPayable(
          keyword,
          page,
          pageSize,
          token
        );

        if (!accountsPayables) return;

        setPurchaseData(accountsPayables.data);
        setPage(accountsPayables.pagination.page);
        setPageSize(accountsPayables.pagination.limit);
        setTotalPages(accountsPayables.pagination.totalPages);
      } else {
        const accountsPayablesBySupplier = await getAccountsPayableBySupplier(
          keyword,
          page,
          pageSize,
          token
        );

        if (!accountsPayablesBySupplier) return;

        setSupplierData(accountsPayablesBySupplier.data);
        setPage(accountsPayablesBySupplier.pagination.page);
        setPageSize(accountsPayablesBySupplier.pagination.limit);
        setTotalPages(accountsPayablesBySupplier.pagination.totalPages);
      }
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : "Error desconocido");
    } finally {
      setLoading(false);
    }
  }, [keyword, page, pageSize, token, viewMode]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  return (
    <Card>
      <CardContent>
        <AccountsPayableListHeader
          keyword={keyword}
          viewMode={viewMode}
          setKeyword={setKeyword}
          setPage={setPage}
          setViewMode={setViewMode}
        />
        {error ? (
          <AlertMessage message={error} variant="destructive" />
        ) : (
          <DataTable
            columns={viewMode === "purchase" ? columns : bySupplierColumns}
            data={viewMode === "purchase" ? purchaseData : supplierData}
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
