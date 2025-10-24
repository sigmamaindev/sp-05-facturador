import {
  getCoreRowModel,
  useReactTable,
  type ColumnDef,
  flexRender,
} from "@tanstack/react-table";

import {
  ChevronLeftIcon,
  ChevronRightIcon,
  ChevronsLeftIcon,
  ChevronsRightIcon,
} from "lucide-react";

import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "../ui/select";
import { Button } from "../ui/button";

import Loading from "./Loading";

interface DataTableProps<TData, TValue> {
  columns: ColumnDef<TData, TValue>[];
  data: TData[];
  page: number;
  pageSize: number;
  totalPages: number;
  onPageChange: (page: number) => void;
  onPageSizeChange: (page: number) => void;
  loading?: boolean;
}

export default function DataTable<TData, TValue>({
  columns,
  data,
  page,
  pageSize,
  totalPages,
  onPageChange,
  onPageSizeChange,
  loading = false,
}: DataTableProps<TData, TValue>) {
  const table = useReactTable({
    data,
    columns,
    getCoreRowModel: getCoreRowModel(),
  });

  return (
    <div className="space-y-4">
      <div className="rounded-md border relative overflow-x-auto">
        <Table className="min-w-full">
          <TableHeader>
            {table.getHeaderGroups().map((headerGroup) => (
              <TableRow key={headerGroup.id}>
                {headerGroup.headers.map((header) => (
                  <TableHead key={header.id}>
                    {header.isPlaceholder
                      ? null
                      : flexRender(
                          header.column.columnDef.header,
                          header.getContext()
                        )}
                  </TableHead>
                ))}
              </TableRow>
            ))}
          </TableHeader>
          <TableBody>
            {loading ? (
              <TableRow>
                <TableCell
                  colSpan={columns.length}
                  className="h-24 text-center"
                >
                  <Loading />
                </TableCell>
              </TableRow>
            ) : table.getRowModel().rows?.length > 0 ? (
              table.getRowModel().rows.map((row) => (
                <TableRow key={row.id}>
                  {row.getVisibleCells().map((cell) => (
                    <TableCell key={cell.id}>
                      {flexRender(
                        cell.column.columnDef.cell,
                        cell.getContext()
                      )}
                    </TableCell>
                  ))}
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell
                  colSpan={columns.length}
                  className="h-24 text-center"
                >
                  Sin resultados
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>
      {!loading ? (
        table.getRowModel().rows?.length > 0 ? (
          <>
            <div className="flex w-full items-center justify-between gap-2">
              <span className="text-sm sm:text-base">
                Página {page} de {totalPages}
              </span>
              <div className="flex gap-4">
                <div className="hidden sm:block">
                  <Select
                    onValueChange={(value) => onPageSizeChange(Number(value))}
                    value={pageSize.toString()}
                  >
                    <SelectTrigger className="w-[100px]">
                      <SelectValue placeholder="10" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="5">5</SelectItem>
                      <SelectItem value="10">10</SelectItem>
                      <SelectItem value="20">20</SelectItem>
                      <SelectItem value="50">50</SelectItem>
                      <SelectItem value="100">100</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
                <div className="flex items-center gap-1 sm:gap-2">
                  <Button
                    variant="outline"
                    className="hidden sm:flex h-8 w-8 p-0"
                    disabled={page === 1}
                    onClick={() => onPageChange(1)}
                  >
                    <ChevronsLeftIcon className="h-4 w-4" />
                  </Button>

                  <Button
                    variant="outline"
                    className="h-8 w-8 p-0"
                    disabled={page === 1}
                    onClick={() => onPageChange(page - 1)}
                  >
                    <ChevronLeftIcon className="h-4 w-4" />
                  </Button>

                  <Button
                    variant="outline"
                    className="h-8 w-8 p-0"
                    disabled={page === totalPages}
                    onClick={() => onPageChange(page + 1)}
                  >
                    <ChevronRightIcon className="h-4 w-4" />
                  </Button>

                  <Button
                    variant="outline"
                    className="hidden sm:flex h-8 w-8 p-0"
                    disabled={page === totalPages}
                    onClick={() => onPageChange(totalPages)}
                  >
                    <ChevronsRightIcon className="h-4 w-4" />
                  </Button>
                </div>
              </div>
            </div>
          </>
        ) : null
      ) : null}
    </div>
  );
}
