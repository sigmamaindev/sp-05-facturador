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
      id: "defaultPrice",
      accessorFn: (product) =>
        product.defaultPresentation?.price01 ?? product.price ?? 0,
      header: () => <div className="text-right">Precio</div>,
      cell: ({ row }) => {
        const product = row.original;
        const price =
          product.defaultPresentation?.price01 ?? product.price ?? 0;
        return (
          <div className="text-right whitespace-nowrap">
            ${Number(price).toFixed(2)}
          </div>
        );
      },
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
