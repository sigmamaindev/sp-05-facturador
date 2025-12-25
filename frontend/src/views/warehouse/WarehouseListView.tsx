import { useEffect, useState } from "react";

import { useAuth } from "@/contexts/AuthContext";

import { getWarehouses } from "@/api/warehouse";

import type { Warehouse } from "@/types/warehouse.types";

import { Card, CardContent } from "@/components/ui/card";

import AlertMessage from "@/components/shared/AlertMessage";
import DataTable from "@/components/shared/DataTable";

import WarehouseListHeader from "./WarehouseListHeader";
import { columns } from "./WarehouseListColumns";

export default function WarehouseListView() {
  const { token } = useAuth();

  const [data, setData] = useState<Warehouse[]>([]);
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
      const warehouses = await getWarehouses(keyword, page, pageSize, token);

      setData(warehouses.data);
      setPage(warehouses.pagination.page);
      setPageSize(warehouses.pagination.limit);
      setTotalPages(warehouses.pagination.totalPages);
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
        <WarehouseListHeader
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
