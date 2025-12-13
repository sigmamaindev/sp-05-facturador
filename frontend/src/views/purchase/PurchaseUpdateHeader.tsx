import { ArrowLeft } from "lucide-react";
import { useNavigate } from "react-router-dom";

import { Button } from "@/components/ui/button";
import { Card, CardHeader, CardTitle } from "@/components/ui/card";

interface PurchaseUpdateHeaderProps {
  onSave: () => void;
  saving: boolean;
}

export default function PurchaseUpdateHeader({
  onSave,
  saving,
}: PurchaseUpdateHeaderProps) {
  const navigate = useNavigate();

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <div className="flex items-center gap-2">
          <Button variant="ghost" onClick={() => navigate("/compras")}>
            <ArrowLeft className="h-4 w-4" />
          </Button>
          <CardTitle>Actualizar compra</CardTitle>
        </div>
        <div className="flex items-center gap-2">
          <Button variant="secondary" onClick={() => navigate("/compras")}>Cancelar</Button>
          <Button onClick={onSave} disabled={saving}>
            {saving ? "Guardando..." : "Actualizar"}
          </Button>
        </div>
      </CardHeader>
    </Card>
  );
}
