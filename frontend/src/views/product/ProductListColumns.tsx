import { Link } from "react-router-dom";
import type { ColumnDef } from "@tanstack/react-table";

import type { Product } from "@/types/product.types";

import { useAuth } from "@/contexts/AuthContext";

import { buttonVariants } from "@/components/ui/button";
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from "@/components/ui/tooltip";

import { cn } from "@/lib/utils";

import { Eye, Pencil } from "lucide-react";

export const columns: ColumnDef<Product>[] = [
  {
    id: "product",
    header: "Producto",
    accessorFn: (product) => `${product.sku} ${product.name}`,
    cell: ({ row }) => (
      <div className="flex flex-col">
        <span className="font-semibold">{row.original.sku}</span>
        <span className="text-muted-foreground">{row.original.name}</span>
      </div>
    ),
  },
  {
    accessorKey: "price",
    header: "Precio",
    cell: ({ row }) => {
      const price = row.original.price.toFixed(2);

      return <span>{price}</span>;
    },
  },
  {
    id: "actions",
    header: "Acciones",
    cell: ({ row }) => {
      const { user } = useAuth();

      const hasPermission =
        user?.roles?.includes("SuperAdmin") || user?.roles?.includes("Admin");

      const product = row.original;

      return (
        <div className="flex flex-col sm:flex-row items-center justify-center gap-2">
          <Tooltip>
            <TooltipTrigger asChild>
              <Link
                to={`/productos/${product.id}`}
                aria-label="Ver detalles"
                className={cn(
                  buttonVariants({ size: "icon-sm" }),
                  "bg-gray-200 text-white hover:bg-gray-300 dark:bg-gray-700 dark:hover:bg-gray-600"
                )}
              >
                <Eye className="size-4 text-white" />
              </Link>
            </TooltipTrigger>
            <TooltipContent side="top" sideOffset={6}>
              Detalles
            </TooltipContent>
          </Tooltip>

          {hasPermission ? (
            <Tooltip>
              <TooltipTrigger asChild>
                <Link
                  to={`/productos/actualizar/${product.id}`}
                  aria-label="Editar cliente"
                  className={cn(
                    buttonVariants({ size: "icon-sm" }),
                    "bg-blue-600 text-white hover:bg-blue-700 dark:bg-blue-500 dark:hover:bg-blue-600"
                  )}
                >
                  <Pencil className="size-4 text-white" />
                </Link>
              </TooltipTrigger>
              <TooltipContent side="top" sideOffset={6}>
                Editar
              </TooltipContent>
            </Tooltip>
          ) : null}
        </div>
      );
    },
  },
];
