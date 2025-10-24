import React from "react";

import { Input } from "@/components/ui/input";

interface InvoiceProductModalHeaderProps {
  keyword: string;
  setPage: React.Dispatch<React.SetStateAction<number>>;
  setKeyword: React.Dispatch<React.SetStateAction<string>>;
}

export default function InvoiceProductModalHeader({
  keyword,
  setPage,
  setKeyword,
}: InvoiceProductModalHeaderProps) {
  return (
    <div className="grid grid-cols-1 md:flex md:flex-row md:justify-between md:items-center items-center gap-4 pb-4">
      <div className="inline-flex items-center gap-4 w-full">
        <Input
          placeholder="Buscar producto..."
          value={keyword}
          onChange={(e) => {
            setPage(1);
            setKeyword(e.target.value);
          }}
          className="w-full"
        />
      </div>
    </div>
  );
}
