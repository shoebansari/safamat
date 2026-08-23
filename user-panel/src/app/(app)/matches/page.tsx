"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { socialApi } from "@/lib/services";
import type { Match } from "@/lib/types";
import { ClickablePhoto } from "@/components/ui/PhotoLightbox";
import { Button } from "@/components/ui/Button";
import { Card } from "@/components/ui/Card";
import { PageHeader } from "@/components/ui/PageHeader";
import { EmptyState, LoadingSpinner } from "@/components/ui/LoadingSpinner";

export default function MatchesPage() {
  const [items, setItems] = useState<Match[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    socialApi.matches().then(setItems).finally(() => setLoading(false));
  }, []);

  return (
    <div>
      <PageHeader title="Matches" description="People who accepted your interest" />
      <Card>
        {loading ? <LoadingSpinner /> : items.length === 0 ? <EmptyState message="No matches yet. Send interests from Discover!" /> : (
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {items.map((m) => (
              <div key={m.matchId} className="rounded-xl border border-slate-100 p-4 text-center">
                <div className="mx-auto mb-3 h-24 w-24 overflow-hidden rounded-full">
                  <ClickablePhoto
                    src={m.photoUrl}
                    alt={m.name}
                    className="h-24 w-24 rounded-full object-cover"
                    fallbackClassName="h-24 w-24 rounded-full"
                  />
                </div>
                <h3 className="font-semibold">{m.name}</h3>
                <p className="text-sm text-rose-600">{m.matchPercentage}% match</p>
                <Link href={`/messages?user=${m.userId}`}>
                  <Button size="sm" className="mt-3 w-full">Message</Button>
                </Link>
              </div>
            ))}
          </div>
        )}
      </Card>
    </div>
  );
}
