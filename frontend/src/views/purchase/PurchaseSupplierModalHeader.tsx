import { Input } from "@/components/ui/input";

interface PurchaseSupplierModalHeaderProps {
  keyword: string;
  setKeyword: (keyword: string) => void;
}

export default function PurchaseSupplierModalHeader({
  keyword,
  setKeyword,
}: PurchaseSupplierModalHeaderProps) {
  return (
    <div className="flex items-center gap-2 py-4">
      <Input
        placeholder="Buscar proveedores..."
        value={keyword}
        onChange={(e) => setKeyword(e.target.value)}
      />
    </div>
  );
}
