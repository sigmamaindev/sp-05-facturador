import { Card, CardContent } from "@/components/ui/card";

import { useAuth } from "@/contexts/AuthContext";

import SupplierCreateHeader from "./SupplierCreateHeader";
import SupplierCreateForm from "./SupplierCreateForm";

export default function SupplierCreateView() {
  const { token } = useAuth();

  return (
    <Card>
      <CardContent>
        <SupplierCreateHeader />
        <SupplierCreateForm token={token} />
      </CardContent>
    </Card>
  );
}
