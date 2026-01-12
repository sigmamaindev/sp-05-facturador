import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";
import { useNavigate, useParams } from "react-router-dom";
import { toast } from "sonner";

import { useAuth } from "@/contexts/AuthContext";
import {
  addAccountsReceivableBulkPayments,
  getAccountsReceivableByCustomerId,
} from "@/api/accountsReceivable";
import type { AccountsReceivable } from "@/types/accountsReceivable.types";
import type { PaymentTypeValue } from "@/constants/paymentMethods";
import {
  isPaymentType,
  PAYMENT_TYPE_OPTIONS,
} from "@/constants/paymentMethods";

import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import AlertMessage from "@/components/shared/AlertMessage";
import DataTable from "@/components/shared/DataTable";
import type { ColumnDef } from "@tanstack/react-table";
import AccountsReceivableCustomerPaymentsHeader from "./AccountsReceivableCustomerPaymentsHeader";

type PaymentDraft = {
  amount: string;
  paymentMethod: PaymentTypeValue | "";
  notes: string;
};

type DraftsContextValue = {
  drafts: Record<number, PaymentDraft>;
  setDrafts: React.Dispatch<React.SetStateAction<Record<number, PaymentDraft>>>;
  sending: boolean;
};

const DraftsContext = createContext<DraftsContextValue | null>(null);

function useDraftsContext() {
  const ctx = useContext(DraftsContext);
  if (!ctx) throw new Error("DraftsContext missing");
  return ctx;
}

function AmountCell({
  accountsReceivableId,
}: {
  accountsReceivableId: number;
}) {
  const { drafts, setDrafts, sending } = useDraftsContext();
  const value = drafts[accountsReceivableId]?.amount ?? "";

  return (
    <Input
      type="number"
      inputMode="decimal"
      step="0.01"
      min="0"
      value={value}
      disabled={sending}
      onChange={(e) => {
        const amount = e.target.value;
        setDrafts((prev) => ({
          ...prev,
          [accountsReceivableId]: {
            ...(prev[accountsReceivableId] ?? {
              notes: "",
              paymentMethod: "",
              amount: "",
            }),
            amount,
          },
        }));
      }}
      className="w-[130px]"
    />
  );
}

function PaymentMethodCell({
  accountsReceivableId,
}: {
  accountsReceivableId: number;
}) {
  const { drafts, setDrafts, sending } = useDraftsContext();
  const value = drafts[accountsReceivableId]?.paymentMethod ?? "";

  return (
    <Select
      value={value}
      onValueChange={(val) => {
        if (!isPaymentType(val)) return;
        setDrafts((prev) => ({
          ...prev,
          [accountsReceivableId]: {
            ...(prev[accountsReceivableId] ?? {
              amount: "",
              notes: "",
              paymentMethod: "",
            }),
            paymentMethod: val,
          },
        }));
      }}
      disabled={sending}
    >
      <SelectTrigger className="w-[190px]">
        <SelectValue placeholder="Seleccione..." />
      </SelectTrigger>
      <SelectContent>
        {PAYMENT_TYPE_OPTIONS.map((opt) => (
          <SelectItem key={opt.value} value={opt.value}>
            {opt.label}
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  );
}

function NotesCell({
  accountsReceivableId,
}: {
  accountsReceivableId: number;
}) {
  const { drafts, setDrafts, sending } = useDraftsContext();
  const value = drafts[accountsReceivableId]?.notes ?? "";

  return (
    <Input
      value={value}
      disabled={sending}
      onChange={(e) => {
        const notes = e.target.value;
        setDrafts((prev) => ({
          ...prev,
          [accountsReceivableId]: {
            ...(prev[accountsReceivableId] ?? {
              amount: "",
              notes: "",
              paymentMethod: "",
            }),
            notes,
          },
        }));
      }}
      className="min-w-[220px]"
      placeholder="Observaciones..."
    />
  );
}

export default function AccountsReceivableCustomerPaymentsView() {
  const { token } = useAuth();
  const { customerId } = useParams<{ customerId: string }>();
  const navigate = useNavigate();

  const parsedCustomerId = Number(customerId);
  const backTo = "/cuentas-por-cobrar";

  const [data, setData] = useState<AccountsReceivable[]>([]);
  const [drafts, setDrafts] = useState<Record<number, PaymentDraft>>({});
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(1);
  const [keyword, setKeyword] = useState("");
  const [loading, setLoading] = useState(false);
  const [sending, setSending] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchData = useCallback(async () => {
    if (!token) return;
    if (!Number.isFinite(parsedCustomerId) || parsedCustomerId <= 0) {
      setError("Cliente inválido");
      return;
    }

    setLoading(true);
    setError(null);

    try {
      const response = await getAccountsReceivableByCustomerId(
        parsedCustomerId,
        keyword,
        page,
        pageSize,
        token
      );

      setData(response.data);
      setPage(response.pagination.page);
      setPageSize(response.pagination.limit);
      setTotalPages(response.pagination.totalPages);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : "Error desconocido");
    } finally {
      setLoading(false);
    }
  }, [keyword, page, pageSize, parsedCustomerId, token]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  useEffect(() => {
    setDrafts((prev) => {
      const next = { ...prev };
      for (const ar of data) {
        if (!next[ar.id]) {
          next[ar.id] = { amount: "", paymentMethod: "", notes: "" };
        }
      }
      return next;
    });
  }, [data]);

  const columns = useMemo<ColumnDef<AccountsReceivable>[]>(() => {
    return [
      {
        id: "invoiceDate",
        header: "Fecha de emisión",
        cell: ({ row }) => {
          const date = new Date(row.original.invoice.invoiceDate);
          const dateStr = date.toLocaleDateString("es-EC", {
            year: "numeric",
            month: "2-digit",
            day: "2-digit",
          });
          return <span className="font-semibold">{dateStr}</span>;
        },
      },
      {
        id: "code",
        header: "Código",
        accessorFn: (ar) =>
          `${ar.invoice.establishmentCode} ${ar.invoice.emissionPointCode} ${ar.invoice.sequential}`,
        cell: ({ row }) => (
          <span className="font-semibold">
            {row.original.invoice.establishmentCode}-
            {row.original.invoice.emissionPointCode}-
            {row.original.invoice.sequential}
          </span>
        ),
      },
      {
        id: "dueDate",
        header: "Vencimiento",
        cell: ({ row }) => {
          const due = new Date(row.original.dueDate);
          const dueStr = due.toLocaleDateString("es-EC", {
            year: "numeric",
            month: "2-digit",
            day: "2-digit",
          });
          return <span className="font-semibold">{dueStr}</span>;
        },
      },
      {
        id: "balance",
        header: () => <div className="text-right">Saldo</div>,
        cell: ({ row }) => (
          <p className="text-right">{row.original.balance.toFixed(2)}</p>
        ),
      },
      {
        id: "amount",
        header: "Monto",
        cell: ({ row }) => <AmountCell accountsReceivableId={row.original.id} />,
      },
      {
        id: "paymentMethod",
        header: "Método",
        cell: ({ row }) => (
          <PaymentMethodCell accountsReceivableId={row.original.id} />
        ),
      },
      {
        id: "notes",
        header: "Notas",
        cell: ({ row }) => <NotesCell accountsReceivableId={row.original.id} />,
      },
    ];
  }, []);

  const onSendPayments = useCallback(async () => {
    if (!token) return;
    if (!Number.isFinite(parsedCustomerId) || parsedCustomerId <= 0) {
      toast.error("Cliente inválido");
      return;
    }

    try {
      setSending(true);

      const arById = new Map<number, AccountsReceivable>();
      for (const ar of data) arById.set(ar.id, ar);

      const pending = Object.entries(drafts).flatMap(([idStr, draft]) => {
        const id = Number(idStr);
        const ar = arById.get(id);
        if (!ar) return [];

        const amount = Number(draft.amount);
        if (!draft.amount || !Number.isFinite(amount) || amount <= 0) return [];

        const method = draft.paymentMethod;
        if (!method) {
          throw new Error(`Seleccione un método de pago para el ID ${id}`);
        }

        if (amount > ar.balance) {
          throw new Error(
            `El monto (${amount.toFixed(2)}) no puede ser mayor al saldo (${ar.balance.toFixed(2)}) para el ID ${id}`
          );
        }

        return [
          {
            accountsReceivableId: id,
            amount,
            paymentMethod: method,
            notes: draft.notes,
          },
        ];
      });

      if (pending.length === 0) {
        toast.error("Debe ingresar al menos un pago válido");
        return;
      }

      const groups = new Map<
        PaymentTypeValue,
        Map<
          string | null,
          {
            notes: string | null;
            items: { accountsReceivableId: number; amount: number }[];
          }
        >
      >();

      for (const p of pending) {
        const normalizedNotes = p.notes.trim() ? p.notes.trim() : null;

        let byNotes = groups.get(p.paymentMethod);
        if (!byNotes) {
          byNotes = new Map();
          groups.set(p.paymentMethod, byNotes);
        }

        let group = byNotes.get(normalizedNotes);
        if (!group) {
          group = { notes: normalizedNotes, items: [] };
          byNotes.set(normalizedNotes, group);
        }

        group.items.push({
          accountsReceivableId: p.accountsReceivableId,
          amount: p.amount,
        });
      }

      for (const [paymentMethod, byNotes] of groups.entries()) {
        for (const group of byNotes.values()) {
          const response = await addAccountsReceivableBulkPayments(
            { paymentMethod, notes: group.notes, items: group.items },
            token
          );
          toast.success(response.message);
        }
      }

      navigate(backTo);
    } catch (err: unknown) {
      toast.error(err instanceof Error ? err.message : "Error desconocido");
    } finally {
      setSending(false);
    }
  }, [backTo, data, drafts, navigate, parsedCustomerId, token]);

  return (
    <Card>
      <CardContent className="space-y-4">
        <AccountsReceivableCustomerPaymentsHeader
          backTo={backTo}
          keyword={keyword}
          sending={sending}
          setKeyword={setKeyword}
          setPage={setPage}
          onSendPayments={onSendPayments}
        />

        {error ? (
          <AlertMessage message={error} variant="destructive" />
        ) : (
          <DraftsContext.Provider value={{ drafts, setDrafts, sending }}>
            <DataTable
              columns={columns}
              data={data}
              page={page}
              pageSize={pageSize}
              totalPages={totalPages}
              onPageChange={setPage}
              onPageSizeChange={setPageSize}
              loading={loading || sending}
            />
          </DraftsContext.Provider>
        )}
      </CardContent>
    </Card>
  );
}
