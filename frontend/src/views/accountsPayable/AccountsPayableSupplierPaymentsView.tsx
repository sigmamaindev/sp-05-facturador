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
  addAccountsPayableBulkPayments,
  getAccountsPayableBySupplierId,
} from "@/api/accountsPayable";
import type { AccountsPayable } from "@/types/accountsPayable.types";
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
import AccountsPayableSupplierPaymentsHeader from "./AccountsPayableSupplierPaymentsHeader";

type PaymentDraft = {
  amount: string;
  paymentMethod: PaymentTypeValue | "";
  notes: string;
  payFullBalance: boolean;
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

function AmountCell({ accountsPayableId }: { accountsPayableId: number }) {
  const { drafts, setDrafts, sending } = useDraftsContext();
  const value = drafts[accountsPayableId]?.amount ?? "";
  const payFullBalance = drafts[accountsPayableId]?.payFullBalance ?? false;

  return (
    <Input
      type="number"
      inputMode="decimal"
      step="0.01"
      min="0"
      value={value}
      disabled={sending || payFullBalance}
      onChange={(e) => {
        const amount = e.target.value;
        setDrafts((prev) => ({
          ...prev,
          [accountsPayableId]: {
            ...(prev[accountsPayableId] ?? {
              notes: "",
              paymentMethod: "",
              amount: "",
              payFullBalance: false,
            }),
            amount,
            payFullBalance: false,
          },
        }));
      }}
      className="w-[130px]"
    />
  );
}

function PaymentMethodCell({ accountsPayableId }: { accountsPayableId: number }) {
  const { drafts, setDrafts, sending } = useDraftsContext();
  const value = drafts[accountsPayableId]?.paymentMethod ?? "";

  return (
    <Select
      value={value}
      onValueChange={(val) => {
        if (!isPaymentType(val)) return;
        setDrafts((prev) => ({
          ...prev,
          [accountsPayableId]: {
            ...(prev[accountsPayableId] ?? {
              amount: "",
              notes: "",
              paymentMethod: "",
              payFullBalance: false,
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

function NotesCell({ accountsPayableId }: { accountsPayableId: number }) {
  const { drafts, setDrafts, sending } = useDraftsContext();
  const value = drafts[accountsPayableId]?.notes ?? "";

  return (
    <Input
      value={value}
      disabled={sending}
      onChange={(e) => {
        const notes = e.target.value;
        setDrafts((prev) => ({
          ...prev,
          [accountsPayableId]: {
            ...(prev[accountsPayableId] ?? {
              amount: "",
              notes: "",
              paymentMethod: "",
              payFullBalance: false,
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

function PayFullBalanceCell({
  accountsPayableId,
  balance,
}: {
  accountsPayableId: number;
  balance: number;
}) {
  const { drafts, setDrafts, sending } = useDraftsContext();
  const checked = drafts[accountsPayableId]?.payFullBalance ?? false;

  return (
    <div className="flex justify-center">
      <input
        type="checkbox"
        className="h-4 w-4 accent-primary"
        checked={checked}
        disabled={sending || balance <= 0}
        onChange={(e) => {
          const nextChecked = e.target.checked;
          setDrafts((prev) => ({
            ...prev,
            [accountsPayableId]: {
              ...(prev[accountsPayableId] ?? {
                amount: "",
                notes: "",
                paymentMethod: "",
                payFullBalance: false,
              }),
              payFullBalance: nextChecked,
              amount: nextChecked ? balance.toFixed(2) : "",
            },
          }));
        }}
        aria-label="Pagar saldo completo"
      />
    </div>
  );
}

export default function AccountsPayableSupplierPaymentsView() {
  const { token } = useAuth();
  const { supplierId } = useParams<{ supplierId: string }>();
  const navigate = useNavigate();

  const parsedSupplierId = Number(supplierId);
  const backTo = "/cuentas-por-pagar";

  const [data, setData] = useState<AccountsPayable[]>([]);
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
    if (!Number.isFinite(parsedSupplierId) || parsedSupplierId <= 0) {
      setError("Proveedor inválido");
      return;
    }

    setLoading(true);
    setError(null);

    try {
      const response = await getAccountsPayableBySupplierId(
        parsedSupplierId,
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
  }, [keyword, page, pageSize, parsedSupplierId, token]);

  useEffect(() => {
    fetchData();
  }, [fetchData]);

  useEffect(() => {
    setDrafts((prev) => {
      const next = { ...prev };
      for (const ap of data) {
        if (!next[ap.id]) {
          next[ap.id] = {
            amount: "",
            paymentMethod: "",
            notes: "",
            payFullBalance: false,
          };
        } else if (next[ap.id]?.payFullBalance) {
          next[ap.id] = { ...next[ap.id], amount: ap.balance.toFixed(2) };
        }
      }
      return next;
    });
  }, [data]);

  const totalAmount = useMemo(() => {
    const apById = new Map<number, AccountsPayable>();
    for (const ap of data) apById.set(ap.id, ap);

    let total = 0;
    for (const [idStr, draft] of Object.entries(drafts)) {
      const id = Number(idStr);
      const ap = apById.get(id);
      if (!ap) continue;

      const amount = Number(draft.amount);
      if (!draft.amount || !Number.isFinite(amount) || amount <= 0) continue;
      if (amount > ap.balance) continue;

      total += amount;
    }
    return total;
  }, [data, drafts]);

  const columns = useMemo<ColumnDef<AccountsPayable>[]>(() => {
    return [
      {
        id: "issueDate",
        header: "Fecha de emisión",
        cell: ({ row }) => {
          const date = new Date(row.original.purchase.issueDate);
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
        accessorFn: (ap) =>
          ap.purchase.establishmentCode && ap.purchase.emissionPointCode
            ? `${ap.purchase.establishmentCode} ${ap.purchase.emissionPointCode} ${ap.purchase.sequential}`
            : ap.purchase.sequential,
        cell: ({ row }) => {
          const establishmentCode = row.original.purchase.establishmentCode;
          const emissionPointCode = row.original.purchase.emissionPointCode;

          return (
            <span className="font-semibold">
              {establishmentCode && emissionPointCode
                ? `${establishmentCode}-${emissionPointCode}-${row.original.purchase.sequential}`
                : row.original.purchase.sequential}
            </span>
          );
        },
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
        cell: ({ row }) => <AmountCell accountsPayableId={row.original.id} />,
      },
      {
        id: "paymentMethod",
        header: "Método",
        cell: ({ row }) => (
          <PaymentMethodCell accountsPayableId={row.original.id} />
        ),
      },
      {
        id: "notes",
        header: "Notas",
        cell: ({ row }) => <NotesCell accountsPayableId={row.original.id} />,
      },
      {
        id: "payFullBalance",
        header: () => <div className="text-center">Pagar saldo</div>,
        cell: ({ row }) => (
          <PayFullBalanceCell
            accountsPayableId={row.original.id}
            balance={row.original.balance}
          />
        ),
      },
    ];
  }, []);

  const onSendPayments = useCallback(async () => {
    if (!token) return;
    if (!Number.isFinite(parsedSupplierId) || parsedSupplierId <= 0) {
      toast.error("Proveedor inválido");
      return;
    }

    try {
      setSending(true);

      const apById = new Map<number, AccountsPayable>();
      for (const ap of data) apById.set(ap.id, ap);

      const pending = Object.entries(drafts).flatMap(([idStr, draft]) => {
        const id = Number(idStr);
        const ap = apById.get(id);
        if (!ap) return [];

        const amount = Number(draft.amount);
        if (!draft.amount || !Number.isFinite(amount) || amount <= 0) return [];

        const method = draft.paymentMethod;
        if (!method) {
          throw new Error(`Seleccione un método de pago para el ID ${id}`);
        }

        if (amount > ap.balance) {
          throw new Error(
            `El monto (${amount.toFixed(2)}) no puede ser mayor al saldo (${ap.balance.toFixed(2)}) para el ID ${id}`
          );
        }

        return [
          {
            accountsPayableId: id,
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
            items: { accountsPayableId: number; amount: number }[];
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
          accountsPayableId: p.accountsPayableId,
          amount: p.amount,
        });
      }

      for (const [paymentMethod, byNotes] of groups.entries()) {
        for (const group of byNotes.values()) {
          const response = await addAccountsPayableBulkPayments(
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
  }, [backTo, data, drafts, navigate, parsedSupplierId, token]);

  return (
    <Card>
      <CardContent className="space-y-4">
        <AccountsPayableSupplierPaymentsHeader
          backTo={backTo}
          keyword={keyword}
          sending={sending}
          totalAmount={totalAmount}
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
