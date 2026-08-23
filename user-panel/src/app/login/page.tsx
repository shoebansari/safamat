"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { Heart, Lock, User } from "lucide-react";
import { useAuth } from "@/context/AuthContext";
import { isAuthenticated } from "@/lib/auth";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Alert, LoadingSpinner } from "@/components/ui/LoadingSpinner";

export default function LoginPage() {
  const { login, loading: authLoading } = useAuth();
  const router = useRouter();
  const [tenantCode, setTenantCode] = useState("008");
  const [userName, setUserName] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (!authLoading && isAuthenticated()) router.replace("/dashboard");
  }, [authLoading, router]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setLoading(true);
    try {
      await login(tenantCode, userName, password);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Login failed");
    } finally {
      setLoading(false);
    }
  };

  if (authLoading) return <LoadingSpinner fullPage />;

  return (
    <div className="flex min-h-screen">
      <div className="hidden w-1/2 bg-gradient-to-br from-rose-600 via-rose-700 to-slate-900 lg:flex lg:flex-col lg:justify-between lg:p-12">
        <div className="flex items-center gap-3 text-white">
          <Heart size={28} fill="white" />
          <div>
            <h1 className="text-2xl font-bold">Matrimonial</h1>
            <p className="text-rose-200">Find your perfect match</p>
          </div>
        </div>
        <h2 className="text-4xl font-bold text-white">Your journey to forever starts here</h2>
        <p className="text-sm text-rose-200">© 2026 Matrimonial SaaS</p>
      </div>
      <div className="flex w-full items-center justify-center p-4 sm:p-8 lg:w-1/2">
        <div className="w-full max-w-md">
          <h2 className="text-2xl font-bold text-slate-900">Sign in</h2>
          <p className="mt-1 text-slate-500">Enter your tenant code and credentials</p>
          {error && <div className="mt-4"><Alert message={error} /></div>}
          <form onSubmit={handleSubmit} className="mt-8 space-y-4">
            <Input label="Tenant Code" value={tenantCode} onChange={(e) => setTenantCode(e.target.value)} required />
            <div className="relative">
              <User size={18} className="absolute left-3 top-[38px] text-slate-400" />
              <Input label="Username" value={userName} onChange={(e) => setUserName(e.target.value)} className="pl-10" required />
            </div>
            <div className="relative">
              <Lock size={18} className="absolute left-3 top-[38px] text-slate-400" />
              <Input label="Password" type="password" value={password} onChange={(e) => setPassword(e.target.value)} className="pl-10" required />
            </div>
            <Button type="submit" className="w-full" disabled={loading}>
              {loading ? "Signing in..." : "Sign In"}
            </Button>
          </form>
          <p className="mt-6 text-center text-sm text-slate-500">
            No account? <Link href="/register" className="font-medium text-rose-600 hover:underline">Register</Link>
          </p>
          <p className="mt-2 text-center text-xs text-slate-400">Demo: tenant 008, rahul / User@123</p>
        </div>
      </div>
    </div>
  );
}
