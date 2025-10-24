import type { ColumnDef } from "@tanstack/react-table";

import type { User } from "@/types/user.types";

import RowActions from "@/components/shared/RowActions";

export const columns: ColumnDef<User>[] = [
  {
    accessorKey: "document",
    header: "Documento",
  },
  {
    accessorKey: "username",
    header: "Usuario",
  },
  {
    accessorKey: "fullName",
    header: "Nombre",
  },
  {
    accessorKey: "email",
    header: "Correo",
  },
  {
    accessorKey: "createdAt",
    header: "Fecha Creación",
    cell: ({ row }) => {
      const date = new Date(row.original.createdAt);
      return <span>{date.toLocaleDateString("es-EC")}</span>;
    },
  },
  {
    id: "actions",
    header: "Acciones",
    cell: ({ row }) => {
      const user = row.original;
      const actions = [
        {
          label: "Detalles",
          onClick: () => console.log("Detalles", user.id),
        },
        {
          label: "Editar",
          onClick: () => console.log("Editar", user.id),
        },
      ];

      return <RowActions actions={actions} />;
    },
  },
];
