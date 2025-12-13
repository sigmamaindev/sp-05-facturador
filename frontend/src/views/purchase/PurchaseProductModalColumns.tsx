import { Button } from "@/components/ui/button";
import type { ColumnDef } from "@tanstack/react-table";

import type { Product } from "@/types/product.types";

interface ColumnsProps {
  onSelect: (product: Product) => void;
}

export const columns = ({ onSelect }: ColumnsProps): ColumnDef<Product>[] => [
  {
    accessorKey: "sku",
    header: "Código",
  },
  {
    accessorKey: "name",
    header: "Nombre",
  },
  {
    accessorKey: "price",
    header: () => <div className="text-right">Costo</div>,
    cell: ({ row }) => (
      <p className="text-right">${row.original.price?.toFixed(2) ?? "0.00"}</p>
    ),
  },
  {
    accessorKey: "tax.name",
    header: "Impuesto",
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
