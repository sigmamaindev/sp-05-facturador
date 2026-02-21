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
  FileText,
  BarChart2,
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
  inventory: {
    label: "Inventario",
    icon: ClipboardList,
    items: [
      { to: "/productos", label: "Productos", icon: List },
      { to: "/kardex", label: "Kardex", icon: ClipboardList },
    ],
  },
  reports: {
    label: "Reporte",
    icon: BarChart2,
    items: [
      { to: "/reportes/ventas", label: "Venta", icon: BarChart2 },
      { to: "/reportes/compras", label: "Compra", icon: BarChart2 },
    ],
  },
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
    "organization" | "invoices" | "purchases" | "inventory" | "reports" | null
  >(null);

  const { logout, user } = useAuth();

  const showAts =
    user?.roles?.includes("SuperAdmin") || user?.roles?.includes("Admin");

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
            <NavFinance
              items={data.inventory}
              open={openNav === "inventory"}
              onOpenChange={(open) => setOpenNav(open ? "inventory" : null)}
            />
            <NavFinance
              items={data.reports}
              open={openNav === "reports"}
              onOpenChange={(open) => setOpenNav(open ? "reports" : null)}
            />
            <SidebarMenu>
              {showAts && (
                <SidebarMenuItem key="/ats">
                  <SidebarMenuButton
                    asChild
                    isActive={isPathActive(pathname, "/ats")}
                  >
                    <Link to="/ats">
                      <FileText />
                      <span>ATS</span>
                    </Link>
                  </SidebarMenuButton>
                </SidebarMenuItem>
              )}
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
