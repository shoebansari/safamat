"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import {
  Building2,
  CreditCard,
  Mail,
  Receipt,
  Shield,
  Users,
  Wallet,
} from "lucide-react";
import { Card } from "@/components/ui/Card";
import { PageHeader } from "@/components/ui/PageHeader";
import { LoadingSpinner } from "@/components/ui/LoadingSpinner";
import {
  adminUsersApi,
  emailTemplatesApi,
  paymentsApi,
  subscriptionPlansApi,
  tenantSubscriptionsApi,
  tenantsApi,
} from "@/lib/services";

interface StatCard {
  label: string;
  value: number;
  icon: React.ElementType;
  href: string;
  color: string;
}

export default function DashboardPage() {
  const [stats, setStats] = useState<StatCard[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function loadStats() {
      try {
        const [users, tenants, plans, subs, payments, templates] = await Promise.all([
          adminUsersApi.list(1, 1),
          tenantsApi.list(1, 1),
          subscriptionPlansApi.list(1, 1),
          tenantSubscriptionsApi.list(1, 1),
          paymentsApi.list(1, 1),
          emailTemplatesApi.list(1, 1),
        ]);

        setStats([
          { label: "Admin Users", value: users.totalCount, icon: Shield, href: "/admin-users", color: "bg-blue-500" },
          { label: "Tenants", value: tenants.totalCount, icon: Building2, href: "/tenants", color: "bg-rose-500" },
          { label: "Subscription Plans", value: plans.totalCount, icon: Wallet, href: "/subscription-plans", color: "bg-purple-500" },
          { label: "Active Subscriptions", value: subs.totalCount, icon: CreditCard, href: "/tenant-subscriptions", color: "bg-green-500" },
          { label: "Payments", value: payments.totalCount, icon: Receipt, href: "/payments", color: "bg-amber-500" },
          { label: "Email Templates", value: templates.totalCount, icon: Mail, href: "/email-templates", color: "bg-indigo-500" },
        ]);
      } catch {
        setStats([]);
      } finally {
        setLoading(false);
      }
    }
    loadStats();
  }, []);

  if (loading) return <LoadingSpinner />;

  return (
    <div>
      <PageHeader
        title="Dashboard"
        description="Overview of your matrimonial platform"
      />

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {stats.map(({ label, value, icon: Icon, href, color }) => (
          <Link key={label} href={href}>
            <Card className="transition hover:shadow-md">
              <div className="flex items-center gap-4">
                <div className={`flex h-12 w-12 items-center justify-center rounded-xl ${color} text-white`}>
                  <Icon size={24} />
                </div>
                <div>
                  <p className="text-sm text-slate-500">{label}</p>
                  <p className="text-2xl font-bold text-slate-900">{value}</p>
                </div>
              </div>
            </Card>
          </Link>
        ))}
      </div>

      <div className="mt-8 grid gap-6 lg:grid-cols-2">
        <Card title="Quick Actions">
          <div className="grid gap-3 sm:grid-cols-2">
            {[
              { label: "Add Tenant", href: "/tenants" },
              { label: "Create Plan", href: "/subscription-plans" },
              { label: "New Subscription", href: "/tenant-subscriptions" },
              { label: "Add Admin User", href: "/admin-users" },
            ].map(({ label, href }) => (
              <Link
                key={label}
                href={href}
                className="rounded-lg border border-slate-200 px-4 py-3 text-sm font-medium text-slate-700 transition hover:border-rose-300 hover:bg-rose-50 hover:text-rose-700"
              >
                {label}
              </Link>
            ))}
          </div>
        </Card>

        <Card title="Platform Info">
          <div className="space-y-3 text-sm">
            <div className="flex justify-between">
              <span className="text-slate-500">Platform</span>
              <span className="font-medium">Matrimonial SaaS</span>
            </div>
            <div className="flex justify-between">
              <span className="text-slate-500">API</span>
              <span className="font-medium">ASP.NET Core 8</span>
            </div>
            <div className="flex justify-between">
              <span className="text-slate-500">Database</span>
              <span className="font-medium">PostgreSQL</span>
            </div>
            <div className="flex items-center justify-between">
              <span className="text-slate-500">Status</span>
              <span className="inline-flex items-center gap-1.5 rounded-full bg-green-100 px-2.5 py-0.5 text-xs font-medium text-green-700">
                <span className="h-1.5 w-1.5 rounded-full bg-green-500" />
                Online
              </span>
            </div>
          </div>
        </Card>
      </div>
    </div>
  );
}
