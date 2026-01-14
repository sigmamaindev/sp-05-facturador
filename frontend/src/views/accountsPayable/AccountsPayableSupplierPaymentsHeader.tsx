import { ArrowLeft, Loader2 } from "lucide-react";
import { useNavigate } from "react-router-dom";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardHeader, CardTitle } from "@/components/ui/card";

interface AccountsPayableSupplierPaymentsHeaderProps {
  backTo: string;
  keyword: string;
  sending: boolean;
  totalAmount: number;
  setPage: React.Dispatch<React.SetStateAction<number>>;
  setKeyword: React.Dispatch<React.SetStateAction<string>>;
  onSendPayments: () => void;
}

export default function AccountsPayableSupplierPaymentsHeader({
  backTo,
  keyword,
  sending,
  totalAmount,
  setPage,
  setKeyword,
  onSendPayments,
}: AccountsPayableSupplierPaymentsHeaderProps) {
  const navigate = useNavigate();

  return (
    <Card>
      <CardHeader className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div className="flex items-center gap-2">
          <Button variant="ghost" onClick={() => navigate(backTo)}>
            <ArrowLeft className="h-4 w-4" />
          </Button>
          <CardTitle>Pagos de cuentas por pagar</CardTitle>
        </div>

        <div className="flex flex-col md:flex-row md:items-center gap-3 md:gap-4 w-full md:w-auto">
          <Input
            placeholder="Buscar por secuencial o estado..."
            value={keyword}
            onChange={(e) => {
              setPage(1);
              setKeyword(e.target.value);
            }}
            className="max-w-sm"
          />
          <Input
            value={totalAmount.toFixed(2)}
            readOnly
            className="w-[140px] text-right"
            aria-label="Total a enviar"
            title="Total a enviar"
          />
          <Button onClick={onSendPayments} disabled={sending}>
            {sending ? <Loader2 className="animate-spin" /> : null}
            Enviar pagos
          </Button>
        </div>
      </CardHeader>
    </Card>
  );
}

