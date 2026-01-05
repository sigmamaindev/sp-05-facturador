import { useCallback, useEffect, useState } from "react";

import { useAuth } from "@/contexts/AuthContext";

import { getKardexMovements } from "@/api/kardex";

import type { KardexMovement } from "@/types/kardex.types";

import { Card, CardContent } from "@/components/ui/card";

import AlertMessage from "@/components/shared/AlertMessage";
import DataTable from "@/components/shared/DataTable";

import KardexListHeader from "./KardexListHeader";
import { columns } from "./KardexListColumns";

export default function KardexListView() {
  const { token } = useAuth();

  const [data, setData] = useState<KardexMovement[]>([]);
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
  }, [keyword, page, pageSize, token]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  return (
    <Card>
      <CardContent>
        <KardexListHeader
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
