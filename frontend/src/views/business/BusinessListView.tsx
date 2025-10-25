import { useEffect, useState } from "react";

import { useAuth } from "@/contexts/AuthContext";

import { getBusiness } from "@/api/business";

import type { Business } from "@/types/business.types";

import { Card, CardContent } from "@/components/ui/card";

import DataTable from "@/components/shared/DataTable";
import AlertMessage from "@/components/shared/AlertMessage";

import BusinessListHeader from "./BusinessListHeader";
import { columns } from "./BusinessListColumns";

export default function BusinessListView() {
  const { token } = useAuth();

  const [data, setData] = useState<Business[]>([]);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchData = async () => {
    if (!token) return;

    setLoading(true);

    try {
      const business = await getBusiness("", page, pageSize, token);

      if (!business) return;

      setData(business.data);
      setPage(business.pagination.page);
      setPageSize(business.pagination.limit);
      setTotalPages(business.pagination.totalPages);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, [page, pageSize, token]);

  return (
    <Card>
      <CardContent>
        <BusinessListHeader />
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
