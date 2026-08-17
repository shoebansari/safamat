"use client";

import { useCallback, useEffect, useState } from "react";
import { Ban, Edit, Plus, Search } from "lucide-react";
import { adminUsersApi } from "@/lib/services";
import type { AdminUser } from "@/lib/types";
import type { FieldErrors } from "@/lib/validation";
import { email, hasErrors, maxLength, minLength, password, patchFieldError, phone, required } from "@/lib/validation";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Card } from "@/components/ui/Card";
import { PageHeader } from "@/components/ui/PageHeader";
import { Modal, ConfirmModal } from "@/components/ui/Modal";
import { Pagination } from "@/components/ui/Pagination";
import { StatusBadge } from "@/components/ui/Badge";
import { Alert, EmptyState, LoadingSpinner } from "@/components/ui/LoadingSpinner";

const emptyForm = {
  adminUserName: "",
  password: "",
  firstName: "",
  lastName: "",
  email: "",
  phone: "",
  isActive: true,
};

export default function AdminUsersPage() {
  const [items, setItems] = useState<AdminUser[]>([]);
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [search, setSearch] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [modalOpen, setModalOpen] = useState(false);
  const [deleteOpen, setDeleteOpen] = useState(false);
  const [editing, setEditing] = useState<AdminUser | null>(null);
  const [deleting, setDeleting] = useState<AdminUser | null>(null);
  const [form, setForm] = useState(emptyForm);
  const [errors, setErrors] = useState<FieldErrors>({});
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      const result = await adminUsersApi.list(page, 10, search);
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

  const validateUsername = (value: string) =>
    required(value, "Username") || minLength(value, 3, "Username");

  const validatePassword = (value: string, isRequired: boolean) => password(value, isRequired);

  const validateFirstName = (value: string) =>
    required(value, "First name") || maxLength(value, 100, "First name");

  const checkUsernameAvailable = async (username: string) => {
    if (!username.trim() || validateUsername(username)) return;
    try {
      const exists = await adminUsersApi.usernameExists(username.trim());
      patchFieldError(setErrors, "adminUserName", exists ? "Username already exists" : undefined);
    } catch {
      /* ignore lookup errors */
    }
  };

  const validate = (): boolean => {
    const e: FieldErrors = {};
    if (!editing) {
      e.adminUserName = validateUsername(form.adminUserName);
      e.password = validatePassword(form.password, true);
    } else if (form.password) {
      e.password = validatePassword(form.password, false);
    }
    e.firstName = validateFirstName(form.firstName);
    if (form.lastName) e.lastName = maxLength(form.lastName, 100, "Last name");
    if (form.email) e.email = email(form.email);
    if (form.phone) e.phone = phone(form.phone);
    Object.keys(e).forEach((k) => !e[k] && delete e[k]);
    setErrors(e);
    return !hasErrors(e);
  };

  const openCreate = () => {
    setEditing(null);
    setForm(emptyForm);
    setErrors({});
    setModalOpen(true);
  };

  const openEdit = (item: AdminUser) => {
    setEditing(item);
    setForm({
      adminUserName: item.adminUserName,
      password: "",
      firstName: item.firstName,
      lastName: item.lastName,
      email: item.email,
      phone: item.phone || "",
      isActive: item.isActive,
    });
    setErrors({});
    setModalOpen(true);
  };

  const handleSave = async () => {
    if (!validate()) return;

    if (!editing) {
      try {
        const exists = await adminUsersApi.usernameExists(form.adminUserName.trim());
        if (exists) {
          patchFieldError(setErrors, "adminUserName", "Username already exists");
          return;
        }
      } catch (err) {
        alert(err instanceof Error ? err.message : "Could not verify username");
        return;
      }
    }

    setSaving(true);
    try {
      if (editing) {
        await adminUsersApi.update(editing.adminId, {
          firstName: form.firstName.trim(),
          lastName: form.lastName.trim(),
          email: form.email.trim(),
          phone: form.phone.trim() || undefined,
          isActive: form.isActive,
          ...(form.password ? { password: form.password } : {}),
        });
      } else {
        await adminUsersApi.create({
          adminUserName: form.adminUserName.trim(),
          password: form.password,
          firstName: form.firstName.trim(),
          lastName: form.lastName.trim(),
          email: form.email.trim() || `${form.adminUserName.trim()}@matrimonial.local`,
          phone: form.phone.trim() || undefined,
          isActive: form.isActive,
        });
      }
      setModalOpen(false);
      load();
    } catch (err) {
      const message = err instanceof Error ? err.message : "Save failed";
      if (message.toLowerCase().includes("username")) {
        patchFieldError(setErrors, "adminUserName", message);
      } else {
        alert(message);
      }
    } finally {
      setSaving(false);
    }
  };

  const handleDeactivate = async () => {
    if (!deleting) return;
    setSaving(true);
    try {
      await adminUsersApi.delete(deleting.adminId);
      setDeleteOpen(false);
      load();
    } catch (err) {
      alert(err instanceof Error ? err.message : "Failed to inactive user");
    } finally {
      setSaving(false);
    }
  };

  return (
    <div>
      <PageHeader title="Admin Users" description="Manage platform administrator accounts"
        action={<Button onClick={openCreate}><Plus size={16} /> Add Admin</Button>} />
      {error && <Alert message={error} />}
      <p className="mb-4 text-xs text-slate-500"><span className="text-red-500">*</span> Required field</p>

      <Card>
        <div className="mb-4 flex gap-3">
          <div className="relative flex-1">
            <Search size={16} className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" />
            <input placeholder="Search users..." value={search}
              onChange={(e) => { setSearch(e.target.value); setPage(1); }}
              className="w-full rounded-lg border border-slate-300 py-2 pl-9 pr-3 text-sm outline-none focus:border-rose-500" />
          </div>
        </div>

        {loading ? <LoadingSpinner /> : items.length === 0 ? <EmptyState message="No admin users found" /> : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-slate-100 text-left text-slate-500">
                  <th className="pb-3 font-medium">Username</th>
                  <th className="pb-3 font-medium">Name</th>
                  <th className="pb-3 font-medium">Email</th>
                  <th className="pb-3 font-medium">Phone</th>
                  <th className="pb-3 font-medium">Status</th>
                  <th className="pb-3 font-medium">Actions</th>
                </tr>
              </thead>
              <tbody>
                {items.map((item) => (
                  <tr key={item.adminId} className="border-b border-slate-50 hover:bg-slate-50">
                    <td className="py-3 font-medium">{item.adminUserName}</td>
                    <td className="py-3">{item.firstName} {item.lastName}</td>
                    <td className="py-3">{item.email}</td>
                    <td className="py-3">{item.phone || "-"}</td>
                    <td className="py-3"><StatusBadge active={item.isActive} /></td>
                    <td className="py-3">
                      <div className="flex gap-2">
                        <button onClick={() => openEdit(item)} className="rounded p-1 text-slate-400 hover:text-blue-600"><Edit size={16} /></button>
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

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title={editing ? "Edit Admin User" : "Add Admin User"}>
        <div className="grid gap-4 sm:grid-cols-2">
          {!editing && (
            <Input label="Username" value={form.adminUserName} error={errors.adminUserName}
              onChange={(e) => {
                const value = e.target.value;
                setForm({ ...form, adminUserName: value });
                patchFieldError(setErrors, "adminUserName", validateUsername(value));
              }}
              onBlur={(e) => checkUsernameAvailable(e.target.value)}
              required maxLength={100} />
          )}
          <Input label={editing ? "New Password (optional)" : "Password"} type="password" value={form.password}
            error={errors.password}
            onChange={(e) => {
              const value = e.target.value;
              setForm({ ...form, password: value });
              patchFieldError(setErrors, "password", validatePassword(value, !editing));
            }}
            required={!editing} />
          <Input label="First Name" value={form.firstName} error={errors.firstName}
            onChange={(e) => {
              const value = e.target.value;
              setForm({ ...form, firstName: value });
              patchFieldError(setErrors, "firstName", validateFirstName(value));
            }}
            required maxLength={100} />
          <Input label="Last Name" value={form.lastName} error={errors.lastName}
            onChange={(e) => {
              const value = e.target.value;
              setForm({ ...form, lastName: value });
              patchFieldError(setErrors, "lastName", value ? maxLength(value, 100, "Last name") : undefined);
            }}
            maxLength={100} />
          <Input label="Email" type="email" value={form.email} error={errors.email}
            onChange={(e) => {
              const value = e.target.value;
              setForm({ ...form, email: value });
              patchFieldError(setErrors, "email", value ? email(value) : undefined);
            }}
            maxLength={200} />
          <Input label="Phone" value={form.phone} error={errors.phone}
            onChange={(e) => {
              const value = e.target.value;
              setForm({ ...form, phone: value });
              patchFieldError(setErrors, "phone", value ? phone(value) : undefined);
            }}
            maxLength={20} />
          <label className="flex items-center gap-2 sm:col-span-2">
            <input type="checkbox" checked={form.isActive} onChange={(e) => setForm({ ...form, isActive: e.target.checked })} />
            <span className="text-sm text-slate-700">Active</span>
          </label>
        </div>
        <div className="mt-6 flex justify-end gap-3">
          <Button variant="secondary" onClick={() => setModalOpen(false)}>Cancel</Button>
          <Button onClick={handleSave} disabled={saving}>{saving ? "Saving..." : "Save"}</Button>
        </div>
      </Modal>

      <ConfirmModal open={deleteOpen} onClose={() => setDeleteOpen(false)} onConfirm={handleDeactivate}
        title="Inactive Admin User"
        message={`Are you sure you want to inactive user "${deleting?.adminUserName}"? They will no longer be able to login.`}
        loading={saving} />
    </div>
  );
}
