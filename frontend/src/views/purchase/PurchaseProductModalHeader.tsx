import { Input } from "@/components/ui/input";

interface PurchaseProductModalHeaderProps {
  keyword: string;
  setKeyword: (keyword: string) => void;
  setPage: (page: number) => void;
}

export default function PurchaseProductModalHeader({
  keyword,
  setKeyword,
  setPage,
}: PurchaseProductModalHeaderProps) {
  const handleSearch = (value: string) => {
    setKeyword(value);
    setPage(1);
  };

  return (
    <div className="flex items-center gap-2 py-4">
      <Input
        placeholder="Buscar productos..."
        value={keyword}
        onChange={(e) => handleSearch(e.target.value)}
      />
    </div>
  );
}
