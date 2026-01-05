import React from "react";

import { Input } from "@/components/ui/input";

interface KardexListHeaderProps {
  keyword: string;
  setPage: React.Dispatch<React.SetStateAction<number>>;
  setKeyword: React.Dispatch<React.SetStateAction<string>>;
}

export default function KardexListHeader({
  keyword,
  setPage,
  setKeyword,
}: KardexListHeaderProps) {
  return (
    <div className="grid grid-cols-1 md:flex md:flex-row md:justify-between md:items-center items-center gap-4 pb-4">
      <div className="grid grid-cols-2 items-center gap-4 md:inline-flex w-auto">
        <h1 className="text-lg font-semibold">KARDEX</h1>
      </div>

      <div className="flex flex-col md:flex-row md:items-center gap-3 md:gap-4 w-full md:w-auto">
        <Input
          placeholder="Buscar movimientos..."
          value={keyword}
          onChange={(e) => {
            setPage(1);
            setKeyword(e.target.value);
          }}
          className="max-w-sm"
        />
      </div>
    </div>
  );
}

