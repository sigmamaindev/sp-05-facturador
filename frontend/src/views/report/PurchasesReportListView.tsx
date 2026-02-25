import { useCallback, useEffect, useState } from "react";

import { useAuth } from "@/contexts/AuthContext";

import { getPurchasesReport } from "@/api/report";

import type { PurchasesReport } from "@/types/report.types";

import { Card, CardContent } from "@/components/ui/card";

import AlertMessage from "@/components/shared/AlertMessage";
import DataTable from "@/components/shared/DataTable";

import PurchasesReportListHeader from "./PurchasesReportListHeader";
import { columns } from "./PurchasesReportListColumns";

export default function PurchasesReportListView() {
  const { token } = useAuth();

  const [data, setData] = useState<PurchasesReport[]>([]);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(1);
  const [keyword, setKeyword] = useState("");
  const [dateFrom, setDateFrom] = useState("");
  const [dateTo, setDateTo] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchData = useCallback(async () => {
    if (!token) return;

    setLoading(true);

    try {
      const result = await getPurchasesReport(
        keyword,
        dateFrom,
        dateTo,
        page,
        pageSize,
        token
      );

      if (!result) return;

      setData(result.data);
      setPage(result.pagination.page);
      setPageSize(result.pagination.limit);
      setTotalPages(result.pagination.totalPages);
      setError(null);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : "Error desconocido");
    } finally {
      setLoading(false);
    }
  }, [keyword, dateFrom, dateTo, page, pageSize, token]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  return (
    <Card>
      <CardContent>
        <PurchasesReportListHeader
          keyword={keyword}
          dateFrom={dateFrom}
          dateTo={dateTo}
          token={token ?? ""}
          setPage={setPage}
          setKeyword={setKeyword}
          setDateFrom={setDateFrom}
          setDateTo={setDateTo}
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
