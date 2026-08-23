"use client";

import { useCallback, useEffect, useState } from "react";
import Link from "next/link";
import { Filter, Heart, RotateCcw, Search, Star, X } from "lucide-react";
import { socialApi } from "@/lib/services";
import { useToast } from "@/context/ToastContext";
import type { DiscoverFilterOptions, DiscoverFilters, DiscoverProfile } from "@/lib/types";
import { ClickablePhoto } from "@/components/ui/PhotoLightbox";
import { Button } from "@/components/ui/Button";
import { Card } from "@/components/ui/Card";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { PageHeader } from "@/components/ui/PageHeader";
import { Alert, EmptyState, LoadingSpinner } from "@/components/ui/LoadingSpinner";

const EMPTY_FILTERS: DiscoverFilters = {};

export default function DiscoverPage() {
  const { showToast } = useToast();
  const [items, setItems] = useState<DiscoverProfile[]>([]);
  const [options, setOptions] = useState<DiscoverFilterOptions | null>(null);
  const [filters, setFilters] = useState<DiscoverFilters>(EMPTY_FILTERS);
  const [draft, setDraft] = useState<DiscoverFilters>(EMPTY_FILTERS);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [showFilters, setShowFilters] = useState(true);

  const load = useCallback(async (activeFilters: DiscoverFilters) => {
    setLoading(true);
    try {
      const [profiles, filterOptions] = await Promise.all([
        socialApi.discover(activeFilters),
        socialApi.discoverFilterOptions().catch(() => null),
      ]);
      setItems(profiles);
      if (filterOptions) setOptions(filterOptions);
      setError("");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { load(filters); }, [filters, load]);

  const applyFilters = () => setFilters({ ...draft });
  const clearFilters = () => {
    setDraft(EMPTY_FILTERS);
    setFilters(EMPTY_FILTERS);
  };

  const activeCount = Object.values(filters).filter((v) => v !== undefined && v !== "").length;

  return (
    <div>
      <PageHeader
        title="Discover"
        description={`Browse ${items.length} approved profiles · Filter by age, city, religion, language & more`}
        action={
          <Button variant="secondary" size="sm" onClick={() => setShowFilters((s) => !s)}>
            <Filter size={14} /> {showFilters ? "Hide filters" : "Show filters"}
          </Button>
        }
      />
      {error && <Alert message={error} />}

      <div className="grid gap-6 lg:grid-cols-4">
        {showFilters && (
          <Card className="lg:col-span-1">
            <div className="mb-4 flex items-center justify-between">
              <h3 className="font-semibold text-slate-900">Search filters</h3>
              {activeCount > 0 && (
                <span className="rounded-full bg-rose-100 px-2 py-0.5 text-xs font-medium text-rose-700">
                  {activeCount} active
                </span>
              )}
            </div>
            <div className="space-y-3">
              <div className="grid grid-cols-2 gap-2">
                <Input
                  label="Min age"
                  type="number"
                  value={draft.minAge ?? ""}
                  onChange={(e) => setDraft({ ...draft, minAge: e.target.value ? Number(e.target.value) : undefined })}
                />
                <Input
                  label="Max age"
                  type="number"
                  value={draft.maxAge ?? ""}
                  onChange={(e) => setDraft({ ...draft, maxAge: e.target.value ? Number(e.target.value) : undefined })}
                />
              </div>
              <div className="grid grid-cols-2 gap-2">
                <Input
                  label="Min height (cm)"
                  type="number"
                  value={draft.minHeight ?? ""}
                  onChange={(e) => setDraft({ ...draft, minHeight: e.target.value ? Number(e.target.value) : undefined })}
                />
                <Input
                  label="Max height (cm)"
                  type="number"
                  value={draft.maxHeight ?? ""}
                  onChange={(e) => setDraft({ ...draft, maxHeight: e.target.value ? Number(e.target.value) : undefined })}
                />
              </div>
              <Select
                label="Gender"
                value={draft.gender || ""}
                onChange={(e) => setDraft({ ...draft, gender: e.target.value || undefined })}
                options={[
                  { value: "", label: "Any (opposite by default)" },
                  ...(options?.genders || ["Male", "Female"]).map((g) => ({ value: g, label: g })),
                ]}
              />
              <Select
                label="City"
                value={draft.city || ""}
                onChange={(e) => setDraft({ ...draft, city: e.target.value || undefined })}
                options={[
                  { value: "", label: "All cities" },
                  ...(options?.cities || []).map((c) => ({ value: c, label: c })),
                ]}
              />
              <Select
                label="State"
                value={draft.state || ""}
                onChange={(e) => setDraft({ ...draft, state: e.target.value || undefined })}
                options={[
                  { value: "", label: "All states" },
                  ...(options?.states || []).map((s) => ({ value: s, label: s })),
                ]}
              />
              <Select
                label="Religion"
                value={draft.religion || ""}
                onChange={(e) => setDraft({ ...draft, religion: e.target.value || undefined })}
                options={[
                  { value: "", label: "All religions" },
                  ...(options?.religions || []).map((r) => ({ value: r, label: r })),
                ]}
              />
              <Select
                label="Mother tongue"
                value={draft.motherTongue || ""}
                onChange={(e) => setDraft({ ...draft, motherTongue: e.target.value || undefined })}
                options={[
                  { value: "", label: "All languages" },
                  ...(options?.motherTongues || []).map((l) => ({ value: l, label: l })),
                ]}
              />
              <Select
                label="Marital status"
                value={draft.maritalStatus || ""}
                onChange={(e) => setDraft({ ...draft, maritalStatus: e.target.value || undefined })}
                options={[
                  { value: "", label: "Any" },
                  ...(options?.maritalStatuses || []).map((m) => ({ value: m, label: m })),
                ]}
              />
              <Select
                label="Occupation"
                value={draft.occupation || ""}
                onChange={(e) => setDraft({ ...draft, occupation: e.target.value || undefined })}
                options={[
                  { value: "", label: "All occupations" },
                  ...(options?.occupations || []).map((o) => ({ value: o, label: o })),
                ]}
              />
              <Select
                label="Education"
                value={draft.education || ""}
                onChange={(e) => setDraft({ ...draft, education: e.target.value || undefined })}
                options={[
                  { value: "", label: "All education" },
                  ...(options?.educations || []).map((ed) => ({ value: ed, label: ed })),
                ]}
              />
              <div className="flex gap-2 pt-2">
                <Button className="flex-1" onClick={applyFilters}>
                  <Search size={14} /> Apply
                </Button>
                <Button variant="ghost" onClick={clearFilters} title="Clear filters">
                  <RotateCcw size={14} />
                </Button>
              </div>
            </div>
          </Card>
        )}

        <div className={showFilters ? "lg:col-span-3" : "lg:col-span-4"}>
          {activeCount > 0 && (
            <div className="mb-4 flex flex-wrap gap-2">
              {Object.entries(filters).map(([key, value]) =>
                value !== undefined && value !== "" ? (
                  <span key={key} className="inline-flex items-center gap-1 rounded-full bg-rose-50 px-3 py-1 text-xs font-medium text-rose-700 ring-1 ring-rose-100">
                    {key}: {value}
                    <button
                      type="button"
                      onClick={() => {
                        const next = { ...filters, [key]: undefined };
                        setFilters(next);
                        setDraft(next);
                      }}
                    >
                      <X size={12} />
                    </button>
                  </span>
                ) : null
              )}
            </div>
          )}

          <Card>
            {loading ? (
              <LoadingSpinner />
            ) : items.length === 0 ? (
              <EmptyState message="No profiles match your filters. Try adjusting your search." />
            ) : (
              <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3">
                {items.map((p) => (
                  <div key={p.userId} className="overflow-hidden rounded-xl border border-slate-100 bg-white shadow-sm transition hover:border-rose-200 hover:shadow-md">
                    <ClickablePhoto
                      src={p.primaryPhotoUrl}
                      photos={p.photoUrls}
                      alt={p.firstName}
                      className="h-48 w-full object-cover"
                      fallbackClassName="h-48 w-full"
                    />
                    <div className="p-4">
                      <h3 className="font-semibold text-slate-900">{p.firstName} {p.lastName}</h3>
                      <p className="mt-1 text-sm text-slate-500">
                        {p.age} yrs · {p.city}{p.state ? `, ${p.state}` : ""}
                      </p>
                      <p className="text-xs text-slate-400">
                        {p.religion} · {p.motherTongue} · {p.maritalStatus}
                      </p>
                      <p className="mt-1 text-xs text-slate-500">{p.occupation} · {p.education}</p>
                      <p className="mt-1 text-xs font-medium text-rose-600">{p.matchPercentage}% match</p>
                      <div className="mt-3 flex gap-2">
                        <Link href={`/members/${p.userId}`} className="flex-1">
                          <Button variant="secondary" size="sm" className="w-full">View</Button>
                        </Link>
                        <Button
                          size="sm"
                          onClick={async () => {
                            try {
                              await socialApi.sendInterest(p.userId);
                              showToast("Interest sent successfully!");
                            } catch (err) {
                              showToast(err instanceof Error ? err.message : "Failed", "error");
                            }
                          }}
                        >
                          <Heart size={14} />
                        </Button>
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={async () => {
                            try {
                              await socialApi.toggleFavorite(p.userId);
                              showToast("Favorites updated");
                            } catch (err) {
                              showToast(err instanceof Error ? err.message : "Failed", "error");
                            }
                          }}
                        >
                          <Star size={14} />
                        </Button>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}
          </Card>
        </div>
      </div>
    </div>
  );
}
