"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { Bell, LogOut, User } from "lucide-react";
import { useAuth } from "@/context/AuthContext";
import { socialApi } from "@/lib/services";
import { NOTIFICATIONS_UPDATED_EVENT } from "@/lib/notificationEvents";
import { Button } from "@/components/ui/Button";

export function Header() {
  const { user, logout } = useAuth();
  const [unreadCount, setUnreadCount] = useState(0);

  useEffect(() => {
    const load = () => {
      socialApi.unreadCount().then(setUnreadCount).catch(() => setUnreadCount(0));
    };
    load();
    const interval = setInterval(load, 30000);
    window.addEventListener(NOTIFICATIONS_UPDATED_EVENT, load);
    return () => {
      clearInterval(interval);
      window.removeEventListener(NOTIFICATIONS_UPDATED_EVENT, load);
    };
  }, []);

  return (
    <header className="sticky top-0 z-20 flex h-16 items-center justify-between border-b border-slate-200 bg-white px-6">
      <p className="text-sm text-slate-500">
        Tenant: <span className="font-medium text-slate-700">{user?.tenantCode}</span>
        {" · "}
        ID: <span className="font-medium text-slate-700">{user?.userCode}</span>
      </p>
      <div className="flex items-center gap-3">
        <Link
          href="/notifications"
          className="relative rounded-full p-2 text-slate-500 transition hover:bg-rose-50 hover:text-rose-600"
          title="Notifications"
        >
          <Bell size={20} />
          {unreadCount > 0 && (
            <span className="absolute -right-0.5 -top-0.5 flex h-5 min-w-5 items-center justify-center rounded-full bg-rose-500 px-1 text-[10px] font-bold text-white">
              {unreadCount > 99 ? "99+" : unreadCount}
            </span>
          )}
        </Link>
        <div className="flex items-center gap-3 border-l border-slate-200 pl-3">
          <div className="flex h-9 w-9 items-center justify-center rounded-full bg-rose-100 text-rose-600">
            <User size={18} />
          </div>
          <div className="hidden sm:block">
            <p className="text-sm font-medium text-slate-800">{user?.firstName} {user?.lastName}</p>
            <p className="text-xs text-slate-500">{user?.email}</p>
          </div>
        </div>
        <Button variant="ghost" size="sm" onClick={logout}>
          <LogOut size={16} /> Logout
        </Button>
      </div>
    </header>
  );
}
