import { BrowserRouter, Routes, Route } from "react-router-dom";

import AuthProvider from "@/contexts/AuthContext";

import AppLayout from "@/layouts/AppLayout";
import AuthLayout from "@/layouts/AuthLayout";
import ProtectedRoute from "./protectedRoute";
import PublicRoute from "./publicRoute";
import DashboardView from "@/views/dashboard/DashboardView";
import LoginView from "@/views/auth/LoginView";
import UserListView from "@/views/user/UserListView";
import UserCreateView from "@/views/user/UserCreateView";
import InvoiceListView from "@/views/invoice/InvoiceListView";
import InvoiceCreateView from "@/views/invoice/InvoiceCreateView";
import CustomerListView from "@/views/customer/CustomerListView";
import UserUpdateView from "@/views/user/UserUpdateView";

export default function Router() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route element={<ProtectedRoute />}>
            <Route element={<AppLayout />}>
              <Route path="/" element={<DashboardView />} />
              <Route path="/usuarios" element={<UserListView />} />
              <Route path="/usuarios/crear" element={<UserCreateView />} />
              <Route path="/usuarios/actualizar/:id" element={<UserUpdateView />} />
              <Route path="/clientes" element={<CustomerListView />} />
              <Route path="/facturas" element={<InvoiceListView />} />
              <Route path="/facturas/crear" element={<InvoiceCreateView />} />
            </Route>
          </Route>
          <Route element={<PublicRoute />}>
            <Route element={<AuthLayout />}>
              <Route path="/auth/login" element={<LoginView />} />
            </Route>
          </Route>
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}
