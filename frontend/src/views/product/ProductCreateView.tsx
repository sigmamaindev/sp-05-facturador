import { useEffect, useState } from "react";

import { useAuth } from "@/contexts/AuthContext";

import type { Product } from "@/types/product.types";
import type { UnitMeasure } from "@/types/unitMeasure.types";
import type { Tax } from "@/types/tax.types";

import { getUnitMeasures } from "@/api/unitMeasure";
import { getTaxes } from "@/api/tax";

import { Card, CardContent } from "@/components/ui/card";

import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";

import ProductCreateHeader from "./ProductCreateHeader";
import ProductCreateForm from "./ProductCreateForm";
import ProductCreateInventoryForm from "./ProductCreateInventoryForm";

export default function ProductCreateView() {
  const { token } = useAuth();

  const [step, setStep] = useState<number>(1);

  const [product, setProduct] = useState<Product | null>(null);
  const [unitMeasures, setUnitMeasures] = useState<UnitMeasure[]>([]);
  const [taxes, setTaxes] = useState<Tax[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchAllData = async () => {
    try {
      setLoading(true);

      const [unitMeaRes, taxRes] = await Promise.all([
        getUnitMeasures("", 1, 100, token!),
        getTaxes("", 1, 100, token!),
      ]);

      setUnitMeasures(unitMeaRes.data);
      setTaxes(taxRes.data);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchAllData();
  }, [token]);

  const handleNext = (data: Product) => {
    setProduct(data);
    setStep(2);
  };

  return (
    <Card>
      <CardContent>
        <ProductCreateHeader step={step} />
        {loading ? (
          <Loading />
        ) : error ? (
          <AlertMessage message={error} variant="destructive" />
        ) : !unitMeasures || !taxes ? (
          <AlertMessage
            message="Los datos del catálogo no se han cargado completamente."
            variant="destructive"
          />
        ) : (
          <>
            {step === 1 && (
              <ProductCreateForm
                unitMeasures={unitMeasures}
                taxes={taxes}
                token={token}
                onNext={handleNext}
              />
            )}
            {step === 2 && <ProductCreateInventoryForm />}
          </>
        )}
      </CardContent>
    </Card>
  );
}
