"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import {
  Building2,
  CreditCard,
  Heart,
  LayoutDashboard,
  Mail,
  Receipt,
  Settings,
  Shield,
  Users,
  Wallet,
} from "lucide-react";

const navItems = [
  { href: "/dashboard", label: "Dashboard", icon: LayoutDashboard },
  { href: "/admin-users", label: "Admin Users", icon: Shield },
  { href: "/tenants", label: "Tenants", icon: Building2 },
  { href: "/subscription-plans", label: "Subscription Plans", icon: Wallet },
  { href: "/tenant-subscriptions", label: "Subscriptions", icon: CreditCard },
  { href: "/payments", label: "Payments", icon: Receipt },
  { href: "/email-templates", label: "Email Templates", icon: Mail },
  { href: "/system-settings", label: "System Settings", icon: Settings },
];

export function Sidebar() {
  const pathname = usePathname();

  return (
    <aside className="fixed inset-y-0 left-0 z-30 flex w-64 flex-col bg-slate-900 text-white">
      <div className="flex h-16 items-center gap-2 border-b border-slate-800 px-6">
        <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-rose-600">
          <Heart size={20} className="text-white" fill="white" />
        </div>
        <div>
          <p className="text-sm font-bold">Matrimonial</p>
          <p className="text-xs text-slate-400">Admin Panel</p>
        </div>
      </div>

      <nav className="flex-1 space-y-1 overflow-y-auto p-4">
        {navItems.map(({ href, label, icon: Icon }) => {
          const active = pathname === href || pathname.startsWith(`${href}/`);
          return (
            <Link
              key={href}
              href={href}
              className={`flex items-center gap-3 rounded-lg px-3 py-2.5 text-sm font-medium transition ${
                active
                  ? "bg-rose-600 text-white"
                  : "text-slate-300 hover:bg-slate-800 hover:text-white"
              }`}
            >
              <Icon size={18} />
              {label}
            </Link>
          );
        })}
      </nav>

      <div className="border-t border-slate-800 p-4">
        <div className="flex items-center gap-2 text-xs text-slate-500">
          <Users size={14} />
          <span>Matrimonial SaaS v1.0</span>
        </div>
      </div>
    </aside>
  );
}
