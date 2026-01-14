import { useEffect, useState } from "react";

import { useAuth } from "@/contexts/AuthContext";

import { getPurchases } from "@/api/purchase";

import type { Purchase } from "@/types/purchase.type";

import { Card, CardContent } from "@/components/ui/card";

import AlertMessage from "@/components/shared/AlertMessage";
import DataTable from "@/components/shared/DataTable";

import PurchaseListHeader from "./PurchaseListHeader";
import { columns } from "./PurchaseListColumns";

export default function PurchaseListView() {
  const { token } = useAuth();

  const [data, setData] = useState<Purchase[]>([]);
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
      const purchases = await getPurchases(keyword, page, pageSize, token);

      if (!purchases) return;

      setData(purchases.data);
      setPage(purchases.pagination.page);
      setPageSize(purchases.pagination.limit);
      setTotalPages(purchases.pagination.totalPages);
    } catch (err: any) {
      setError(
        err.message ??
          "Aún no hay un endpoint público para listar compras, pero la vista ya está disponible"
      );
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
        <PurchaseListHeader
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
