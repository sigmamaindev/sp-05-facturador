import type { EmissionPoint } from "@/types/emissionPoint.types";

import { Card, CardContent } from "@/components/ui/card";

interface EmissionPointDetailInfoProps {
  emissionPoint: EmissionPoint;
}

export default function EmissionPointDetailInfo({
  emissionPoint,
}: EmissionPointDetailInfoProps) {
  const date = new Date(emissionPoint.createdAt);

  return (
    <Card>
      <CardContent>
        <dl>
          <div className="bg-gray-100 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4">
            <dt className="text-sm font-medium text-gray-500">Código</dt>
            <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              {emissionPoint.code}
            </dd>
          </div>
          <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4">
            <dt className="text-sm font-medium text-gray-500">Descripción</dt>
            <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              {emissionPoint.description}
            </dd>
          </div>
          <div className="bg-gray-100 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4">
            <dt className="text-sm font-medium text-gray-500">Activo</dt>
            <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              {emissionPoint.isActive ? "ACTIVO" : "INACTIVO"}
            </dd>
          </div>
          <div className="bg-gray-50 px-4 py-5 sm:grid sm:grid-cols-3 sm:gap-4">
            <dt className="text-sm font-medium text-gray-500">
              Fecha de creación
            </dt>
            <dd className="mt-1 text-sm text-gray-900 sm:mt-0 sm:col-span-2">
              {date.toLocaleDateString("es-EC")}
            </dd>
          </div>
        </dl>
      </CardContent>
    </Card>
  );
}
