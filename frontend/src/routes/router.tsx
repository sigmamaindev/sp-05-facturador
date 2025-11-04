import { BrowserRouter, Routes, Route } from "react-router-dom";

import AuthProvider from "@/contexts/AuthContext";

import AppLayout from "@/layouts/AppLayout";
import AuthLayout from "@/layouts/AuthLayout";
import ProtectedRoute from "./protectedRoute";
import PublicRoute from "./publicRoute";
import DashboardView from "@/views/dashboard/DashboardView";
import LoginView from "@/views/auth/LoginView";
import BusinessListView from "@/views/business/BusinessListView";
import BusinessDetailView from "@/views/business/BusinessDetailView";
import EstablishmentListView from "@/views/establishment/EstablishmentListView";
import EstablishmentDetailView from "@/views/establishment/EstablishmentDetailView";
import EstablishmentCreateView from "@/views/establishment/EstablishmentCreateView";
import EstablishmentUpdateView from "@/views/establishment/EstablishmentUpdateView";
import EmissionPointListView from "@/views/emissionPoint/EmissionPointListView";
import EmissionPointDetailView from "@/views/emissionPoint/EmissionPointDetailView";
import EmissionPointCreateView from "@/views/emissionPoint/EmissionPointCreateView";
import EmissionPointUpdateView from "@/views/emissionPoint/EmissionPointUpdateView";
import UserListView from "@/views/user/UserListView";
import UserCreateView from "@/views/user/UserCreateView";
import UserUpdateView from "@/views/user/UserUpdateView";
import CustomerListView from "@/views/customer/CustomerListView";
import ProductListView from "@/views/product/ProductListView";
import InvoiceListView from "@/views/invoice/InvoiceListView";
import InvoiceCreateView from "@/views/invoice/InvoiceCreateView";
import WarehouseListView from "@/views/warehouse/WarehouseListView";

export default function Router() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route element={<ProtectedRoute />}>
            <Route element={<AppLayout />}>
              <Route path="/" element={<DashboardView />} />
              <Route path="/empresas" element={<BusinessListView />} />
              <Route path="/empresas/:id" element={<BusinessDetailView />} />
              <Route
                path="/establecimientos"
                element={<EstablishmentListView />}
              />
              <Route
                path="/establecimientos/:id"
                element={<EstablishmentDetailView />}
              />
              <Route
                path="/establecimientos/crear"
                element={<EstablishmentCreateView />}
              />
              <Route
                path="/establecimientos/actualizar/:id"
                element={<EstablishmentUpdateView />}
              />
              <Route
                path="/puntos-emision"
                element={<EmissionPointListView />}
              />
              <Route
                path="/puntos-emision/:id"
                element={<EmissionPointDetailView />}
              />
              <Route
                path="/puntos-emision/crear"
                element={<EmissionPointCreateView />}
              />
              <Route
                path="/puntos-emision/actualizar/:id"
                element={<EmissionPointUpdateView />}
              />
              <Route path="/bodegas" element={<WarehouseListView />} />
              <Route path="/usuarios" element={<UserListView />} />
              <Route path="/usuarios/crear" element={<UserCreateView />} />
              <Route
                path="/usuarios/actualizar/:id"
                element={<UserUpdateView />}
              />
              <Route path="/clientes" element={<CustomerListView />} />
              <Route path="/productos" element={<ProductListView />} />
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
