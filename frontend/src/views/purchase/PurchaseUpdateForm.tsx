import { AlertCircle } from "lucide-react";

import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";

export default function PurchaseUpdateForm() {
  return (
    <Alert variant="secondary">
      <AlertCircle className="h-4 w-4" />
      <AlertTitle>Funcionalidad futura</AlertTitle>
      <AlertDescription>
        La API actual no expone un endpoint para actualizar compras. Esta vista replica la estructura de facturas y quedará lista para conectarse en cuanto el backend lo permita.
      </AlertDescription>
    </Alert>
  );
}
