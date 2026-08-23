"use client";

import { createContext, useContext, useEffect, useState, type ReactNode } from "react";
import { useRouter } from "next/navigation";
import { userAuthApi } from "@/lib/services";
import { clearAuth, getUser, isAuthenticated, setAuth } from "@/lib/auth";
import type { UserSession } from "@/lib/types";

interface AuthContextType {
  user: UserSession | null;
  loading: boolean;
  login: (tenantCode: string, userName: string, password: string) => Promise<void>;
  register: (data: {
    tenantCode: string;
    memberPlanId: string;
    userName: string;
    firstName: string;
    lastName: string;
    email: string;
    phone?: string;
    password: string;
  }) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<UserSession | null>(null);
  const [loading, setLoading] = useState(true);
  const router = useRouter();

  useEffect(() => {
    if (isAuthenticated()) setUser(getUser());
    setLoading(false);
  }, []);

  const login = async (tenantCode: string, userName: string, password: string) => {
    const result = await userAuthApi.login(tenantCode, userName, password);
    setAuth(result.token, result.user);
    setUser(result.user);
    router.push("/dashboard");
  };

  const register = async (data: Parameters<AuthContextType["register"]>[0]) => {
    const result = await userAuthApi.register(data);
    setAuth(result.token, result.user);
    setUser(result.user);
    router.push("/profile");
  };

  const logout = () => {
    clearAuth();
    setUser(null);
    router.push("/login");
  };

  return (
    <AuthContext.Provider value={{ user, loading, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within AuthProvider");
  return ctx;
}
