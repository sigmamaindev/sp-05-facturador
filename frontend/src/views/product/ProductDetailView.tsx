import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import { useAuth } from "@/contexts/AuthContext";

import { getProductById } from "@/api/product";

import type { Product } from "@/types/product.types";

import { Card, CardContent } from "@/components/ui/card";

import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";

import ProductDetailHeader from "./ProductDetailHeader";
import ProductDetailInfo from "./ProductDetailInfo";

export default function ProductDetailView() {
  const { id } = useParams<{ id: string }>();
  const { token } = useAuth();

  const [product, setProduct] = useState<Product | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchData = async () => {
    try {
      setLoading(true);
      setError(null);

      const response = await getProductById(Number(id), token!);

      setProduct(response.data);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (id && token) fetchData();
  }, [id, token]);

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
          <ProductDetailInfo product={product} />
        )}
      </CardContent>
    </Card>
  );
}

