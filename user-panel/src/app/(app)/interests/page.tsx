"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { socialApi } from "@/lib/services";
import type { Interest } from "@/lib/types";
import { ClickablePhoto } from "@/components/ui/PhotoLightbox";
import { Button } from "@/components/ui/Button";
import { Card } from "@/components/ui/Card";
import { PageHeader } from "@/components/ui/PageHeader";
import { EmptyState, LoadingSpinner } from "@/components/ui/LoadingSpinner";

export default function InterestsPage() {
  const [tab, setTab] = useState<"received" | "sent">("received");
  const [items, setItems] = useState<Interest[]>([]);
  const [loading, setLoading] = useState(true);

  const load = () => {
    setLoading(true);
    socialApi.interests(tab).then(setItems).finally(() => setLoading(false));
  };

  useEffect(() => { load(); }, [tab]);

  const respond = async (id: string, status: string) => {
    await socialApi.respondInterest(id, status);
    load();
  };

  return (
    <div>
      <PageHeader title="Interests" description="Manage interest requests" />
      <div className="mb-4 flex gap-2">
        {(["received", "sent"] as const).map((t) => (
          <button key={t} onClick={() => setTab(t)}
            className={`rounded-lg px-4 py-2 text-sm font-medium capitalize ${tab === t ? "bg-rose-600 text-white" : "bg-white ring-1 ring-slate-200"}`}>
            {t}
          </button>
        ))}
      </div>
      <Card>
        {loading ? <LoadingSpinner /> : items.length === 0 ? <EmptyState message="No interests yet" /> : (
          <div className="space-y-3">
            {items.map((i) => {
              const memberId = tab === "received" ? i.senderUserId : i.receiverUserId;
              const memberName = tab === "received" ? i.senderName : i.receiverName;
              const photoUrl = tab === "received" ? i.senderPhotoUrl : i.receiverPhotoUrl;
              return (
                <div key={i.interestId} className="flex items-center justify-between rounded-lg border border-slate-100 p-4">
                  <div className="flex min-w-0 flex-1 items-center gap-3">
                    <div className="h-14 w-14 shrink-0 overflow-hidden rounded-full">
                      <ClickablePhoto
                        src={photoUrl}
                        alt={memberName}
                        className="h-14 w-14 rounded-full object-cover"
                        fallbackClassName="h-14 w-14 rounded-full text-sm"
                      />
                    </div>
                    <Link href={`/members/${memberId}`} className="min-w-0 hover:opacity-90">
                      <p className="font-medium text-slate-900">{memberName}</p>
                      <p className="text-sm text-slate-500">{i.status} · {new Date(i.sentOn).toLocaleDateString()}</p>
                      <p className="text-xs text-rose-600">View profile →</p>
                    </Link>
                  </div>
                  {tab === "received" && i.status === "Pending" && (
                    <div className="flex shrink-0 gap-2">
                      <Button size="sm" onClick={() => respond(i.interestId, "Accepted")}>Accept</Button>
                      <Button size="sm" variant="ghost" onClick={() => respond(i.interestId, "Rejected")}>Reject</Button>
                    </div>
                  )}
                </div>
              );
            })}
          </div>
        )}
      </Card>
    </div>
  );
}
