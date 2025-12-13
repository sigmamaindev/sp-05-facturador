import { Button } from "@/components/ui/button";
import type { ColumnDef } from "@tanstack/react-table";

import type { PurchaseSupplier } from "@/types/purchase.type";

interface ColumnsProps {
  onSelect: (supplier: PurchaseSupplier) => void;
}

export const columns = ({ onSelect }: ColumnsProps): ColumnDef<PurchaseSupplier>[] => [
  {
    accessorKey: "document",
    header: "Documento",
  },
  {
    accessorKey: "name",
    header: "Nombre",
  },
  {
    accessorKey: "email",
    header: "Correo",
  },
  {
    id: "actions",
    header: "",
    cell: ({ row }) => (
      <div className="text-right">
        <Button size="sm" onClick={() => onSelect(row.original)}>
          Seleccionar
        </Button>
      </div>
    ),
  },
];
