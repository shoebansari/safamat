import type { DiscoverProfile } from "@/lib/types";
import { ClickablePhoto } from "@/components/ui/PhotoLightbox";

interface Props {
  percent: number;
}

export function ProfileProgress({ percent }: Props) {
  return (
    <div className="rounded-xl border border-rose-100 bg-gradient-to-r from-rose-50 to-white p-5">
      <div className="mb-2 flex items-center justify-between">
        <span className="text-sm font-medium text-slate-700">Profile completion</span>
        <span className="text-lg font-bold text-rose-600">{percent}%</span>
      </div>
      <div className="h-3 overflow-hidden rounded-full bg-slate-200">
        <div
          className="h-full rounded-full bg-gradient-to-r from-rose-500 to-rose-600 transition-all duration-500"
          style={{ width: `${percent}%` }}
        />
      </div>
      <p className="mt-2 text-xs text-slate-500">
        {percent < 100 ? "Complete all sections to improve your visibility." : "Your profile is fully complete!"}
      </p>
    </div>
  );
}

interface MemberCardProps {
  profile: DiscoverProfile;
}

export function MemberCard({ profile }: MemberCardProps) {
  return (
    <div className="rounded-xl border border-slate-100 bg-white p-4 shadow-sm transition hover:border-rose-200 hover:shadow-md">
      <ClickablePhoto
        src={profile.primaryPhotoUrl}
        photos={profile.photoUrls}
        alt={profile.firstName}
        className="mb-3 h-36 w-full rounded-lg object-cover"
        fallbackClassName="mb-3 h-36 w-full rounded-lg"
      />
      <h3 className="font-semibold text-slate-900">{profile.firstName} {profile.lastName}</h3>
      <p className="text-sm text-slate-500">
        {profile.age ? `${profile.age} yrs` : "—"} · {profile.city || "—"} · {profile.gender || "—"}
      </p>
      {profile.matchPercentage > 0 && (
        <p className="mt-1 text-xs font-medium text-rose-600">{profile.matchPercentage}% match</p>
      )}
    </div>
  );
}
