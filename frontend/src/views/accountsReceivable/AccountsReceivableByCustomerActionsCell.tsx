import { Link } from "react-router-dom";
import { CreditCard } from "lucide-react";

import type { AccountsReceivableCustomerSummary } from "@/types/accountsReceivable.types";

import { buttonVariants } from "@/components/ui/button";
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
} from "@/components/ui/tooltip";

import { cn } from "@/lib/utils";

export default function AccountsReceivableByCustomerActionsCell({
  summary,
}: {
  summary: AccountsReceivableCustomerSummary;
}) {
  return (
    <div className="flex items-center justify-center gap-2">
      <Tooltip>
        <TooltipTrigger asChild>
          <Link
            to={`/cuentas-por-cobrar/cliente/${summary.customer.id}/pagos`}
            aria-label="Registrar pagos"
            className={cn(
              buttonVariants({ size: "icon-sm" }),
              "bg-emerald-600 text-white hover:bg-emerald-700 dark:bg-emerald-500 dark:hover:bg-emerald-600"
            )}
          >
            <CreditCard className="size-4 text-white" />
          </Link>
        </TooltipTrigger>
        <TooltipContent side="top" sideOffset={6}>
          Pagos
        </TooltipContent>
      </Tooltip>
    </div>
  );
}

