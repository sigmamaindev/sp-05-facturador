import { Link } from "react-router-dom";
import type { ColumnDef } from "@tanstack/react-table";

import type { KardexMovement } from "@/types/kardex.types";

import { Badge } from "@/components/ui/badge";
import { buttonVariants } from "@/components/ui/button";

import { cn } from "@/lib/utils";

function formatDateTime(dateValue: string) {
  const date = new Date(dateValue);

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

  return { dateStr, timeStr };
}

export const columns: ColumnDef<KardexMovement>[] = [
  {
    accessorKey: "movementDate",
    header: "Fecha",
    cell: ({ row }) => {
      const { dateStr, timeStr } = formatDateTime(row.original.movementDate);

      return (
        <div className="flex flex-col leading-tight">
          <span className="font-semibold">{dateStr}</span>
          <span className="text-muted-foreground">{timeStr}</span>
        </div>
      );
    },
  },
  {
    id: "product",
    header: "Producto",
    accessorFn: (movement) => `${movement.productSku} ${movement.productName}`,
    cell: ({ row }) => (
      <div className="flex flex-col">
        <Link
          to={`/productos/${row.original.productId}`}
          className={cn(buttonVariants({ variant: "link" }), "h-auto p-0")}
        >
          <span className="font-semibold">{row.original.productSku}</span>
        </Link>
        <span className="text-muted-foreground">{row.original.productName}</span>
      </div>
    ),
  },
  {
    id: "warehouse",
    header: "Bodega",
    accessorFn: (movement) =>
      `${movement.warehouseCode} ${movement.warehouseName}`,
    cell: ({ row }) => (
      <div className="flex flex-col">
        <span className="font-semibold">{row.original.warehouseCode}</span>
        <span className="text-muted-foreground">
          {row.original.warehouseName}
        </span>
      </div>
    ),
  },
  {
    accessorKey: "movementType",
    header: "Movimiento",
    cell: ({ row }) => {
      const movement = row.original;

      const hasIn = Number(movement.quantityIn) > 0;
      const hasOut = Number(movement.quantityOut) > 0;

      const variant = hasOut ? "destructive" : hasIn ? "default" : "secondary";
      const label = movement.movementType || (hasOut ? "SALIDA" : "ENTRADA");

      return (
        <Badge
          variant={variant}
          className={cn(
            hasIn && !hasOut ? "bg-green-600 text-white hover:bg-green-600" : ""
          )}
        >
          {label}
        </Badge>
      );
    },
  },
  {
    accessorKey: "reference",
    header: "Referencia",
    cell: ({ row }) => (
      <span className="whitespace-nowrap">{row.original.reference}</span>
    ),
  },
  {
    accessorKey: "quantityIn",
    header: () => <div className="text-right">Entrada</div>,
    cell: ({ row }) => (
      <p className="text-right">{Number(row.original.quantityIn).toFixed(2)}</p>
    ),
  },
  {
    accessorKey: "quantityOut",
    header: () => <div className="text-right">Salida</div>,
    cell: ({ row }) => (
      <p className="text-right">{Number(row.original.quantityOut).toFixed(2)}</p>
    ),
  },
  {
    accessorKey: "unitCost",
    header: () => <div className="text-right">Costo Unit.</div>,
    cell: ({ row }) => (
      <p className="text-right">{Number(row.original.unitCost).toFixed(2)}</p>
    ),
  },
  {
    accessorKey: "totalCost",
    header: () => <div className="text-right">Costo Total</div>,
    cell: ({ row }) => (
      <p className="text-right">{Number(row.original.totalCost).toFixed(2)}</p>
    ),
  },
];

