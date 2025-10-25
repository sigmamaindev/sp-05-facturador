import React from "react";
import { useNavigate } from "react-router-dom";
import { PlusIcon } from "lucide-react";

import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

import { useAuth } from "@/contexts/AuthContext";

interface BusinessListHeaderProps {
  keyword: string;
  setPage: React.Dispatch<React.SetStateAction<number>>;
  setKeyword: React.Dispatch<React.SetStateAction<string>>;
}

export default function BusinessListHeader({
  keyword,
  setPage,
  setKeyword,
}: BusinessListHeaderProps) {
  const { user } = useAuth();
  const navigate = useNavigate();

  const hasPermission =
    user?.roles?.includes("SuperAdmin");

  return (
    <div className="grid grid-cols-1 md:flex md:flex-row md:justify-between md:items-center items-center gap-4 pb-4">
      <div className="grid grid-cols-2 items-center gap-4 md:inline-flex w-auto">
        <h1 className="text-lg font-semibold">EMPRESAS</h1>
      </div>
      <div className="inline-flex items-center gap-4 w-auto">
        <Input
          placeholder="Buscar usuario..."
          value={keyword}
          onChange={(e) => {
            setPage(1);
            setKeyword(e.target.value);
          }}
          className="max-w-sm"
        />
        {hasPermission && (
          <Button onClick={() => navigate("/negocios/crear")}>
            <PlusIcon />
            Empresa
          </Button>
        )}
      </div>
    </div>
  );
}
