"use client";

import Link from "next/link";
import { ClipboardList, UserCheck, Wallet } from "lucide-react";
import { useAuth } from "@/context/AuthContext";
import { Card } from "@/components/ui/Card";
import { PageHeader } from "@/components/ui/PageHeader";

const quickLinks = [
  {
    href: "/plans",
    title: "Member Plans",
    description: "Create and manage subscription plans for your members",
    icon: Wallet,
  },
  {
    href: "/member-plans",
    title: "Assign Plans",
    description: "Look up a user and update their plan and payment status",
    icon: ClipboardList,
  },
  {
    href: "/profile-approvals",
    title: "Profile Approvals",
    description: "Approve or reject member profiles and photos",
    icon: UserCheck,
  },
];

export default function DashboardPage() {
  const { user } = useAuth();

  return (
    <div>
      <PageHeader
        title="Dashboard"
        description={`Welcome back, ${user?.companyName || "Tenant"}`}
      />

      <div className="grid gap-4 md:grid-cols-3">
        {quickLinks.map(({ href, title, description, icon: Icon }) => (
          <Link key={href} href={href}>
            <Card className="h-full transition hover:border-rose-200 hover:shadow-md">
              <div className="flex items-start gap-4">
                <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-rose-100 text-rose-600">
                  <Icon size={20} />
                </div>
                <div>
                  <h3 className="font-semibold text-slate-900">{title}</h3>
                  <p className="mt-1 text-sm text-slate-500">{description}</p>
                </div>
              </div>
            </Card>
          </Link>
        ))}
      </div>
    </div>
  );
}
