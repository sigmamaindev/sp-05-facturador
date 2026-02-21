import React from "react";

import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

interface PurchasesReportListHeaderProps {
  keyword: string;
  dateFrom: string;
  dateTo: string;
  setPage: React.Dispatch<React.SetStateAction<number>>;
  setKeyword: React.Dispatch<React.SetStateAction<string>>;
  setDateFrom: React.Dispatch<React.SetStateAction<string>>;
  setDateTo: React.Dispatch<React.SetStateAction<string>>;
}

export default function PurchasesReportListHeader({
  keyword,
  dateFrom,
  dateTo,
  setPage,
  setKeyword,
  setDateFrom,
  setDateTo,
}: PurchasesReportListHeaderProps) {
  function handleChange(setter: React.Dispatch<React.SetStateAction<string>>) {
    return (e: React.ChangeEvent<HTMLInputElement>) => {
      setPage(1);
      setter(e.target.value);
    };
  }

  const hasFilters = keyword || dateFrom || dateTo;

  function clearFilters() {
    setPage(1);
    setKeyword("");
    setDateFrom("");
    setDateTo("");
  }

  return (
    <div className="flex flex-col gap-4 pb-4">
      <h1 className="text-lg font-semibold">REPORTE DE COMPRAS</h1>

      <div className="flex flex-col md:flex-row md:flex-wrap md:items-center gap-3">
        <Input
          placeholder="Buscar por proveedor o documento..."
          value={keyword}
          onChange={handleChange(setKeyword)}
          className="max-w-sm"
        />
        <div className="flex items-center gap-2">
          <Input
            type="date"
            value={dateFrom}
            onChange={handleChange(setDateFrom)}
            className="w-44"
            aria-label="Desde"
          />
          <span className="text-muted-foreground text-sm">—</span>
          <Input
            type="date"
            value={dateTo}
            onChange={handleChange(setDateTo)}
            className="w-44"
            aria-label="Hasta"
          />
        </div>
        {hasFilters && (
          <Button type="button" variant="outline" size="sm" onClick={clearFilters}>
            Limpiar filtros
          </Button>
        )}
      </div>
    </div>
  );
}
