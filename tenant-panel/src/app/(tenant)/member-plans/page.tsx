"use client";

import { useEffect, useState } from "react";
import { Search } from "lucide-react";
import { memberPlansApi, membersApi } from "@/lib/services";
import type { Member, MemberPlan } from "@/lib/types";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { Card } from "@/components/ui/Card";
import { PageHeader } from "@/components/ui/PageHeader";
import { StatusBadge } from "@/components/ui/Badge";
import { Alert, LoadingSpinner } from "@/components/ui/LoadingSpinner";

const PAYMENT_OPTIONS = [
  { value: "Pending", label: "Pending" },
  { value: "Paid", label: "Paid" },
  { value: "Rejected", label: "Rejected" },
];

export default function MemberPlansPage() {
  const [userCode, setUserCode] = useState("");
  const [member, setMember] = useState<Member | null>(null);
  const [plans, setPlans] = useState<MemberPlan[]>([]);
  const [selectedPlanId, setSelectedPlanId] = useState("");
  const [paymentStatus, setPaymentStatus] = useState("Pending");
  const [searching, setSearching] = useState(false);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  useEffect(() => {
    memberPlansApi
      .list()
      .then((data) => {
        setPlans(data.filter((p) => p.isActive));
      })
      .catch(() => setPlans([]));
  }, []);

  const handleSearch = async () => {
    if (!userCode.trim()) return;
    setSearching(true);
    setError("");
    setSuccess("");
    setMember(null);
    try {
      const result = await membersApi.getByUserCode(userCode.trim());
      setMember(result);
      setSelectedPlanId(result.currentSubscription?.memberPlanId || "");
      setPaymentStatus(result.currentSubscription?.paymentStatus || "Pending");
    } catch (err) {
      setError(err instanceof Error ? err.message : "User not found");
    } finally {
      setSearching(false);
    }
  };

  const handleUpdate = async () => {
    if (!member || !selectedPlanId) {
      alert("Please select a plan");
      return;
    }
    setSaving(true);
    setError("");
    setSuccess("");
    try {
      const updated = await membersApi.updatePlan(member.userCode, {
        memberPlanId: selectedPlanId,
        paymentStatus,
      });
      setMember(updated);
      setSuccess("Plan and payment status updated successfully.");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Update failed");
    } finally {
      setSaving(false);
    }
  };

  const planOptions = [
    { value: "", label: "Select a plan" },
    ...plans.map((p) => ({
      value: p.memberPlanId,
      label: `${p.planName} (₹${p.price})`,
    })),
  ];

  return (
    <div>
      <PageHeader
        title="Assign Member Plan"
        description="Search by user ID and update plan with payment status"
      />

      <Card className="mb-6">
        <div className="flex flex-col gap-4 sm:flex-row sm:items-end">
          <div className="flex-1">
            <Input
              label="User ID"
              placeholder="e.g. USR001"
              value={userCode}
              onChange={(e) => setUserCode(e.target.value)}
              onKeyDown={(e) => e.key === "Enter" && handleSearch()}
            />
          </div>
          <Button onClick={handleSearch} disabled={searching || !userCode.trim()}>
            <Search size={16} />
            {searching ? "Searching..." : "Search"}
          </Button>
        </div>
      </Card>

      {error && <Alert message={error} />}
      {success && (
        <div className="mb-4 rounded-lg border border-green-200 bg-green-50 px-4 py-3 text-sm text-green-700">
          {success}
        </div>
      )}

      {searching && <LoadingSpinner />}

      {member && !searching && (
        <div className="grid gap-6 lg:grid-cols-2">
          <Card>
            <h3 className="mb-4 text-lg font-semibold text-slate-900">Member Details</h3>
            <dl className="space-y-3 text-sm">
              <div className="flex justify-between">
                <dt className="text-slate-500">User ID</dt>
                <dd className="font-medium">{member.userCode}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-slate-500">Full Name</dt>
                <dd className="font-medium">{member.fullName}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-slate-500">Email</dt>
                <dd>{member.email || "—"}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-slate-500">Phone</dt>
                <dd>{member.phone || "—"}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-slate-500">Profile Status</dt>
                <dd>
                  <StatusBadge
                    active={member.profileStatus === "Approved"}
                    label={member.profileStatus}
                  />
                </dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-slate-500">Photo Status</dt>
                <dd>
                  <StatusBadge
                    active={member.photoStatus === "Approved"}
                    label={member.photoStatus}
                  />
                </dd>
              </div>
              {member.currentSubscription && (
                <>
                  <div className="flex justify-between border-t border-slate-100 pt-3">
                    <dt className="text-slate-500">Current Plan</dt>
                    <dd className="font-medium">{member.currentSubscription.planName}</dd>
                  </div>
                  <div className="flex justify-between">
                    <dt className="text-slate-500">Payment Status</dt>
                    <dd>{member.currentSubscription.paymentStatus}</dd>
                  </div>
                </>
              )}
            </dl>
            {member.bio && (
              <p className="mt-4 rounded-lg bg-slate-50 p-3 text-sm text-slate-600">{member.bio}</p>
            )}
          </Card>

          <Card>
            <h3 className="mb-4 text-lg font-semibold text-slate-900">Update Plan</h3>
            <div className="space-y-4">
              <Select
                label="Plan"
                options={planOptions}
                value={selectedPlanId}
                onChange={(e) => setSelectedPlanId(e.target.value)}
                required
              />
              <Select
                label="Payment Status"
                options={PAYMENT_OPTIONS}
                value={paymentStatus}
                onChange={(e) => setPaymentStatus(e.target.value)}
              />
              <Button className="w-full" onClick={handleUpdate} disabled={saving || !selectedPlanId}>
                {saving ? "Updating..." : "Update Plan"}
              </Button>
            </div>
          </Card>
        </div>
      )}
    </div>
  );
}
