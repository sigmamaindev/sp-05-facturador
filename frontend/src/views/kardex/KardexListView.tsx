import { useCallback, useEffect, useState } from "react";

import { useAuth } from "@/contexts/AuthContext";

import { getKardexMovements, getKardexReport } from "@/api/kardex";

import type { KardexMovement, KardexReportRow } from "@/types/kardex.types";

import { Card, CardContent } from "@/components/ui/card";

import AlertMessage from "@/components/shared/AlertMessage";
import DataTable from "@/components/shared/DataTable";

import KardexListHeader from "./KardexListHeader";
import { columns } from "./KardexListColumns";
import { kardexReportColumns } from "./KardexReportColumns";

export default function KardexListView() {
  const { token } = useAuth();

  // Flat list state
  const [data, setData] = useState<KardexMovement[]>([]);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(1);
  const [keyword, setKeyword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Report mode state
  const [selectedProductId, setSelectedProductId] = useState<number | null>(
    null
  );
  const [selectedProductLabel, setSelectedProductLabel] = useState("");
  const [dateFrom, setDateFrom] = useState("");
  const [dateTo, setDateTo] = useState("");
  const [reportData, setReportData] = useState<KardexReportRow[]>([]);

  const isReportMode = selectedProductId !== null && dateFrom !== "" && dateTo !== "";

  function handleSetSelectedProduct(id: number | null, label: string) {
    setSelectedProductId(id);
    setSelectedProductLabel(label);
    if (id === null) {
      setReportData([]);
      setDateFrom("");
      setDateTo("");
    }
  }

  // Flat list fetch
  const fetchFlatData = useCallback(async () => {
    if (!token || selectedProductId !== null) return;

    setLoading(true);
    try {
      const movements = await getKardexMovements(keyword, page, pageSize, token);
      if (!movements) return;

      setData(movements.data);
      setPage(movements.pagination.page);
      setPageSize(movements.pagination.limit);
      setTotalPages(movements.pagination.totalPages);
      setError(null);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : "Error desconocido");
    } finally {
      setLoading(false);
    }
  }, [keyword, page, pageSize, token, selectedProductId]);

  // Report fetch
  const fetchReportData = useCallback(async () => {
    if (!token || !selectedProductId || !dateFrom || !dateTo) return;

    setLoading(true);
    try {
      const response = await getKardexReport(
        selectedProductId,
        dateFrom,
        dateTo,
        token
      );

      if (response.data) {
        setReportData(response.data.movements);
      }
      setError(null);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : "Error desconocido");
    } finally {
      setLoading(false);
    }
  }, [selectedProductId, dateFrom, dateTo, token]);

  useEffect(() => {
    if (selectedProductId === null) {
      fetchFlatData();
    }
  }, [fetchFlatData, selectedProductId]);

  useEffect(() => {
    if (isReportMode) {
      fetchReportData();
    }
  }, [fetchReportData, isReportMode]);

  return (
    <Card>
      <CardContent>
        <KardexListHeader
          keyword={keyword}
          setPage={setPage}
          setKeyword={setKeyword}
          selectedProductId={selectedProductId}
          selectedProductLabel={selectedProductLabel}
          setSelectedProduct={handleSetSelectedProduct}
          dateFrom={dateFrom}
          setDateFrom={setDateFrom}
          dateTo={dateTo}
          setDateTo={setDateTo}
        />
        {error ? (
          <AlertMessage message={error} variant="destructive" />
        ) : isReportMode ? (
          <DataTable
            columns={kardexReportColumns}
            data={reportData}
            page={1}
            pageSize={reportData.length || 10}
            totalPages={1}
            onPageChange={() => {}}
            onPageSizeChange={() => {}}
            loading={loading}
          />
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
