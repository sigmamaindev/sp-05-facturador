import { Link } from "react-router-dom";

import {
  BriefcaseBusiness,
  Edit,
  HandHelping,
  Home,
  List,
  Power,
  User,
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

const menuItems = [
  { to: "/", label: "Dashboard", icon: Home },
  { to: "/empresas", label: "Empresas", icon: BriefcaseBusiness },
  { to: "/usuarios", label: "Usuarios", icon: User },
  { to: "/clientes", label: "Clientes", icon: HandHelping },
  { to: "/productos", label: "Productos", icon: List },
  { to: "/facturas", label: "Facturas", icon: DollarSign },
];

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
              {menuItems.map(({ to, label, icon: Icon }) => (
                <SidebarMenuItem key={to}>
                  <SidebarMenuButton asChild>
                    <Link to={to}>
                      <Icon />
                      <span>{label}</span>
                    </Link>
                  </SidebarMenuButton>
                </SidebarMenuItem>
              ))}
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
