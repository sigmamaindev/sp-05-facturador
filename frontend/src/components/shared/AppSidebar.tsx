import { Link } from "react-router-dom";

import {
  BriefcaseBusiness,
  Edit,
  HandHelping,
  Home,
  List,
  Power,
  User,
  Users,
  ShoppingCart,
  Building2,
  MapPin,
  Warehouse,
  Wallet,
  HandCoins,
  CreditCard,
  DollarSign,
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
import NavPeople from "./NavPeople";
import NavFinance from "./NavFinance";

const data = {
  dashboard: { to: "/", label: "Dashboard", icon: Home },
  organization: {
    label: "Organización",
    icon: Building2,
    items: [
      { to: "/empresas", label: "Empresas", icon: BriefcaseBusiness },
      { to: "/establecimientos", label: "Establecimientos", icon: MapPin },
      { to: "/puntos-emision", label: "Puntos de Emisión", icon: List },
      { to: "/bodegas", label: "Bodegas", icon: Warehouse },
    ],
  },
  people: {
    label: "Personas",
    icon: Users,
    items: [
      { to: "/usuarios", label: "Usuarios", icon: User },
      { to: "/clientes", label: "Clientes", icon: HandHelping },
      { to: "/proveedores", label: "Proveedores", icon: HandHelping },
    ],
  },
  product: { to: "/productos", label: "Productos", icon: List },
  finance: {
    label: "Finanzas",
    icon: Wallet,
    items: [
      { to: "/facturas", label: "Facturas", icon: DollarSign },
      {
        to: "/cuentas-por-cobrar",
        label: "Cuentas por cobrar",
        icon: HandCoins,
      },
      { to: "/compras", label: "Compras", icon: ShoppingCart },
      {
        to: "/cuentas-por-pagar",
        label: "Cuentas por pagar",
        icon: CreditCard,
      },
    ],
  },
};

export default function AppSidebar() {
  const { setOpenMobile } = useSidebar();

  const { logout } = useAuth();

  return (
    <Sidebar collapsible="icon">
      <SidebarHeader className="py-5">
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton asChild onClick={() => setOpenMobile(false)}>
              <div className="flex items-center gap-2">
                <Edit width={20} height={20}>
                  <span>Facturador</span>
                </Edit>
              </div>
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
                <SidebarMenuButton asChild>
                  <Link to={data.dashboard.to}>
                    <data.dashboard.icon />
                    <span>{data.dashboard.label}</span>
                  </Link>
                </SidebarMenuButton>
              </SidebarMenuItem>
            </SidebarMenu>
            <NavOrganization items={data.organization} />
            <NavPeople items={data.people} />
            <SidebarMenu>
              <SidebarMenuItem key={data.product.to}>
                <SidebarMenuButton asChild>
                  <Link to={data.product.to}>
                    <data.product.icon />
                    <span>{data.product.label}</span>
                  </Link>
                </SidebarMenuButton>
              </SidebarMenuItem>
            </SidebarMenu>
            <NavFinance items={data.finance} />
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
