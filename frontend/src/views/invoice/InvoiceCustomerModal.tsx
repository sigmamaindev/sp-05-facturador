import { useEffect, useState } from "react";

import { useAuth } from "@/contexts/AuthContext";

import { getCustomers } from "@/api/customer";

import type { Customer } from "@/types/customer.types";

import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";

import AlertMessage from "@/components/shared/AlertMessage";
import DataTable from "@/components/shared/DataTable";

import InvoiceCustomerModalHeader from "./InvoiceCustomerModalHeader";
import { columns } from "./InvoiceCustomerModalColumns";
import InvoiceCustomerCreateModal from "./InvoiceCustomerCreateModal";

interface InvoiceCustomerModalProps {
  open: boolean;
  onClose: () => void;
  onSelect: (customer: Customer) => void;
}

export default function InvoiceCustomerModal({
  open,
  onClose,
  onSelect,
}: InvoiceCustomerModalProps) {
  const { token } = useAuth();

  const [data, setData] = useState<Customer[]>([]);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(5);
  const [totalPages, setTotalPages] = useState(1);
  const [keyword, setKeyword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [openCreateModal, setOpenCreateModal] = useState(false);

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
    <Dialog open={open} onOpenChange={onClose}>
      <DialogContent className="!max-w-fit">
        <DialogHeader>
          <DialogTitle>Seleccionar Cliente</DialogTitle>
        </DialogHeader>
        <Card>
          <CardContent>
            <div className="flex flex-col gap-3">
              <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-2">
                <InvoiceCustomerModalHeader
                  keyword={keyword}
                  setKeyword={setKeyword}
                  setPage={setPage}
                />
                <Button onClick={() => setOpenCreateModal(true)}>Nuevo cliente</Button>
              </div>
              {error ? (
                <AlertMessage message={error} variant="destructive" />
              ) : (
                <DataTable
                  columns={columns({ onSelect })}
                  data={data}
                  page={page}
                  pageSize={pageSize}
                  totalPages={totalPages}
                  onPageChange={setPage}
                  onPageSizeChange={setPageSize}
                  loading={loading}
                />
              )}
            </div>
          </CardContent>
        </Card>
        <InvoiceCustomerCreateModal
          open={openCreateModal}
          onClose={() => setOpenCreateModal(false)}
          onCreated={(customer) => {
            onSelect(customer);
            setOpenCreateModal(false);
            onClose();
          }}
        />
      </DialogContent>
    </Dialog>
  );
}
