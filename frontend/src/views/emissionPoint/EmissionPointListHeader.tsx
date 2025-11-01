import React from "react";
import { useNavigate } from "react-router-dom";
import { PlusIcon } from "lucide-react";

import { useAuth } from "@/contexts/AuthContext";

import type { Establishment } from "@/types/establishment.types";

import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

interface EmissionPointListHeaderProps {
  keyword: string;
  setPage: React.Dispatch<React.SetStateAction<number>>;
  setKeyword: React.Dispatch<React.SetStateAction<string>>;
  establishments: Establishment[];
  selectedEstablishment: number | null;
  setSelectedEstablishment: React.Dispatch<React.SetStateAction<number | null>>;
}

export default function EmissionPointListHeader({
  keyword,
  setPage,
  setKeyword,
  establishments,
  selectedEstablishment,
  setSelectedEstablishment,
}: EmissionPointListHeaderProps) {
  const navigate = useNavigate();

  const { user } = useAuth();

  const hasPermission =
    user?.roles?.includes("SuperAdmin") || user?.roles?.includes("Admin");

  return (
    <div className="grid grid-cols-1 md:flex md:flex-row md:justify-between md:items-center items-center gap-4 pb-4">
      <div className="grid grid-cols-2 items-center gap-4 md:inline-flex w-auto">
        <h1 className="text-lg font-semibold">PUNTOS DE EMISIÓN</h1>
      </div>
      <div className="inline-flex items-center gap-4 w-auto">
        <Select
          value={selectedEstablishment ? selectedEstablishment.toString() : ""}
          onValueChange={(val) => setSelectedEstablishment(Number(val))}
        >
          <SelectTrigger id="establishment">
            <SelectValue placeholder="Seleccione un establecimiento" />
          </SelectTrigger>
          <SelectContent>
            {establishments.map((e) => (
              <SelectItem key={e.id} value={e.id.toString()}>
                {`${e.code} ${e.name}`}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
        <Input
          placeholder="Buscar puntos de emisión..."
          value={keyword}
          onChange={(e) => {
            setPage(1);
            setKeyword(e.target.value);
          }}
          className="max-w-sm"
        />
        {hasPermission && (
          <Button
            onClick={() =>
              navigate(
                `/puntos-emision/crear?establecimiento=${selectedEstablishment}`
              )
            }
          >
            <PlusIcon />
            Punto de emisión
          </Button>
        )}
      </div>
    </div>
  );
}
