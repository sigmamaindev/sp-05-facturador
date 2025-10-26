import { useEffect, useState } from "react";

import { useAuth } from "@/contexts/AuthContext";

import { getEstablishments } from "@/api/establishment";

import type { Establishment } from "@/types/establishment.types";

import { Card, CardContent } from "@/components/ui/card";

import AlertMessage from "@/components/shared/AlertMessage";
import DataTable from "@/components/shared/DataTable";

import EstablishmentListHeader from "./EstablishmentListHeader";
import { columns } from "./EstablishmentListColumns";

export default function EstablishmentListView() {
  const { token } = useAuth();

  const [data, setData] = useState<Establishment[]>([]);
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
      const establishments = await getEstablishments(
        keyword,
        page,
        pageSize,
        token
      );

      if (!establishments) return;

      setData(establishments.data);
      setPage(establishments.pagination.page);
      setPageSize(establishments.pagination.limit);
      setTotalPages(establishments.pagination.totalPages);
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
        <EstablishmentListHeader
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
