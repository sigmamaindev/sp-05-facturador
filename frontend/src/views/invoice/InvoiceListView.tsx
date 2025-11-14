import { useEffect, useState } from "react";

import { useAuth } from "@/contexts/AuthContext";

import { getInvoices } from "@/api/invoice";

import type { Invoice } from "@/types/invoice.type";

import { Card, CardContent } from "@/components/ui/card";

import AlertMessage from "@/components/shared/AlertMessage";
import DataTable from "@/components/shared/DataTable";

import InvoiceListHeader from "./InvoiceListHeader";
import { columns } from "./InvoiceListColumns";

export default function InvoiceListView() {
  const { token } = useAuth();

  const [data, setData] = useState<Invoice[]>([]);
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
      const invoices = await getInvoices(keyword, page, pageSize, token);

      if (!invoices) return;

      setData(invoices.data);
      setPage(invoices.pagination.page);
      setPageSize(invoices.pagination.limit);
      setTotalPages(invoices.pagination.totalPages);
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
        <InvoiceListHeader
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
