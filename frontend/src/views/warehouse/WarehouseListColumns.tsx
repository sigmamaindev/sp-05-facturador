import { Link } from "react-router-dom";
import type { ColumnDef } from "@tanstack/react-table";
import { Eye, Pencil } from "lucide-react";

import type { Warehouse } from "@/types/warehouse.types";

import { useAuth } from "@/contexts/AuthContext";

import { buttonVariants } from "@/components/ui/button";
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from "@/components/ui/tooltip";
import { Badge } from "@/components/ui/badge";

import { cn } from "@/lib/utils";

export const columns: ColumnDef<Warehouse>[] = [
  {
    accessorKey: "code",
    header: "Código",
  },
  {
    accessorKey: "name",
    header: "Nombre",
  },
  {
    accessorKey: "address",
    header: "Dirección",
  },
  {
    accessorKey: "isMain",
    header: "Principal",
    cell: ({ row }) => {
      const isMain = row.original.isMain;

      return (
        <Badge
          className={cn(
            "min-w-10 justify-center",
            isMain
              ? "bg-blue-600 text-white hover:bg-blue-600"
              : "bg-gray-600 text-white hover:bg-gray-600"
          )}
        >
          {isMain ? "SI" : "NO"}
        </Badge>
      );
    },
  },
  {
    accessorKey: "isActive",
    header: "Activo",
    cell: ({ row }) => {
      const isActive = row.original.isActive;

      return (
        <Badge
          className={cn(
            "min-w-10 justify-center",
            isActive
              ? "bg-green-600 text-white hover:bg-green-600"
              : "bg-red-600 text-white hover:bg-red-600"
          )}
        >
          {isActive ? "SI" : "NO"}
        </Badge>
      );
    },
  },
  {
    id: "actions",
    header: "Acciones",
    cell: ({ row }) => {
      const { user } = useAuth();
      const warehouse = row.original;

      const hasPermission =
        user?.roles?.includes("SuperAdmin") || user?.roles?.includes("Admin");

      return (
        <div className="flex flex-col sm:flex-row items-center justify-center gap-2">
          <Tooltip>
            <TooltipTrigger asChild>
              <Link
                to={`/bodegas/${warehouse.id}`}
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
                  to={`/bodegas/actualizar/${warehouse.id}`}
                  aria-label="Editar bodega"
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

