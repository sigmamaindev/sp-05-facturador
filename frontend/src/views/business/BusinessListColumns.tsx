import type { ColumnDef } from "@tanstack/react-table";
import { useNavigate } from "react-router-dom";

import type { Business } from "@/types/business.types";

import RowActions from "@/components/shared/RowActions";

export const columns: ColumnDef<Business>[] = [
  {
    accessorKey: "document",
    header: "Documento",
  },
  {
    accessorKey: "name",
    header: "Nombre",
  },
  {
    accessorKey: "address",
    header: "Dirección",
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
      const navigate = useNavigate();

      const business = row.original;
      const actions = [
        {
          label: "Detalles",
          onClick: () => navigate(`/empresas/${business.id}`),
        },
      ];
      return <RowActions actions={actions} />;
    },
  },
];
