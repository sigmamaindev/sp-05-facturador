import { Card, CardContent } from "@/components/ui/card";
import { useSearchParams } from "react-router-dom";

import { useAuth } from "@/contexts/AuthContext";

import EmissionPointCreateHeader from "./EmissionPointCreateHeader";
import EmissionPointCreateForm from "./EmissionPointCreateForm";

export default function EmissionPointCreateView() {
  const [searchParams] = useSearchParams();
  const establishmentId = searchParams.get("establecimiento");

  const { token } = useAuth();

  return (
    <Card>
      <CardContent>
        <EmissionPointCreateHeader />
        <EmissionPointCreateForm
          establishmentId={establishmentId!}
          token={token}
        />
      </CardContent>
    </Card>
  );
}
