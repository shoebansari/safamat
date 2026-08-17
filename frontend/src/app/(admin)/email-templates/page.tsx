"use client";

import { useCallback, useEffect, useState } from "react";
import { Ban, Edit, Plus } from "lucide-react";
import { emailTemplatesApi } from "@/lib/services";
import type { EmailTemplate } from "@/lib/types";
import type { FieldErrors } from "@/lib/validation";
import { hasErrors, maxLength, patchFieldError, required } from "@/lib/validation";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Textarea } from "@/components/ui/Textarea";
import { Card } from "@/components/ui/Card";
import { PageHeader } from "@/components/ui/PageHeader";
import { Modal, ConfirmModal } from "@/components/ui/Modal";
import { Pagination } from "@/components/ui/Pagination";
import { StatusBadge } from "@/components/ui/Badge";
import { Alert, EmptyState, LoadingSpinner } from "@/components/ui/LoadingSpinner";

const emptyForm = { templateName: "", subject: "", body: "", isActive: true };

export default function EmailTemplatesPage() {
  const [items, setItems] = useState<EmailTemplate[]>([]);
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [modalOpen, setModalOpen] = useState(false);
  const [deleteOpen, setDeleteOpen] = useState(false);
  const [editing, setEditing] = useState<EmailTemplate | null>(null);
  const [deleting, setDeleting] = useState<EmailTemplate | null>(null);
  const [form, setForm] = useState(emptyForm);
  const [errors, setErrors] = useState<FieldErrors>({});
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const result = await emailTemplatesApi.list(page, 10);
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
    if (!editing) e.templateName = required(form.templateName, "Template name") || maxLength(form.templateName, 200, "Template name");
    e.subject = required(form.subject, "Subject") || maxLength(form.subject, 500, "Subject");
    e.body = required(form.body, "Body");
    Object.keys(e).forEach((k) => !e[k] && delete e[k]);
    setErrors(e);
    return !hasErrors(e);
  };

  const handleSave = async () => {
    if (!validate()) return;
    setSaving(true);
    try {
      if (editing) await emailTemplatesApi.update(editing.templateId, { subject: form.subject, body: form.body, isActive: form.isActive });
      else await emailTemplatesApi.create(form);
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
      await emailTemplatesApi.delete(deleting.templateId);
      setDeleteOpen(false);
      load();
    } catch (err) {
      alert(err instanceof Error ? err.message : "Failed to inactive template");
    } finally {
      setSaving(false);
    }
  };

  return (
    <div>
      <PageHeader title="Email Templates" description="Manage email notification templates"
        action={<Button onClick={() => { setEditing(null); setForm(emptyForm); setErrors({}); setModalOpen(true); }}><Plus size={16} /> Add Template</Button>} />
      {error && <Alert message={error} />}
      <p className="mb-4 text-xs text-slate-500"><span className="text-red-500">*</span> Required field</p>

      <Card>
        {loading ? <LoadingSpinner /> : items.length === 0 ? <EmptyState message="No templates found" /> : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-slate-100 text-left text-slate-500">
                  <th className="pb-3 font-medium">Template Name</th>
                  <th className="pb-3 font-medium">Subject</th>
                  <th className="pb-3 font-medium">Status</th>
                  <th className="pb-3 font-medium">Actions</th>
                </tr>
              </thead>
              <tbody>
                {items.map((item) => (
                  <tr key={item.templateId} className="border-b border-slate-50 hover:bg-slate-50">
                    <td className="py-3 font-medium">{item.templateName}</td>
                    <td className="py-3">{item.subject}</td>
                    <td className="py-3"><StatusBadge active={item.isActive} /></td>
                    <td className="py-3">
                      <div className="flex gap-2">
                        <button onClick={() => { setEditing(item); setForm({ templateName: item.templateName, subject: item.subject, body: item.body, isActive: item.isActive }); setErrors({}); setModalOpen(true); }} className="rounded p-1 text-slate-400 hover:text-blue-600"><Edit size={16} /></button>
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

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title={editing ? "Edit Template" : "Add Template"} size="lg">
        <div className="grid gap-4">
          {!editing && <Input label="Template Name" required value={form.templateName} error={errors.templateName}
            onChange={(e) => {
              const value = e.target.value;
              setForm({ ...form, templateName: value });
              patchFieldError(setErrors, "templateName", required(value, "Template name") || maxLength(value, 200, "Template name"));
            }} maxLength={200} />}
          <Input label="Subject" required value={form.subject} error={errors.subject}
            onChange={(e) => {
              const value = e.target.value;
              setForm({ ...form, subject: value });
              patchFieldError(setErrors, "subject", required(value, "Subject") || maxLength(value, 500, "Subject"));
            }} maxLength={500} />
          <Textarea label="Body (HTML supported)" required value={form.body} error={errors.body}
            onChange={(e) => {
              const value = e.target.value;
              setForm({ ...form, body: value });
              patchFieldError(setErrors, "body", required(value, "Body"));
            }} rows={8} />
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
        title="Inactive Email Template" message={`Are you sure you want to inactive template "${deleting?.templateName}"?`} loading={saving} />
    </div>
  );
}
