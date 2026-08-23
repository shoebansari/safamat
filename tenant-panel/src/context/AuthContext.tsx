"use client";

import { createContext, useContext, useEffect, useState, type ReactNode } from "react";
import { useRouter } from "next/navigation";
import { tenantAuthApi } from "@/lib/services";
import { clearAuth, getUser, isAuthenticated, setAuth } from "@/lib/auth";
import type { TenantUser } from "@/lib/types";

interface AuthContextType {
  user: TenantUser | null;
  loading: boolean;
  login: (username: string, password: string) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<TenantUser | null>(null);
  const [loading, setLoading] = useState(true);
  const router = useRouter();

  useEffect(() => {
    if (isAuthenticated()) setUser(getUser());
    setLoading(false);
  }, []);

  const login = async (username: string, password: string) => {
    const result = await tenantAuthApi.login(username, password);
    setAuth(result.token, result.tenant);
    setUser(result.tenant);
    router.push("/dashboard");
  };

  const logout = () => {
    clearAuth();
    setUser(null);
    router.push("/login");
  };

  return (
    <AuthContext.Provider value={{ user, loading, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth must be used within AuthProvider");
  return context;
}
