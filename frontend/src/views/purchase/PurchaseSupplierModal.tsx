import { useEffect, useState } from "react";

import { useAuth } from "@/contexts/AuthContext";

import { getSuppliers } from "@/api/supplier";

import { Card, CardContent } from "@/components/ui/card";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";

import AlertMessage from "@/components/shared/AlertMessage";
import DataTable from "@/components/shared/DataTable";

import type { PurchaseSupplier } from "@/types/purchase.type";
import type { Supplier } from "@/types/supplier.types";

import PurchaseSupplierModalHeader from "./PurchaseSupplierModalHeader";
import { columns } from "./PurchaseSupplierModalColumns";
import PurchaseSupplierCreateModal from "./PurchaseSupplierCreateModal";

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
  const [openCreateModal, setOpenCreateModal] = useState(false);

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
            <div className="flex flex-col gap-3">
              <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-2">
                <PurchaseSupplierModalHeader
                  keyword={keyword}
                  setKeyword={setKeyword}
                />
                <Button onClick={() => setOpenCreateModal(true)}>
                  Nuevo proveedor
                </Button>
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

        <PurchaseSupplierCreateModal
          open={openCreateModal}
          onClose={() => setOpenCreateModal(false)}
          onCreated={(supplier: Supplier) => {
            onSelect({
              id: supplier.id,
              name: supplier.businessName,
              document: supplier.document,
              email: supplier.email,
            });
            setOpenCreateModal(false);
            onClose();
          }}
        />
      </DialogContent>
    </Dialog>
  );
}
