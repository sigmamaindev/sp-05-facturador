import { useNavigate } from "react-router-dom";
import { PlusIcon } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

import { useAuth } from "@/contexts/AuthContext";

interface PurchaseListHeaderProps {
  keyword: string;
  setPage: React.Dispatch<React.SetStateAction<number>>;
  setKeyword: React.Dispatch<React.SetStateAction<string>>;
  atsYear: number;
  atsMonth: number;
  setAtsYear: React.Dispatch<React.SetStateAction<number>>;
  setAtsMonth: React.Dispatch<React.SetStateAction<number>>;
  onShowAtsData: () => void;
  onDownloadAtsXml: () => void;
}

export default function PurchaseListHeader({
  keyword,
  setKeyword,
  setPage,
  atsYear,
  atsMonth,
  setAtsYear,
  setAtsMonth,
  onShowAtsData,
  onDownloadAtsXml,
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
          <div className="flex flex-col sm:flex-row justify-end w-full md:w-auto gap-2">
            <div className="flex flex-row items-center justify-end gap-2">
              <Input
                type="number"
                inputMode="numeric"
                min={2000}
                max={2100}
                value={atsYear}
                onChange={(e) => {
                  if (e.target.value === "") return;
                  const next = Number(e.target.value);
                  if (!Number.isFinite(next)) return;
                  setAtsYear(Math.min(2100, Math.max(2000, next)));
                }}
                className="w-24"
                aria-label="Año ATS"
                placeholder="Año"
              />
              <Input
                type="number"
                inputMode="numeric"
                min={1}
                max={12}
                value={atsMonth}
                onChange={(e) => {
                  if (e.target.value === "") return;
                  const next = Number(e.target.value);
                  if (!Number.isFinite(next)) return;
                  setAtsMonth(Math.min(12, Math.max(1, next)));
                }}
                className="w-20"
                aria-label="Mes ATS"
                placeholder="Mes"
              />

              <Button variant="outline" onClick={onShowAtsData}>
                Show ATS Data
              </Button>
              <Button variant="outline" onClick={onDownloadAtsXml}>
                Download ATS XML
              </Button>
            </div>

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
