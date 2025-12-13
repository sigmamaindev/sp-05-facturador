import { ArrowLeft } from "lucide-react";
import { useNavigate } from "react-router-dom";

import { Button } from "@/components/ui/button";
import { Card, CardHeader, CardTitle } from "@/components/ui/card";

interface PurchaseCreateHeaderProps {
  onSave: () => void;
  saving: boolean;
}

export default function PurchaseCreateHeader({
  onSave,
  saving,
}: PurchaseCreateHeaderProps) {
  const navigate = useNavigate();

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <div className="flex items-center gap-2">
          <Button variant="ghost" onClick={() => navigate("/compras")}> 
            <ArrowLeft className="h-4 w-4" />
          </Button>
          <CardTitle>Registrar compra</CardTitle>
        </div>
        <div className="flex items-center gap-2">
          <Button variant="secondary" onClick={() => navigate("/compras")}>
            Cancelar
          </Button>
          <Button onClick={onSave} disabled={saving}>
            {saving ? "Guardando..." : "Guardar compra"}
          </Button>
        </div>
      </CardHeader>
    </Card>
  );
}
