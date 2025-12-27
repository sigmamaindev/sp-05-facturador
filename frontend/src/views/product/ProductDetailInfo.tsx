import type { Product } from "@/types/product.types";

import { Card, CardContent } from "@/components/ui/card";

interface ProductDetailInfoProps {
  product: Product;
}

function renderText(value?: string | null) {
  if (!value) return "—";
  const trimmed = value.trim();
  return trimmed.length ? trimmed : "—";
}

export default function ProductDetailInfo({ product }: ProductDetailInfoProps) {
  const date = product.createdAt ? new Date(product.createdAt) : null;

  return (
    <Card>
      <CardContent>
        <dl>
          <div className="bg-gray-100 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4">
            <dt className="text-sm font-medium text-gray-500">Código</dt>
            <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              {renderText(product.sku)}
            </dd>
          </div>
          <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4">
            <dt className="text-sm font-medium text-gray-500">Nombre</dt>
            <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              {renderText(product.name)}
            </dd>
          </div>
          <div className="bg-gray-100 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4">
            <dt className="text-sm font-medium text-gray-500">Descripción</dt>
            <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              {renderText(product.description)}
            </dd>
          </div>
          <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4">
            <dt className="text-sm font-medium text-gray-500">Precio</dt>
            <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              {Number(product.price).toFixed(2)}
            </dd>
          </div>
          <div className="bg-gray-100 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4">
            <dt className="text-sm font-medium text-gray-500">IVA</dt>
            <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              {product.iva ? "SÍ" : "NO"}
            </dd>
          </div>
          <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4">
            <dt className="text-sm font-medium text-gray-500">Tipo de IVA</dt>
            <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              {renderText(product.tax?.name)}
            </dd>
          </div>
          <div className="bg-gray-100 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4">
            <dt className="text-sm font-medium text-gray-500">
              Unidad de medida
            </dt>
            <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              {product.unitMeasure
                ? `${product.unitMeasure.code} - ${product.unitMeasure.name}`
                : "—"}
            </dd>
          </div>
          <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4">
            <dt className="text-sm font-medium text-gray-500">Tipo</dt>
            <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              {renderText(product.type)}
            </dd>
          </div>
          <div className="bg-gray-100 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4">
            <dt className="text-sm font-medium text-gray-500">Estado</dt>
            <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              {product.isActive ? "ACTIVO" : "INACTIVO"}
            </dd>
          </div>
          {date ? (
            <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4">
              <dt className="text-sm font-medium text-gray-500">
                Fecha de creación
              </dt>
              <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
                {date.toLocaleDateString("es-EC")}
              </dd>
            </div>
          ) : null}
        </dl>

        <div className="mt-6">
          <h2 className="text-base font-semibold mb-3">Inventario</h2>
          {product.inventory?.length ? (
            <div className="w-full overflow-auto rounded-md border">
              <table className="w-full text-sm">
                <thead className="bg-muted/50">
                  <tr className="text-left">
                    <th className="p-3 font-medium">Bodega</th>
                    <th className="p-3 font-medium">Stock</th>
                    <th className="p-3 font-medium">Mínimo</th>
                    <th className="p-3 font-medium">Máximo</th>
                  </tr>
                </thead>
                <tbody>
                  {product.inventory.map((inv) => (
                    <tr key={inv.id} className="border-t">
                      <td className="p-3">{`${inv.warehouseCode} - ${inv.warehouseName}`}</td>
                      <td className="p-3">{inv.stock}</td>
                      <td className="p-3">{inv.minStock}</td>
                      <td className="p-3">{inv.maxStock}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          ) : (
            <p className="text-sm text-muted-foreground">
              No hay inventario configurado para este producto.
            </p>
          )}
        </div>
      </CardContent>
    </Card>
  );
}

