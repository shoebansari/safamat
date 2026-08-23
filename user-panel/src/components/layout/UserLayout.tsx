"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/context/AuthContext";
import { isAuthenticated } from "@/lib/auth";
import { LoadingSpinner } from "@/components/ui/LoadingSpinner";
import { Header } from "./Header";
import { Sidebar } from "./Sidebar";

export function UserLayout({ children }: { children: React.ReactNode }) {
  const { loading } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (!loading && !isAuthenticated()) router.replace("/login");
  }, [loading, router]);

  if (loading || !isAuthenticated()) return <LoadingSpinner fullPage />;

  return (
    <div className="min-h-screen bg-slate-50">
      <Sidebar />
      <div className="pl-64">
        <Header />
        <main className="p-6">{children}</main>
      </div>
    </div>
  );
}
