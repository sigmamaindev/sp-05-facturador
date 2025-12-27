import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import { useAuth } from "@/contexts/AuthContext";

import { getUserById } from "@/api/user";

import type { User } from "@/types/user.types";

import { Card, CardContent } from "@/components/ui/card";

import Loading from "@/components/shared/Loading";
import AlertMessage from "@/components/shared/AlertMessage";

import UserDetailHeader from "./UserDetailHeader";
import UserDetailInfo from "./UserDetailInfo";

export default function UserDetailView() {
  const { id } = useParams<{ id: string }>();
  const { token } = useAuth();

  const [user, setUser] = useState<User | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchData = async () => {
    try {
      setLoading(true);
      setError(null);

      const response = await getUserById(Number(id), token!);

      setUser(response.data);
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
        <UserDetailHeader />
        {loading ? (
          <Loading />
        ) : error ? (
          <AlertMessage message={error} variant="destructive" />
        ) : !user ? (
          <AlertMessage
            message="Los datos del usuario no se han cargado completamente"
            variant="destructive"
          />
        ) : (
          <UserDetailInfo user={user} />
        )}
      </CardContent>
    </Card>
  );
}
