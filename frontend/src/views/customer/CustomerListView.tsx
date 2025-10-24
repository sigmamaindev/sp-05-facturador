import { useEffect, useState } from "react";

import { useAuth } from "@/contexts/AuthContext";

import { getCustomers } from "@/api/customer";

import { type Customer } from "@/types/customer.types";

import { Card, CardContent } from "@/components/ui/card";

import AlertMessage from "@/components/shared/AlertMessage";
import DataTable from "@/components/shared/DataTable";

import CustomerListHeader from "./CustomerListHeader";
import { columns } from "./CustomerListColumns";

export default function CustomerListView() {
  const { token } = useAuth();

  const [data, setData] = useState<Customer[]>([]);
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
      const customers = await getCustomers(keyword, page, pageSize, token);

      if (!customers) return;

      setData(customers.data);
      setPage(customers.pagination.page);
      setPageSize(customers.pagination.limit);
      setTotalPages(customers.pagination.totalPages);
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
        <CustomerListHeader
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
