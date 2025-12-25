import { Card, CardContent } from "@/components/ui/card";

import { useAuth } from "@/contexts/AuthContext";

import WarehouseCreateHeader from "./WarehouseCreateHeader";
import WarehouseCreateForm from "./WarehouseCreateForm";

export default function WarehouseCreateView() {
  const { token } = useAuth();

  return (
    <Card>
      <CardContent>
        <WarehouseCreateHeader />
        <WarehouseCreateForm token={token} />
      </CardContent>
    </Card>
  );
}

