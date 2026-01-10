import { Link, useLocation } from "react-router-dom";
import { useState } from "react";

import {
  BriefcaseBusiness,
  HandHelping,
  Home,
  List,
  Power,
  User,
  ShoppingCart,
  ShoppingBag,
  Building2,
  DollarSign,
  MapPin,
  BadgeCheck,
  Edit,
  ClipboardList,
} from "lucide-react";

import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupContent,
  SidebarGroupLabel,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  useSidebar,
} from "../ui/sidebar";

import { useAuth } from "@/contexts/AuthContext";
import NavOrganization from "./NavOrganization";
import NavFinance from "./NavFinance";

const data = {
  dashboard: { to: "/", label: "Principal", icon: Home },
  organization: {
    label: "Organización",
    icon: Building2,
    items: [
      { to: "/empresas", label: "Empresa", icon: BriefcaseBusiness },
      { to: "/certificados", label: "Cetificados", icon: BadgeCheck },
      { to: "/usuarios", label: "Usuarios", icon: User },
      { to: "/establecimientos", label: "Establecimientos", icon: MapPin },
      { to: "/puntos-emision", label: "Puntos de emisión", icon: List },
    ],
  },
  product: { to: "/productos", label: "Productos", icon: List },
  kardex: { to: "/kardex", label: "Kardex", icon: ClipboardList },
  invoices: {
    label: "Facturas",
    icon: DollarSign,
    items: [
      { to: "/clientes", label: "Clientes", icon: HandHelping },
      { to: "/facturas", label: "Ventas", icon: ShoppingCart },
      {
        to: "/cuentas-por-cobrar",
        label: "Cuentas por cobrar",
        icon: DollarSign,
      },
    ],
  },
  purchases: {
    label: "Compras",
    icon: ShoppingBag,
    items: [
      { to: "/proveedores", label: "Proveedores", icon: HandHelping },
      { to: "/compras", label: "Compras", icon: ShoppingBag },
      {
        to: "/cuentas-por-pagar",
        label: "Cuentas por pagar",
        icon: ShoppingBag,
      },
    ],
  },
};

function isPathActive(pathname: string, to: string) {
  if (to === "/") return pathname === "/";
  return pathname === to || pathname.startsWith(`${to}/`);
}

export default function AppSidebar() {
  const { setOpenMobile } = useSidebar();
  const { pathname } = useLocation();
  const [openNav, setOpenNav] = useState<
    "organization" | "invoices" | "purchases" | null
  >(null);

  const { logout } = useAuth();

  return (
    <Sidebar collapsible="icon">
      <SidebarHeader className="py-5">
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton asChild onClick={() => setOpenMobile(false)}>
              <Link to={data.dashboard.to} className="flex items-center gap-2">
                <Edit width={20} height={20} />
                <span>Facturador</span>
              </Link>
            </SidebarMenuButton>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarHeader>

      <SidebarContent>
        <SidebarGroup>
          <SidebarGroupLabel>Inicio</SidebarGroupLabel>
          <SidebarGroupContent>
            <SidebarMenu>
              <SidebarMenuItem key={data.dashboard.to}>
                <SidebarMenuButton
                  asChild
                  isActive={isPathActive(pathname, data.dashboard.to)}
                >
                  <Link to={data.dashboard.to}>
                    <data.dashboard.icon />
                    <span>{data.dashboard.label}</span>
                  </Link>
                </SidebarMenuButton>
              </SidebarMenuItem>
            </SidebarMenu>
            <NavOrganization
              items={data.organization}
              open={openNav === "organization"}
              onOpenChange={(open) => setOpenNav(open ? "organization" : null)}
            />
            <NavFinance
              items={data.invoices}
              open={openNav === "invoices"}
              onOpenChange={(open) => setOpenNav(open ? "invoices" : null)}
            />
            <NavFinance
              items={data.purchases}
              open={openNav === "purchases"}
              onOpenChange={(open) => setOpenNav(open ? "purchases" : null)}
            />
            <SidebarMenu>
              <SidebarMenuItem key={data.product.to}>
                <SidebarMenuButton
                  asChild
                  isActive={isPathActive(pathname, data.product.to)}
                >
                  <Link to={data.product.to}>
                    <data.product.icon />
                    <span>{data.product.label}</span>
                  </Link>
                </SidebarMenuButton>
              </SidebarMenuItem>
              <SidebarMenuItem key={data.kardex.to}>
                <SidebarMenuButton
                  asChild
                  isActive={isPathActive(pathname, data.kardex.to)}
                >
                  <Link to={data.kardex.to}>
                    <data.kardex.icon />
                    <span>{data.kardex.label}</span>
                  </Link>
                </SidebarMenuButton>
              </SidebarMenuItem>
            </SidebarMenu>
          </SidebarGroupContent>
        </SidebarGroup>
      </SidebarContent>

      <SidebarFooter>
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton onClick={() => logout()}>
              <Power /> Salir
            </SidebarMenuButton>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarFooter>
    </Sidebar>
  );
}
