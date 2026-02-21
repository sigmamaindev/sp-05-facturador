import React from "react";

import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

interface SalesReportListHeaderProps {
  keyword: string;
  creditDays: string;
  dateFrom: string;
  dateTo: string;
  setPage: React.Dispatch<React.SetStateAction<number>>;
  setKeyword: React.Dispatch<React.SetStateAction<string>>;
  setCreditDays: React.Dispatch<React.SetStateAction<string>>;
  setDateFrom: React.Dispatch<React.SetStateAction<string>>;
  setDateTo: React.Dispatch<React.SetStateAction<string>>;
}

export default function SalesReportListHeader({
  keyword,
  creditDays,
  dateFrom,
  dateTo,
  setPage,
  setKeyword,
  setCreditDays,
  setDateFrom,
  setDateTo,
}: SalesReportListHeaderProps) {
  function handleChange(setter: React.Dispatch<React.SetStateAction<string>>) {
    return (e: React.ChangeEvent<HTMLInputElement>) => {
      setPage(1);
      setter(e.target.value);
    };
  }

  const hasFilters = keyword || creditDays || dateFrom || dateTo;

  function clearFilters() {
    setPage(1);
    setKeyword("");
    setCreditDays("");
    setDateFrom("");
    setDateTo("");
  }

  return (
    <div className="flex flex-col gap-4 pb-4">
      <h1 className="text-lg font-semibold">REPORTE DE VENTAS</h1>

      <div className="flex flex-col md:flex-row md:flex-wrap md:items-center gap-3">
        <Input
          placeholder="Buscar por cliente o documento..."
          value={keyword}
          onChange={handleChange(setKeyword)}
          className="max-w-sm"
        />
        <Input
          type="number"
          placeholder="Plazo de crédito (días)"
          value={creditDays}
          onChange={handleChange(setCreditDays)}
          min={0}
          className="w-52"
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
