"use client";

import { useEffect, useState } from "react";
import { useParams } from "next/navigation";
import Link from "next/link";
import { Heart, Star } from "lucide-react";
import { profileApi, socialApi } from "@/lib/services";
import { resolvePhotoUrl } from "@/lib/media";
import { useToast } from "@/context/ToastContext";
import type { UserProfile } from "@/lib/types";
import { ClickablePhoto, PhotoLightbox } from "@/components/ui/PhotoLightbox";
import { Button } from "@/components/ui/Button";
import { Card } from "@/components/ui/Card";
import { PageHeader } from "@/components/ui/PageHeader";
import { Alert, LoadingSpinner } from "@/components/ui/LoadingSpinner";

export default function MemberProfilePage() {
  const { userId } = useParams<{ userId: string }>();
  const { showToast } = useToast();
  const [profile, setProfile] = useState<UserProfile | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [lightboxIndex, setLightboxIndex] = useState<number | null>(null);

  useEffect(() => {
    if (!userId) return;
    profileApi
      .getPublic(userId)
      .then(setProfile)
      .catch((err) => setError(err instanceof Error ? err.message : "Profile not found"))
      .finally(() => setLoading(false));
  }, [userId]);

  if (loading) return <LoadingSpinner fullPage />;
  if (error || !profile) return <Alert message={error || "Profile not available"} />;

  const approvedPhotos = profile.photos
    .filter((p) => p.isApproved)
    .sort((a, b) => (a.isPrimary ? 0 : 1) - (b.isPrimary ? 0 : 1))
    .map((p) => p.photoUrl);
  const primaryPhoto = approvedPhotos[0] || profile.primaryPhotoUrl;

  return (
    <div>
      <PageHeader
        title={`${profile.firstName} ${profile.lastName}`}
        description={`${profile.userCode} · ${profile.gender || ""} · ${profile.location?.city || ""}`}
        action={
          <Link href="/discover">
            <Button variant="secondary" size="sm">Back to Discover</Button>
          </Link>
        }
      />
      <Card>
        <div className="flex flex-col gap-6 lg:flex-row">
          <div className="shrink-0 space-y-3">
            <ClickablePhoto
              src={primaryPhoto}
              photos={approvedPhotos}
              alt={profile.firstName}
              className="h-64 w-64 rounded-xl object-cover shadow-md"
              fallbackClassName="h-64 w-64 rounded-xl"
            />
            {approvedPhotos.length > 1 && (
              <div className="flex gap-2 overflow-x-auto pb-1">
                {approvedPhotos.map((url, i) => (
                  <button
                    key={url + i}
                    type="button"
                    onClick={() => setLightboxIndex(i)}
                    className="shrink-0 overflow-hidden rounded-lg ring-2 ring-transparent hover:ring-rose-300"
                  >
                    {/* eslint-disable-next-line @next/next/no-img-element */}
                    <img src={resolvePhotoUrl(url)} alt="" className="h-16 w-16 object-cover" />
                  </button>
                ))}
              </div>
            )}
          </div>
          <div className="flex-1 space-y-2 text-sm">
            <Row label="Age" value={profile.age?.toString()} />
            <Row label="Height" value={profile.height ? `${profile.height} cm` : undefined} />
            <Row label="Religion" value={profile.religion} />
            <Row label="Caste" value={profile.caste} />
            <Row label="Mother tongue" value={profile.motherTongue} />
            <Row label="Marital status" value={profile.maritalStatus} />
            <Row label="Education" value={profile.education?.qualification} />
            <Row label="Occupation" value={profile.occupation?.occupation} />
            <Row label="City" value={profile.location?.city} />
            <Row label="State" value={profile.location?.state} />
            {profile.aboutMe && (
              <p className="mt-4 rounded-lg bg-slate-50 p-4 text-slate-700">{profile.aboutMe}</p>
            )}
            <div className="flex gap-2 pt-4">
              <Button size="sm" onClick={async () => {
                try {
                  await socialApi.sendInterest(profile.userId);
                  showToast("Interest sent successfully!");
                } catch (err) {
                  showToast(err instanceof Error ? err.message : "Failed", "error");
                }
              }}>
                <Heart size={14} /> Send interest
              </Button>
              <Button variant="ghost" size="sm" onClick={async () => {
                try {
                  await socialApi.toggleFavorite(profile.userId);
                  showToast("Added to favorites");
                } catch (err) {
                  showToast(err instanceof Error ? err.message : "Failed", "error");
                }
              }}>
                <Star size={14} /> Favorite
              </Button>
            </div>
          </div>
        </div>
      </Card>

      {lightboxIndex !== null && (
        <PhotoLightbox
          photos={approvedPhotos}
          initialIndex={lightboxIndex}
          alt={profile.firstName}
          onClose={() => setLightboxIndex(null)}
        />
      )}
    </div>
  );
}

function Row({ label, value }: { label: string; value?: string | null }) {
  return (
    <p>
      <span className="text-slate-500">{label}:</span>{" "}
      <span className="font-medium text-slate-800">{value || "—"}</span>
    </p>
  );
}
