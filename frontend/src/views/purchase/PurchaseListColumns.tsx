import type { ColumnDef } from "@tanstack/react-table";

import type { Purchase } from "@/types/purchase.type";

import { useAuth } from "@/contexts/AuthContext";

import RowActions from "@/components/shared/RowActions";
import { Badge } from "@/components/ui/badge";

export const columns: ColumnDef<Purchase>[] = [
  {
    accessorKey: "issueDate",
    header: "Fecha",
    cell: ({ row }) => {
      const date = new Date(row.original.issueDate);

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
    accessorFn: (purchase) =>
      `${purchase.establishmentCode} ${purchase.emissionPointCode} ${purchase.sequential}`,
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
    accessorFn: (purchase) =>
      `${purchase.supplier?.document ?? ""} ${purchase.supplier?.businessName ?? ""}`,
    cell: ({ row }) => {
      const supplier = row.original.supplier;

      return supplier ? (
        <div className="flex flex-col">
          <span className="font-semibold">{supplier.document}</span>
          <span className="text-muted-foreground">{supplier.businessName}</span>
        </div>
      ) : (
        <span className="text-muted-foreground">
          Proveedor #{row.original.supplierId}
        </span>
      );
    },
  },
  {
    accessorKey: "totalPurchase",
    header: () => <div className="text-right">Total</div>,
    cell: ({ row }) => {
      const total = row.original.totalPurchase?.toFixed(2) ?? "0.00";

      return <p className="text-right">{total}</p>;
    },
  },
  {
    id: "actions",
    header: () => <div className="text-right">Acciones</div>,
    cell: ({ row }) => {
      const { user } = useAuth();
      const purchase = row.original;

      const hasPermission =
        user?.roles?.includes("SuperAdmin") || user?.roles?.includes("Admin");

      const actions = [
        {
          label: "Detalles",
          to: `/compras/${purchase.id}`,
        },
        ...(hasPermission
          ? [
              {
                label: "Editar",
                to: `/compras/actualizar/${purchase.id}`,
              },
            ]
          : []),
      ];

      return (
        <div className="text-right">
          <RowActions actions={actions} />
        </div>
      );
    },
  },
];
