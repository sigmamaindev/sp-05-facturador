import { ArrowLeft } from "lucide-react";
import { useNavigate } from "react-router-dom";

import { Button } from "@/components/ui/button";
import { Card, CardHeader, CardTitle } from "@/components/ui/card";

interface PurchaseDetailHeaderProps {
  documentNumber?: string;
}

export default function PurchaseDetailHeader({
  documentNumber,
}: PurchaseDetailHeaderProps) {
  const navigate = useNavigate();

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <div className="flex items-center gap-2">
          <Button variant="ghost" onClick={() => navigate("/compras")}>
            <ArrowLeft className="h-4 w-4" />
          </Button>
          <CardTitle>
            Compra #{documentNumber ?? "sin número"}
          </CardTitle>
        </div>
        <Button onClick={() => navigate("/compras/crear")}>Nueva compra</Button>
      </CardHeader>
    </Card>
  );
}
