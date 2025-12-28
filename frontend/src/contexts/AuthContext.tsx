import { createContext, useContext, useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useMutation } from "@tanstack/react-query";
import { jwtDecode } from "jwt-decode";

import type { ApiResponseSingle } from "@/types/api.types";
import type { AuthData, UserLoginForm } from "@/types/auth.types";

import { loginUser } from "@/api/auth";

interface AuthContextType {
  user: AuthData | null;
  token: string | null;
  login: (formdata: UserLoginForm) => void;
  logout: () => void;
  isAuthenticated: boolean;
  isAuthReady: boolean;
  error: string | null;
  isPending: boolean;
}

interface JwtPayload {
  exp: number;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export default function AuthProvider({
  children,
}: {
  children: React.ReactNode;
}) {
  const [user, setUser] = useState<AuthData | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [isAuthReady, setIsAuthReady] = useState(false);

  const navigate = useNavigate();

  useEffect(() => {
    const storedToken = localStorage.getItem("AUTH_TOKEN");
    const storedUser = localStorage.getItem("AUTH_USER");

    let timer: ReturnType<typeof setTimeout> | undefined;

    if (storedToken && storedUser) {
      try {
        const decoded = jwtDecode<JwtPayload>(storedToken);
        const now = Date.now() / 1000;

        if (decoded.exp < now) {
          logout();
        } else {
          setToken(storedToken);
          setUser(JSON.parse(storedUser));

          const timeLeft = decoded.exp - now;
          timer = setTimeout(() => logout(), timeLeft * 1000);
        }
      } catch {
        logout();
      }
    }

    setIsAuthReady(true);

    return () => {
      if (timer) clearTimeout(timer);
    };
  }, []);

  const { mutate: loginMutation, isPending } = useMutation<
    ApiResponseSingle<AuthData>,
    Error,
    UserLoginForm
  >({
    mutationFn: loginUser,
    onError: (err) => {
      setError(err.message);
    },
    onSuccess: (response) => {
      const loggedUser = response.data;
      if (!loggedUser) return null;

      setUser(loggedUser);
      setToken(loggedUser.token);
      setError(null);

      localStorage.setItem("AUTH_TOKEN", loggedUser.token);
      localStorage.setItem("AUTH_USER", JSON.stringify(loggedUser));

      try {
        const decoded = jwtDecode<JwtPayload>(loggedUser.token);
        const now = Date.now() / 1000;
        const timeLeft = decoded.exp - now;
        setTimeout(() => logout(), timeLeft * 1000);
      } catch (error) {
        logout();
      }

      navigate("/");
    },
  });

  const login = (formData: UserLoginForm) => loginMutation(formData);

  const logout = () => {
    setUser(null);
    setToken(null);
    setError(null);
    localStorage.removeItem("AUTH_TOKEN");
    localStorage.removeItem("AUTH_USER");

    navigate("/auth/login");
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        login,
        logout,
        isAuthenticated: !!token,
        isAuthReady,
        error,
        isPending,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context)
    throw new Error("useAuth debe usarse dentro de un AuthProvider");
  return context;
}
