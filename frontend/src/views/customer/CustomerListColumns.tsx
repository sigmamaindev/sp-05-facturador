import type { ColumnDef } from "@tanstack/react-table";

import type { Customer } from "@/types/customer.types";

import RowActions from "@/components/shared/RowActions";

export const columns: ColumnDef<Customer>[] = [
  {
    accessorKey: "id",
    header: "ID",
  },
  {
    accessorKey: "document",
    header: "Documento",
  },
  {
    accessorKey: "name",
    header: "Nombre",
  },
  {
    accessorKey: "email",
    header: "Correo",
  },
  {
    accessorKey: "cellphone",
    header: "Celular",
  },
  {
    id: "actions",
    header: "Acciones",
    cell: ({ row }) => {
      const customer = row.original;
      const actions = [
        {
          label: "Detalles",
          onClick: () => console.log("Detalles", customer.id),
        },
      ];

      return <RowActions actions={actions} />;
    },
  },
];
