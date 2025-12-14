import { useNavigate } from "react-router-dom";
import type { ColumnDef } from "@tanstack/react-table";

import type { Supplier } from "@/types/supplier.types";

import { useAuth } from "@/contexts/AuthContext";

import RowActions from "@/components/shared/RowActions";

export const columns: ColumnDef<Supplier>[] = [
  {
    accessorKey: "id",
    header: "ID",
  },
  {
    accessorKey: "document",
    header: "Documento",
  },
  {
    accessorKey: "businessName",
    header: "Nombre",
  },
  {
    accessorKey: "cellphone",
    header: "Celular",
  },
  {
    id: "actions",
    header: "Acciones",
    cell: ({ row }) => {
      const navigate = useNavigate();

      const { user } = useAuth();

      const supplier = row.original;

      const hasPermission =
        user?.roles?.includes("SuperAdmin") || user?.roles?.includes("Admin");

      const actions = [
        {
          label: "Detalles",
          onClick: () => navigate(`/proveedores/${supplier.id}`),
        },
      ];

      if (hasPermission) {
        actions.push({
          label: "Editar",
          onClick: () => navigate(`/proveedores/actualizar/${supplier.id}`),
        });
      }

      return <RowActions actions={actions} />;
    },
  },
];
