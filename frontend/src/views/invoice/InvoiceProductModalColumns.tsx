import type { ColumnDef } from "@tanstack/react-table";

import type { Product } from "@/types/product.types";

import { Button } from "@/components/ui/button";

export const columns = ({ onSelect }: { onSelect: (p: Product) => void }) => {
  const cols: ColumnDef<Product>[] = [
    {
      accessorKey: "sku",
      header: "Código",
    },
    {
      accessorKey: "name",
      header: "Producto",
    },
    {
      id: "actions",
      header: "",
      cell: ({ row }) => {
        const product = row.original;
        return <Button onClick={() => onSelect(product)}>Seleccionar</Button>;
      },
    },
  ];

  return cols;
};
