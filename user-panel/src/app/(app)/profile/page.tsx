"use client";

import { useCallback, useEffect, useRef, useState } from "react";
import { Trash2, Upload } from "lucide-react";
import { profileApi } from "@/lib/services";
import { calculateProfileCompletion } from "@/lib/profileCompletion";
import { PHOTO_MAX_COUNT, PHOTO_MAX_SIZE_BYTES, PHOTO_MAX_SIZE_MB, resolvePhotoUrl } from "@/lib/media";
import { useToast } from "@/context/ToastContext";
import type { UserProfile } from "@/lib/types";
import { ProfileProgress } from "@/components/profile/ProfileWidgets";
import { PhotoLightbox } from "@/components/ui/PhotoLightbox";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Textarea } from "@/components/ui/Textarea";
import { Select } from "@/components/ui/Select";
import { Card } from "@/components/ui/Card";
import { PageHeader } from "@/components/ui/PageHeader";
import { Alert, LoadingSpinner } from "@/components/ui/LoadingSpinner";

const TABS = ["Basic", "Education", "Occupation", "Family", "Lifestyle", "Location", "Photos"] as const;

export default function ProfilePage() {
  const { showToast } = useToast();
  const fileRef = useRef<HTMLInputElement>(null);
  const [tab, setTab] = useState<(typeof TABS)[number]>("Basic");
  const [profile, setProfile] = useState<UserProfile | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [uploading, setUploading] = useState(false);
  const [error, setError] = useState("");
  const [lightboxIndex, setLightboxIndex] = useState<number | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const me = await profileApi.getMe();
      setProfile(me);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load profile");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { load(); }, [load]);

  const save = async (fn: () => Promise<UserProfile>, successMsg: string) => {
    setSaving(true);
    try {
      const updated = await fn();
      setProfile(updated);
      showToast(successMsg);
    } catch (err) {
      showToast(err instanceof Error ? err.message : "Save failed", "error");
    } finally {
      setSaving(false);
    }
  };

  const handleFileUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file || !profile) return;
    if (profile.photos.length >= PHOTO_MAX_COUNT) {
      showToast(`Maximum ${PHOTO_MAX_COUNT} photos allowed`, "error");
      return;
    }
    if (file.size > PHOTO_MAX_SIZE_BYTES) {
      showToast("Could not upload: image must not be more than 2 MB.", "error");
      return;
    }
    setUploading(true);
    try {
      await profileApi.uploadPhoto(file, profile.photos.length === 0);
      await load();
      showToast("Photo uploaded successfully!");
    } catch (err) {
      showToast(err instanceof Error ? err.message : "Upload failed", "error");
    } finally {
      setUploading(false);
      if (fileRef.current) fileRef.current.value = "";
    }
  };

  const deletePhoto = async (photoId: string) => {
    try {
      await profileApi.deletePhoto(photoId);
      await load();
      showToast("Photo removed");
    } catch (err) {
      showToast(err instanceof Error ? err.message : "Failed", "error");
    }
  };

  if (loading) return <LoadingSpinner fullPage />;
  if (!profile) return <Alert message={error || "Profile not found"} />;

  const completion = calculateProfileCompletion(profile);

  return (
    <div>
      <PageHeader
        title="My Profile"
        description={`Status: ${profile.profileStatus} · ID: ${profile.userCode}`}
      />

      <div className="mb-6">
        <ProfileProgress percent={completion} />
      </div>

      <div className="mb-4 flex flex-wrap gap-2">
        {TABS.map((t) => (
          <button
            key={t}
            onClick={() => setTab(t)}
            className={`rounded-lg px-3 py-1.5 text-sm font-medium ${
              tab === t ? "bg-rose-600 text-white" : "bg-white text-slate-600 ring-1 ring-slate-200"
            }`}
          >
            {t}
          </button>
        ))}
      </div>

      <Card>
        {tab === "Basic" && (
          <div className="grid gap-4 sm:grid-cols-2">
            <Select
              label="Gender"
              options={[
                { value: "", label: "Select" },
                { value: "Male", label: "Male" },
                { value: "Female", label: "Female" },
              ]}
              value={profile.gender || ""}
              onChange={(e) => setProfile({ ...profile, gender: e.target.value })}
            />
            <Input
              label="Date of Birth"
              type="date"
              value={profile.dateOfBirth?.slice(0, 10) || ""}
              onChange={(e) => setProfile({ ...profile, dateOfBirth: e.target.value })}
            />
            <Input label="Height (cm)" type="number" value={profile.height || ""} onChange={(e) => setProfile({ ...profile, height: Number(e.target.value) })} />
            <Input label="Weight (kg)" type="number" value={profile.weight || ""} onChange={(e) => setProfile({ ...profile, weight: Number(e.target.value) })} />
            <Input label="Religion" value={profile.religion || ""} onChange={(e) => setProfile({ ...profile, religion: e.target.value })} />
            <Input label="Caste" value={profile.caste || ""} onChange={(e) => setProfile({ ...profile, caste: e.target.value })} />
            <Input label="Marital Status" value={profile.maritalStatus || ""} onChange={(e) => setProfile({ ...profile, maritalStatus: e.target.value })} />
            <Input label="Mother Tongue" value={profile.motherTongue || ""} onChange={(e) => setProfile({ ...profile, motherTongue: e.target.value })} />
            <div className="sm:col-span-2">
              <Textarea label="About Me" value={profile.aboutMe || ""} onChange={(e) => setProfile({ ...profile, aboutMe: e.target.value })} rows={4} />
            </div>
            <Button
              disabled={saving}
              onClick={() =>
                save(
                  () =>
                    profileApi.saveBasic({
                      gender: profile.gender,
                      dateOfBirth: profile.dateOfBirth,
                      height: profile.height,
                      weight: profile.weight,
                      maritalStatus: profile.maritalStatus,
                      religion: profile.religion,
                      caste: profile.caste,
                      subCaste: profile.subCaste,
                      motherTongue: profile.motherTongue,
                      bloodGroup: profile.bloodGroup,
                      aboutMe: profile.aboutMe,
                    }),
                  "Basic profile saved successfully!"
                )
              }
            >
              Save Basic Info
            </Button>
          </div>
        )}

        {tab === "Education" && (
          <div className="grid gap-4 sm:grid-cols-2">
            <Input label="Qualification" value={profile.education?.qualification || ""} onChange={(e) => setProfile({ ...profile, education: { educationId: profile.education?.educationId || "", qualification: e.target.value } })} />
            <Input label="College" value={profile.education?.college || ""} onChange={(e) => setProfile({ ...profile, education: { ...profile.education!, educationId: profile.education?.educationId || "", college: e.target.value } })} />
            <Input label="University" value={profile.education?.university || ""} onChange={(e) => setProfile({ ...profile, education: { ...profile.education!, educationId: profile.education?.educationId || "", university: e.target.value } })} />
            <Input label="Passing Year" type="number" value={profile.education?.passingYear || ""} onChange={(e) => setProfile({ ...profile, education: { ...profile.education!, educationId: profile.education?.educationId || "", passingYear: Number(e.target.value) } })} />
            <Button disabled={saving} onClick={() => save(() => profileApi.saveEducation((profile.education || {}) as Record<string, unknown>), "Education details saved!")}>
              Save Education
            </Button>
          </div>
        )}

        {tab === "Occupation" && (
          <div className="grid gap-4 sm:grid-cols-2">
            <Input label="Occupation" value={profile.occupation?.occupation || ""} onChange={(e) => setProfile({ ...profile, occupation: { ...profile.occupation!, occupationId: profile.occupation?.occupationId || "", occupation: e.target.value } })} />
            <Input label="Company" value={profile.occupation?.companyName || ""} onChange={(e) => setProfile({ ...profile, occupation: { ...profile.occupation!, occupationId: profile.occupation?.occupationId || "", companyName: e.target.value } })} />
            <Input label="Designation" value={profile.occupation?.designation || ""} onChange={(e) => setProfile({ ...profile, occupation: { ...profile.occupation!, occupationId: profile.occupation?.occupationId || "", designation: e.target.value } })} />
            <Input label="Annual Income" type="number" value={profile.occupation?.annualIncome || ""} onChange={(e) => setProfile({ ...profile, occupation: { ...profile.occupation!, occupationId: profile.occupation?.occupationId || "", annualIncome: Number(e.target.value) } })} />
            <Input label="Work Location" value={profile.occupation?.workLocation || ""} onChange={(e) => setProfile({ ...profile, occupation: { ...profile.occupation!, occupationId: profile.occupation?.occupationId || "", workLocation: e.target.value } })} />
            <Button disabled={saving} onClick={() => save(() => profileApi.saveOccupation((profile.occupation || {}) as Record<string, unknown>), "Occupation details saved!")}>
              Save Occupation
            </Button>
          </div>
        )}

        {tab === "Family" && (
          <div className="grid gap-4 sm:grid-cols-2">
            <Input label="Family Type" value={profile.family?.familyType || ""} onChange={(e) => setProfile({ ...profile, family: { ...profile.family!, familyId: profile.family?.familyId || "", familyType: e.target.value } })} />
            <Input label="Father Name" value={profile.family?.fatherName || ""} onChange={(e) => setProfile({ ...profile, family: { ...profile.family!, familyId: profile.family?.familyId || "", fatherName: e.target.value } })} />
            <Input label="Mother Name" value={profile.family?.motherName || ""} onChange={(e) => setProfile({ ...profile, family: { ...profile.family!, familyId: profile.family?.familyId || "", motherName: e.target.value } })} />
            <Input label="Brothers" type="number" value={profile.family?.brothers ?? ""} onChange={(e) => setProfile({ ...profile, family: { ...profile.family!, familyId: profile.family?.familyId || "", brothers: Number(e.target.value) } })} />
            <Input label="Sisters" type="number" value={profile.family?.sisters ?? ""} onChange={(e) => setProfile({ ...profile, family: { ...profile.family!, familyId: profile.family?.familyId || "", sisters: Number(e.target.value) } })} />
            <Button disabled={saving} onClick={() => save(() => profileApi.saveFamily((profile.family || {}) as Record<string, unknown>), "Family details saved!")}>
              Save Family
            </Button>
          </div>
        )}

        {tab === "Lifestyle" && (
          <div className="grid gap-4 sm:grid-cols-2">
            <Input label="Diet" value={profile.lifestyle?.diet || ""} onChange={(e) => setProfile({ ...profile, lifestyle: { ...profile.lifestyle!, lifestyleId: profile.lifestyle?.lifestyleId || "", diet: e.target.value, smoking: profile.lifestyle?.smoking || false, drinking: profile.lifestyle?.drinking || false } })} />
            <Textarea label="Hobbies" value={profile.lifestyle?.hobbies || ""} onChange={(e) => setProfile({ ...profile, lifestyle: { ...profile.lifestyle!, lifestyleId: profile.lifestyle?.lifestyleId || "", hobbies: e.target.value, smoking: profile.lifestyle?.smoking || false, drinking: profile.lifestyle?.drinking || false } })} />
            <Input label="Languages Known" value={profile.lifestyle?.languagesKnown || ""} onChange={(e) => setProfile({ ...profile, lifestyle: { ...profile.lifestyle!, lifestyleId: profile.lifestyle?.lifestyleId || "", languagesKnown: e.target.value, smoking: profile.lifestyle?.smoking || false, drinking: profile.lifestyle?.drinking || false } })} />
            <label className="flex items-center gap-2 text-sm"><input type="checkbox" checked={profile.lifestyle?.smoking} onChange={(e) => setProfile({ ...profile, lifestyle: { ...profile.lifestyle!, lifestyleId: profile.lifestyle?.lifestyleId || "", smoking: e.target.checked, drinking: profile.lifestyle?.drinking || false } })} /> Smoking</label>
            <label className="flex items-center gap-2 text-sm"><input type="checkbox" checked={profile.lifestyle?.drinking} onChange={(e) => setProfile({ ...profile, lifestyle: { ...profile.lifestyle!, lifestyleId: profile.lifestyle?.lifestyleId || "", drinking: e.target.checked, smoking: profile.lifestyle?.smoking || false } })} /> Drinking</label>
            <Button disabled={saving} onClick={() => save(() => profileApi.saveLifestyle((profile.lifestyle || { smoking: false, drinking: false }) as Record<string, unknown>), "Lifestyle saved!")}>
              Save Lifestyle
            </Button>
          </div>
        )}

        {tab === "Location" && (
          <div className="grid gap-4 sm:grid-cols-2">
            <Input label="Country" value={profile.location?.country || ""} onChange={(e) => setProfile({ ...profile, location: { ...profile.location!, locationId: profile.location?.locationId || "", country: e.target.value } })} />
            <Input label="State" value={profile.location?.state || ""} onChange={(e) => setProfile({ ...profile, location: { ...profile.location!, locationId: profile.location?.locationId || "", state: e.target.value } })} />
            <Input label="City" value={profile.location?.city || ""} onChange={(e) => setProfile({ ...profile, location: { ...profile.location!, locationId: profile.location?.locationId || "", city: e.target.value } })} />
            <Input label="Pincode" value={profile.location?.pincode || ""} onChange={(e) => setProfile({ ...profile, location: { ...profile.location!, locationId: profile.location?.locationId || "", pincode: e.target.value } })} />
            <div className="sm:col-span-2"><Textarea label="Address" value={profile.location?.address || ""} onChange={(e) => setProfile({ ...profile, location: { ...profile.location!, locationId: profile.location?.locationId || "", address: e.target.value } })} /></div>
            <Button disabled={saving} onClick={() => save(() => profileApi.saveLocation((profile.location || {}) as Record<string, unknown>), "Location saved!")}>
              Save Location
            </Button>
          </div>
        )}

        {tab === "Photos" && (
          <div className="space-y-4">
            <div className="rounded-lg border border-dashed border-slate-300 bg-slate-50 p-6 text-center">
              <input
                ref={fileRef}
                type="file"
                accept="image/jpeg,image/png,image/webp"
                className="hidden"
                onChange={handleFileUpload}
              />
              <Upload className="mx-auto mb-2 text-slate-400" size={32} />
              <p className="text-sm text-slate-600">
                Upload JPG, PNG or WEBP (max {PHOTO_MAX_SIZE_MB} MB each)
              </p>
              <p className="mt-1 text-xs text-slate-500">
                {profile.photos.length} / {PHOTO_MAX_COUNT} photos uploaded
              </p>
              <Button
                className="mt-4"
                disabled={uploading || profile.photos.length >= PHOTO_MAX_COUNT}
                onClick={() => fileRef.current?.click()}
              >
                {uploading ? "Uploading..." : "Choose photo"}
              </Button>
            </div>
            <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
              {profile.photos.map((ph, idx) => (
                <div key={ph.photoId} className="relative overflow-hidden rounded-xl border border-slate-200">
                  <button type="button" onClick={() => setLightboxIndex(idx)} className="block w-full">
                    {/* eslint-disable-next-line @next/next/no-img-element */}
                    <img src={resolvePhotoUrl(ph.photoUrl)} alt="" className="h-40 w-full object-cover transition hover:opacity-90" />
                  </button>
                  <span className={`absolute left-2 top-2 rounded px-2 py-0.5 text-xs font-medium ${ph.isApproved ? "bg-green-500 text-white" : "bg-amber-500 text-white"}`}>
                    {ph.isApproved ? "Approved" : "Pending approval"}
                  </span>
                  {ph.isPrimary && (
                    <span className="absolute right-2 top-2 rounded bg-rose-600 px-2 py-0.5 text-xs text-white">Primary</span>
                  )}
                  <button
                    type="button"
                    onClick={() => deletePhoto(ph.photoId)}
                    className="absolute bottom-2 right-2 rounded-full bg-white/90 p-1.5 text-red-500 shadow hover:bg-white"
                  >
                    <Trash2 size={14} />
                  </button>
                </div>
              ))}
            </div>
          </div>
        )}
      </Card>

      {lightboxIndex !== null && profile && (
        <PhotoLightbox
          photos={profile.photos.map((p) => p.photoUrl)}
          initialIndex={lightboxIndex}
          alt="My photo"
          onClose={() => setLightboxIndex(null)}
        />
      )}
    </div>
  );
}
