import type { ColumnDef } from "@tanstack/react-table";

import type { SalesReport } from "@/types/report.types";

import { Badge } from "@/components/ui/badge";
import RowActions from "@/components/shared/RowActions";

function formatDate(dateValue: string) {
  const date = new Date(dateValue);

  return date.toLocaleDateString("es-EC", {
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
  });
}

export const columns: ColumnDef<SalesReport>[] = [
  {
    accessorKey: "invoiceDate",
    header: "Fecha",
    cell: ({ row }) => (
      <span className="whitespace-nowrap">
        {formatDate(row.original.invoiceDate)}
      </span>
    ),
  },
  {
    accessorKey: "sequential",
    header: "Secuencial",
    cell: ({ row }) => (
      <span className="font-semibold">{row.original.sequential}</span>
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
    accessorFn: (row) => `${row.customerDocument} ${row.customerName}`,
    cell: ({ row }) => (
      <div className="flex flex-col">
        <span className="font-semibold">{row.original.customerDocument}</span>
        <span className="text-muted-foreground">{row.original.customerName}</span>
      </div>
    ),
  },
  {
    accessorKey: "userFullName",
    header: "Vendedor",
    cell: ({ row }) => (
      <span className="text-muted-foreground">{row.original.userFullName}</span>
    ),
  },
  {
    accessorKey: "paymentTermDays",
    header: () => <div className="text-right">Plazo (días)</div>,
    cell: ({ row }) => (
      <p className="text-right">{row.original.paymentTermDays}</p>
    ),
  },
  {
    accessorKey: "totalInvoice",
    header: () => <div className="text-right">Total</div>,
    cell: ({ row }) => (
      <p className="text-right">{Number(row.original.totalInvoice).toFixed(2)}</p>
    ),
  },
  {
    id: "actions",
    header: () => <div className="text-center">Acciones</div>,
    cell: ({ row }) => (
      <div className="flex justify-center">
        <RowActions
          actions={[
            {
              label: "Ver detalle",
              to: `/reportes/ventas/${row.original.id}`,
            },
          ]}
        />
      </div>
    ),
  },
];
