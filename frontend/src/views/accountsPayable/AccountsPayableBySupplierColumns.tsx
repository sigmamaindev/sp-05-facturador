import type { ColumnDef } from "@tanstack/react-table";

import type { AccountsPayableSupplierSummary } from "@/types/accountsPayable.types";

import AccountsPayableBySupplierActionsCell from "./AccountsPayableBySupplierActionsCell";

export const bySupplierColumns: ColumnDef<AccountsPayableSupplierSummary>[] = [
  {
    id: "supplier",
    header: "Proveedor",
    accessorFn: (row) => `${row.supplier.document} ${row.supplier.businessName}`,
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
    accessorKey: "totalBalance",
    header: () => <div className="text-right">Saldo total</div>,
    cell: ({ row }) => (
      <p className="text-right">{row.original.totalBalance.toFixed(2)}</p>
    ),
  },
  {
    id: "actions",
    header: () => <div className="text-center">Acciones</div>,
    cell: ({ row }) => (
      <AccountsPayableBySupplierActionsCell summary={row.original} />
    ),
  },
];

