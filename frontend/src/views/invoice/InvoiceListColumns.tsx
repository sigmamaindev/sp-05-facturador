import { Link } from "react-router-dom";
import type { ColumnDef } from "@tanstack/react-table";

import { downloadInvoicePdf } from "@/api/invoice";
import type { Invoice } from "@/types/invoice.type";

import { useAuth } from "@/contexts/AuthContext";

import { buttonVariants } from "@/components/ui/button";
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from "@/components/ui/tooltip";
import { Badge } from "@/components/ui/badge";

import { cn } from "@/lib/utils";

import { Eye, FileDown, Pencil } from "lucide-react";

export const columns: ColumnDef<Invoice>[] = [
  {
    accessorKey: "invoiceDate",
    header: "Fecha",
    cell: ({ row }) => {
      const date = new Date(row.original.invoiceDate);

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
        <div className="flex flex-col leading-tight">
          <span className="font-semibold">{dateStr}</span>
          <span className="text-muted-foreground">{timeStr}</span>
        </div>
      );
    },
  },
  {
    id: "code",
    header: "Código",
    accessorFn: (invoice) =>
      `${invoice.establishmentCode} ${invoice.emissionPointCode} ${invoice.sequential}`,
    cell: ({ row }) => (
      <div className="flex flex-col">
        <span className="font-semibold">
          {row.original.establishmentCode}-{row.original.emissionPointCode}-
          {row.original.sequential}
        </span>
      </div>
    ),
  },
  {
    accessorKey: "status",
    header: "Estado",
    cell: ({ row }) => {
      const status = row.original.status ?? "Sin estado";
      const normalized = status.toUpperCase();

      const variant = normalized.includes("AUTORIZADO")
        ? "default"
        : normalized.includes("RECIBIDO")
        ? "secondary"
        : normalized.includes("RECHAZADO") || normalized.includes("DEVUELTO")
        ? "destructive"
        : "outline";

      return <Badge variant={variant}>{status}</Badge>;
    },
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
    accessorKey: "totalInvoice",
    header: () => <div className="text-right">Total</div>,
    cell: ({ row }) => {
      const total = row.original.totalInvoice.toFixed(2);

      return <p className="text-right">{total}</p>;
    },
  },
  {
    id: "actions",
    header: () => <div className="text-center">Acciones</div>,
    cell: ({ row }) => {
      const { user, token } = useAuth();

      const hasPermission =
        user?.roles?.includes("SuperAdmin") || user?.roles?.includes("Admin");

      const invoice = row.original;

      const isAuthorized = invoice.status?.toUpperCase() === "AUTORIZADO";

      const isReceived =
        invoice.status?.toUpperCase().includes("RECIBIDO") ?? false;

      const handlePdfDownload = async () => {
        if (!token) return;

        try {
          const file = await downloadInvoicePdf(invoice.id, token);
          const url = window.URL.createObjectURL(file);
          const link = document.createElement("a");

          link.href = url;
          link.download = `Factura_${invoice.sequential}.pdf`;
          document.body.appendChild(link);
          link.click();
          document.body.removeChild(link);

          window.URL.revokeObjectURL(url);
        } catch (error: any) {
          alert(error.message ?? "No se pudo generar el PDF de la factura");
        }
      };
      return (
        <div className="flex flex-col sm:flex-row items-center justify-center gap-2">
          <Tooltip>
            <TooltipTrigger asChild>
              <Link
                to={`/facturas/${invoice.id}`}
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

          {isAuthorized ? (
            <Tooltip>
              <TooltipTrigger asChild>
                <button
                  type="button"
                  onClick={handlePdfDownload}
                  aria-label="Descargar PDF"
                  className={cn(
                    buttonVariants({ size: "icon-sm" }),
                    "bg-emerald-600 text-white hover:bg-emerald-700 dark:bg-emerald-500 dark:hover:bg-emerald-600"
                  )}
                >
                  <FileDown className="size-4 text-white" />
                </button>
              </TooltipTrigger>
              <TooltipContent side="top" sideOffset={6}>
                Descargar PDF
              </TooltipContent>
            </Tooltip>
          ) : null}

          {hasPermission && !isReceived && !isAuthorized ? (
            <Tooltip>
              <TooltipTrigger asChild>
                <Link
                  to={`/facturas/actualizar/${invoice.id}`}
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
