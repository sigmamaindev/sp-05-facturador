import React, { useCallback, useEffect, useRef, useState } from "react";
import { FileDown, FileSpreadsheet, Search, X } from "lucide-react";

import { useAuth } from "@/contexts/AuthContext";

import { getProducts } from "@/api/product";
import { downloadKardexReportPdf, downloadKardexReportExcel } from "@/api/kardex";

import type { Product } from "@/types/product.types";

import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

interface KardexListHeaderProps {
  keyword: string;
  setPage: React.Dispatch<React.SetStateAction<number>>;
  setKeyword: React.Dispatch<React.SetStateAction<string>>;
  selectedProductId: number | null;
  selectedProductLabel: string;
  setSelectedProduct: (id: number | null, label: string) => void;
  dateFrom: string;
  setDateFrom: React.Dispatch<React.SetStateAction<string>>;
  dateTo: string;
  setDateTo: React.Dispatch<React.SetStateAction<string>>;
}

function triggerDownload(blob: Blob, fileName: string) {
  const url = window.URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = fileName;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  window.URL.revokeObjectURL(url);
}

export default function KardexListHeader({
  keyword,
  setPage,
  setKeyword,
  selectedProductId,
  selectedProductLabel,
  setSelectedProduct,
  dateFrom,
  setDateFrom,
  dateTo,
  setDateTo,
}: KardexListHeaderProps) {
  const { token } = useAuth();

  const [productSearch, setProductSearch] = useState("");
  const [productResults, setProductResults] = useState<Product[]>([]);
  const [showDropdown, setShowDropdown] = useState(false);
  const [loadingProducts, setLoadingProducts] = useState(false);
  const [loadingPdf, setLoadingPdf] = useState(false);
  const [loadingExcel, setLoadingExcel] = useState(false);

  const dropdownRef = useRef<HTMLDivElement>(null);
  const debounceRef = useRef<ReturnType<typeof setTimeout>>();

  const isReportMode = selectedProductId !== null;
  const canExport = isReportMode && dateFrom && dateTo;

  const searchProducts = useCallback(
    async (query: string) => {
      if (!token || query.length < 1) {
        setProductResults([]);
        return;
      }

      setLoadingProducts(true);
      try {
        const response = await getProducts(query, 1, 15, token);
        setProductResults(response.data);
        setShowDropdown(true);
      } catch {
        setProductResults([]);
      } finally {
        setLoadingProducts(false);
      }
    },
    [token]
  );

  useEffect(() => {
    if (debounceRef.current) clearTimeout(debounceRef.current);

    if (productSearch.length >= 1) {
      debounceRef.current = setTimeout(() => {
        searchProducts(productSearch);
      }, 300);
    } else {
      setProductResults([]);
      setShowDropdown(false);
    }

    return () => {
      if (debounceRef.current) clearTimeout(debounceRef.current);
    };
  }, [productSearch, searchProducts]);

  useEffect(() => {
    function handleClickOutside(e: MouseEvent) {
      if (
        dropdownRef.current &&
        !dropdownRef.current.contains(e.target as Node)
      ) {
        setShowDropdown(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  function selectProduct(product: Product) {
    setSelectedProduct(product.id, `${product.sku} - ${product.name}`);
    setProductSearch("");
    setShowDropdown(false);
    setPage(1);
  }

  function clearProduct() {
    setSelectedProduct(null, "");
    setProductSearch("");
    setPage(1);
  }

  async function handlePdf() {
    if (!token || !selectedProductId || !dateFrom || !dateTo) return;
    setLoadingPdf(true);
    try {
      const blob = await downloadKardexReportPdf(
        selectedProductId,
        dateFrom,
        dateTo,
        token
      );
      triggerDownload(blob, "ReporteKardex.pdf");
    } catch (err: unknown) {
      alert(
        err instanceof Error ? err.message : "No se pudo descargar el PDF"
      );
    } finally {
      setLoadingPdf(false);
    }
  }

  async function handleExcel() {
    if (!token || !selectedProductId || !dateFrom || !dateTo) return;
    setLoadingExcel(true);
    try {
      const blob = await downloadKardexReportExcel(
        selectedProductId,
        dateFrom,
        dateTo,
        token
      );
      triggerDownload(blob, "ReporteKardex.xlsx");
    } catch (err: unknown) {
      alert(
        err instanceof Error ? err.message : "No se pudo descargar el Excel"
      );
    } finally {
      setLoadingExcel(false);
    }
  }

  return (
    <div className="flex flex-col gap-4 pb-4">
      {/* Title + Export buttons */}
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-3">
        <h1 className="text-lg font-semibold">KARDEX</h1>
        {canExport && (
          <div className="flex items-center gap-2">
            <Button
              variant="outline"
              size="sm"
              onClick={handlePdf}
              disabled={loadingPdf}
              className="flex items-center gap-2"
            >
              <FileDown size={16} />
              {loadingPdf ? "Generando..." : "PDF"}
            </Button>
            <Button
              variant="outline"
              size="sm"
              onClick={handleExcel}
              disabled={loadingExcel}
              className="flex items-center gap-2"
            >
              <FileSpreadsheet size={16} />
              {loadingExcel ? "Generando..." : "Excel"}
            </Button>
          </div>
        )}
      </div>

      {/* Filters row */}
      <div className="flex flex-col md:flex-row md:flex-wrap md:items-center gap-3">
        {/* Product selector */}
        <div className="relative" ref={dropdownRef}>
          {isReportMode ? (
            <div className="flex items-center gap-2 border rounded-md px-3 py-1.5 bg-muted text-sm max-w-sm">
              <span className="truncate">{selectedProductLabel}</span>
              <button
                onClick={clearProduct}
                className="ml-auto shrink-0 text-muted-foreground hover:text-foreground"
              >
                <X size={14} />
              </button>
            </div>
          ) : (
            <>
              <div className="relative">
                <Search
                  size={14}
                  className="absolute left-2.5 top-1/2 -translate-y-1/2 text-muted-foreground"
                />
                <Input
                  placeholder="Buscar producto..."
                  value={productSearch}
                  onChange={(e) => setProductSearch(e.target.value)}
                  onFocus={() => {
                    if (productResults.length > 0) setShowDropdown(true);
                  }}
                  className="pl-8 max-w-sm"
                />
              </div>
              {showDropdown && productResults.length > 0 && (
                <div className="absolute z-50 mt-1 w-full max-w-sm bg-background border rounded-md shadow-lg max-h-60 overflow-auto">
                  {loadingProducts && (
                    <div className="px-3 py-2 text-sm text-muted-foreground">
                      Buscando...
                    </div>
                  )}
                  {productResults.map((p) => (
                    <button
                      key={p.id}
                      onClick={() => selectProduct(p)}
                      className="w-full text-left px-3 py-2 hover:bg-accent text-sm flex flex-col"
                    >
                      <span className="font-medium">{p.sku}</span>
                      <span className="text-muted-foreground text-xs">
                        {p.name}
                      </span>
                    </button>
                  ))}
                </div>
              )}
            </>
          )}
        </div>

        {/* Date range (only in report mode) */}
        {isReportMode && (
          <div className="flex items-center gap-2">
            <Input
              type="date"
              value={dateFrom}
              onChange={(e) => {
                setPage(1);
                setDateFrom(e.target.value);
              }}
              className="w-44"
              aria-label="Desde"
            />
            <span className="text-muted-foreground text-sm">—</span>
            <Input
              type="date"
              value={dateTo}
              onChange={(e) => {
                setPage(1);
                setDateTo(e.target.value);
              }}
              className="w-44"
              aria-label="Hasta"
            />
          </div>
        )}

        {/* Keyword search (only in flat list mode) */}
        {!isReportMode && (
          <Input
            placeholder="Buscar movimientos..."
            value={keyword}
            onChange={(e) => {
              setPage(1);
              setKeyword(e.target.value);
            }}
            className="max-w-sm"
          />
        )}
      </div>
    </div>
  );
}
