import { useState } from "react";
import { toast } from "sonner";
import { lookupPersonByDocument } from "@/api/personLookup";
import type { PersonLookupResult } from "@/types/personLookup.types";

export function useDocumentLookup(token: string | null) {
  const [isSearching, setIsSearching] = useState(false);

  const lookup = async (
    document: string
  ): Promise<PersonLookupResult | null> => {
    if (!token || !document || document.length < 10) {
      toast.error("Ingrese un documento válido (mínimo 10 dígitos)");
      return null;
    }

    try {
      setIsSearching(true);
      const response = await lookupPersonByDocument(document, token);

      if (response.success && response.data) {
        const sourceLabel =
          response.data.source === "customer"
            ? "clientes"
            : response.data.source === "supplier"
              ? "proveedores"
              : "SRI";
        toast.success(`Persona encontrada en ${sourceLabel}`);
        return response.data;
      }

      toast.info("No se encontró información para este documento");
      return null;
    } catch (err: any) {
      toast.error(err.message);
      return null;
    } finally {
      setIsSearching(false);
    }
  };

  return { lookup, isSearching };
}
