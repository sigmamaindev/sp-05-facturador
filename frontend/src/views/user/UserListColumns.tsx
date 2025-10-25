import { useNavigate } from "react-router-dom";
import type { ColumnDef } from "@tanstack/react-table";

import type { User } from "@/types/user.types";

import RowActions from "@/components/shared/RowActions";
import { Badge } from "@/components/ui/badge";

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
    accessorKey: "roles",
    header: "Roles",
    cell: ({ row }) => {
      const roles = row.original.roles;

      if (!roles || roles.length === 0) {
        return <span className="text-muted-foreground text-sm">Sin roles</span>;
      }

      return (
        <div className="flex flex-wrap gap-1">
          {roles.map((role) => (
            <Badge key={role.id} variant="default" className="text-xs">
              {role.name}
            </Badge>
          ))}
        </div>
      );
    },
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

      const user = row.original;
      const actions = [
        {
          label: "Detalles",
          onClick: () => console.log("Detalles", user.id),
        },
        {
          label: "Editar",
          onClick: () => navigate(`/usuarios/actualizar/${user.id}`),
        },
      ];

      return <RowActions actions={actions} />;
    },
  },
];
