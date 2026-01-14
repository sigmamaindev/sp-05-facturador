import type { ColumnDef } from "@tanstack/react-table";

import type { AtsSale } from "@/types/ats.types";

export const atsSalesColumns: ColumnDef<AtsSale>[] = [
  {
    id: "cliente",
    header: "Cliente",
    accessorFn: (s) => `${s.idCliente} ${s.clienteRazonSocial}`,
    cell: ({ row }) => (
      <div className="flex flex-col leading-tight">
        <span className="font-semibold">{row.original.idCliente}</span>
        <span className="text-muted-foreground">
          {row.original.clienteRazonSocial}
        </span>
      </div>
    ),
  },
  {
    id: "comprobante",
    header: "Comprobante",
    accessorFn: (s) => `${s.tipoComprobante} ${s.tipoEmision}`,
    cell: ({ row }) => (
      <div className="flex flex-col leading-tight">
        <span className="font-semibold">{row.original.tipoComprobante}</span>
        <span className="text-muted-foreground">{row.original.tipoEmision}</span>
      </div>
    ),
  },
  {
    accessorKey: "numeroComprobantes",
    header: () => <div className="text-right">#</div>,
    cell: ({ row }) => (
      <div className="text-right">{row.original.numeroComprobantes}</div>
    ),
  },
  {
    accessorKey: "montoIva",
    header: () => <div className="text-right">IVA</div>,
    cell: ({ row }) => (
      <div className="text-right">{row.original.montoIva.toFixed(2)}</div>
    ),
  },
  {
    accessorKey: "total",
    header: () => <div className="text-right">Total</div>,
    cell: ({ row }) => (
      <div className="text-right font-semibold">
        {row.original.total.toFixed(2)}
      </div>
    ),
  },
];

