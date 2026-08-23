"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { Bell, Compass, Heart, MessageCircle, Star, User } from "lucide-react";
import { useAuth } from "@/context/AuthContext";
import { profileApi } from "@/lib/services";
import { calculateProfileCompletion } from "@/lib/profileCompletion";
import { Card } from "@/components/ui/Card";
import { PageHeader } from "@/components/ui/PageHeader";
import { LoadingSpinner } from "@/components/ui/LoadingSpinner";

const links = [
  { href: "/discover", label: "Discover Profiles", icon: Compass, desc: "Browse approved profiles" },
  { href: "/profile", label: "Complete Profile", icon: User, desc: "Add photos and details" },
  { href: "/preferences", label: "Partner Preferences", icon: Heart, desc: "Set your match criteria" },
  { href: "/interests", label: "Interests", icon: Heart, desc: "Sent and received interests" },
  { href: "/matches", label: "Matches", icon: Star, desc: "View your matches" },
  { href: "/messages", label: "Messages", icon: MessageCircle, desc: "Chat with matches" },
  { href: "/notifications", label: "Notifications", icon: Bell, desc: "Stay updated" },
];

export default function DashboardPage() {
  const { user } = useAuth();
  const [completion, setCompletion] = useState<number | null>(null);

  useEffect(() => {
    profileApi.getMe()
      .then((p) => setCompletion(calculateProfileCompletion(p)))
      .catch(() => setCompletion(null));
  }, []);

  return (
    <div>
      <PageHeader title="Welcome" description={`Hello, ${user?.firstName}! Your ID is ${user?.userCode}`} />
      {completion !== null && completion < 100 && (
        <Card className="mb-6 border-amber-200 bg-amber-50">
          <p className="text-sm text-amber-800">
            Complete your profile to get better matches ({completion}% done).{" "}
            <Link href="/profile" className="font-medium underline">Go to profile →</Link>
          </p>
        </Card>
      )}
      {completion === null && (
        <div className="mb-6"><LoadingSpinner /></div>
      )}
      <div className="grid gap-4 md:grid-cols-3">
        {links.map(({ href, label, icon: Icon, desc }) => (
          <Link key={href} href={href}>
            <Card className="h-full transition hover:border-rose-200 hover:shadow-md">
              <div className="flex items-start gap-3">
                <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-rose-100 text-rose-600">
                  <Icon size={20} />
                </div>
                <div>
                  <h3 className="font-semibold text-slate-900">{label}</h3>
                  <p className="mt-1 text-sm text-slate-500">{desc}</p>
                </div>
              </div>
            </Card>
          </Link>
        ))}
      </div>
    </div>
  );
}
