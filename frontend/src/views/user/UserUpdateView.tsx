import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import { useAuth } from "@/contexts/AuthContext";

import type { User } from "@/types/user.types";
import type { Role } from "@/types/role.types";
import type { Establishment } from "@/types/establishment.types";
import type { EmissionPoint } from "@/types/emissionPoint.types";

import { getUserById } from "@/api/user";
import { getRoles } from "@/api/role";
import { getEstablishments } from "@/api/establishment";
import { getEmissionPoints } from "@/api/emissionPoint";

import { Card, CardContent } from "@/components/ui/card";

import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";

import UserUpdateHeader from "./UserUpdateHeader";
import UserUpdateForm from "./UserUpdateForm";

export default function UserUpdateView() {
  const { id } = useParams<{ id: string }>();
  const { token } = useAuth();

  const [user, setUser] = useState<User | null>(null);
  const [roles, setRoles] = useState<Role[]>([]);
  const [establishments, setEstablishments] = useState<Establishment[]>([]);
  const [emissionPoints, setEmissionPoints] = useState<EmissionPoint[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchAllData = async () => {
    try {
      setLoading(true);

      const [rolRes, estRes, emiRes, userRes] = await Promise.all([
        getRoles(),
        getEstablishments("", 1, 100, token!),
        getEmissionPoints("", 1, 100, token!),
        getUserById(Number(id), token!),
      ]);

      setRoles(rolRes.data);
      setEstablishments(estRes.data);
      setEmissionPoints(emiRes.data);
      setUser(userRes.data);
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
        <UserUpdateHeader />
        {loading ? (
          <Loading />
        ) : error ? (
          <AlertMessage message={error} variant="destructive" />
        ) : !user ||
          !roles.length ||
          !establishments.length ||
          !emissionPoints.length ? (
          <AlertMessage
            message="Los datos del usuario no se han cargado completamente"
            variant="destructive"
          />
        ) : (
          <UserUpdateForm
            user={user}
            roles={roles}
            establishments={establishments}
            emissionPoints={emissionPoints}
            token={token}
          />
        )}
      </CardContent>
    </Card>
  );
}
