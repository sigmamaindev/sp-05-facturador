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
import {
  AlertDialog,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog";

import { cn } from "@/lib/utils";

import { AlertCircleIcon, Eye, FileDown, Pencil } from "lucide-react";

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

      const normalizedStatus = (invoice.status ?? "").trim().toUpperCase();
      const hasSriErrorStatus =
        normalizedStatus.length === 0 ||
        normalizedStatus.includes("ERROR") ||
        normalizedStatus.includes("RECHAZADO") ||
        normalizedStatus.includes("DEVUELTO");

      const sriMessage =
        (invoice.sriMessage ?? "").trim() || "Sin mensaje del SRI";

      const isAuthorized = invoice.status?.toUpperCase() === "AUTORIZADO";

      const isReceived =
        invoice.status?.toUpperCase().includes("RECIBIDO") ?? false;

      const isPending =
        normalizedStatus.includes("PENDIENTE") ||
        normalizedStatus.includes("PENDING");

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

          {hasSriErrorStatus ? (
            <AlertDialog>
              <Tooltip>
                <TooltipTrigger asChild>
                  <span className="inline-flex">
                    <AlertDialogTrigger asChild>
                      <button
                        type="button"
                        aria-label="Ver error SRI"
                        className={cn(
                          buttonVariants({ size: "icon-sm" }),
                          "bg-rose-600 text-white hover:bg-rose-700 dark:bg-rose-500 dark:hover:bg-rose-600"
                        )}
                      >
                        <AlertCircleIcon className="size-4 text-white" />
                      </button>
                    </AlertDialogTrigger>
                  </span>
                </TooltipTrigger>
                <TooltipContent side="top" sideOffset={6}>
                  Ver error SRI
                </TooltipContent>
              </Tooltip>

              <AlertDialogContent>
                <AlertDialogHeader>
                  <AlertDialogTitle>Respuesta del SRI</AlertDialogTitle>
                  <AlertDialogDescription>
                    Mensaje recibido desde el backend para esta factura.
                  </AlertDialogDescription>
                </AlertDialogHeader>

                <div className="max-h-60 overflow-auto whitespace-pre-wrap break-words rounded-md border bg-muted/30 p-3 text-sm">
                  {sriMessage}
                </div>

                <AlertDialogFooter>
                  <AlertDialogCancel>Cerrar</AlertDialogCancel>
                </AlertDialogFooter>
              </AlertDialogContent>
            </AlertDialog>
          ) : null}

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

          {hasPermission && !isReceived && !isAuthorized && !isPending ? (
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
