import type { ColumnDef } from "@tanstack/react-table";

import type { Product } from "@/types/product.types";

import { Button } from "@/components/ui/button";

interface ColumnsProps {
  onSelect?: (product: Product) => void;
}

export const columns = ({ onSelect }: ColumnsProps): ColumnDef<Product>[] => [
  {
    accessorKey: "id",
    header: "ID",
  },
  {
    accessorKey: "sku",
    header: "Codigo",
  },
  {
    accessorKey: "name",
    header: "Nombre",
  },
  {
    id: "actions",
    header: "Acciones",
    cell: ({ row }) => {
      const product = row.original;
      return (
        <Button size="sm" onClick={() => onSelect?.(product)}>
          Escoger
        </Button>
      );
    },
  },
];
