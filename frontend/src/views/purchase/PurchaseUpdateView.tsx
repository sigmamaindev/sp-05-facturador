import { useState } from "react";
import { useParams } from "react-router-dom";

import PurchaseUpdateHeader from "./PurchaseUpdateHeader";
import PurchaseUpdateForm from "./PurchaseUpdateForm";

export default function PurchaseUpdateView() {
  const { id } = useParams();
  const [saving, setSaving] = useState(false);

  const handleSave = () => {
    setSaving(true);
    setTimeout(() => setSaving(false), 500);
  };

  return (
    <div className="space-y-4">
      <PurchaseUpdateHeader
        onSave={handleSave}
        saving={saving}
      />
      <PurchaseUpdateForm />
      <p className="text-sm text-muted-foreground">
        Edición preparada para la compra #{id}. Se activará cuando el backend lo soporte.
      </p>
    </div>
  );
}
