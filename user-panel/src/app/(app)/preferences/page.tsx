"use client";

import { useEffect, useState } from "react";
import { profileApi } from "@/lib/services";
import type { Preference } from "@/lib/types";
import { useToast } from "@/context/ToastContext";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Card } from "@/components/ui/Card";
import { PageHeader } from "@/components/ui/PageHeader";
import { LoadingSpinner } from "@/components/ui/LoadingSpinner";

export default function PreferencesPage() {
  const { showToast } = useToast();
  const [pref, setPref] = useState<Preference>({});
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    profileApi.getPreferences().then(setPref).finally(() => setLoading(false));
  }, []);

  const save = async () => {
    setSaving(true);
    try {
      setPref(await profileApi.savePreferences(pref));
      showToast("Partner preferences saved successfully!");
    } catch (err) {
      showToast(err instanceof Error ? err.message : "Failed", "error");
    } finally {
      setSaving(false);
    }
  };

  if (loading) return <LoadingSpinner fullPage />;

  return (
    <div>
      <PageHeader title="Partner Preferences" description="Set criteria for your ideal match" />
      <Card>
        <div className="grid gap-4 sm:grid-cols-2">
          <Input label="Min Age" type="number" value={pref.minAge ?? ""} onChange={(e) => setPref({ ...pref, minAge: Number(e.target.value) })} />
          <Input label="Max Age" type="number" value={pref.maxAge ?? ""} onChange={(e) => setPref({ ...pref, maxAge: Number(e.target.value) })} />
          <Input label="Min Height (cm)" type="number" value={pref.minHeight ?? ""} onChange={(e) => setPref({ ...pref, minHeight: Number(e.target.value) })} />
          <Input label="Max Height (cm)" type="number" value={pref.maxHeight ?? ""} onChange={(e) => setPref({ ...pref, maxHeight: Number(e.target.value) })} />
          <Input label="Religion" value={pref.religion || ""} onChange={(e) => setPref({ ...pref, religion: e.target.value })} />
          <Input label="Caste" value={pref.caste || ""} onChange={(e) => setPref({ ...pref, caste: e.target.value })} />
          <Input label="Education" value={pref.education || ""} onChange={(e) => setPref({ ...pref, education: e.target.value })} />
          <Input label="Occupation" value={pref.occupation || ""} onChange={(e) => setPref({ ...pref, occupation: e.target.value })} />
          <Input label="Country" value={pref.country || ""} onChange={(e) => setPref({ ...pref, country: e.target.value })} />
          <Input label="State" value={pref.state || ""} onChange={(e) => setPref({ ...pref, state: e.target.value })} />
          <Input label="City" value={pref.city || ""} onChange={(e) => setPref({ ...pref, city: e.target.value })} />
        </div>
        <Button className="mt-6" onClick={save} disabled={saving}>{saving ? "Saving..." : "Save Preferences"}</Button>
      </Card>
    </div>
  );
}
