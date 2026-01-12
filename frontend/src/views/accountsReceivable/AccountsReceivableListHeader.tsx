import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

interface AccountsReceivableListHeaderProps {
  keyword: string;
  viewMode: "invoice" | "customer";
  setPage: React.Dispatch<React.SetStateAction<number>>;
  setKeyword: React.Dispatch<React.SetStateAction<string>>;
  setViewMode: React.Dispatch<React.SetStateAction<"invoice" | "customer">>;
}

export default function AccountsReceivableListHeader({
  keyword,
  viewMode,
  setPage,
  setKeyword,
  setViewMode,
}: AccountsReceivableListHeaderProps) {
  return (
    <div className="grid grid-cols-1 md:flex md:flex-row md:justify-between md:items-center items-center gap-4 pb-4">
      <div className="grid grid-cols-2 items-center gap-4 md:inline-flex w-auto">
        <h1 className="text-lg font-semibold">CUENTAS POR COBRAR</h1>
      </div>
      <div className="flex flex-col md:flex-row md:items-center gap-3 md:gap-4 w-full md:w-auto">
        <Input
          placeholder="Buscar cuentas por cobrar..."
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
            variant={viewMode === "invoice" ? "default" : "outline"}
            onClick={() => {
              setPage(1);
              setViewMode("invoice");
            }}
          >
            Por factura
          </Button>
          <Button
            type="button"
            size="sm"
            variant={viewMode === "customer" ? "default" : "outline"}
            onClick={() => {
              setPage(1);
              setViewMode("customer");
            }}
          >
            Por cliente
          </Button>
        </div>
      </div>
    </div>
  );
}
