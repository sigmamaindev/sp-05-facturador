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
    <div className="grid grid-cols-1 md:flex md:flex-row md:justify-between md:items-center gap-4 pb-4">
      <div className="grid grid-cols-2 items-center gap-4 md:inline-flex w-auto">
        <h1 className="text-lg font-semibold">PUNTOS DE EMISIÓN</h1>
      </div>
      <div className="flex flex-col md:flex-row md:items-center gap-3 md:gap-4 w-full md:w-auto">
        <Select
          value={selectedEstablishment ? selectedEstablishment.toString() : ""}
          onValueChange={(val) => setSelectedEstablishment(Number(val))}
        >
          <SelectTrigger id="establishment" className="w-full md:w-[320px]">
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
          className="w-full md:w-auto md:max-w-sm"
        />

        {hasPermission && (
          <div className="flex justify-end w-full md:w-auto">
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
          </div>
        )}
      </div>
    </div>
  );
}
