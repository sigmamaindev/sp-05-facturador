import { Link } from "react-router-dom";
import type { ColumnDef } from "@tanstack/react-table";

import { Badge } from "@/components/ui/badge";

import type { User } from "@/types/user.types";

import { useAuth } from "@/contexts/AuthContext";

import { buttonVariants } from "@/components/ui/button";
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from "@/components/ui/tooltip";

import { cn } from "@/lib/utils";

import { Eye, Pencil } from "lucide-react";

export const columns: ColumnDef<User>[] = [
  {
    id: "person",
    accessorFn: (user) => `${user.document} ${user.fullName}`,
    header: "Documento",
    cell: ({ row }) => (
      <div className="flex flex-col">
        <span className="font-semibold">{row.original.document}</span>
        <span className="text-muted-foreground">{row.original.fullName}</span>
      </div>
    ),
  },
  {
    accessorKey: "username",
    header: "Usuario",
  },
  {
    accessorKey: "roles",
    header: "Roles",
    cell: ({ row }) => {
      const roles = row.original.roles;

      if (!roles || roles.length === 0) {
        return <span className="text-muted-foreground">Sin roles</span>;
      }

      return (
        <div className="flex flex-wrap gap-1">
          {roles.map((role) => (
            <Badge key={role.id} variant="default">
              {role.name}
            </Badge>
          ))}
        </div>
      );
    },
  },
  {
    accessorKey: "createdAt",
    header: "Fecha Creación",
    cell: ({ row }) => {
      const date = new Date(row.original.createdAt);
      return <span>{date.toLocaleDateString("es-EC")}</span>;
    },
  },
  {
    id: "actions",
    header: "Acciones",
    cell: ({ row }) => {
      const { user } = useAuth();

      const person = row.original;

      const hasPermission =
        user?.roles?.includes("SuperAdmin") || user?.roles?.includes("Admin");

      return (
        <div className="flex flex-col sm:flex-row items-center justify-center gap-2">
          <Tooltip>
            <TooltipTrigger asChild>
              <Link
                to={`/usuarios/${person.id}`}
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
                  to={`/usuarios/actualizar/${person.id}`}
                  aria-label="Editar empresa"
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
