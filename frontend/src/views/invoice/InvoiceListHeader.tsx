import { useNavigate } from "react-router-dom";
import { PlusIcon } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

import { useAuth } from "@/contexts/AuthContext";

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
  const { user } = useAuth();
  const navigate = useNavigate();

  const hasPermission =
    user?.roles?.includes("SuperAdmin") || user?.roles?.includes("Admin");

  return (
    <div className="grid grid-cols-1 md:flex md:flex-row md:justify-between md:items-center items-center gap-4 pb-4">
      <div className="grid grid-cols-2 items-center gap-4 md:inline-flex w-auto">
        <h1 className="text-lg font-semibold">FACTURAS</h1>
      </div>
      <div className="inline-flex items-center gap-4 w-auto">
        <Input
          placeholder="Buscar facturas..."
          value={keyword}
          onChange={(e) => {
            setPage(1);
            setKeyword(e.target.value);
          }}
          className="max-w-sm"
        />
        {hasPermission && (
          <Button onClick={() => navigate("/facturas/crear")}>
            <PlusIcon />
            Factura
          </Button>
        )}
      </div>
    </div>
  );
}
