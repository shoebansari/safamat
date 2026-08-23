import type { UserProfile } from "./types";

function filled(value: unknown): boolean {
  if (value === null || value === undefined) return false;
  if (typeof value === "string") return value.trim().length > 0;
  if (typeof value === "number") return value > 0;
  return true;
}

export function calculateProfileCompletion(profile: UserProfile): number {
  const sections = [
    // Basic (30%)
    [
      filled(profile.gender),
      filled(profile.dateOfBirth),
      filled(profile.height),
      filled(profile.religion),
      filled(profile.aboutMe),
    ],
    // Education (10%)
    [filled(profile.education?.qualification), filled(profile.education?.college)],
    // Occupation (15%)
    [filled(profile.occupation?.occupation), filled(profile.occupation?.workLocation)],
    // Family (10%)
    [filled(profile.family?.fatherName), filled(profile.family?.motherName)],
    // Lifestyle (10%)
    [filled(profile.lifestyle?.diet)],
    // Location (10%)
    [filled(profile.location?.city), filled(profile.location?.state)],
    // Photos (15%)
    [profile.photos.length > 0],
  ];

  const weights = [30, 10, 15, 10, 10, 10, 15];
  let total = 0;

  sections.forEach((fields, i) => {
    const done = fields.filter(Boolean).length;
    const sectionPct = (done / fields.length) * weights[i];
    total += sectionPct;
  });

  return Math.min(100, Math.round(total));
}
