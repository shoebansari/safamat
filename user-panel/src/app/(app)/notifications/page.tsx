"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { socialApi } from "@/lib/services";
import { emitNotificationsUpdated } from "@/lib/notificationEvents";
import type { Notification } from "@/lib/types";
import { Button } from "@/components/ui/Button";
import { Card } from "@/components/ui/Card";
import { PageHeader } from "@/components/ui/PageHeader";
import { EmptyState, LoadingSpinner } from "@/components/ui/LoadingSpinner";

export default function NotificationsPage() {
  const router = useRouter();
  const [items, setItems] = useState<Notification[]>([]);
  const [loading, setLoading] = useState(true);

  const load = () => {
    setLoading(true);
    socialApi.notifications().then(setItems).finally(() => setLoading(false));
  };

  useEffect(() => { load(); }, []);

  const markAllRead = async () => {
    await socialApi.markAllRead();
    emitNotificationsUpdated();
    load();
  };

  const handleClick = async (n: Notification) => {
    if (!n.isRead) {
      await socialApi.markRead(n.notificationId);
      emitNotificationsUpdated();
    }
    if (n.title === "New Message" && n.relatedUserId) {
      router.push(`/messages?user=${n.relatedUserId}`);
      return;
    }
    if (n.title === "New Interest" && n.relatedUserId) {
      router.push(`/members/${n.relatedUserId}`);
      return;
    }
    load();
  };

  return (
    <div>
      <PageHeader
        title="Notifications"
        description="Your recent activity"
        action={<Button variant="secondary" size="sm" onClick={markAllRead}>Mark all read</Button>}
      />
      <Card>
        {loading ? <LoadingSpinner /> : items.length === 0 ? <EmptyState message="No notifications" /> : (
          <div className="space-y-2">
            {items.map((n) => (
              <button
                key={n.notificationId}
                type="button"
                onClick={() => handleClick(n)}
                className={`w-full rounded-lg border p-4 text-left transition hover:border-rose-300 ${
                  n.isRead ? "border-slate-100" : "border-rose-200 bg-rose-50"
                }`}
              >
                <p className="font-medium">{n.title}</p>
                <p className="text-sm text-slate-600">{n.message}</p>
                <p className="mt-1 text-xs text-slate-400">{new Date(n.createdOn).toLocaleString()}</p>
                {(n.title === "New Message" || n.title === "New Interest") && n.relatedUserId && (
                  <p className="mt-1 text-xs font-medium text-rose-600">Tap to open →</p>
                )}
              </button>
            ))}
          </div>
        )}
      </Card>
    </div>
  );
}
