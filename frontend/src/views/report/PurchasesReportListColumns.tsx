import type { ColumnDef } from "@tanstack/react-table";

import type { PurchasesReport } from "@/types/report.types";

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

export const columns: ColumnDef<PurchasesReport>[] = [
  {
    accessorKey: "issueDate",
    header: "Fecha",
    cell: ({ row }) => (
      <span className="whitespace-nowrap">
        {formatDate(row.original.issueDate)}
      </span>
    ),
  },
  {
    accessorKey: "sequential",
    header: "Comprobante",
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

      const variant = normalized.includes("ISSUED")
        ? "default"
        : normalized.includes("DRAFT")
        ? "secondary"
        : normalized.includes("RECHAZADO") || normalized.includes("CANCELED")
        ? "destructive"
        : "outline";

      return <Badge variant={variant}>{status}</Badge>;
    },
  },
  {
    id: "supplier",
    header: "Proveedor",
    accessorFn: (row) => `${row.supplierDocument} ${row.supplierName}`,
    cell: ({ row }) => (
      <div className="flex flex-col">
        <span className="font-semibold">{row.original.supplierDocument}</span>
        <span className="text-muted-foreground">{row.original.supplierName}</span>
      </div>
    ),
  },
  {
    accessorKey: "userFullName",
    header: "Comprador",
    cell: ({ row }) => (
      <span className="text-muted-foreground">{row.original.userFullName}</span>
    ),
  },
  {
    accessorKey: "totalPurchase",
    header: () => <div className="text-right">Total</div>,
    cell: ({ row }) => (
      <p className="text-right">
        {Number(row.original.totalPurchase).toFixed(2)}
      </p>
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
              to: `/reportes/compras/${row.original.id}`,
            },
          ]}
        />
      </div>
    ),
  },
];
