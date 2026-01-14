import { useEffect, useState } from "react";

import { useAuth } from "@/contexts/AuthContext";

import { getPurchases } from "@/api/purchase";
import { downloadAtsPurchasesXml } from "@/api/ats";

import type { Purchase } from "@/types/purchase.type";

import { Card, CardContent } from "@/components/ui/card";

import AlertMessage from "@/components/shared/AlertMessage";
import DataTable from "@/components/shared/DataTable";

import PurchaseListHeader from "./PurchaseListHeader";
import { columns } from "./PurchaseListColumns";
import PurchaseAtsModal from "./PurchaseAtsModal";

export default function PurchaseListView() {
  const { token } = useAuth();

  const [data, setData] = useState<Purchase[]>([]);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(1);
  const [keyword, setKeyword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [openAtsModal, setOpenAtsModal] = useState(false);

  const now = new Date();
  const [atsYear, setAtsYear] = useState(now.getFullYear());
  const [atsMonth, setAtsMonth] = useState(now.getMonth() + 1);

  const fetchData = async () => {
    if (!token) return;

    setLoading(true);

    try {
      const purchases = await getPurchases(keyword, page, pageSize, token);

      if (!purchases) return;

      setData(purchases.data);
      setPage(purchases.pagination.page);
      setPageSize(purchases.pagination.limit);
      setTotalPages(purchases.pagination.totalPages);
    } catch (err: any) {
      setError(
        err.message ??
          "Aún no hay un endpoint público para listar compras, pero la vista ya está disponible"
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, [keyword, page, pageSize, token]);

  const handleDownloadAtsXml = async () => {
    if (!token) return;

    try {
      const file = await downloadAtsPurchasesXml(atsYear, atsMonth, token);
      const url = window.URL.createObjectURL(file);
      const link = document.createElement("a");

      link.href = url;
      link.download = `ATS_Compras_${atsYear}_${String(atsMonth).padStart(2, "0")}.xml`;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);

      window.URL.revokeObjectURL(url);
    } catch (err: any) {
      alert(err.message ?? "No se pudo descargar el XML del ATS");
    }
  };

  return (
    <Card>
      <CardContent>
        <PurchaseListHeader
          keyword={keyword}
          setKeyword={setKeyword}
          setPage={setPage}
          atsYear={atsYear}
          atsMonth={atsMonth}
          setAtsYear={setAtsYear}
          setAtsMonth={setAtsMonth}
          onShowAtsData={() => setOpenAtsModal(true)}
          onDownloadAtsXml={handleDownloadAtsXml}
        />
        {error ? (
          <AlertMessage message={error} variant="destructive" />
        ) : (
          <DataTable
            columns={columns}
            data={data}
            page={page}
            pageSize={pageSize}
            totalPages={totalPages}
            onPageChange={setPage}
            onPageSizeChange={setPageSize}
            loading={loading}
          />
        )}

        <PurchaseAtsModal
          open={openAtsModal}
          onClose={() => setOpenAtsModal(false)}
          year={atsYear}
          month={atsMonth}
          setYear={setAtsYear}
          setMonth={setAtsMonth}
        />
      </CardContent>
    </Card>
  );
}
