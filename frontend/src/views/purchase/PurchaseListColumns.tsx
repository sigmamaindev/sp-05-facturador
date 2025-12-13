import { useNavigate } from "react-router-dom";
import type { ColumnDef } from "@tanstack/react-table";

import type { Purchase } from "@/types/purchase.type";

import RowActions from "@/components/shared/RowActions";
import { Badge } from "@/components/ui/badge";

export const columns: ColumnDef<Purchase>[] = [
  {
    accessorKey: "id",
    header: "ID",
  },
  {
    accessorKey: "purchaseDate",
    header: "Fecha",
    cell: ({ row }) => {
      const date = new Date(row.original.purchaseDate);

      return (
        <span>
          {date.toLocaleString("es-EC", {
            year: "numeric",
            month: "2-digit",
            day: "2-digit",
            hour: "2-digit",
            minute: "2-digit",
            second: "2-digit",
            hour12: false,
          })}
        </span>
      );
    },
  },
  {
    accessorKey: "documentNumber",
    header: "Documento",
  },
  {
    accessorKey: "status",
    header: "Estado",
    cell: ({ row }) => {
      const status = row.original.status ?? "Sin estado";
      const normalized = status.toUpperCase();

      const variant = normalized.includes("APROBAD")
        ? "default"
        : normalized.includes("BORRADOR")
          ? "secondary"
          : normalized.includes("RECHAZ")
            ? "destructive"
            : "outline";

      return <Badge variant={variant}>{status}</Badge>;
    },
  },
  {
    accessorKey: "total",
    header: () => <div className="text-right">Total</div>,
    cell: ({ row }) => {
      const total = row.original.total?.toFixed(2) ?? "0.00";

      return <p className="text-right">{total}</p>;
    },
  },
  {
    id: "actions",
    header: () => <div className="text-right">Acciones</div>,
    cell: ({ row }) => {
      const navigate = useNavigate();

      const purchase = row.original;

      const actions = [
        {
          label: "Detalles",
          onClick: () => navigate(`/compras/${purchase.id}`),
        },
        {
          label: "Editar",
          onClick: () => navigate(`/compras/actualizar/${purchase.id}`),
        },
      ];

      return (
        <div className="text-right">
          <RowActions actions={actions} />
        </div>
      );
    },
  },
];
