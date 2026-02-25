import { useCallback, useEffect, useState } from "react";

import { useAuth } from "@/contexts/AuthContext";

import { getSalesReport } from "@/api/report";

import type { SalesReport } from "@/types/report.types";

import { Card, CardContent } from "@/components/ui/card";

import AlertMessage from "@/components/shared/AlertMessage";
import DataTable from "@/components/shared/DataTable";

import SalesReportListHeader from "./SalesReportListHeader";
import { columns } from "./SalesReportListColumns";

export default function SalesReportListView() {
  const { token } = useAuth();

  const [data, setData] = useState<SalesReport[]>([]);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(1);
  const [keyword, setKeyword] = useState("");
  const [creditDays, setCreditDays] = useState("");
  const [dateFrom, setDateFrom] = useState("");
  const [dateTo, setDateTo] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchData = useCallback(async () => {
    if (!token) return;

    setLoading(true);

    try {
      const result = await getSalesReport(
        keyword,
        creditDays !== "" ? Number(creditDays) : null,
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
  }, [keyword, creditDays, dateFrom, dateTo, page, pageSize, token]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  return (
    <Card>
      <CardContent>
        <SalesReportListHeader
          keyword={keyword}
          creditDays={creditDays}
          dateFrom={dateFrom}
          dateTo={dateTo}
          token={token ?? ""}
          setPage={setPage}
          setKeyword={setKeyword}
          setCreditDays={setCreditDays}
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
