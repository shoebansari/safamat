"use client";

import { useCallback, useEffect, useState } from "react";
import { Ban, Edit, Plus } from "lucide-react";
import { subscriptionPlansApi } from "@/lib/services";
import type { SubscriptionPlan } from "@/lib/types";
import type { FieldErrors } from "@/lib/validation";
import { hasErrors, maxLength, patchFieldError, positiveNumber, required } from "@/lib/validation";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Textarea } from "@/components/ui/Textarea";
import { Card } from "@/components/ui/Card";
import { PageHeader } from "@/components/ui/PageHeader";
import { Modal, ConfirmModal } from "@/components/ui/Modal";
import { Pagination } from "@/components/ui/Pagination";
import { StatusBadge } from "@/components/ui/Badge";
import { Alert, EmptyState, LoadingSpinner } from "@/components/ui/LoadingSpinner";

const emptyForm = { planName: "", description: "", price: 0, durationDays: 30, isActive: true };

export default function SubscriptionPlansPage() {
  const [items, setItems] = useState<SubscriptionPlan[]>([]);
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [modalOpen, setModalOpen] = useState(false);
  const [deleteOpen, setDeleteOpen] = useState(false);
  const [editing, setEditing] = useState<SubscriptionPlan | null>(null);
  const [deleting, setDeleting] = useState<SubscriptionPlan | null>(null);
  const [form, setForm] = useState(emptyForm);
  const [errors, setErrors] = useState<FieldErrors>({});
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const result = await subscriptionPlansApi.list(page, 10);
      setItems(result.items);
      setTotalCount(result.totalCount);
      setTotalPages(result.totalPages);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load");
    } finally {
      setLoading(false);
    }
  }, [page]);

  useEffect(() => { load(); }, [load]);

  const validate = (): boolean => {
    const e: FieldErrors = {};
    e.planName = required(form.planName, "Plan name") || maxLength(form.planName, 200, "Plan name");
    e.description = required(form.description, "Description") || maxLength(form.description, 500, "Description");
    if (form.price > 0) e.price = positiveNumber(form.price, "Price", true);
    if (form.durationDays <= 0) e.durationDays = "Duration must be at least 1 day";
    Object.keys(e).forEach((k) => !e[k] && delete e[k]);
    setErrors(e);
    return !hasErrors(e);
  };

  const handleSave = async () => {
    if (!validate()) return;
    setSaving(true);
    try {
      if (editing) await subscriptionPlansApi.update(editing.planId, form);
      else await subscriptionPlansApi.create(form);
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
      await subscriptionPlansApi.delete(deleting.planId);
      setDeleteOpen(false);
      load();
    } catch (err) {
      alert(err instanceof Error ? err.message : "Failed to inactive plan");
    } finally {
      setSaving(false);
    }
  };

  return (
    <div>
      <PageHeader title="Subscription Plans" description="Manage pricing and subscription plans"
        action={<Button onClick={() => { setEditing(null); setForm(emptyForm); setErrors({}); setModalOpen(true); }}><Plus size={16} /> Add Plan</Button>} />
      {error && <Alert message={error} />}
      <p className="mb-4 text-xs text-slate-500"><span className="text-red-500">*</span> Required field</p>

      <Card>
        {loading ? <LoadingSpinner /> : items.length === 0 ? <EmptyState message="No plans found" /> : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-slate-100 text-left text-slate-500">
                  <th className="pb-3 font-medium">Plan Name</th>
                  <th className="pb-3 font-medium">Price</th>
                  <th className="pb-3 font-medium">Duration</th>
                  <th className="pb-3 font-medium">Status</th>
                  <th className="pb-3 font-medium">Actions</th>
                </tr>
              </thead>
              <tbody>
                {items.map((item) => (
                  <tr key={item.planId} className="border-b border-slate-50 hover:bg-slate-50">
                    <td className="py-3">
                      <p className="font-medium">{item.planName}</p>
                      <p className="text-xs text-slate-400">{item.description}</p>
                    </td>
                    <td className="py-3">₹{item.price.toLocaleString()}</td>
                    <td className="py-3">{item.durationDays} days</td>
                    <td className="py-3"><StatusBadge active={item.isActive} /></td>
                    <td className="py-3">
                      <div className="flex gap-2">
                        <button onClick={() => { setEditing(item); setForm({ planName: item.planName, description: item.description || "", price: item.price, durationDays: item.durationDays, isActive: item.isActive }); setErrors({}); setModalOpen(true); }} className="rounded p-1 text-slate-400 hover:text-blue-600"><Edit size={16} /></button>
                        {item.isActive && (
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

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title={editing ? "Edit Plan" : "Add Plan"}>
        <div className="grid gap-4">
          <Input label="Plan Name" required value={form.planName} error={errors.planName}
            onChange={(e) => {
              const value = e.target.value;
              setForm({ ...form, planName: value });
              patchFieldError(setErrors, "planName", required(value, "Plan name") || maxLength(value, 200, "Plan name"));
            }} maxLength={200} />
          <Textarea label="Description" required value={form.description} error={errors.description}
            onChange={(e) => {
              const value = e.target.value;
              setForm({ ...form, description: value });
              patchFieldError(setErrors, "description", required(value, "Description") || maxLength(value, 500, "Description"));
            }} rows={3} maxLength={500} />
          <div className="grid gap-4 sm:grid-cols-2">
            <Input label="Price (₹)" type="number" value={form.price} error={errors.price}
              onChange={(e) => {
                const value = Number(e.target.value);
                setForm({ ...form, price: value });
                patchFieldError(setErrors, "price", value > 0 ? positiveNumber(value, "Price", true) : undefined);
              }} />
            <Input label="Duration (days)" type="number" required value={form.durationDays} error={errors.durationDays}
              onChange={(e) => {
                const value = Number(e.target.value);
                setForm({ ...form, durationDays: value });
                patchFieldError(setErrors, "durationDays", value <= 0 ? "Duration must be at least 1 day" : undefined);
              }} />
          </div>
          <p className="text-xs text-slate-500">Set price to 0 or name containing &quot;Free&quot; for a free plan.</p>
          <label className="flex items-center gap-2">
            <input type="checkbox" checked={form.isActive} onChange={(e) => setForm({ ...form, isActive: e.target.checked })} />
            <span className="text-sm">Active</span>
          </label>
        </div>
        <div className="mt-6 flex justify-end gap-3">
          <Button variant="secondary" onClick={() => setModalOpen(false)}>Cancel</Button>
          <Button onClick={handleSave} disabled={saving}>{saving ? "Saving..." : "Save"}</Button>
        </div>
      </Modal>

      <ConfirmModal open={deleteOpen} onClose={() => setDeleteOpen(false)} onConfirm={handleDeactivate}
        title="Inactive Plan" message={`Are you sure you want to inactive plan "${deleting?.planName}"?`} loading={saving} />
    </div>
  );
}
