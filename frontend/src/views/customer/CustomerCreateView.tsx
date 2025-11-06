import { Card, CardContent } from "@/components/ui/card";

import { useAuth } from "@/contexts/AuthContext";

import CustomerCreateHeader from "./CustomerCreateHeader";
import CustomerCreateForm from "./CustomerCreateForm";

export default function CustomerCreateView() {
  const { token } = useAuth();

  return (
    <Card>
      <CardContent>
        <CustomerCreateHeader />
        <CustomerCreateForm token={token} />
      </CardContent>
    </Card>
  );
}
