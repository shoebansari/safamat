"use client";

import { useCallback, useEffect, useState } from "react";
import { Check, X } from "lucide-react";
import { membersApi } from "@/lib/services";
import { resolvePhotoUrl } from "@/lib/media";
import type { Member, TenantMemberDetail } from "@/lib/types";
import { DefaultAvatar } from "@/components/ui/DefaultAvatar";
import { Button } from "@/components/ui/Button";
import { Card } from "@/components/ui/Card";
import { Modal } from "@/components/ui/Modal";
import { PageHeader } from "@/components/ui/PageHeader";
import { StatusBadge } from "@/components/ui/Badge";
import { Alert, EmptyState, LoadingSpinner } from "@/components/ui/LoadingSpinner";

function MemberPhoto({ url, name, className }: { url?: string | null; name: string; className: string }) {
  const [error, setError] = useState(false);
  const resolved = resolvePhotoUrl(url);
  if (!resolved || error) return <DefaultAvatar name={name} className={className} />;
  return (
    // eslint-disable-next-line @next/next/no-img-element
    <img src={resolved} alt={name} className={className} onError={() => setError(true)} />
  );
}

export default function ProfileApprovalsPage() {
  const [items, setItems] = useState<Member[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [updatingId, setUpdatingId] = useState<string | null>(null);
  const [detail, setDetail] = useState<TenantMemberDetail | null>(null);
  const [detailLoading, setDetailLoading] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      setItems(await membersApi.getPendingApprovals());
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load approvals");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { load(); }, [load]);

  const openDetail = async (userId: string) => {
    setDetailLoading(true);
    try {
      setDetail(await membersApi.getDetail(userId));
    } catch (err) {
      alert(err instanceof Error ? err.message : "Failed to load member details");
    } finally {
      setDetailLoading(false);
    }
  };

  const updateProfile = async (userId: string, status: string) => {
    setUpdatingId(`${userId}-profile`);
    try {
      await membersApi.updateProfileApproval(userId, status);
      load();
      if (detail?.userId === userId) await openDetail(userId);
    } catch (err) {
      alert(err instanceof Error ? err.message : "Failed to update profile");
    } finally {
      setUpdatingId(null);
    }
  };

  const updatePhoto = async (photoId: string, status: string) => {
    setUpdatingId(`${photoId}-photo`);
    try {
      await membersApi.updatePhotoApproval(photoId, status);
      load();
      if (detail) await openDetail(detail.userId);
    } catch (err) {
      alert(err instanceof Error ? err.message : "Failed to update photo");
    } finally {
      setUpdatingId(null);
    }
  };

  const updatePlan = async (userCode: string, memberPlanId: string, paymentStatus: string) => {
    setUpdatingId(`${userCode}-plan`);
    try {
      await membersApi.updatePlan(userCode, { memberPlanId, paymentStatus });
      load();
    } catch (err) {
      alert(err instanceof Error ? err.message : "Failed to update plan");
    } finally {
      setUpdatingId(null);
    }
  };

  const displayPhoto = (member: Member) =>
    member.pendingPhotoUrl || member.primaryPhotoUrl || member.profilePhotoUrl;

  return (
    <div>
      <PageHeader
        title="Profile Approvals"
        description="Only approved profiles and photos are visible to other members"
      />
      {error && <Alert message={error} />}

      <Card>
        {loading ? (
          <LoadingSpinner />
        ) : items.length === 0 ? (
          <EmptyState message="No pending profile or photo approvals" />
        ) : (
          <div className="space-y-6">
            {items.map((member) => {
              const userId = member.userId || member.memberId;
              return (
                <div key={userId} className="rounded-xl border border-slate-100 p-4 sm:p-6">
                  <div className="flex flex-col gap-6 lg:flex-row">
                    <div className="shrink-0 space-y-2">
                      <MemberPhoto
                        url={displayPhoto(member)}
                        name={member.fullName}
                        className="h-32 w-32 rounded-xl border border-slate-200 object-cover"
                      />
                      {member.hasPendingPhoto && member.pendingPhotoUrl && (
                        <p className="text-center text-xs text-amber-600">Pending photo</p>
                      )}
                    </div>

                    <div className="flex-1">
                      <button
                        type="button"
                        onClick={() => openDetail(userId)}
                        className="text-left hover:text-rose-600"
                      >
                        <h3 className="text-lg font-semibold text-slate-900">{member.fullName}</h3>
                        <p className="text-sm text-slate-500">
                          {member.userCode} · {member.email || "No email"}
                        </p>
                        <p className="mt-1 text-xs font-medium text-rose-600">View full profile →</p>
                      </button>

                      {member.bio && (
                        <p className="mt-3 text-sm text-slate-600">{member.bio}</p>
                      )}

                      <div className="mt-4 grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
                        <div className="rounded-lg bg-slate-50 p-4 sm:col-span-2 lg:col-span-3">
                          <div className="mb-3 flex items-center justify-between">
                            <span className="text-sm font-medium text-slate-700">Membership Plan</span>
                            <StatusBadge
                              active={member.currentSubscription?.paymentStatus === "Paid"}
                              label={member.currentSubscription?.paymentStatus || "No plan"}
                            />
                          </div>
                          {member.currentSubscription ? (
                            <>
                              <p className="text-sm text-slate-600">
                                {member.currentSubscription.planName} · ₹{member.currentSubscription.planPrice}
                              </p>
                              {member.currentSubscription.paymentStatus === "Pending" && (
                                <div className="mt-3 flex gap-2">
                                  <Button size="sm" onClick={() => updatePlan(member.userCode, member.currentSubscription!.memberPlanId, "Paid")} disabled={updatingId === `${member.userCode}-plan`}>
                                    <Check size={14} /> Approve Plan
                                  </Button>
                                  <Button size="sm" variant="ghost" onClick={() => updatePlan(member.userCode, member.currentSubscription!.memberPlanId, "Rejected")} disabled={updatingId === `${member.userCode}-plan`}>
                                    <X size={14} className="text-red-500" /> Reject Plan
                                  </Button>
                                </div>
                              )}
                            </>
                          ) : (
                            <p className="text-sm text-slate-500">No plan assigned</p>
                          )}
                        </div>

                        <div className="rounded-lg bg-slate-50 p-4">
                          <div className="mb-3 flex items-center justify-between">
                            <span className="text-sm font-medium text-slate-700">Profile</span>
                            <StatusBadge active={member.profileStatus === "Approved"} label={member.profileStatus} />
                          </div>
                          {member.profileStatus === "Pending" && (
                            <div className="flex gap-2">
                              <Button size="sm" onClick={() => updateProfile(userId, "Approved")} disabled={updatingId === `${userId}-profile`}>
                                <Check size={14} /> Approve
                              </Button>
                              <Button size="sm" variant="ghost" onClick={() => updateProfile(userId, "Rejected")} disabled={updatingId === `${userId}-profile`}>
                                <X size={14} className="text-red-500" /> Reject
                              </Button>
                            </div>
                          )}
                        </div>

                        <div className="rounded-lg bg-slate-50 p-4">
                          <div className="mb-3 flex items-center justify-between">
                            <span className="text-sm font-medium text-slate-700">Photo</span>
                            <StatusBadge active={!member.hasPendingPhoto} label={member.hasPendingPhoto ? "Pending" : "Approved"} />
                          </div>
                          {member.hasPendingPhoto && member.pendingPhotoId && (
                            <div className="flex gap-2">
                              <Button size="sm" onClick={() => updatePhoto(member.pendingPhotoId!, "Approved")} disabled={updatingId === `${member.pendingPhotoId}-photo`}>
                                <Check size={14} /> Approve
                              </Button>
                              <Button size="sm" variant="ghost" onClick={() => updatePhoto(member.pendingPhotoId!, "Rejected")} disabled={updatingId === `${member.pendingPhotoId}-photo`}>
                                <X size={14} className="text-red-500" /> Reject
                              </Button>
                            </div>
                          )}
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              );
            })}
          </div>
        )}
      </Card>

      <Modal open={!!detail || detailLoading} onClose={() => setDetail(null)} title={detail ? `${detail.firstName} ${detail.lastName}` : "Member details"}>
        {detailLoading ? (
          <LoadingSpinner />
        ) : detail ? (
          <div className="space-y-4">
            <div className="flex flex-wrap gap-3">
              {detail.photos.length > 0 ? detail.photos.map((ph) => (
                <div key={ph.photoId} className="relative">
                  <MemberPhoto url={ph.photoUrl} name={detail.firstName} className="h-24 w-24 rounded-lg object-cover" />
                  <span className={`absolute left-1 top-1 rounded px-1.5 py-0.5 text-[10px] text-white ${ph.isApproved ? "bg-green-500" : "bg-amber-500"}`}>
                    {ph.isApproved ? "Approved" : "Pending"}
                  </span>
                </div>
              )) : (
                <DefaultAvatar name={detail.firstName} className="h-24 w-24 rounded-lg" />
              )}
            </div>
            <dl className="grid gap-2 text-sm sm:grid-cols-2">
              <div><dt className="text-slate-500">User ID</dt><dd className="font-medium">{detail.userCode}</dd></div>
              <div><dt className="text-slate-500">Email</dt><dd>{detail.email || "—"}</dd></div>
              <div><dt className="text-slate-500">Phone</dt><dd>{detail.phone || "—"}</dd></div>
              <div><dt className="text-slate-500">Gender</dt><dd>{detail.gender || "—"}</dd></div>
              <div><dt className="text-slate-500">Age</dt><dd>{detail.age ?? "—"}</dd></div>
              <div><dt className="text-slate-500">Religion</dt><dd>{detail.religion || "—"}</dd></div>
              <div><dt className="text-slate-500">City</dt><dd>{detail.location?.city || "—"}</dd></div>
              <div><dt className="text-slate-500">Education</dt><dd>{detail.education?.qualification || "—"}</dd></div>
              <div><dt className="text-slate-500">Occupation</dt><dd>{detail.occupation?.occupation || "—"}</dd></div>
              <div><dt className="text-slate-500">Profile status</dt><dd>{detail.profileStatus}</dd></div>
            </dl>
            {detail.aboutMe && (
              <p className="rounded-lg bg-slate-50 p-3 text-sm text-slate-700">{detail.aboutMe}</p>
            )}
          </div>
        ) : null}
      </Modal>
    </div>
  );
}
