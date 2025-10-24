import { useForm } from "react-hook-form";

import { useAuth } from "@/contexts/AuthContext";
import type { UserLoginForm } from "@/types/auth.types";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";

import AlertMessage from "@/components/shared/AlertMessage";

export default function LoginView() {
  const { register, handleSubmit } = useForm<UserLoginForm>();

  const { login, error, isPending } = useAuth();

  const handleLogin = (formdata: UserLoginForm) => login(formdata);

  return (
    <div className="bg-muted flex min-h-svh flex-col items-center justify-center gap-6 p-6 md:p-10">
      <Card className="w-full max-w-sm">
        <CardHeader>
          <CardTitle className="text-center">FACTURADOR</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit(handleLogin)}>
            <div className="flex flex-col gap-6">
              {error && <AlertMessage message={error} variant="destructive" />}
              <div className="grid gap-2">
                <Label htmlFor="username">Usuario</Label>
                <Input
                  id="username"
                  type="text"
                  placeholder="Usuario"
                  {...register("username", {
                    required: "El usuario es obligatorio",
                  })}
                />
              </div>
              <div className="grid gap-2">
                <Label htmlFor="password">Contraseña</Label>
                <Input
                  id="password"
                  type="password"
                  placeholder="Contraseña"
                  {...register("password", {
                    required: "La contraseña es obligatoria",
                  })}
                />
              </div>
              <Button type="submit" disabled={isPending}>
                {isPending ? "Cargando..." : "Iniciar Sesión"}
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
