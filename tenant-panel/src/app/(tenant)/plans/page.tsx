"use client";

import { useCallback, useEffect, useState } from "react";
import { Ban, Edit, Plus } from "lucide-react";
import { memberPlansApi } from "@/lib/services";
import type { MemberPlan } from "@/lib/types";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Textarea } from "@/components/ui/Textarea";
import { Card } from "@/components/ui/Card";
import { PageHeader } from "@/components/ui/PageHeader";
import { Modal, ConfirmModal } from "@/components/ui/Modal";
import { StatusBadge } from "@/components/ui/Badge";
import { Alert, EmptyState, LoadingSpinner } from "@/components/ui/LoadingSpinner";

const emptyForm = { planName: "", description: "", price: 0, durationDays: 30, isActive: true };

export default function PlansPage() {
  const [items, setItems] = useState<MemberPlan[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [modalOpen, setModalOpen] = useState(false);
  const [deleteOpen, setDeleteOpen] = useState(false);
  const [editing, setEditing] = useState<MemberPlan | null>(null);
  const [deleting, setDeleting] = useState<MemberPlan | null>(null);
  const [form, setForm] = useState(emptyForm);
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      setItems(await memberPlansApi.list());
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load plans");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    load();
  }, [load]);

  const openCreate = () => {
    setEditing(null);
    setForm(emptyForm);
    setModalOpen(true);
  };

  const openEdit = (item: MemberPlan) => {
    setEditing(item);
    setForm({
      planName: item.planName,
      description: item.description,
      price: item.price,
      durationDays: item.durationDays,
      isActive: item.isActive,
    });
    setModalOpen(true);
  };

  const handleSave = async () => {
    if (!form.planName.trim()) {
      alert("Plan name is required");
      return;
    }
    setSaving(true);
    try {
      if (editing) await memberPlansApi.update(editing.memberPlanId, form);
      else await memberPlansApi.create(form);
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
      await memberPlansApi.delete(deleting.memberPlanId);
      setDeleteOpen(false);
      load();
    } catch (err) {
      alert(err instanceof Error ? err.message : "Failed to deactivate plan");
    } finally {
      setSaving(false);
    }
  };

  return (
    <div>
      <PageHeader
        title="Member Plans"
        description="Create plans that members can subscribe to"
        action={
          <Button onClick={openCreate}>
            <Plus size={16} /> Add Plan
          </Button>
        }
      />
      {error && <Alert message={error} />}

      <Card>
        {loading ? (
          <LoadingSpinner />
        ) : items.length === 0 ? (
          <EmptyState message="No plans yet. Create your first plan." />
        ) : (
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
                  <tr key={item.memberPlanId} className="border-b border-slate-50 hover:bg-slate-50">
                    <td className="py-3">
                      <p className="font-medium">{item.planName}</p>
                      <p className="text-xs text-slate-400">{item.description}</p>
                    </td>
                    <td className="py-3">₹{item.price.toLocaleString()}</td>
                    <td className="py-3">{item.durationDays} days</td>
                    <td className="py-3">
                      <StatusBadge active={item.isActive} />
                    </td>
                    <td className="py-3">
                      <div className="flex gap-2">
                        <Button variant="ghost" size="sm" onClick={() => openEdit(item)}>
                          <Edit size={14} />
                        </Button>
                        {item.isActive && (
                          <Button
                            variant="ghost"
                            size="sm"
                            onClick={() => {
                              setDeleting(item);
                              setDeleteOpen(true);
                            }}
                          >
                            <Ban size={14} className="text-red-500" />
                          </Button>
                        )}
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </Card>

      <Modal
        open={modalOpen}
        onClose={() => setModalOpen(false)}
        title={editing ? "Edit Plan" : "Create Plan"}
      >
        <div className="space-y-4">
          <Input
            label="Plan Name"
            value={form.planName}
            onChange={(e) => setForm({ ...form, planName: e.target.value })}
            required
          />
          <Textarea
            label="Description"
            value={form.description}
            onChange={(e) => setForm({ ...form, description: e.target.value })}
          />
          <div className="grid grid-cols-2 gap-4">
            <Input
              label="Price (₹)"
              type="number"
              min={0}
              value={form.price}
              onChange={(e) => setForm({ ...form, price: Number(e.target.value) })}
            />
            <Input
              label="Duration (days)"
              type="number"
              min={1}
              value={form.durationDays}
              onChange={(e) => setForm({ ...form, durationDays: Number(e.target.value) })}
            />
          </div>
        </div>
        <div className="mt-6 flex justify-end gap-3">
          <Button variant="secondary" onClick={() => setModalOpen(false)}>
            Cancel
          </Button>
          <Button onClick={handleSave} disabled={saving}>
            {saving ? "Saving..." : "Save"}
          </Button>
        </div>
      </Modal>

      <ConfirmModal
        open={deleteOpen}
        onClose={() => setDeleteOpen(false)}
        onConfirm={handleDeactivate}
        title="Deactivate Plan"
        message={`Are you sure you want to deactivate "${deleting?.planName}"?`}
        confirmLabel="Deactivate"
        loading={saving}
      />
    </div>
  );
}
