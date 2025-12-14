import { useEffect, useState } from "react";

import { useAuth } from "@/contexts/AuthContext";

import { getSuppliers } from "@/api/supplier";

import { Card, CardContent } from "@/components/ui/card";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";

import AlertMessage from "@/components/shared/AlertMessage";
import DataTable from "@/components/shared/DataTable";

import type { PurchaseSupplier } from "@/types/purchase.type";

import PurchaseSupplierModalHeader from "./PurchaseSupplierModalHeader";
import { columns } from "./PurchaseSupplierModalColumns";

interface PurchaseSupplierModalProps {
  open: boolean;
  onClose: () => void;
  onSelect: (supplier: PurchaseSupplier) => void;
}

export default function PurchaseSupplierModal({
  open,
  onClose,
  onSelect,
}: PurchaseSupplierModalProps) {
  const { token } = useAuth();

  const [keyword, setKeyword] = useState("");
  const [data, setData] = useState<PurchaseSupplier[]>([]);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!token || !open) return;

    const fetchData = async () => {
      setLoading(true);
      setError(null);

      try {
        const suppliers = await getSuppliers(keyword, page, pageSize, token);

        if (!suppliers) return;

        const mappedSuppliers: PurchaseSupplier[] = suppliers.data.map(
          (supplier) => ({
            id: supplier.id,
            name: supplier.businessName,
            document: supplier.document,
            email: supplier.email,
          })
        );

        setData(mappedSuppliers);
        setPage(suppliers.pagination.page);
        setPageSize(suppliers.pagination.limit);
        setTotalPages(suppliers.pagination.totalPages);
      } catch (err: any) {
        setError(
          err.message ?? "No se pudieron cargar los proveedores desde la API"
        );
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [keyword, page, pageSize, token, open]);

  return (
    <Dialog open={open} onOpenChange={onClose}>
      <DialogContent className="!max-w-fit">
        <DialogHeader>
          <DialogTitle>Seleccionar proveedor</DialogTitle>
        </DialogHeader>
        <Card>
          <CardContent>
            <PurchaseSupplierModalHeader
              keyword={keyword}
              setKeyword={setKeyword}
            />
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
            {error ? (
              <AlertMessage message={error} variant="destructive" />
            ) : null}
          </CardContent>
        </Card>
      </DialogContent>
    </Dialog>
  );
}
