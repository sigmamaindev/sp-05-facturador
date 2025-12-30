import { useEffect, useState } from "react";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";

interface InvoiceAdditionalInfoModalProps {
  open: boolean;
  onClose: () => void;
  initialDescription: string;
  initialAdditionalInformation: string;
  onSave: (values: {
    description: string;
    additionalInformation: string;
  }) => void;
}

export default function InvoiceAdditionalInfoModal({
  open,
  onClose,
  initialDescription,
  initialAdditionalInformation,
  onSave,
}: InvoiceAdditionalInfoModalProps) {
  const [description, setDescription] = useState("");
  const [additionalInformation, setAdditionalInformation] = useState("");

  useEffect(() => {
    if (!open) return;
    setDescription(initialDescription ?? "");
    setAdditionalInformation(initialAdditionalInformation ?? "");
  }, [open, initialAdditionalInformation, initialDescription]);

  const handleSave = () => {
    onSave({
      description,
      additionalInformation,
    });
  };

  return (
    <Dialog
      open={open}
      onOpenChange={(nextOpen) => {
        if (!nextOpen) onClose();
      }}
    >
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Información adicional</DialogTitle>
          <DialogDescription>
            Opcional. Se imprimirá/guardará junto a la factura.
          </DialogDescription>
        </DialogHeader>

        <div className="grid gap-2">
          <Label htmlFor="invoiceDescription">Descripción</Label>
          <Textarea
            id="invoiceDescription"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            maxLength={1000}
            placeholder="Descripción (opcional)"
          />
        </div>

        <div className="grid gap-2">
          <Label htmlFor="invoiceAdditionalInformation">
            Información adicional
          </Label>
          <Textarea
            id="invoiceAdditionalInformation"
            value={additionalInformation}
            onChange={(e) => setAdditionalInformation(e.target.value)}
            maxLength={1000}
            placeholder="Información adicional (opcional)"
          />
        </div>

        <DialogFooter>
          <Button type="button" variant="outline" onClick={onClose}>
            Cancelar
          </Button>
          <Button type="button" onClick={handleSave}>
            Guardar
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
