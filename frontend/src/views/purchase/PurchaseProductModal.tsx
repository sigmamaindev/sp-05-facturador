import { useEffect, useState } from "react";

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

import PurchaseProductModalHeader from "./PurchaseProductModalHeader";
import { columns } from "./PurchaseProductModalColumns";

interface PurchaseProductModalProps {
  open: boolean;
  onClose: () => void;
  onSelect: (product: Product) => void;
}

export default function PurchaseProductModal({
  open,
  onClose,
  onSelect,
}: PurchaseProductModalProps) {
  const { token } = useAuth();

  const [data, setData] = useState<Product[]>([]);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(5);
  const [totalPages, setTotalPages] = useState(1);
  const [keyword, setKeyword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchData = async () => {
    if (!token) return;

    setLoading(true);

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
  };

  useEffect(() => {
    fetchData();
  }, [keyword, page, pageSize, token]);

  return (
    <Dialog open={open} onOpenChange={onClose}>
      <DialogContent className="!max-w-fit">
        <DialogHeader>
          <DialogTitle>Seleccionar Producto</DialogTitle>
        </DialogHeader>
        <Card>
          <CardContent>
            <PurchaseProductModalHeader
              keyword={keyword}
              setKeyword={setKeyword}
              setPage={setPage}
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
  );
}
