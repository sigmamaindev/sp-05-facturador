import { useCallback, useEffect, useState } from "react";

import { useAuth } from "@/contexts/AuthContext";

import type { Product } from "@/types/product.types";

import { getProducts } from "@/api/product";

import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Card, CardContent } from "@/components/ui/card";

import AlertMessage from "@/components/shared/AlertMessage";
import DataTable from "@/components/shared/DataTable";

import InvoiceProductModalHeader from "./InvoiceProductModalHeader";
import { columns } from "./InvoiceProductModalColumns";
import ProductCreateModal from "../product/ProductCreateModal";

interface InvoiceProductModalProps {
  open: boolean;
  onClose: () => void;
  onSelect: (product: Product) => void;
}

export default function InvoiceProductModal({
  open,
  onClose,
  onSelect,
}: InvoiceProductModalProps) {
  const { token, user } = useAuth();

  const [data, setData] = useState<Product[]>([]);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(5);
  const [totalPages, setTotalPages] = useState(1);
  const [keyword, setKeyword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [openCreateProductModal, setOpenCreateProductModal] = useState(false);
  const [refreshKey, setRefreshKey] = useState(0);

  const canCreateProduct =
    user?.roles?.includes("SuperAdmin") || user?.roles?.includes("Admin");

  const fetchData = useCallback(async () => {
    if (!token) return;

    setLoading(true);
    setError(null);

    try {
      const products = await getProducts(keyword, page, pageSize, token);

      if (!products) return;

      setData(products.data);
      setPage(products.pagination.page);
      setPageSize(products.pagination.limit);
      setTotalPages(products.pagination.totalPages);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, [keyword, page, pageSize, token]);

  useEffect(() => {
    if (!open) return;
    void fetchData();
  }, [fetchData, open, refreshKey]);

  return (
    <>
      <Dialog open={open} onOpenChange={onClose}>
        <DialogContent className="!max-w-fit">
          <DialogHeader>
            <DialogTitle>Seleccionar Producto</DialogTitle>
          </DialogHeader>
          <Card>
            <CardContent>
              <InvoiceProductModalHeader
                keyword={keyword}
                setKeyword={setKeyword}
                setPage={setPage}
                canCreate={canCreateProduct}
                onCreate={() => {
                  onClose();
                  setOpenCreateProductModal(true);
                }}
              />
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
            </CardContent>
          </Card>
        </DialogContent>
      </Dialog>

      <ProductCreateModal
        open={openCreateProductModal}
        onClose={() => setOpenCreateProductModal(false)}
        onCreated={(product) => {
          setOpenCreateProductModal(false);
          setRefreshKey((k) => k + 1);
          onSelect(product);
        }}
      />
    </>
  );
}
