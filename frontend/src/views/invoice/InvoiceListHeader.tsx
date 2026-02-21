import { useNavigate } from "react-router-dom";
import { PlusIcon } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

interface InvoiceListHeaderProps {
  keyword: string;
  setPage: React.Dispatch<React.SetStateAction<number>>;
  setKeyword: React.Dispatch<React.SetStateAction<string>>;
}

export default function InvoiceListHeader({
  keyword,
  setPage,
  setKeyword,
}: InvoiceListHeaderProps) {
  const navigate = useNavigate();

  return (
    <div className="grid grid-cols-1 md:flex md:flex-row md:justify-between md:items-center items-center gap-4 pb-4">
      <div className="grid grid-cols-2 items-center gap-4 md:inline-flex w-auto">
        <h1 className="text-lg font-semibold">FACTURAS</h1>
      </div>
      <div className="flex flex-col md:flex-row md:items-center gap-3 md:gap-4 w-full md:w-auto">
        <Input
          placeholder="Buscar facturas..."
          value={keyword}
          onChange={(e) => {
            setPage(1);
            setKeyword(e.target.value);
          }}
          className="max-w-sm"
        />
        <div className="flex justify-end w-full md:w-auto">
          <Button onClick={() => navigate("/facturas/crear")}>
            <PlusIcon />
            Factura
          </Button>
        </div>
      </div>
    </div>
  );
}
