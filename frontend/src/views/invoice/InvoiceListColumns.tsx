import { useNavigate } from "react-router-dom";
import type { ColumnDef } from "@tanstack/react-table";

import type { Invoice } from "@/types/invoice.type";

import { useAuth } from "@/contexts/AuthContext";

import RowActions from "@/components/shared/RowActions";

export const columns: ColumnDef<Invoice>[] = [
  {
    accessorKey: "id",
    header: "ID",
  },
  {
    accessorKey: "sequential",
    header: "Código",
  },
  {
    accessorKey: "customer.name",
    header: "Cliente",
  },
  {
    accessorKey: "price",
    header: "Precio",
    cell: ({ row }) => {
      const price = row.original.totalInvoice.toFixed(2);

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
          onClick: () => navigate(`/facturas/actualizar/${product.id}`),
        });
      }

      return <RowActions actions={actions} />;
    },
  },
];
