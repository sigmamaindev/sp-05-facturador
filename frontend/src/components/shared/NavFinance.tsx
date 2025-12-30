import { ChevronRight, type LucideIcon } from "lucide-react";

import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@/components/ui/collapsible";

import {
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarMenuSub,
  SidebarMenuSubButton,
  SidebarMenuSubItem,
} from "@/components/ui/sidebar";
import { Link, useLocation } from "react-router-dom";

function isPathActive(pathname: string, to: string) {
  if (to === "/") return pathname === "/";
  return pathname === to || pathname.startsWith(`${to}/`);
}

export default function NavFinance({
  items,
  open,
  onOpenChange,
}: {
  items: {
    label: string;
    icon: LucideIcon;
    items: {
      to: string;
      label: string;
      icon: LucideIcon;
    }[];
  };
  open?: boolean;
  onOpenChange?: (open: boolean) => void;
}) {
  const { pathname } = useLocation();
  const isGroupActive = items.items.some((subItem) =>
    isPathActive(pathname, subItem.to),
  );

  return (
    <SidebarMenu>
      <Collapsible
        key={items.label}
        asChild
        className="group/collapsible"
        open={open}
        onOpenChange={onOpenChange}
      >
        <SidebarMenuItem>
          <CollapsibleTrigger asChild>
            <SidebarMenuButton tooltip={items.label} isActive={isGroupActive}>
              {<items.icon />}
              <span>{items.label}</span>
              <ChevronRight className="ml-auto transition-transform duration-200 group-data-[state=open]/collapsible:rotate-90" />
            </SidebarMenuButton>
          </CollapsibleTrigger>
          <CollapsibleContent>
            <SidebarMenuSub>
              {items.items.map((subItem) => (
                <SidebarMenuSubItem key={subItem.label}>
                  <SidebarMenuSubButton
                    asChild
                    isActive={isPathActive(pathname, subItem.to)}
                  >
                    <Link to={subItem.to}>
                      <span>{subItem.label}</span>
                    </Link>
                  </SidebarMenuSubButton>
                </SidebarMenuSubItem>
              ))}
            </SidebarMenuSub>
          </CollapsibleContent>
        </SidebarMenuItem>
      </Collapsible>
    </SidebarMenu>
  );
}
