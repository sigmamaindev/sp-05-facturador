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

  const navigate = useNavigate();

  useEffect(() => {
    const storedToken = localStorage.getItem("AUTH_TOKEN");
    const storedUser = localStorage.getItem("AUTH_USER");

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
          const timer = setTimeout(() => logout(), timeLeft * 1000);

          return () => clearTimeout(timer);
        }
      } catch {
        logout();
      }
    }
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
