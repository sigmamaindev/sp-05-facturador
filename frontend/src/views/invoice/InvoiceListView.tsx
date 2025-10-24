import { Card, CardContent } from "@/components/ui/card";

import InvoiceListHeader from "./InvoiceListHeader";

export default function InvoiceListView() {
  return (
    <Card>
      <CardContent>
        <InvoiceListHeader />
      </CardContent>
    </Card>
  );
}
