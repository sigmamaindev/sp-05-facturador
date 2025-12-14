import { useEffect, useState } from "react";

import { useAuth } from "@/contexts/AuthContext";

import { getSuppliers } from "@/api/supplier";

import type { Supplier } from "@/types/supplier.types";

import { Card, CardContent } from "@/components/ui/card";

import AlertMessage from "@/components/shared/AlertMessage";
import DataTable from "@/components/shared/DataTable";

import SupplierListHeader from "./SupplierListHeader";
import { columns } from "./SupplierListColumns";

export default function SupplierListView() {
  const { token } = useAuth();

  const [data, setData] = useState<Supplier[]>([]);
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
      const suppliers = await getSuppliers(keyword, page, pageSize, token);

      if (!suppliers) return;

      setData(suppliers.data);
      setPage(suppliers.pagination.page);
      setPageSize(suppliers.pagination.limit);
      setTotalPages(suppliers.pagination.totalPages);
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
        <SupplierListHeader
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
