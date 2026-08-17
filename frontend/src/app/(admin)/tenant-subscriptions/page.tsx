"use client";

import { useCallback, useEffect, useState } from "react";
import { computeSubscriptionFromPlan, isFreePlan, todayISO } from "@/lib/subscription-utils";
import type { FieldErrors } from "@/lib/validation";
import { hasErrors, patchFieldError, positiveNumber, requiredSelect } from "@/lib/validation";
import { Edit, Plus, Ban } from "lucide-react";
import { subscriptionPlansApi, tenantSubscriptionsApi, tenantsApi } from "@/lib/services";
import type { SubscriptionPlan, Tenant, TenantSubscription } from "@/lib/types";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { Card } from "@/components/ui/Card";
import { PageHeader } from "@/components/ui/PageHeader";
import { Modal, ConfirmModal } from "@/components/ui/Modal";
import { Pagination } from "@/components/ui/Pagination";
import { Badge } from "@/components/ui/Badge";
import { Alert, EmptyState, LoadingSpinner } from "@/components/ui/LoadingSpinner";

const emptyForm = {
  tenantId: "", planId: "", startDate: todayISO(), endDate: "", nextBillingDate: "",
  amount: 0, paymentStatus: "Pending", subscriptionStatus: "Active",
};

export default function TenantSubscriptionsPage() {
  const [items, setItems] = useState<TenantSubscription[]>([]);
  const [tenants, setTenants] = useState<Tenant[]>([]);
  const [plans, setPlans] = useState<SubscriptionPlan[]>([]);
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [modalOpen, setModalOpen] = useState(false);
  const [deleteOpen, setDeleteOpen] = useState(false);
  const [editing, setEditing] = useState<TenantSubscription | null>(null);
  const [deleting, setDeleting] = useState<TenantSubscription | null>(null);
  const [form, setForm] = useState(emptyForm);
  const [errors, setErrors] = useState<FieldErrors>({});
  const [saving, setSaving] = useState(false);

  const selectedPlan = plans.find((p) => p.planId === form.planId);
  const freePlan = isFreePlan(selectedPlan);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const result = await tenantSubscriptionsApi.list(page, 10);
      setItems(result.items);
      setTotalCount(result.totalCount);
      setTotalPages(result.totalPages);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load");
    } finally {
      setLoading(false);
    }
  }, [page]);

  useEffect(() => {
    load();
    tenantsApi.list(1, 100, "", true).then((r) => setTenants(r.items));
    subscriptionPlansApi.list(1, 100, true).then((r) => setPlans(r.items));
  }, [load]);

  const applyPlanDates = (planId: string, startDate?: string) => {
    const plan = plans.find((p) => p.planId === planId);
    if (!plan) {
      setForm((f) => ({ ...f, planId }));
      patchFieldError(setErrors, "planId", requiredSelect(planId, "Plan"));
      return;
    }
    const computed = computeSubscriptionFromPlan(plan, startDate || form.startDate || todayISO());
    setForm((f) => ({ ...f, planId, ...computed, subscriptionStatus: "Active" }));
    patchFieldError(setErrors, "planId", undefined);
    patchFieldError(setErrors, "startDate", undefined);
    patchFieldError(setErrors, "endDate", undefined);
    patchFieldError(setErrors, "amount", undefined);
  };

  const validate = (): boolean => {
    const e: FieldErrors = {};
    if (!editing) {
      e.tenantId = requiredSelect(form.tenantId, "Tenant");
      e.planId = requiredSelect(form.planId, "Plan");
      if (!freePlan) {
        if (!form.startDate) e.startDate = "Start date is required";
        if (!form.endDate) e.endDate = "End date is required";
        e.amount = positiveNumber(form.amount, "Amount", true);
      }
    }
    Object.keys(e).forEach((k) => !e[k] && delete e[k]);
    setErrors(e);
    return !hasErrors(e);
  };

  const openCreate = () => {
    setEditing(null);
    setForm({ ...emptyForm, startDate: todayISO() });
    setErrors({});
    setModalOpen(true);
  };

  const handleSave = async () => {
    if (!validate()) return;
    setSaving(true);
    try {
      if (editing) {
        await tenantSubscriptionsApi.update(editing.tenantSubscriptionsId, {
          endDate: form.endDate,
          nextBillingDate: form.nextBillingDate || null,
          amount: form.amount,
          subscriptionStatus: form.subscriptionStatus,
        });
      } else {
        await tenantSubscriptionsApi.create({
          tenantId: form.tenantId,
          planId: form.planId,
          startDate: form.startDate,
          endDate: form.endDate || form.startDate,
          nextBillingDate: form.nextBillingDate || null,
          amount: form.amount,
          paymentStatus: "Pending",
          subscriptionStatus: "Active",
        });
      }
      setModalOpen(false);
      load();
    } catch (err) {
      alert(err instanceof Error ? err.message : "Save failed");
    } finally {
      setSaving(false);
    }
  };

  const handleDeactivate = async () => {
    if (!deleting) return;
    setSaving(true);
    try {
      await tenantSubscriptionsApi.delete(deleting.tenantSubscriptionsId);
      setDeleteOpen(false);
      load();
    } catch (err) {
      alert(err instanceof Error ? err.message : "Failed to inactive subscription");
    } finally {
      setSaving(false);
    }
  };

  const paymentStatusVariant = (s: string) =>
    s === "Paid" ? "success" : s === "Pending" ? "warning" : s === "Failed" ? "danger" : "default";
  const subscriptionStatusVariant = (s: string) =>
    s === "Active" ? "success" : s === "Pending" ? "warning" : "default";

  return (
    <div>
      <PageHeader title="Tenant Subscriptions" description="Manage tenant subscription records"
        action={<Button onClick={openCreate}><Plus size={16} /> Add Subscription</Button>} />
      {error && <Alert message={error} />}
      <p className="mb-4 text-xs text-slate-500"><span className="text-red-500">*</span> Required field</p>

      <Card>
        {loading ? <LoadingSpinner /> : items.length === 0 ? <EmptyState message="No subscriptions found" /> : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-slate-100 text-left text-slate-500">
                  <th className="pb-3 font-medium">Tenant</th>
                  <th className="pb-3 font-medium">Plan</th>
                  <th className="pb-3 font-medium">Period</th>
                  <th className="pb-3 font-medium">Amount</th>
                  <th className="pb-3 font-medium">Payment</th>
                  <th className="pb-3 font-medium">Status</th>
                  <th className="pb-3 font-medium">Actions</th>
                </tr>
              </thead>
              <tbody>
                {items.map((item) => (
                  <tr key={item.tenantSubscriptionsId} className="border-b border-slate-50 hover:bg-slate-50">
                    <td className="py-3 font-medium">{item.tenantName}</td>
                    <td className="py-3">{item.planName}</td>
                    <td className="py-3 text-slate-500">
                      {new Date(item.startDate).toLocaleDateString()} - {new Date(item.endDate).toLocaleDateString()}
                    </td>
                    <td className="py-3">₹{item.amount.toLocaleString()}</td>
                    <td className="py-3"><Badge variant={paymentStatusVariant(item.paymentStatus)}>{item.paymentStatus}</Badge></td>
                    <td className="py-3"><Badge variant={subscriptionStatusVariant(item.subscriptionStatus)}>{item.subscriptionStatus}</Badge></td>
                    <td className="py-3">
                      <div className="flex gap-2">
                        <button onClick={() => {
                          setEditing(item);
                          setForm({
                            tenantId: item.tenantId, planId: item.planId,
                            startDate: item.startDate.split("T")[0], endDate: item.endDate.split("T")[0],
                            nextBillingDate: item.nextBillingDate?.split("T")[0] || "",
                            amount: item.amount, paymentStatus: item.paymentStatus,
                            subscriptionStatus: item.subscriptionStatus,
                          });
                          setErrors({});
                          setModalOpen(true);
                        }} className="rounded p-1 text-slate-400 hover:text-blue-600"><Edit size={16} /></button>
                        {item.subscriptionStatus !== "Inactive" && (
                          <button onClick={() => { setDeleting(item); setDeleteOpen(true); }} className="rounded p-1 text-slate-400 hover:text-amber-600"><Ban size={16} /></button>
                        )}
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
        <Pagination page={page} totalPages={totalPages} totalCount={totalCount} onPageChange={setPage} />
      </Card>

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title={editing ? "Edit Subscription" : "Add Subscription"} size="lg">
        <div className="grid gap-4 sm:grid-cols-2">
          {!editing && (
            <>
              <Select label="Tenant" required value={form.tenantId} error={errors.tenantId}
                onChange={(e) => {
                  const value = e.target.value;
                  setForm({ ...form, tenantId: value });
                  patchFieldError(setErrors, "tenantId", requiredSelect(value, "Tenant"));
                }}
                options={[{ value: "", label: "Select tenant *" }, ...tenants.map((t) => ({ value: t.tenantId, label: t.companyName }))]} />
              <Select label="Plan" required value={form.planId} error={errors.planId}
                onChange={(e) => applyPlanDates(e.target.value)}
                options={[{ value: "", label: "Select plan *" }, ...plans.map((p) => ({
                  value: p.planId,
                  label: `${p.planName} (${p.durationDays} days - ₹${p.price})`,
                }))]} />
              <Input label="Start Date" type="date" required value={form.startDate} error={errors.startDate}
                disabled={freePlan}
                onChange={(e) => selectedPlan ? applyPlanDates(form.planId, e.target.value) : setForm({ ...form, startDate: e.target.value })} />
            </>
          )}
          <Input label="End Date" type="date" value={form.endDate} error={errors.endDate}
            disabled={freePlan || !editing && !form.planId}
            onChange={(e) => {
              const value = e.target.value;
              setForm({ ...form, endDate: value });
              patchFieldError(setErrors, "endDate", value ? undefined : "End date is required");
            }} />
          <Input label="Next Billing Date" type="date" value={form.nextBillingDate}
            disabled={freePlan || !editing && !form.planId}
            onChange={(e) => setForm({ ...form, nextBillingDate: e.target.value })} />
          <Input label="Amount (₹)" type="number" value={form.amount} error={errors.amount}
            disabled={freePlan || !editing && !form.planId}
            onChange={(e) => {
              const value = Number(e.target.value);
              setForm({ ...form, amount: value });
              patchFieldError(setErrors, "amount", positiveNumber(value, "Amount", true));
            }} />
          <Select label="Payment Status" value={form.paymentStatus} disabled
            onChange={(e) => setForm({ ...form, paymentStatus: e.target.value })}
            options={["Pending", "Paid", "Failed", "Refunded"].map((s) => ({ value: s, label: s }))} />
          <p className="text-xs text-slate-500 sm:col-span-2">Payment status is updated automatically from the Payments page.</p>
          <Select label="Subscription Status" value={form.subscriptionStatus}
            onChange={(e) => setForm({ ...form, subscriptionStatus: e.target.value })}
            options={["Active", "Expired", "Cancelled", "Inactive", "Suspended"].map((s) => ({ value: s, label: s }))} />
        </div>
        {freePlan && !editing && (
          <p className="mt-3 rounded-lg bg-blue-50 px-3 py-2 text-xs text-blue-700">
            Free plan selected — billing dates and amount are auto-set and disabled.
          </p>
        )}
        <div className="mt-6 flex justify-end gap-3">
          <Button variant="secondary" onClick={() => setModalOpen(false)}>Cancel</Button>
          <Button onClick={handleSave} disabled={saving}>{saving ? "Saving..." : "Save"}</Button>
        </div>
      </Modal>

      <ConfirmModal open={deleteOpen} onClose={() => setDeleteOpen(false)} onConfirm={handleDeactivate}
        title="Inactive Subscription"
        message={`Are you sure you want to inactive subscription for "${deleting?.tenantName}"?`}
        loading={saving} />
    </div>
  );
}
