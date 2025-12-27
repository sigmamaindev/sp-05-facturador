import { useNavigate } from "react-router-dom";
import { PlusIcon } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

import { useAuth } from "@/contexts/AuthContext";

interface PurchaseListHeaderProps {
  keyword: string;
  setPage: React.Dispatch<React.SetStateAction<number>>;
  setKeyword: React.Dispatch<React.SetStateAction<string>>;
}

export default function PurchaseListHeader({
  keyword,
  setKeyword,
  setPage,
}: PurchaseListHeaderProps) {
  const { user } = useAuth();
  const navigate = useNavigate();

  const hasPermission =
    user?.roles?.includes("SuperAdmin") || user?.roles?.includes("Admin");

  return (
    <div className="grid grid-cols-1 md:flex md:flex-row md:justify-between md:items-center items-center gap-4 pb-4">
      <div className="grid grid-cols-2 items-center gap-4 md:inline-flex w-auto">
        <h1 className="text-lg font-semibold">COMPRAS</h1>
      </div>
      <div className="flex flex-col md:flex-row md:items-center gap-3 md:gap-4 w-full md:w-auto">
        <Input
          placeholder="Buscar compras..."
          value={keyword}
          onChange={(e) => {
            setPage(1);
            setKeyword(e.target.value);
          }}
          className="max-w-sm"
        />
        {hasPermission && (
          <div className="flex justify-end w-full md:w-auto">
            <Button onClick={() => navigate("/compras/crear")}>
              <PlusIcon />
              Compra
            </Button>
          </div>
        )}
      </div>
    </div>
  );
}
