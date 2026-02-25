import type { ColumnDef } from "@tanstack/react-table";

import type { PurchasesReport } from "@/types/report.types";

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
    header: "Id",
    cell: ({ row }) => (
      <span className="font-semibold">{row.original.sequential}</span>
    ),
  },
  {
    accessorKey: "supplierName",
    header: "Nombre",
    cell: ({ row }) => (
      <span>{row.original.supplierName}</span>
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
    accessorKey: "productName",
    header: "Producto",
    cell: ({ row }) => (
      <span>{row.original.productName}</span>
    ),
  },
  {
    accessorKey: "quantity",
    header: () => <div className="text-right">Cant</div>,
    cell: ({ row }) => (
      <p className="text-right">{row.original.quantity}</p>
    ),
  },
  {
    accessorKey: "grossWeight",
    header: () => <div className="text-right">P Bruto</div>,
    cell: ({ row }) => (
      <p className="text-right whitespace-nowrap">
        Lb {Number(row.original.grossWeight).toFixed(2)}
      </p>
    ),
  },
  {
    id: "merma",
    header: () => <div className="text-right">Merma</div>,
    cell: ({ row }) => {
      const merma = row.original.grossWeight - row.original.netWeight;
      return (
        <p className="text-right whitespace-nowrap">
          Lb {merma.toFixed(2)}
        </p>
      );
    },
  },
  {
    accessorKey: "netWeight",
    header: () => <div className="text-right">P Neto</div>,
    cell: ({ row }) => (
      <p className="text-right whitespace-nowrap">
        Lb {Number(row.original.netWeight).toFixed(2)}
      </p>
    ),
  },
  {
    accessorKey: "unitCost",
    header: () => <div className="text-right">Precio</div>,
    cell: ({ row }) => (
      <p className="text-right">$ {Number(row.original.unitCost).toFixed(2)}</p>
    ),
  },
  {
    accessorKey: "total",
    header: () => <div className="text-right">Total</div>,
    cell: ({ row }) => (
      <p className="text-right">$ {Number(row.original.total).toFixed(2)}</p>
    ),
  },
  {
    id: "promedio",
    header: () => <div className="text-right">Promedio</div>,
    cell: ({ row }) => {
      const promedio =
        row.original.quantity !== 0
          ? row.original.netWeight / row.original.quantity
          : 0;
      return (
        <p className="text-right whitespace-nowrap">
          % {promedio.toFixed(2)}
        </p>
      );
    },
  },
];
