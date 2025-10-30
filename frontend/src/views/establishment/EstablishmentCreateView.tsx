import { Card, CardContent } from "@/components/ui/card";

import { useAuth } from "@/contexts/AuthContext";

import EstablishmentCreateHeader from "./EstablishmentCreateHeader";
import EstablishmentCreateForm from "./EstablishmentCreateForm";

export default function EstablishmentCreateView() {
    const { token } = useAuth();

  return (
    <Card>
      <CardContent>
        <EstablishmentCreateHeader />
        <EstablishmentCreateForm token={token}/>
      </CardContent>
    </Card>
  );
}
