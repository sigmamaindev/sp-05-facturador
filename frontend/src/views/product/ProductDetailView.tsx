import { useCallback, useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import { useAuth } from "@/contexts/AuthContext";

import { getProductById } from "@/api/product";

import type { Product } from "@/types/product.types";
import type { Inventory } from "@/types/inventory.types";

import { Card, CardContent } from "@/components/ui/card";

import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";

import ProductDetailHeader from "./ProductDetailHeader";
import ProductDetailInfo from "./ProductDetailInfo";

export default function ProductDetailView() {
  const { id } = useParams<{ id: string }>();
  const { token, user } = useAuth();

  const [product, setProduct] = useState<Product | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const canEditInventory =
    user?.roles?.includes("SuperAdmin") || user?.roles?.includes("Admin");

  const fetchData = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);

      const response = await getProductById(Number(id), token!);

      setProduct(response.data);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : "Error desconocido");
    } finally {
      setLoading(false);
    }
  }, [id, token]);

  const handleInventoryUpdated = (updated: Inventory) => {
    setProduct((current) => {
      if (!current) return current;
      return {
        ...current,
        inventory: current.inventory.map((inv) =>
          inv.id === updated.id ? { ...inv, ...updated } : inv
        ),
      };
    });
  };

  useEffect(() => {
    if (id && token) fetchData();
  }, [fetchData, id, token]);

  return (
    <Card>
      <CardContent>
        <ProductDetailHeader />
        {loading ? (
          <Loading />
        ) : error ? (
          <AlertMessage message={error} variant="destructive" />
        ) : !product ? (
          <AlertMessage
            message="Los datos del producto no se han cargado completamente"
            variant="destructive"
          />
        ) : (
          <ProductDetailInfo
            product={product}
            token={token!}
            canEditInventory={canEditInventory}
            onInventoryUpdated={handleInventoryUpdated}
          />
        )}
      </CardContent>
    </Card>
  );
}
