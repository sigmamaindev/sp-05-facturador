import { Link, useLocation } from "react-router-dom";

import { useAuth } from "@/contexts/AuthContext";

import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";

export default function NotFoundView() {
  const { isAuthenticated } = useAuth();
  const location = useLocation();

  const primaryHref = isAuthenticated ? "/" : "/auth/login";
  const primaryLabel = isAuthenticated ? "Ir al dashboard" : "Ir al login";

  return (
    <div className="flex min-h-[60vh] items-center justify-center px-4">
      <Card className="w-full max-w-xl">
        <CardHeader className="text-center">
          <div className="text-7xl md:text-8xl font-extrabold tracking-tight leading-none">
            404
          </div>

          <CardTitle className="mt-2 text-xl md:text-2xl">
            Página no encontrada
          </CardTitle>

          <CardDescription className="mt-1">
            No existe una ruta para{" "}
            <span className="font-medium">{location.pathname}</span>.
          </CardDescription>
        </CardHeader>

        <CardContent className="text-center text-sm text-muted-foreground">
          Verifica la URL o vuelve a una sección válida del sistema.
        </CardContent>

        <CardFooter className="flex justify-center">
          <Button asChild>
            <Link to={primaryHref}>{primaryLabel}</Link>
          </Button>
        </CardFooter>
      </Card>
    </div>
  );
}
