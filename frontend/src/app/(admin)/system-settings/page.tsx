"use client";

import { useCallback, useEffect, useState } from "react";
import { Edit, Plus, Trash2 } from "lucide-react";
import { systemSettingsApi } from "@/lib/services";
import type { SystemSetting } from "@/lib/types";
import type { FieldErrors } from "@/lib/validation";
import { hasErrors, maxLength, patchFieldError, required } from "@/lib/validation";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Textarea } from "@/components/ui/Textarea";
import { Card } from "@/components/ui/Card";
import { PageHeader } from "@/components/ui/PageHeader";
import { Modal, ConfirmModal } from "@/components/ui/Modal";
import { Pagination } from "@/components/ui/Pagination";
import { Alert, EmptyState, LoadingSpinner } from "@/components/ui/LoadingSpinner";

const emptyForm = { settingKey: "", settingValue: "" };

export default function SystemSettingsPage() {
  const [items, setItems] = useState<SystemSetting[]>([]);
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [search, setSearch] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [modalOpen, setModalOpen] = useState(false);
  const [deleteOpen, setDeleteOpen] = useState(false);
  const [editing, setEditing] = useState<SystemSetting | null>(null);
  const [deleting, setDeleting] = useState<SystemSetting | null>(null);
  const [form, setForm] = useState(emptyForm);
  const [errors, setErrors] = useState<FieldErrors>({});
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const result = await systemSettingsApi.list(page, 10, search);
      setItems(result.items);
      setTotalCount(result.totalCount);
      setTotalPages(result.totalPages);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load");
    } finally {
      setLoading(false);
    }
  }, [page, search]);

  useEffect(() => { load(); }, [load]);

  const validate = (): boolean => {
    const e: FieldErrors = {};
    if (!editing) e.settingKey = required(form.settingKey, "Setting key") || maxLength(form.settingKey, 200, "Setting key");
    e.settingValue = required(form.settingValue, "Setting value");
    Object.keys(e).forEach((k) => !e[k] && delete e[k]);
    setErrors(e);
    return !hasErrors(e);
  };

  const handleSave = async () => {
    if (!validate()) return;
    setSaving(true);
    try {
      if (editing) await systemSettingsApi.update(editing.settingId, { settingValue: form.settingValue });
      else await systemSettingsApi.create(form);
      setModalOpen(false);
      load();
    } catch (err) {
      alert(err instanceof Error ? err.message : "Save failed");
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async () => {
    if (!deleting) return;
    setSaving(true);
    try {
      await systemSettingsApi.delete(deleting.settingId);
      setDeleteOpen(false);
      load();
    } catch (err) {
      alert(err instanceof Error ? err.message : "Delete failed");
    } finally {
      setSaving(false);
    }
  };

  return (
    <div>
      <PageHeader title="System Settings" description="Global platform configuration key-value store"
        action={<Button onClick={() => { setEditing(null); setForm(emptyForm); setErrors({}); setModalOpen(true); }}><Plus size={16} /> Add Setting</Button>} />
      {error && <Alert message={error} />}
      <p className="mb-4 text-xs text-slate-500"><span className="text-red-500">*</span> Required field</p>

      <Card>
        <div className="mb-4">
          <input placeholder="Search settings..." value={search}
            onChange={(e) => { setSearch(e.target.value); setPage(1); }}
            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm outline-none focus:border-rose-500" />
        </div>
        {loading ? <LoadingSpinner /> : items.length === 0 ? <EmptyState message="No settings found" /> : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-slate-100 text-left text-slate-500">
                  <th className="pb-3 font-medium">Key</th>
                  <th className="pb-3 font-medium">Value</th>
                  <th className="pb-3 font-medium">Actions</th>
                </tr>
              </thead>
              <tbody>
                {items.map((item) => (
                  <tr key={item.settingId} className="border-b border-slate-50 hover:bg-slate-50">
                    <td className="py-3 font-mono font-medium text-rose-600">{item.settingKey}</td>
                    <td className="py-3 max-w-md truncate">{item.settingValue}</td>
                    <td className="py-3">
                      <div className="flex gap-2">
                        <button onClick={() => { setEditing(item); setForm({ settingKey: item.settingKey, settingValue: item.settingValue }); setErrors({}); setModalOpen(true); }} className="rounded p-1 text-slate-400 hover:text-blue-600"><Edit size={16} /></button>
                        <button onClick={() => { setDeleting(item); setDeleteOpen(true); }} className="rounded p-1 text-slate-400 hover:text-red-600"><Trash2 size={16} /></button>
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

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title={editing ? "Edit Setting" : "Add Setting"}>
        <div className="grid gap-4">
          {!editing && <Input label="Setting Key" required value={form.settingKey} error={errors.settingKey}
            onChange={(e) => {
              const value = e.target.value;
              setForm({ ...form, settingKey: value });
              patchFieldError(setErrors, "settingKey", required(value, "Setting key") || maxLength(value, 200, "Setting key"));
            }} maxLength={200} />}
          {editing && <Input label="Setting Key" value={form.settingKey} disabled />}
          <Textarea label="Setting Value" required value={form.settingValue} error={errors.settingValue}
            onChange={(e) => {
              const value = e.target.value;
              setForm({ ...form, settingValue: value });
              patchFieldError(setErrors, "settingValue", required(value, "Setting value"));
            }} rows={4} />
        </div>
        <div className="mt-6 flex justify-end gap-3">
          <Button variant="secondary" onClick={() => setModalOpen(false)}>Cancel</Button>
          <Button onClick={handleSave} disabled={saving}>{saving ? "Saving..." : "Save"}</Button>
        </div>
      </Modal>

      <ConfirmModal open={deleteOpen} onClose={() => setDeleteOpen(false)} onConfirm={handleDelete}
        title="Delete Setting" message={`Are you sure you want to delete setting "${deleting?.settingKey}"? This cannot be undone.`}
        confirmLabel="Delete" loadingLabel="Deleting..." loading={saving} />
    </div>
  );
}
