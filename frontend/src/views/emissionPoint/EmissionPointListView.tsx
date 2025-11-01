import { useEffect, useState } from "react";

import { useAuth } from "@/contexts/AuthContext";

import { getEstablishments } from "@/api/establishment";
import { getEmissionPoints } from "@/api/emissionPoint";

import type { EmissionPoint } from "@/types/emissionPoint.types";
import type { Establishment } from "@/types/establishment.types";

import { Card, CardContent } from "@/components/ui/card";

import EmissionPointListHeader from "./EmissionPointListHeader";
import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";
import DataTable from "@/components/shared/DataTable";
import { columns } from "./EmissionPointListColumns";

export default function EmissionPointListView() {
  const { token } = useAuth();

  const [establishments, setEstablishments] = useState<Establishment[]>([]);
  const [selectedEstablishment, setSelectedEstablishment] = useState<
    number | null
  >(null);
  const [emissionPoints, setEmissionPoints] = useState<EmissionPoint[]>([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [keyword, setKeyword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchEstablishments = async () => {
    try {
      setLoading(true);

      const response = await getEstablishments("", 1, 100, token!);

      setEstablishments(response.data || []);

      if (response.data && response.data.length > 0) {
        setSelectedEstablishment(response.data[0].id);
      }
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  const fetchEmissionPoints = async (establishmentId: number) => {
    try {
      setLoading(true);

      const res = await getEmissionPoints(
        establishmentId,
        keyword,
        page,
        pageSize,
        token!
      );
      setEmissionPoints(res.data || []);

      setTotalPages(res.pagination.totalPages || 1);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (token) fetchEstablishments();
  }, [token]);

  useEffect(() => {
    if (selectedEstablishment) {
      fetchEmissionPoints(selectedEstablishment);
    }
  }, [selectedEstablishment, keyword, page]);

  return (
    <Card>
      <CardContent>
        <EmissionPointListHeader
          keyword={keyword}
          setKeyword={setKeyword}
          setPage={setPage}
          establishments={establishments}
          selectedEstablishment={selectedEstablishment}
          setSelectedEstablishment={setSelectedEstablishment}
        />
        {loading ? (
          <Loading />
        ) : error ? (
          <AlertMessage message={error} variant="destructive" />
        ) : !establishments ? (
          <AlertMessage
            message="Los datos del catálogo no se han cargado completamente"
            variant="destructive"
          />
        ) : (
          <>
            <DataTable
              columns={columns(selectedEstablishment)}
              data={emissionPoints}
              page={page}
              pageSize={pageSize}
              totalPages={totalPages}
              onPageChange={setPage}
              onPageSizeChange={setPageSize}
              loading={loading}
            />
          </>
        )}
      </CardContent>
    </Card>
  );
}
