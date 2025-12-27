import { Link } from "react-router-dom";
import type { ColumnDef } from "@tanstack/react-table";

import type { EmissionPoint } from "@/types/emissionPoint.types";

import { useAuth } from "@/contexts/AuthContext";

import { buttonVariants } from "@/components/ui/button";
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from "@/components/ui/tooltip";
import { Badge } from "@/components/ui/badge";

import { cn } from "@/lib/utils";

import { Eye, Pencil } from "lucide-react";

export const columns = (
  establishmentId: number | null
): ColumnDef<EmissionPoint>[] => [
  {
    accessorKey: "code",
    header: "Código",
  },
  {
    accessorKey: "description",
    header: "Descripción",
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

      const hasPermission =
        user?.roles?.includes("SuperAdmin") || user?.roles?.includes("Admin");

      const emissionPoint = row.original;

      return (
        <div className="flex flex-col sm:flex-row items-center justify-center gap-2">
          <Tooltip>
            <TooltipTrigger asChild>
              <Link
                to={`/puntos-emision/${emissionPoint.id}?establecimiento=${establishmentId}`}
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
                  to={`/puntos-emision/actualizar/${emissionPoint.id}?establecimiento=${establishmentId}`}
                  aria-label="Editar punto de emisión"
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
