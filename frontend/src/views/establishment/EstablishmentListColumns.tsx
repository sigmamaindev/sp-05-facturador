import { useNavigate } from "react-router-dom";
import type { ColumnDef } from "@tanstack/react-table";

import type { Establishment } from "@/types/establishment.types";

import RowActions from "@/components/shared/RowActions";

export const columns: ColumnDef<Establishment>[] = [
  {
    accessorKey: "id",
    header: "ID",
  },
  {
    accessorKey: "code",
    header: "Código",
  },
  {
    accessorKey: "name",
    header: "Nombre",
  },
  {
    id: "actions",
    header: "Acciones",
    cell: ({ row }) => {
      const navigate = useNavigate();

      const establishment = row.original;
      const actions = [
        {
          label: "Detalles",
          onClick: () => navigate(`/establecimientos/${establishment.id}`),
        },
      ];

      return <RowActions actions={actions} />;
    },
  },
];
