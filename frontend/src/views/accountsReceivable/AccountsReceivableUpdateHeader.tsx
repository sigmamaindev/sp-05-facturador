import { ArrowLeft } from "lucide-react";
import { useNavigate } from "react-router-dom";

import { Button } from "@/components/ui/button";
import { Card, CardHeader, CardTitle } from "@/components/ui/card";

interface AccountsReceivableUpdateHeaderProps {
  formId: string;
  backTo: string;
  saving: boolean;
}

export default function AccountsReceivableUpdateHeader({
  formId,
  backTo,
  saving,
}: AccountsReceivableUpdateHeaderProps) {
  const navigate = useNavigate();

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between">
        <div className="flex items-center gap-2">
          <Button variant="ghost" onClick={() => navigate(backTo)}>
            <ArrowLeft className="h-4 w-4" />
          </Button>
          <CardTitle>Actualizar cuenta por cobrar</CardTitle>
        </div>
        <div className="flex items-center gap-2">
          <Button variant="secondary" onClick={() => navigate(backTo)}>
            Cancelar
          </Button>
          <Button type="submit" form={formId} disabled={saving}>
            {saving ? "Guardando..." : "Actualizar"}
          </Button>
        </div>
      </CardHeader>
    </Card>
  );
}
