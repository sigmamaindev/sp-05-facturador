import { useEffect, useState } from "react";

import { useAuth } from "@/contexts/AuthContext";

import { getUsers } from "@/api/user";

import { type User } from "@/types/user.types";

import { Card, CardContent } from "@/components/ui/card";

import DataTable from "@/components/shared/DataTable";
import AlertMessage from "@/components/shared/AlertMessage";

import UserListHeader from "./UserListHeader";
import { columns } from "./UserListColumns";

export default function UserListView() {
  const { token } = useAuth();

  const [data, setData] = useState<User[]>([]);
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(1);
  const [keyword, setKeyword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchData = async () => {
    if (!token) return;

    setLoading(true);

    try {
      const users = await getUsers(keyword, page, pageSize, token);

      if (!users) return;

      setData(users.data);
      setPage(users.pagination.page);
      setPageSize(users.pagination.limit);
      setTotalPages(users.pagination.totalPages);
    } catch (err: any) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, [keyword, page, pageSize, token]);

  return (
    <Card>
      <CardContent>
        <UserListHeader
          keyword={keyword}
          setKeyword={setKeyword}
          setPage={setPage}
        />
        {error ? (
          <AlertMessage message={error} variant="destructive" />
        ) : (
          <DataTable
            columns={columns}
            data={data}
            page={page}
            pageSize={pageSize}
            totalPages={totalPages}
            onPageChange={setPage}
            onPageSizeChange={setPageSize}
            loading={loading}
          />
        )}
      </CardContent>
    </Card>
  );
}
