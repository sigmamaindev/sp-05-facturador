import { useCallback, useEffect, useState } from "react";

import { useAuth } from "@/contexts/AuthContext";

import {
  getAccountsReceivable,
  getAccountsReceivableByCustomer,
} from "@/api/accountsReceivable";

import type {
  AccountsReceivable,
  AccountsReceivableCustomerSummary,
} from "@/types/accountsReceivable.types";

import { Card, CardContent } from "@/components/ui/card";

import AlertMessage from "@/components/shared/AlertMessage";
import DataTable from "@/components/shared/DataTable";

import AccountsReceivableListHeader from "./AccountsReceivableListHeader";
import { columns } from "./AccountsReceivableListColumns";
import { byCustomerColumns } from "./AccountsReceivableByCustomerColumns";

export default function AccountsReceivableListView() {
  const { token } = useAuth();

  const [viewMode, setViewMode] = useState<"invoice" | "customer">("invoice");
  const [invoiceData, setInvoiceData] = useState<AccountsReceivable[]>([]);
  const [customerData, setCustomerData] = useState<
    AccountsReceivableCustomerSummary[]
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
      if (viewMode === "invoice") {
        const accountsReceivables = await getAccountsReceivable(
          keyword,
          page,
          pageSize,
          token
        );

        if (!accountsReceivables) return;

        setInvoiceData(accountsReceivables.data);
        setPage(accountsReceivables.pagination.page);
        setPageSize(accountsReceivables.pagination.limit);
        setTotalPages(accountsReceivables.pagination.totalPages);
      } else {
        const accountsReceivablesByCustomer =
          await getAccountsReceivableByCustomer(keyword, page, pageSize, token);

        if (!accountsReceivablesByCustomer) return;

        setCustomerData(accountsReceivablesByCustomer.data);
        setPage(accountsReceivablesByCustomer.pagination.page);
        setPageSize(accountsReceivablesByCustomer.pagination.limit);
        setTotalPages(accountsReceivablesByCustomer.pagination.totalPages);
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
        <AccountsReceivableListHeader
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
            columns={viewMode === "invoice" ? columns : byCustomerColumns}
            data={viewMode === "invoice" ? invoiceData : customerData}
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
