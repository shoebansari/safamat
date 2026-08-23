"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { socialApi } from "@/lib/services";
import type { DiscoverProfile } from "@/lib/types";
import { ClickablePhoto } from "@/components/ui/PhotoLightbox";
import { Button } from "@/components/ui/Button";
import { Card } from "@/components/ui/Card";
import { PageHeader } from "@/components/ui/PageHeader";
import { EmptyState, LoadingSpinner } from "@/components/ui/LoadingSpinner";

export default function FavoritesPage() {
  const [items, setItems] = useState<DiscoverProfile[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    socialApi.favorites().then(setItems).finally(() => setLoading(false));
  }, []);

  return (
    <div>
      <PageHeader title="Favorites" description="Your shortlisted profiles" />
      <Card>
        {loading ? <LoadingSpinner /> : items.length === 0 ? <EmptyState message="No favorites yet" /> : (
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {items.map((p) => (
              <div key={p.userId} className="overflow-hidden rounded-xl border border-slate-100">
                <ClickablePhoto
                  src={p.primaryPhotoUrl}
                  photos={p.photoUrls}
                  alt={p.firstName}
                  className="h-40 w-full object-cover"
                  fallbackClassName="h-40 w-full"
                />
                <div className="p-4">
                  <h3 className="font-semibold">{p.firstName} {p.lastName}</h3>
                  <p className="text-sm text-slate-500">{p.city} · {p.age} yrs</p>
                  <Link href={`/members/${p.userId}`}><Button size="sm" className="mt-2 w-full">View Profile</Button></Link>
                </div>
              </div>
            ))}
          </div>
        )}
      </Card>
    </div>
  );
}
