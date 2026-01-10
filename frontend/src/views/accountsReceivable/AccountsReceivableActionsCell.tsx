import { Link } from "react-router-dom";
import { Eye, Pencil } from "lucide-react";

import type { AccountsReceivable } from "@/types/accountsReceivable.types";

import { useAuth } from "@/contexts/AuthContext";

import { buttonVariants } from "@/components/ui/button";
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from "@/components/ui/tooltip";

import { cn } from "@/lib/utils";

export default function AccountsReceivableActionsCell({
  accountsReceivable,
}: {
  accountsReceivable: AccountsReceivable;
}) {
  const { user } = useAuth();

  const hasPermission =
    user?.roles?.includes("SuperAdmin") || user?.roles?.includes("Admin");

  return (
    <div className="flex flex-col sm:flex-row items-center justify-center gap-2">
      <Tooltip>
        <TooltipTrigger asChild>
          <Link
            to={`/cuentas-por-cobrar/${accountsReceivable.id}`}
            aria-label="Ver detalles"
            className={cn(
              buttonVariants({ size: "icon-sm" }),
              "bg-gray-200 text-white hover:bg-gray-300 dark:bg-gray-700 dark:hover:bg-gray-600"
            )}
          >
            <Eye className="size-4 text-white" />
          </Link>
        </TooltipTrigger>
        <TooltipContent side="top" sideOffset={6}>
          Detalles
        </TooltipContent>
      </Tooltip>

      {hasPermission ? (
        <Tooltip>
          <TooltipTrigger asChild>
            <Link
              to={`/cuentas-por-cobrar/actualizar/${accountsReceivable.id}`}
              aria-label="Editar cuenta por cobrar"
              className={cn(
                buttonVariants({ size: "icon-sm" }),
                "bg-blue-600 text-white hover:bg-blue-700 dark:bg-blue-500 dark:hover:bg-blue-600"
              )}
            >
              <Pencil className="size-4 text-white" />
            </Link>
          </TooltipTrigger>
          <TooltipContent side="top" sideOffset={6}>
            Editar
          </TooltipContent>
        </Tooltip>
      ) : null}
    </div>
  );
}

