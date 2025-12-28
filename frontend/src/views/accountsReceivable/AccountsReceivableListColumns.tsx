import { Link } from "react-router-dom";
import type { ColumnDef } from "@tanstack/react-table";

import type { AccountsReceivable } from "@/types/accountsReceivable.types";

import { useAuth } from "@/contexts/AuthContext";

import { buttonVariants } from "@/components/ui/button";
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from "@/components/ui/tooltip";

import { cn } from "@/lib/utils";

import { Eye, Pencil } from "lucide-react";

function daysUntil(dueDate: Date) {
  const due = new Date(dueDate);
  const today = new Date();

  due.setHours(0, 0, 0, 0);
  today.setHours(0, 0, 0, 0);

  return Math.ceil((due.getTime() - today.getTime()) / 86400000);
}

export const columns: ColumnDef<AccountsReceivable>[] = [
  {
    accessorKey: "invoiceDate",
    header: "Fecha de emisión",
    cell: ({ row }) => {
      const date = new Date(row.original.invoice.invoiceDate);

      const dateStr = date.toLocaleDateString("es-EC", {
        year: "numeric",
        month: "2-digit",
        day: "2-digit",
      });

      const timeStr = date.toLocaleTimeString("es-EC", {
        hour: "2-digit",
        minute: "2-digit",
        second: "2-digit",
        hour12: false,
      });

      return (
        <div className="flex flex-col">
          <span className="font-semibold">{dateStr}</span>
          <span className="text-muted-foreground">{timeStr}</span>
        </div>
      );
    },
  },
  {
    id: "code",
    header: "Código",
    accessorFn: (ar) =>
      `${ar.invoice.establishmentCode} ${ar.invoice.emissionPointCode} ${ar.invoice.sequential}`,
    cell: ({ row }) => (
      <span className="font-semibold">
        {row.original.invoice.establishmentCode}-
        {row.original.invoice.emissionPointCode}-
        {row.original.invoice.sequential}
      </span>
    ),
  },
  {
    id: "customer",
    header: "Cliente",
    accessorFn: (invoice) =>
      `${invoice.customer.document} ${invoice.customer.name}`,
    cell: ({ row }) => (
      <div className="flex flex-col">
        <span className="font-semibold">{row.original.customer.document}</span>
        <span className="text-muted-foreground">
          {row.original.customer.name}
        </span>
      </div>
    ),
  },
  {
    accessorKey: "dueDate",
    header: "Vencimiento",
    cell: ({ row }) => {
      const due = new Date(row.original.dueDate);
      const days = daysUntil(due);

      const dueStr = due.toLocaleDateString("es-EC", {
        year: "numeric",
        month: "2-digit",
        day: "2-digit",
      });

      const label =
        days < 0
          ? `Vencida (${Math.abs(days)} día${Math.abs(days) === 1 ? "" : "s"})`
          : days === 0
          ? "Vence hoy"
          : `Por vencer: ${days} día${days === 1 ? "" : "s"}`;

      return (
        <div className="flex flex-col gap-1">
          <span className="font-semibold">{dueStr}</span>
          <span className="text-muted-foreground">{label}</span>
        </div>
      );
    },
  },
  {
    accessorKey: "totalInvoice",
    header: () => <div className="text-right">Saldo</div>,
    cell: ({ row }) => (
      <p className="text-right">
        {row.original.invoice.totalInvoice.toFixed(2)}
      </p>
    ),
  },
  {
    id: "actions",
    header: () => <div className="text-center">Acciones</div>,
    cell: ({ row }) => {
      const { user } = useAuth();

      const hasPermission =
        user?.roles?.includes("SuperAdmin") || user?.roles?.includes("Admin");

      const ar = row.original;

      return (
        <div className="flex flex-col sm:flex-row items-center justify-center gap-2">
          <Tooltip>
            <TooltipTrigger asChild>
              <Link
                to={`/cuentas-por-cobrar/${ar.id}`}
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
                  to={`/cuentas-por-cobrar/actualizar/${ar.id}`}
                  aria-label="Editar factura"
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
