import type { ColumnDef } from "@tanstack/react-table";

import type { AccountsReceivableCustomerSummary } from "@/types/accountsReceivable.types";

import AccountsReceivableByCustomerActionsCell from "./AccountsReceivableByCustomerActionsCell";

export const byCustomerColumns: ColumnDef<AccountsReceivableCustomerSummary>[] =
  [
    {
      id: "customer",
      header: "Cliente",
      accessorFn: (row) => `${row.customer.document} ${row.customer.name}`,
      cell: ({ row }) => (
        <div className="flex flex-col">
          <span className="font-semibold">{row.original.customer.document}</span>
          <span className="text-muted-foreground">
            {row.original.customer.name}
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
        <AccountsReceivableByCustomerActionsCell summary={row.original} />
      ),
    },
  ];
