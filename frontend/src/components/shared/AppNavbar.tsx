import { SidebarTrigger } from "../ui/sidebar";

import { UserCircle } from "lucide-react";

export default function AppNavbar() {
  return (
    <div className="p-4 flex items-center justify-between border-b-2">
      <SidebarTrigger />
      <UserCircle />
    </div>
  );
}
