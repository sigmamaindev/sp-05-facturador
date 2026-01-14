import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

interface AccountsPayableListHeaderProps {
  keyword: string;
  viewMode: "purchase" | "supplier";
  setPage: React.Dispatch<React.SetStateAction<number>>;
  setKeyword: React.Dispatch<React.SetStateAction<string>>;
  setViewMode: React.Dispatch<React.SetStateAction<"purchase" | "supplier">>;
}

export default function AccountsPayableListHeader({
  keyword,
  viewMode,
  setPage,
  setKeyword,
  setViewMode,
}: AccountsPayableListHeaderProps) {
  return (
    <div className="grid grid-cols-1 md:flex md:flex-row md:justify-between md:items-center items-center gap-4 pb-4">
      <div className="grid grid-cols-2 items-center gap-4 md:inline-flex w-auto">
        <h1 className="text-lg font-semibold">CUENTAS POR PAGAR</h1>
      </div>
      <div className="flex flex-col md:flex-row md:items-center gap-3 md:gap-4 w-full md:w-auto">
        <Input
          placeholder="Buscar cuentas por pagar..."
          value={keyword}
          onChange={(e) => {
            setPage(1);
            setKeyword(e.target.value);
          }}
          className="max-w-sm"
        />
        <div className="flex gap-2 justify-end w-full md:w-auto">
          <Button
            type="button"
            size="sm"
            variant={viewMode === "purchase" ? "default" : "outline"}
            onClick={() => {
              setPage(1);
              setViewMode("purchase");
            }}
          >
            Por compra
          </Button>
          <Button
            type="button"
            size="sm"
            variant={viewMode === "supplier" ? "default" : "outline"}
            onClick={() => {
              setPage(1);
              setViewMode("supplier");
            }}
          >
            Por proveedor
          </Button>
        </div>
      </div>
    </div>
  );
}
