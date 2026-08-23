"use client";

import { useEffect, useState } from "react";
import { usePathname, useRouter } from "next/navigation";
import { useAuth } from "@/context/AuthContext";
import { isAuthenticated } from "@/lib/auth";
import { LoadingSpinner } from "@/components/ui/LoadingSpinner";
import { Header } from "./Header";
import { MobileBottomNav } from "./MobileBottomNav";
import { Sidebar } from "./Sidebar";

export function TenantLayout({ children }: { children: React.ReactNode }) {
  const { loading } = useAuth();
  const router = useRouter();
  const pathname = usePathname();
  const [sidebarOpen, setSidebarOpen] = useState(false);

  useEffect(() => {
    if (!loading && !isAuthenticated()) {
      router.replace("/login");
    }
  }, [loading, router]);

  useEffect(() => {
    setSidebarOpen(false);
  }, [pathname]);

  useEffect(() => {
    document.body.style.overflow = sidebarOpen ? "hidden" : "";
    return () => {
      document.body.style.overflow = "";
    };
  }, [sidebarOpen]);

  if (loading) return <LoadingSpinner fullPage />;
  if (!isAuthenticated()) return <LoadingSpinner fullPage />;

  return (
    <div className="min-h-screen bg-slate-50">
      <Sidebar open={sidebarOpen} onClose={() => setSidebarOpen(false)} />
      <div className="lg:pl-64">
        <Header onMenuClick={() => setSidebarOpen(true)} />
        <main className="p-4 pb-24 lg:p-6 lg:pb-6">{children}</main>
      </div>
      <MobileBottomNav />
    </div>
  );
}
