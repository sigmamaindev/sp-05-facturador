import { useNavigate } from "react-router-dom";
import type { ColumnDef } from "@tanstack/react-table";

import type { EmissionPoint } from "@/types/emissionPoint.types";

import { useAuth } from "@/contexts/AuthContext";

import RowActions from "@/components/shared/RowActions";

export const columns = (
  establishmentId: number | null
): ColumnDef<EmissionPoint>[] => [
  {
    accessorKey: "id",
    header: "ID",
  },
  {
    accessorKey: "code",
    header: "Código",
  },
  {
    accessorKey: "description",
    header: "Descripción",
  },
  {
    id: "actions",
    header: "Acciones",
    cell: ({ row }) => {
      const navigate = useNavigate();

      const { user } = useAuth();

      const hasPermission =
        user?.roles?.includes("SuperAdmin") || user?.roles?.includes("Admin");

      const emissionPoint = row.original;
      const actions = [
        {
          label: "Detalles",
          onClick: () =>
            navigate(
              `/puntos-emision/${emissionPoint.id}?establecimiento=${establishmentId}`
            ),
        },
      ];

      if (hasPermission) {
        actions.push({
          label: "Editar",
          onClick: () =>
            navigate(
              `/puntos-emision/actualizar/${emissionPoint.id}?establecimiento=${establishmentId}`
            ),
        });
      }

      return <RowActions actions={actions} />;
    },
  },
];
