import { useNavigate } from "react-router-dom";
import { ArrowLeftIcon } from "lucide-react";

import { Button } from "@/components/ui/button";

export default function CustomerDetailHeader() {
  const navigate = useNavigate();

  return (
    <div className="grid grid-cols-1 md:flex md:flex-row md:justify-between md:items-center items-center gap-4 pb-4">
      <div className="grid grid-cols-2 items-center gap-4 md:inline-flex w-auto">
        <h1 className="text-lg font-semibold">CLIENTE</h1>
      </div>
      <div className="inline-flex items-center gap-4 w-auto">
        <Button
          variant="outline"
          onClick={() => navigate("/clientes")}
          className="flex items-center gap-2"
        >
          <ArrowLeftIcon size={16} />
          Volver a la lista
        </Button>
      </div>
    </div>
  );
}

