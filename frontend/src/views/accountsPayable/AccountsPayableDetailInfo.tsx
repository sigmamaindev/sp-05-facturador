import { Badge } from "@/components/ui/badge";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";

import type { AccountsPayable } from "@/types/accountsPayable.types";

interface AccountsPayableDetailInfoProps {
  accountsPayable: AccountsPayable;
}

function formatDateTime(value: string | null) {
  if (!value || value.startsWith("0001-01-01")) return "—";

  const date = new Date(value);

  const dateStr = date.toLocaleDateString("es-EC", {
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
  });

  const timeStr = date.toLocaleTimeString("es-EC", {
    hour: "2-digit",
    minute: "2-digit",
    second: "2-digit",
    hour12: false,
  });

  return `${dateStr} ${timeStr}`;
}

function formatDate(value: string | null) {
  if (!value || value.startsWith("0001-01-01")) return "—";

  const date = new Date(value);
  return date.toLocaleDateString("es-EC", {
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
  });
}

function daysUntil(dueDate: Date) {
  const due = new Date(dueDate);
  const today = new Date();

  due.setHours(0, 0, 0, 0);
  today.setHours(0, 0, 0, 0);

  return Math.ceil((due.getTime() - today.getTime()) / 86400000);
}

function statusVariant(status: string | null | undefined) {
  const normalized = (status ?? "").toUpperCase();

  if (normalized.includes("ABIERTO")) return "secondary";
  if (normalized.includes("CERRADO")) return "default";
  if (normalized.includes("ANULADO")) return "destructive";

  return "outline";
}

function displayValue(value: string | null | undefined) {
  if (!value) return "—";
  const trimmed = value.trim();
  return trimmed.length ? trimmed : "—";
}

export default function AccountsPayableDetailInfo({
  accountsPayable,
}: AccountsPayableDetailInfoProps) {
  const establishmentCode = accountsPayable.purchase.establishmentCode;
  const emissionPointCode = accountsPayable.purchase.emissionPointCode;
  const code =
    establishmentCode && emissionPointCode
      ? `${establishmentCode}-${emissionPointCode}-${accountsPayable.purchase.sequential}`
      : accountsPayable.purchase.sequential;

  const due = new Date(accountsPayable.dueDate);
  const dueDays = Number.isNaN(due.getTime()) ? null : daysUntil(due);
  const dueLabel =
    dueDays === null
      ? "—"
      : dueDays < 0
      ? `Vencida (${Math.abs(dueDays)} día${Math.abs(dueDays) === 1 ? "" : "s"})`
      : dueDays === 0
      ? "Vence hoy"
      : `Por vencer: ${dueDays} día${dueDays === 1 ? "" : "s"}`;

  return (
    <div className="flex flex-col md:flex-row gap-4">
      <div className="md:w-3/4">
        <Card className="h-full">
          <CardHeader>
            <CardTitle>MOVIMIENTOS</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="max-h-[400px] overflow-auto rounded-md border">
              <div className="min-w-[700px]">
                <table className="w-full">
                  <thead className="sticky top-0 bg-background z-10">
                    <tr className="border-b">
                      <th className="text-left py-2 px-2 font-semibold">
                        Fecha
                      </th>
                      <th className="text-left py-2 px-2 font-semibold">
                        Tipo
                      </th>
                      <th className="text-right py-2 px-2 font-semibold">
                        Monto
                      </th>
                      <th className="text-left py-2 px-2 font-semibold">
                        Método
                      </th>
                      <th className="text-left py-2 px-2 font-semibold">
                        Referencia
                      </th>
                      <th className="text-left py-2 px-2 font-semibold">
                        Notas
                      </th>
                    </tr>
                  </thead>

                  <tbody>
                    {accountsPayable.transactions.length === 0 ? (
                      <tr>
                        <td
                          colSpan={6}
                          className="text-center py-4 text-muted-foreground"
                        >
                          No hay movimientos
                        </td>
                      </tr>
                    ) : (
                      accountsPayable.transactions.map((t) => (
                        <tr key={t.id} className="border-b last:border-b-0">
                          <td className="py-2 px-2 whitespace-nowrap">
                            {formatDateTime(t.createdAt)}
                          </td>
                          <td className="py-2 px-2 whitespace-nowrap">
                            <Badge variant="outline">
                              {t.apTransactionType}
                            </Badge>
                          </td>
                          <td className="py-2 px-2 text-right whitespace-nowrap">
                            ${t.amount.toFixed(2)}
                          </td>
                          <td className="py-2 px-2 whitespace-nowrap">
                            {displayValue(t.paymentMethod)}
                          </td>
                          <td className="py-2 px-2">
                            {displayValue(t.reference)}
                          </td>
                          <td className="py-2 px-2">{displayValue(t.notes)}</td>
                        </tr>
                      ))
                    )}
                  </tbody>
                </table>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      <div className="md:w-1/4 flex flex-col gap-4">
        <Card>
          <CardHeader>
            <CardTitle>RESUMEN</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2">
            <div className="flex items-center justify-between text-sm gap-2">
              <span className="text-muted-foreground">Estado:</span>
              <Badge variant={statusVariant(accountsPayable.status)}>
                {accountsPayable.status}
              </Badge>
            </div>
            <div className="flex justify-between text-sm">
              <span className="text-muted-foreground">Código:</span>
              <span className="font-medium">{displayValue(code)}</span>
            </div>
            <div className="flex justify-between text-sm">
              <span className="text-muted-foreground">Emisión:</span>
              <span className="font-medium">
                {formatDate(accountsPayable.purchase.issueDate)}
              </span>
            </div>
            <div className="flex flex-col text-sm">
              <div className="flex justify-between">
                <span className="text-muted-foreground">Vencimiento:</span>
                <span className="font-medium">
                  {formatDate(accountsPayable.dueDate)}
                </span>
              </div>
              <span className="text-xs text-muted-foreground">{dueLabel}</span>
            </div>
            <div className="flex justify-between text-sm">
              <span className="text-muted-foreground">Plazo:</span>
              <span className="font-medium">
                {accountsPayable.purchase.paymentTermDays} día
                {accountsPayable.purchase.paymentTermDays === 1 ? "" : "s"}
              </span>
            </div>
            <div className="flex justify-between text-sm">
              <span className="text-muted-foreground">Método pago:</span>
              <span className="font-medium">
                {displayValue(accountsPayable.purchase.paymentMethod)}
              </span>
            </div>
            <div className="flex justify-between text-sm">
              <span className="text-muted-foreground">Saldo:</span>
              <span className="font-medium">
                ${accountsPayable.balance.toFixed(2)}
              </span>
            </div>

            <div className="flex justify-between font-semibold text-base border-t pt-2">
              <span>Monto:</span>
              <span>${accountsPayable.total.toFixed(2)}</span>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>DOCUMENTO</CardTitle>
          </CardHeader>
          <CardContent className="space-y-2 text-sm">
            <div className="flex justify-between gap-2">
              <span className="text-muted-foreground">Clave acceso:</span>
              <span className="font-medium text-right break-all">
                {displayValue(accountsPayable.purchase.accessKey)}
              </span>
            </div>
            <div className="flex justify-between gap-2">
              <span className="text-muted-foreground">N° autorización:</span>
              <span className="font-medium text-right break-all">
                {displayValue(accountsPayable.purchase.authorizationNumber)}
              </span>
            </div>
            <div className="flex justify-between gap-2">
              <span className="text-muted-foreground">F. autorización:</span>
              <span className="font-medium text-right">
                {formatDateTime(accountsPayable.purchase.authorizationDate)}
              </span>
            </div>
            <div className="flex justify-between gap-2">
              <span className="text-muted-foreground">Ambiente:</span>
              <span className="font-medium text-right">
                {displayValue(accountsPayable.purchase.environment)}
              </span>
            </div>
            <div className="flex justify-between gap-2">
              <span className="text-muted-foreground">Tipo comprobante:</span>
              <span className="font-medium text-right">
                {displayValue(accountsPayable.purchase.receiptType)}
              </span>
            </div>
            <div className="flex justify-between gap-2">
              <span className="text-muted-foreground">Electrónica:</span>
              <span className="font-medium text-right">
                {accountsPayable.purchase.isElectronic ? "Sí" : "No"}
              </span>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
