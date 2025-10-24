import type { ColumnDef } from "@tanstack/react-table";

import type { Customer } from "@/types/customer.types";

import { Button } from "@/components/ui/button";

interface ColumnsProps {
  onSelect?: (customer: Customer) => void;
}

export const columns = ({
  onSelect,
}: ColumnsProps): ColumnDef<Customer>[] => [
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
    id: "actions",
    header: "Acciones",
    cell: ({ row }) => {
      const customer = row.original;
      return <Button size="sm" onClick={() => onSelect?.(customer)}>Escoger</Button>;
    },
  },
];
