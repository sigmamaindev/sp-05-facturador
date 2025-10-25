import { useState } from "react";

import { Card, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";

import BusinessListHeader from "./BusinessListHeader";

export default function BusinessListView() {
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(1);
  const [keyword, setKeyword] = useState("");

  return (
    <Card>
      <CardContent>
        <BusinessListHeader
          keyword={keyword}
          setKeyword={setKeyword}
          setPage={setPage}
        />
      </CardContent>
    </Card>
  );
}
