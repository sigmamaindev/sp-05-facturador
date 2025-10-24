import AppNavbar from "@/components/shared/AppNavbar";
import AppSidebar from "@/components/shared/AppSidebar";
import { SidebarProvider } from "@/components/ui/sidebar";
import { Outlet } from "react-router-dom";

export default function AppLayout() {
  return (
    <>
      <SidebarProvider defaultOpen={true}>
        <AppSidebar />
        <main className="w-full">
          <AppNavbar />
          <div className="flex flex-1 flex-col">
            <div className="flex flex-col gap-4 py-4 md:py-6">
              <div className="px-4 lg:px-6">
                <Outlet />
              </div>
            </div>
          </div>
        </main>
      </SidebarProvider>
    </>
  );
}
