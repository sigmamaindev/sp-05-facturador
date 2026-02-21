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
import BusinessUpdateView from "@/views/business/BusinessUpdateView";
import EstablishmentListView from "@/views/establishment/EstablishmentListView";
import EstablishmentDetailView from "@/views/establishment/EstablishmentDetailView";
import EstablishmentCreateView from "@/views/establishment/EstablishmentCreateView";
import EstablishmentUpdateView from "@/views/establishment/EstablishmentUpdateView";
import EmissionPointListView from "@/views/emissionPoint/EmissionPointListView";
import EmissionPointDetailView from "@/views/emissionPoint/EmissionPointDetailView";
import EmissionPointCreateView from "@/views/emissionPoint/EmissionPointCreateView";
import EmissionPointUpdateView from "@/views/emissionPoint/EmissionPointUpdateView";
import CertificateListView from "@/views/certificate/CertificateListView";
import UserListView from "@/views/user/UserListView";
import UserDetailView from "@/views/user/UserDetailView";
import UserCreateView from "@/views/user/UserCreateView";
import UserUpdateView from "@/views/user/UserUpdateView";
import CustomerListView from "@/views/customer/CustomerListView";
import CustomerCreateView from "@/views/customer/CustomerCreateView";
import CustomerDetailView from "@/views/customer/CustomerDetailView";
import CustomerUpdateView from "@/views/customer/CustomerUpdateView";
import SupplierListView from "@/views/supplier/SupplierListView";
import SupplierCreateView from "@/views/supplier/SupplierCreateView";
import SupplierDetailView from "@/views/supplier/SupplierDetailView";
import SupplierUpdateView from "@/views/supplier/SupplierUpdateView";
import ProductListView from "@/views/product/ProductListView";
import ProductCreateView from "@/views/product/ProductCreateView";
import ProductDetailView from "@/views/product/ProductDetailView";
import ProductUpdateView from "@/views/product/ProductUpdateView";
import KardexListView from "@/views/kardex/KardexListView";
import AtsView from "@/views/ats/AtsView";
import PurchaseListView from "@/views/purchase/PurchaseListView";
import PurchaseCreateView from "@/views/purchase/PurchaseCreateView";
import PurchaseUpdateView from "@/views/purchase/PurchaseUpdateView";
import PurchaseDetailView from "@/views/purchase/PurchaseDetailView";
import InvoiceListView from "@/views/invoice/InvoiceListView";
import InvoiceCreateView from "@/views/invoice/InvoiceCreateView";
import InvoiceUpdateView from "@/views/invoice/InvoiceUpdateView";
import WarehouseListView from "@/views/warehouse/WarehouseListView";
import InvoiceDetailView from "@/views/invoice/InvoiceDetailView";
import WarehouseCreateView from "@/views/warehouse/WarehouseCreateView";
import WarehouseDetailView from "@/views/warehouse/WarehouseDetailView";
import WarehouseUpdateView from "@/views/warehouse/WarehouseUpdateView";
import AccountsReceivableListView from "@/views/accountsReceivable/AccountsReceivableListView";
import AccountsReceivableDetailView from "@/views/accountsReceivable/AccountsReceivableDetailView";
import AccountsReceivableUpdateView from "@/views/accountsReceivable/AccountsReceivableUpdateView";
import AccountsReceivableCustomerPaymentsView from "@/views/accountsReceivable/AccountsReceivableCustomerPaymentsView";
import AccountsPayableListView from "@/views/accountsPayable/AccountsPayableListView";
import AccountsPayableDetailView from "@/views/accountsPayable/AccountsPayableDetailView";
import AccountsPayableUpdateView from "@/views/accountsPayable/AccountsPayableUpdateView";
import AccountsPayableSupplierPaymentsView from "@/views/accountsPayable/AccountsPayableSupplierPaymentsView";
import SalesReportListView from "@/views/report/SalesReportListView";
import SalesReportDetailView from "@/views/report/SalesReportDetailView";
import NotFoundRoute from "./notFoundRoute";

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
                path="/empresas/actualizar/:id"
                element={<BusinessUpdateView />}
              />
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
              <Route path="/certificados" element={<CertificateListView />} />
              <Route path="/bodegas" element={<WarehouseListView />} />
              <Route path="/bodegas/:id" element={<WarehouseDetailView />} />
              <Route path="/bodegas/crear" element={<WarehouseCreateView />} />
              <Route
                path="/bodegas/actualizar/:id"
                element={<WarehouseUpdateView />}
              />
              <Route path="/usuarios" element={<UserListView />} />
              <Route path="/usuarios/:id" element={<UserDetailView />} />
              <Route path="/usuarios/crear" element={<UserCreateView />} />
              <Route
                path="/usuarios/actualizar/:id"
                element={<UserUpdateView />}
              />
              <Route path="/clientes" element={<CustomerListView />} />
              <Route path="/clientes/crear" element={<CustomerCreateView />} />
              <Route path="/clientes/:id" element={<CustomerDetailView />} />
              <Route
                path="/clientes/actualizar/:id"
                element={<CustomerUpdateView />}
              />
              <Route path="/proveedores" element={<SupplierListView />} />
              <Route path="/proveedores/crear" element={<SupplierCreateView />} />
              <Route path="/proveedores/:id" element={<SupplierDetailView />} />
              <Route
                path="/proveedores/actualizar/:id"
                element={<SupplierUpdateView />}
              />
              <Route path="/productos" element={<ProductListView />} />
              <Route path="/productos/crear" element={<ProductCreateView />} />
              <Route path="/productos/:id" element={<ProductDetailView />} />
              <Route
                path="/productos/actualizar/:id"
                element={<ProductUpdateView />}
              />
              <Route path="/kardex" element={<KardexListView />} />
              <Route path="/ats" element={<AtsView />} />
              <Route path="/compras" element={<PurchaseListView />} />
              <Route path="/compras/:id" element={<PurchaseDetailView />} />
              <Route path="/compras/crear" element={<PurchaseCreateView />} />
              <Route
                path="/compras/actualizar/:id"
                element={<PurchaseUpdateView />}
              />
              <Route
                path="/cuentas-por-cobrar"
                element={<AccountsReceivableListView />}
              />
              <Route
                path="/cuentas-por-cobrar/cliente/:customerId/pagos"
                element={<AccountsReceivableCustomerPaymentsView />}
              />
              <Route
                path="/cuentas-por-cobrar/:id"
                element={<AccountsReceivableDetailView />}
              />
              <Route
                path="/cuentas-por-cobrar/actualizar/:id"
                element={<AccountsReceivableUpdateView />}
              />
              <Route
                path="/cuentas-por-pagar"
                element={<AccountsPayableListView />}
              />
              <Route
                path="/cuentas-por-pagar/proveedor/:supplierId/pagos"
                element={<AccountsPayableSupplierPaymentsView />}
              />
              <Route
                path="/cuentas-por-pagar/:id"
                element={<AccountsPayableDetailView />}
              />
              <Route
                path="/cuentas-por-pagar/actualizar/:id"
                element={<AccountsPayableUpdateView />}
              />
              <Route
                path="/reportes/ventas"
                element={<SalesReportListView />}
              />
              <Route
                path="/reportes/ventas/:id"
                element={<SalesReportDetailView />}
              />
              <Route path="/facturas" element={<InvoiceListView />} />
              <Route path="/facturas/:id" element={<InvoiceDetailView />} />
              <Route path="/facturas/crear" element={<InvoiceCreateView />} />
              <Route
                path="/facturas/actualizar/:id"
                element={<InvoiceUpdateView />}
              />
            </Route>
          </Route>
          <Route element={<PublicRoute />}>
            <Route element={<AuthLayout />}>
              <Route path="/auth/login" element={<LoginView />} />
            </Route>
          </Route>
          <Route path="*" element={<NotFoundRoute />} />
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}
