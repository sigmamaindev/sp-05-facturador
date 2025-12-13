import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { useNavigate } from "react-router-dom";

interface PurchaseListHeaderProps {
  keyword: string;
  setKeyword: (keyword: string) => void;
  setPage: (page: number) => void;
}

export default function PurchaseListHeader({
  keyword,
  setKeyword,
  setPage,
}: PurchaseListHeaderProps) {
  const navigate = useNavigate();

  const handleSearch = (value: string) => {
    setKeyword(value);
    setPage(1);
  };

  return (
    <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4 py-4">
      <div className="flex items-center gap-2 w-full sm:w-auto">
        <Input
          placeholder="Buscar compras..."
          value={keyword}
          onChange={(e) => handleSearch(e.target.value)}
        />
      </div>

      <Button onClick={() => navigate("/compras/crear")}>
        Registrar compra
      </Button>
    </div>
  );
}
