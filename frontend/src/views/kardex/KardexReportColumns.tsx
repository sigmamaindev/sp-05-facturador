import type { ColumnDef } from "@tanstack/react-table";

import type { KardexReportRow } from "@/types/kardex.types";

function formatDate(dateValue: string) {
  const date = new Date(dateValue);
  return date.toLocaleDateString("es-EC", {
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
  });
}

function formatNumber(value: number, decimals = 4) {
  return value !== 0 ? value.toFixed(decimals) : "";
}

function formatCurrency(value: number) {
  return value !== 0
    ? value.toLocaleString("es-EC", {
        style: "currency",
        currency: "USD",
        minimumFractionDigits: 2,
      })
    : "";
}

export const kardexReportColumns: ColumnDef<KardexReportRow>[] = [
  {
    accessorKey: "movementDate",
    header: "Fecha",
    cell: ({ row }) => {
      const item = row.original;
      if (item.movementType === "SALDO_INICIAL") {
        return (
          <span className="font-semibold text-amber-700">
            Saldo Al: {formatDate(item.movementDate)}
          </span>
        );
      }
      return <span>{formatDate(item.movementDate)}</span>;
    },
  },
  {
    accessorKey: "movementType",
    header: "Movimiento",
    cell: ({ row }) => {
      const item = row.original;
      if (item.movementType === "SALDO_INICIAL") return null;
      return <span>{item.movementType}</span>;
    },
  },
  {
    accessorKey: "warehouseCode",
    header: "Bodega",
    cell: ({ row }) => {
      const item = row.original;
      if (item.movementType === "SALDO_INICIAL") return null;
      return <span>{item.warehouseCode}</span>;
    },
  },
  {
    accessorKey: "reference",
    header: "Referencia",
    cell: ({ row }) => {
      const item = row.original;
      if (item.movementType === "SALDO_INICIAL") return null;
      return <span className="whitespace-nowrap">{item.reference}</span>;
    },
  },
  // ENTRADAS
  {
    accessorKey: "entryQuantity",
    header: () => <div className="text-right">Ent. Cant.</div>,
    cell: ({ row }) => {
      const item = row.original;
      if (item.movementType === "SALDO_INICIAL") return null;
      return (
        <p className="text-right">{formatNumber(item.entryQuantity, 3)}</p>
      );
    },
  },
  {
    accessorKey: "entryCost",
    header: () => <div className="text-right">Ent. Costo</div>,
    cell: ({ row }) => {
      const item = row.original;
      if (item.movementType === "SALDO_INICIAL") return null;
      return <p className="text-right">{formatNumber(item.entryCost)}</p>;
    },
  },
  {
    accessorKey: "entryTotal",
    header: () => <div className="text-right">Ent. Total</div>,
    cell: ({ row }) => {
      const item = row.original;
      if (item.movementType === "SALDO_INICIAL") return null;
      return (
        <p className="text-right">
          {item.entryTotal > 0 ? formatCurrency(item.entryTotal) : ""}
        </p>
      );
    },
  },
  // SALIDAS
  {
    accessorKey: "exitQuantity",
    header: () => <div className="text-right">Sal. Cant.</div>,
    cell: ({ row }) => {
      const item = row.original;
      if (item.movementType === "SALDO_INICIAL") return null;
      return (
        <p className="text-right">{formatNumber(item.exitQuantity, 3)}</p>
      );
    },
  },
  {
    accessorKey: "exitCost",
    header: () => <div className="text-right">Sal. Costo</div>,
    cell: ({ row }) => {
      const item = row.original;
      if (item.movementType === "SALDO_INICIAL") return null;
      return <p className="text-right">{formatNumber(item.exitCost)}</p>;
    },
  },
  {
    accessorKey: "exitTotal",
    header: () => <div className="text-right">Sal. Total</div>,
    cell: ({ row }) => {
      const item = row.original;
      if (item.movementType === "SALDO_INICIAL") return null;
      return (
        <p className="text-right">
          {item.exitTotal > 0 ? formatCurrency(item.exitTotal) : ""}
        </p>
      );
    },
  },
  // TOTALES
  {
    accessorKey: "runningStock",
    header: () => <div className="text-right">Existencia</div>,
    cell: ({ row }) => (
      <p className="text-right font-medium">
        {row.original.runningStock.toFixed(3)}
      </p>
    ),
  },
  {
    accessorKey: "runningValue",
    header: () => <div className="text-right">Saldo</div>,
    cell: ({ row }) => (
      <p className="text-right font-medium">
        {formatCurrency(row.original.runningValue)}
      </p>
    ),
  },
];
