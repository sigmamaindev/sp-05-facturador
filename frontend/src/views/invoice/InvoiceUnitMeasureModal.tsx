import { useEffect, useState } from "react";

import { useAuth } from "@/contexts/AuthContext";

import type { UnitMeasure } from "@/types/unitMeasure.types";

import { getUnitMeasures } from "@/api/unitMeasure";

import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { Card, CardContent } from "@/components/ui/card";

import AlertMessage from "@/components/shared/AlertMessage";
import DataTable from "@/components/shared/DataTable";

import InvoiceUnitMeasureModalHeader from "./InvoiceUnitMeasureModalHeader";
import { columns } from "./InvoiceUnitMeasureModalColumns";

interface InvoiceUnitMeasureModalProps {
  open: boolean;
  onClose: () => void;
  onSelect: (unitMeasure: UnitMeasure) => void;
}

export default function InvoiceUnitMeasureModal({
  open,
  onClose,
  onSelect,
}: InvoiceUnitMeasureModalProps) {
  const { token } = useAuth();

  const [data, setData] = useState<UnitMeasure[]>([]);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(5);
  const [totalPages, setTotalPages] = useState(1);
  const [keyword, setKeyword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchData = async () => {
    if (!token) return;

    setLoading(true);

    try {
      const unitMeasures = await getUnitMeasures(keyword, page, pageSize, token);

      if (!unitMeasures) return;

      setData(unitMeasures.data);
      setPage(unitMeasures.pagination.page);
      setPageSize(unitMeasures.pagination.limit);
      setTotalPages(unitMeasures.pagination.totalPages);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (open) {
      fetchData();
    }
  }, [keyword, page, pageSize, token, open]);

  return (
    <Dialog open={open} onOpenChange={onClose}>
      <DialogContent className="!max-w-fit">
        <DialogHeader>
          <DialogTitle>Seleccionar unidad de medida</DialogTitle>
        </DialogHeader>
        <Card>
          <CardContent>
            <InvoiceUnitMeasureModalHeader
              keyword={keyword}
              setKeyword={setKeyword}
              setPage={setPage}
            />
            {error ? (
              <AlertMessage message={error} variant="destructive" />
            ) : (
              <DataTable
                columns={columns({ onSelect })}
                data={data}
                page={page}
                pageSize={pageSize}
                totalPages={totalPages}
                onPageChange={setPage}
                onPageSizeChange={setPageSize}
                loading={loading}
              />
            )}
          </CardContent>
        </Card>
      </DialogContent>
    </Dialog>
  );
}
