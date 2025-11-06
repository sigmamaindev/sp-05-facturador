import { useNavigate } from "react-router-dom";
import type { ColumnDef } from "@tanstack/react-table";

import type { Product } from "@/types/product.types";

import { useAuth } from "@/contexts/AuthContext";

import RowActions from "@/components/shared/RowActions";

export const columns: ColumnDef<Product>[] = [
  {
    accessorKey: "id",
    header: "ID",
  },
  {
    accessorKey: "sku",
    header: "Código",
  },
  {
    accessorKey: "name",
    header: "Nombre",
  },
  {
    accessorKey: "price",
    header: "Precio",
    cell: ({ row }) => {
      const price = row.original.price.toFixed(2);

      return <span>{price}</span>;
    },
  },
  {
    id: "actions",
    header: "Acciones",
    cell: ({ row }) => {
      const navigate = useNavigate();

      const { user } = useAuth();

      const hasPermission =
        user?.roles?.includes("SuperAdmin") || user?.roles?.includes("Admin");

      const product = row.original;
      const actions = [
        {
          label: "Detalles",
          onClick: () => console.log("Detalles"),
        },
      ];

      if (hasPermission) {
        actions.push({
          label: "Editar",
          onClick: () => navigate(`/productos/actualizar/${product.id}`),
        });
      }

      return <RowActions actions={actions} />;
    },
  },
];
