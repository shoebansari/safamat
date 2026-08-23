"use client";

import { useEffect, useState } from "react";
import { subscriptionApi } from "@/lib/services";
import { useToast } from "@/context/ToastContext";
import type { MemberPlan, UserSubscription } from "@/lib/types";
import { Button } from "@/components/ui/Button";
import { Card } from "@/components/ui/Card";
import { Select } from "@/components/ui/Select";
import { PageHeader } from "@/components/ui/PageHeader";
import { Alert, LoadingSpinner } from "@/components/ui/LoadingSpinner";

const STATUS_LABELS: Record<string, string> = {
  Pending: "Waiting for tenant approval",
  Paid: "Active",
  Rejected: "Rejected by tenant",
};

export default function PlanPage() {
  const { showToast } = useToast();
  const [plans, setPlans] = useState<MemberPlan[]>([]);
  const [subscription, setSubscription] = useState<UserSubscription | null>(null);
  const [selectedPlanId, setSelectedPlanId] = useState("");
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  const load = async () => {
    setLoading(true);
    try {
      const [planList, sub] = await Promise.all([
        subscriptionApi.getPlans(),
        subscriptionApi.getMySubscription(),
      ]);
      setPlans(planList);
      setSubscription(sub);
      setSelectedPlanId(sub?.memberPlanId || "");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load plan details");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, []);

  const handleChange = async () => {
    if (!selectedPlanId) {
      showToast("Please select a plan", "error");
      return;
    }
    setSaving(true);
    try {
      const updated = await subscriptionApi.changePlan(selectedPlanId);
      setSubscription(updated);
      showToast("Plan change requested. Waiting for tenant approval.");
    } catch (err) {
      showToast(err instanceof Error ? err.message : "Failed to change plan", "error");
    } finally {
      setSaving(false);
    }
  };

  if (loading) return <LoadingSpinner fullPage />;

  const planOptions = [
    { value: "", label: "Select a plan" },
    ...plans.map((p) => ({
      value: p.memberPlanId,
      label: `${p.planName} — ₹${p.price} / ${p.durationDays} days`,
    })),
  ];

  return (
    <div>
      <PageHeader
        title="My Membership Plan"
        description="View or change your plan. Plan changes require tenant approval."
      />
      {error && <Alert message={error} />}

      <div className="grid gap-6 lg:grid-cols-2">
        <Card>
          <h3 className="mb-4 text-lg font-semibold text-slate-900">Current Plan</h3>
          {subscription ? (
            <dl className="space-y-3 text-sm">
              <div className="flex justify-between">
                <dt className="text-slate-500">Plan</dt>
                <dd className="font-medium">{subscription.planName}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-slate-500">Price</dt>
                <dd>₹{subscription.planPrice}</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-slate-500">Duration</dt>
                <dd>{subscription.durationDays} days</dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-slate-500">Status</dt>
                <dd className={`font-medium ${
                  subscription.paymentStatus === "Paid" ? "text-green-600" :
                  subscription.paymentStatus === "Rejected" ? "text-red-600" : "text-amber-600"
                }`}>
                  {STATUS_LABELS[subscription.paymentStatus] || subscription.paymentStatus}
                </dd>
              </div>
              <div className="flex justify-between">
                <dt className="text-slate-500">Assigned on</dt>
                <dd>{new Date(subscription.assignedOn).toLocaleDateString()}</dd>
              </div>
            </dl>
          ) : (
            <p className="text-sm text-slate-500">No plan assigned yet. Select a plan below.</p>
          )}
        </Card>

        <Card>
          <h3 className="mb-4 text-lg font-semibold text-slate-900">Change Plan</h3>
          <div className="space-y-4">
            <Select
              label="Select plan"
              options={planOptions}
              value={selectedPlanId}
              onChange={(e) => setSelectedPlanId(e.target.value)}
              required
            />
            <p className="text-xs text-slate-500">
              After you submit, your tenant will review and approve the plan from their panel.
            </p>
            <Button
              className="w-full"
              onClick={handleChange}
              disabled={saving || !selectedPlanId || (selectedPlanId === subscription?.memberPlanId && subscription?.paymentStatus === "Paid")}
            >
              {saving ? "Submitting..." : "Request Plan Change"}
            </Button>
          </div>
        </Card>
      </div>
    </div>
  );
}
