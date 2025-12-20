import type { ColumnDef } from "@tanstack/react-table";

import type { UnitMeasure } from "@/types/unitMeasure.types";

import { Button } from "@/components/ui/button";

interface ColumnsProps {
  onSelect?: (unitMeasure: UnitMeasure) => void;
}

export const columns = ({
  onSelect,
}: ColumnsProps): ColumnDef<UnitMeasure>[] => [
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
      const unitMeasure = row.original;
      return (
        <Button size="sm" onClick={() => onSelect?.(unitMeasure)}>
          Escoger
        </Button>
      );
    },
  },
];

