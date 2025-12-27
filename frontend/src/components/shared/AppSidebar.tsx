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
  DollarSign,
  ShoppingCart,
  Building2,
  MapPin,
  Warehouse,
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
  purchase: { to: "/compras", label: "Compras", icon: ShoppingCart },
  invoice: { to: "/facturas", label: "Facturas", icon: DollarSign },
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
            <SidebarMenu>
              <SidebarMenuItem key={data.invoice.to}>
                <SidebarMenuButton asChild>
                  <Link to={data.invoice.to}>
                    <data.invoice.icon />
                    <span>{data.invoice.label}</span>
                  </Link>
                </SidebarMenuButton>
              </SidebarMenuItem>
            </SidebarMenu>
            <SidebarMenu>
              <SidebarMenuItem key={data.purchase.to}>
                <SidebarMenuButton asChild>
                  <Link to={data.purchase.to}>
                    <data.purchase.icon />
                    <span>{data.purchase.label}</span>
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
