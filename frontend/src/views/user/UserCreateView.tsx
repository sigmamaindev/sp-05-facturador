import { useState, useEffect } from "react";

import { useAuth } from "@/contexts/AuthContext";

import type { Role } from "@/types/role.types";
import type { Establishment } from "@/types/establishment.types";

import { getRoles } from "@/api/role";
import { getEstablishments } from "@/api/establishment";

import { Card, CardContent } from "@/components/ui/card";

import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";

import UserCreateHeader from "./UserCreateHeader";
import UserCreateForm from "./UserCreateForm";

export default function UserCreateView() {
  const { token } = useAuth();

  const [roles, setRoles] = useState<Role[]>([]);
  const [establishments, setEstablishments] = useState<Establishment[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchAllData = async () => {
    try {
      setLoading(true);

      const [rolRes, estRes] = await Promise.all([
        getRoles(token!),
        getEstablishments("", 1, 100, token!),
      ]);

      setRoles(rolRes.data);
      setEstablishments(estRes.data);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (token) fetchAllData();
  }, [token]);

  return (
    <Card>
      <CardContent>
        <UserCreateHeader />
        {loading ? (
          <Loading />
        ) : error ? (
          <AlertMessage message={error} variant="destructive" />
        ) : !roles.length || !establishments.length ? (
          <AlertMessage
            message="Los datos del catálogo no se han cargado completamente."
            variant="destructive"
          />
        ) : (
          <UserCreateForm
            roles={roles}
            establishments={establishments}
            token={token}
          />
        )}
      </CardContent>
    </Card>
  );
}
