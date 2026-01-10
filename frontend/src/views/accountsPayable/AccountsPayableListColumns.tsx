import type { ColumnDef } from "@tanstack/react-table";

import type { AccountsPayable } from "@/types/accountsPayable.types";

import AccountsPayableActionsCell from "./AccountsPayableActionsCell";

function daysUntil(dueDate: Date) {
  const due = new Date(dueDate);
  const today = new Date();

  due.setHours(0, 0, 0, 0);
  today.setHours(0, 0, 0, 0);

  return Math.ceil((due.getTime() - today.getTime()) / 86400000);
}

export const columns: ColumnDef<AccountsPayable>[] = [
  {
    accessorKey: "issueDate",
    header: "Fecha de emisión",
    cell: ({ row }) => {
      const date = new Date(row.original.purchase.issueDate);

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
        <div className="flex flex-col">
          <span className="font-semibold">{dateStr}</span>
          <span className="text-muted-foreground">{timeStr}</span>
        </div>
      );
    },
  },
  {
    id: "code",
    header: "Código",
    accessorFn: (ap) =>
      ap.purchase.establishmentCode && ap.purchase.emissionPointCode
        ? `${ap.purchase.establishmentCode} ${ap.purchase.emissionPointCode} ${ap.purchase.sequential}`
        : ap.purchase.sequential,
    cell: ({ row }) => {
      const establishmentCode = row.original.purchase.establishmentCode;
      const emissionPointCode = row.original.purchase.emissionPointCode;

      return (
        <span className="font-semibold">
          {establishmentCode && emissionPointCode
            ? `${establishmentCode}-${emissionPointCode}-${row.original.purchase.sequential}`
            : row.original.purchase.sequential}
        </span>
      );
    },
  },
  {
    id: "supplier",
    header: "Proveedor",
    accessorFn: (ap) => `${ap.supplier.document} ${ap.supplier.businessName}`,
    cell: ({ row }) => (
      <div className="flex flex-col">
        <span className="font-semibold">{row.original.supplier.document}</span>
        <span className="text-muted-foreground">
          {row.original.supplier.businessName}
        </span>
      </div>
    ),
  },
  {
    accessorKey: "dueDate",
    header: "Vencimiento",
    cell: ({ row }) => {
      const due = new Date(row.original.dueDate);
      const days = daysUntil(due);

      const dueStr = due.toLocaleDateString("es-EC", {
        year: "numeric",
        month: "2-digit",
        day: "2-digit",
      });

      const label =
        days < 0
          ? `Vencida (${Math.abs(days)} día${Math.abs(days) === 1 ? "" : "s"})`
          : days === 0
          ? "Vence hoy"
          : `Por vencer: ${days} día${days === 1 ? "" : "s"}`;

      return (
        <div className="flex flex-col gap-1">
          <span className="font-semibold">{dueStr}</span>
          <span className="text-muted-foreground">{label}</span>
        </div>
      );
    },
  },
  {
    accessorKey: "balance",
    header: () => <div className="text-right">Saldo</div>,
    cell: ({ row }) => <p className="text-right">{row.original.balance.toFixed(2)}</p>,
  },
  {
    id: "actions",
    header: () => <div className="text-center">Acciones</div>,
    cell: ({ row }) => {
      const ap = row.original;

      return <AccountsPayableActionsCell accountsPayable={ap} />;
    },
  },
];
