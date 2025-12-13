import { useMemo, useState } from "react";

import { Card, CardContent } from "@/components/ui/card";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";

import AlertMessage from "@/components/shared/AlertMessage";
import DataTable from "@/components/shared/DataTable";

import type { PurchaseSupplier } from "@/types/purchase.type";

import PurchaseSupplierModalHeader from "./PurchaseSupplierModalHeader";
import { columns } from "./PurchaseSupplierModalColumns";

const MOCK_SUPPLIERS: PurchaseSupplier[] = [
  {
    id: 1,
    name: "Proveedor demo 1",
    document: "0999999999",
    email: "proveedor1@example.com",
  },
  {
    id: 2,
    name: "Proveedor demo 2",
    document: "0123456789",
    email: "compras@example.com",
  },
];

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
  const [keyword, setKeyword] = useState("");

  const filtered = useMemo(() => {
    const lower = keyword.toLowerCase();
    return MOCK_SUPPLIERS.filter(
      (s) =>
        s.name.toLowerCase().includes(lower) ||
        s.document.toLowerCase().includes(lower) ||
        s.email.toLowerCase().includes(lower)
    );
  }, [keyword]);

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
            {filtered.length === 0 ? (
              <AlertMessage
                message="Aún no hay proveedores cargados desde la API. Se muestran datos de ejemplo."
                variant="secondary"
              />
            ) : null}
            <DataTable
              columns={columns({ onSelect })}
              data={filtered}
              page={1}
              pageSize={filtered.length || 5}
              totalPages={1}
              onPageChange={() => null}
              onPageSizeChange={() => null}
              loading={false}
            />
          </CardContent>
        </Card>
      </DialogContent>
    </Dialog>
  );
}
