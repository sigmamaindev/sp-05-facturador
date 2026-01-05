import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "../ui/dropdown-menu";
import { Button } from "../ui/button";
import { MoreHorizontal } from "lucide-react";
import { Link } from "react-router-dom";

interface RowActionsProps {
  actions: Array<
    | { label: string; onClick: () => void; to?: never }
    | { label: string; to: string; onClick?: never }
  >;
}

export default function RowActions({ actions }: RowActionsProps) {
  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button variant="ghost" size="icon">
          <MoreHorizontal className="h-4 w-4" />
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end">
        {actions.map((action, index) => (
          <DropdownMenuItem
            key={index}
            onClick={"onClick" in action ? action.onClick : undefined}
            asChild={"to" in action}
          >
            {"to" in action ? (
              <Link to={action.to}>{action.label}</Link>
            ) : (
              action.label
            )}
          </DropdownMenuItem>
        ))}
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
