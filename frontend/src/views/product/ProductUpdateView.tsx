import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import { useAuth } from "@/contexts/AuthContext";

import type { Product } from "@/types/product.types";
import type { UnitMeasure } from "@/types/unitMeasure.types";
import type { Tax } from "@/types/tax.types";

import { getProductById } from "@/api/product";
import { getUnitMeasures } from "@/api/unitMeasure";
import { getTaxes } from "@/api/tax";

import { Card, CardContent } from "@/components/ui/card";

import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";

import ProductUpdateHeader from "./ProductUpdateHeader";
import ProductUpdateForm from "./ProductUpdateForm";

export default function ProductUpdateView() {
  const { id } = useParams<{ id: string }>();
  const { token } = useAuth();

  const [product, setProduct] = useState<Product | null>(null);
  const [unitMeasures, setUnitMeasures] = useState<UnitMeasure[]>([]);
  const [taxes, setTaxes] = useState<Tax[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchAllData = async () => {
    try {
      setLoading(true);
      setError(null);

      const [productRes, unitMeRes, taxRes] = await Promise.all([
        getProductById(Number(id), token!),
        getUnitMeasures("", 1, 100, token!),
        getTaxes("", 1, 100, token!),
      ]);

      setProduct(productRes.data);
      setUnitMeasures(unitMeRes.data);
      setTaxes(taxRes.data);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (id && token) fetchAllData();
  }, [id, token]);

  return (
    <Card>
      <CardContent>
        {loading ? (
          <Loading />
        ) : error ? (
          <AlertMessage message={error} variant="destructive" />
        ) : !product || !unitMeasures.length || !taxes.length ? (
          <AlertMessage
            message="Los datos del producto no se han cargado completamente"
            variant="destructive"
          />
        ) : (
          <>
            <ProductUpdateHeader sku={product.sku} />
            <ProductUpdateForm
              product={product}
              unitMeasures={unitMeasures}
              taxes={taxes}
              token={token}
            />
          </>
        )}
      </CardContent>
    </Card>
  );
}
