import type { ColumnDef } from "@tanstack/react-table";

import type { AtsPurchase } from "@/types/ats.types";

export const atsPurchasesColumns: ColumnDef<AtsPurchase>[] = [
  {
    accessorKey: "fechaEmision",
    header: "Emisión",
    cell: ({ row }) => {
      const date = new Date(row.original.fechaEmision);
      const dateStr = Number.isNaN(date.getTime())
        ? "-"
        : date.toLocaleDateString("es-EC", {
            year: "numeric",
            month: "2-digit",
            day: "2-digit",
          });

      return <span className="font-semibold">{dateStr}</span>;
    },
  },
  {
    id: "comprobante",
    header: "Comprobante",
    accessorFn: (p) =>
      `${p.establecimiento}-${p.puntoEmision}-${p.secuencial} ${p.tipoComprobante}`,
    cell: ({ row }) => (
      <div className="flex flex-col leading-tight">
        <span className="font-semibold">
          {row.original.establecimiento}-{row.original.puntoEmision}-
          {row.original.secuencial}
        </span>
        <span className="text-muted-foreground">
          {row.original.tipoComprobante}
        </span>
      </div>
    ),
  },
  {
    id: "proveedor",
    header: "Proveedor",
    accessorFn: (p) => `${p.idProv} ${p.proveedorRazonSocial}`,
    cell: ({ row }) => (
      <div className="flex flex-col leading-tight">
        <span className="font-semibold">{row.original.idProv}</span>
        <span className="text-muted-foreground">
          {row.original.proveedorRazonSocial}
        </span>
      </div>
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

