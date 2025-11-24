import { useNavigate } from "react-router-dom";
import type { ColumnDef } from "@tanstack/react-table";

import { downloadInvoicePdf } from "@/api/invoice";
import type { Invoice } from "@/types/invoice.type";

import { useAuth } from "@/contexts/AuthContext";

import RowActions from "@/components/shared/RowActions";

export const columns: ColumnDef<Invoice>[] = [
  {
    accessorKey: "id",
    header: "ID",
  },
  {
    accessorKey: "invoiceDate",
    header: "Fecha",
    cell: ({ row }) => {
      const date = new Date(row.original.invoiceDate);

      return (
        <span>
          {date.toLocaleString("es-EC", {
            year: "numeric",
            month: "2-digit",
            day: "2-digit",
            hour: "2-digit",
            minute: "2-digit",
            second: "2-digit",
            hour12: false,
          })}
        </span>
      );
    },
  },
  {
    accessorKey: "sequential",
    header: "Código",
  },
  {
    accessorKey: "customer.document",
    header: "Cedula Cliente",
  },
  {
    accessorKey: "customer.name",
    header: "Nombre Cliente",
  },
  {
    accessorKey: "totalInvoice",
    header: () => <div className="text-right">Total</div>,
    cell: ({ row }) => {
      const total = row.original.totalInvoice.toFixed(2);

      return <p className="text-right">{total}</p>;
    },
  },
  {
    id: "actions",
    header: () => <div className="text-right">Acciones</div>,
    cell: ({ row }) => {
      const navigate = useNavigate();

      const { user, token } = useAuth();

      const hasPermission =
        user?.roles?.includes("SuperAdmin") || user?.roles?.includes("Admin");

      const invoice = row.original;
      const isAuthorized = invoice.status?.toUpperCase() === "AUTORIZADO";
      const isReceived =
        invoice.status?.toUpperCase().includes("RECIBIDO") ?? false;

      const handlePdfDownload = async () => {
        if (!token) return;

        try {
          const file = await downloadInvoicePdf(invoice.id, token);
          const url = window.URL.createObjectURL(file);
          const link = document.createElement("a");

          link.href = url;
          link.download = `Factura_${invoice.sequential}.pdf`;
          document.body.appendChild(link);
          link.click();
          document.body.removeChild(link);

          window.URL.revokeObjectURL(url);
        } catch (error: any) {
          alert(error.message ?? "No se pudo generar el PDF de la factura");
        }
      };
      const actions = [
        {
          label: "Detalles",
          onClick: () => navigate(`/facturas/${invoice.id}`),
        },
      ];

      if (isAuthorized) {
        actions.push({
          label: "Descargar PDF",
          onClick: handlePdfDownload,
        });
      }

      if (hasPermission && !isReceived && !isAuthorized) {
        actions.push({
          label: "Editar",
          onClick: () => navigate(`/facturas/actualizar/${invoice.id}`),
        });
      }

      return (
        <div className="text-right">
          <RowActions actions={actions} />
        </div>
      );
    },
  },
];
