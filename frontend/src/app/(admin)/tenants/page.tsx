"use client";

import { useCallback, useEffect, useState } from "react";
import { Ban, Edit, Plus, Search } from "lucide-react";
import { tenantsApi } from "@/lib/services";
import type { Tenant } from "@/lib/types";
import type { FieldErrors } from "@/lib/validation";
import { email, hasErrors, maxLength, minLength, patchFieldError, password, phone, required } from "@/lib/validation";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Card } from "@/components/ui/Card";
import { PageHeader } from "@/components/ui/PageHeader";
import { Modal, ConfirmModal } from "@/components/ui/Modal";
import { Pagination } from "@/components/ui/Pagination";
import { StatusBadge } from "@/components/ui/Badge";
import { Alert, EmptyState, LoadingSpinner } from "@/components/ui/LoadingSpinner";

const emptyForm = {
  tenantCode: "", companyName: "", ownerName: "", userName: "", password: "", email: "", phone: "",
  address: "", city: "", state: "", country: "", zipCode: "",
  logoUrl: "", databaseName: "", databaseServer: "", connectionString: "", isActive: true,
};

export default function TenantsPage() {
  const [items, setItems] = useState<Tenant[]>([]);
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [search, setSearch] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [modalOpen, setModalOpen] = useState(false);
  const [deleteOpen, setDeleteOpen] = useState(false);
  const [editing, setEditing] = useState<Tenant | null>(null);
  const [deleting, setDeleting] = useState<Tenant | null>(null);
  const [form, setForm] = useState(emptyForm);
  const [errors, setErrors] = useState<FieldErrors>({});
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError("");
    try {
      const result = await tenantsApi.list(page, 10, search);
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

  const validateTenantCode = (value: string) =>
    required(value, "Tenant code") || minLength(value, 2, "Tenant code");

  const validateCompanyName = (value: string) =>
    required(value, "Company name") || maxLength(value, 200, "Company name");

  const validateOwnerName = (value: string) =>
    required(value, "Owner name") || maxLength(value, 150, "Owner name");

  const validateUserName = (value: string) =>
    required(value, "Username") || minLength(value, 3, "Username") || maxLength(value, 100, "Username");

  const validatePassword = (value: string, isRequired: boolean) =>
    password(value, isRequired) || (value ? maxLength(value, 100, "Password") : undefined);

  const checkDuplicates = async (tenantCode?: string, companyName?: string, userName?: string) => {
    try {
      const result = await tenantsApi.exists(
        tenantCode,
        companyName,
        userName,
        editing?.tenantId
      );
      if (tenantCode) {
        patchFieldError(setErrors, "tenantCode", result.tenantCodeExists ? "Tenant code already exists" : undefined);
      }
      if (companyName) {
        patchFieldError(setErrors, "companyName", result.companyNameExists ? "Company name already exists" : undefined);
      }
      if (userName) {
        patchFieldError(setErrors, "userName", result.userNameExists ? "Username already exists" : undefined);
      }
    } catch {
      /* ignore lookup errors */
    }
  };

  const validate = (): boolean => {
    const e: FieldErrors = {};
    if (!editing) e.tenantCode = validateTenantCode(form.tenantCode);
    e.companyName = validateCompanyName(form.companyName);
    e.ownerName = validateOwnerName(form.ownerName);
    e.userName = validateUserName(form.userName);
    e.password = validatePassword(form.password, !editing);
    if (form.email) e.email = email(form.email);
    if (form.phone) e.phone = phone(form.phone);
    Object.keys(e).forEach((k) => !e[k] && delete e[k]);
    setErrors(e);
    return !hasErrors(e);
  };

  const updateField = (key: keyof typeof emptyForm, value: string | boolean) => {
    setForm((prev) => ({ ...prev, [key]: value }));
    if (key === "tenantCode" && typeof value === "string" && !editing) {
      patchFieldError(setErrors, "tenantCode", validateTenantCode(value));
    }
    if (key === "companyName" && typeof value === "string") {
      patchFieldError(setErrors, "companyName", validateCompanyName(value));
    }
    if (key === "ownerName" && typeof value === "string") {
      patchFieldError(setErrors, "ownerName", validateOwnerName(value));
    }
    if (key === "userName" && typeof value === "string") {
      patchFieldError(setErrors, "userName", validateUserName(value));
    }
    if (key === "password" && typeof value === "string") {
      patchFieldError(setErrors, "password", validatePassword(value, !editing));
    }
    if (key === "email" && typeof value === "string") {
      patchFieldError(setErrors, "email", value ? email(value) : undefined);
    }
    if (key === "phone" && typeof value === "string") {
      patchFieldError(setErrors, "phone", value ? phone(value) : undefined);
    }
  };

  const handleSave = async () => {
    if (!validate()) return;

    try {
      const result = await tenantsApi.exists(
        editing ? undefined : form.tenantCode.trim(),
        form.companyName.trim(),
        form.userName.trim(),
        editing?.tenantId
      );
      const dupErrors: FieldErrors = {};
      if (!editing && result.tenantCodeExists) dupErrors.tenantCode = "Tenant code already exists";
      if (result.companyNameExists) dupErrors.companyName = "Company name already exists";
      if (result.userNameExists) dupErrors.userName = "Username already exists";
      if (Object.keys(dupErrors).length) {
        setErrors((prev) => ({ ...prev, ...dupErrors }));
        return;
      }
    } catch (err) {
        alert(err instanceof Error ? err.message : "Could not verify tenant details");
        return;
      }

    setSaving(true);
    try {
      if (editing) {
        const { tenantCode, ...updateData } = form;
        void tenantCode;
        await tenantsApi.update(editing.tenantId, updateData);
      } else {
        await tenantsApi.create(form);
      }
      setModalOpen(false);
      load();
    } catch (err) {
      const message = err instanceof Error ? err.message : "Save failed";
      if (message.toLowerCase().includes("tenant code")) {
        patchFieldError(setErrors, "tenantCode", message);
      } else if (message.toLowerCase().includes("company name")) {
        patchFieldError(setErrors, "companyName", message);
      } else if (message.toLowerCase().includes("username")) {
        patchFieldError(setErrors, "userName", message);
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
      await tenantsApi.delete(deleting.tenantId);
      setDeleteOpen(false);
      load();
    } catch (err) {
      alert(err instanceof Error ? err.message : "Failed to inactive tenant");
    } finally {
      setSaving(false);
    }
  };

  return (
    <div>
      <PageHeader title="Tenants" description="Manage matrimonial business tenants"
        action={<Button onClick={() => { setEditing(null); setForm(emptyForm); setErrors({}); setModalOpen(true); }}><Plus size={16} /> Add Tenant</Button>} />
      {error && <Alert message={error} />}
      <p className="mb-4 text-xs text-slate-500"><span className="text-red-500">*</span> Required field</p>

      <Card>
        <div className="mb-4">
          <div className="relative">
            <Search size={16} className="absolute left-3 top-1/2 -translate-y-1/2 text-slate-400" />
            <input placeholder="Search tenants..." value={search}
              onChange={(e) => { setSearch(e.target.value); setPage(1); }}
              className="w-full rounded-lg border border-slate-300 py-2 pl-9 pr-3 text-sm outline-none focus:border-rose-500" />
          </div>
        </div>

        {loading ? <LoadingSpinner /> : items.length === 0 ? <EmptyState message="No tenants found" /> : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-slate-100 text-left text-slate-500">
                  <th className="pb-3 font-medium">Code</th>
                  <th className="pb-3 font-medium">Company</th>
                  <th className="pb-3 font-medium">Owner</th>
                  <th className="pb-3 font-medium">Username</th>
                  <th className="pb-3 font-medium">Email</th>
                  <th className="pb-3 font-medium">Status</th>
                  <th className="pb-3 font-medium">Actions</th>
                </tr>
              </thead>
              <tbody>
                {items.map((item) => (
                  <tr key={item.tenantId} className="border-b border-slate-50 hover:bg-slate-50">
                    <td className="py-3 font-medium">{item.tenantCode}</td>
                    <td className="py-3">{item.companyName}</td>
                    <td className="py-3">{item.ownerName}</td>
                    <td className="py-3">{item.userName}</td>
                    <td className="py-3">{item.email}</td>
                    <td className="py-3"><StatusBadge active={item.isActive} /></td>
                    <td className="py-3">
                      <div className="flex gap-2">
                        <button onClick={() => { setEditing(item); setForm({ tenantCode: item.tenantCode, companyName: item.companyName, ownerName: item.ownerName, userName: item.userName, password: item.password, email: item.email, phone: item.phone || "", address: item.address || "", city: item.city || "", state: item.state || "", country: item.country || "", zipCode: item.zipCode || "", logoUrl: item.logoUrl || "", databaseName: item.databaseName || "", databaseServer: item.databaseServer || "", connectionString: "", isActive: item.isActive }); setErrors({}); setModalOpen(true); }} className="rounded p-1 text-slate-400 hover:text-blue-600"><Edit size={16} /></button>
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

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title={editing ? "Edit Tenant" : "Add Tenant"} size="xl">
        <div className="grid gap-4 sm:grid-cols-2">
          {!editing && (
            <Input label="Tenant Code" required value={form.tenantCode} error={errors.tenantCode}
              onChange={(e) => updateField("tenantCode", e.target.value)}
              onBlur={(e) => {
                if (!validateTenantCode(e.target.value)) checkDuplicates(e.target.value, undefined);
              }}
              maxLength={50} />
          )}
          <Input label="Company Name" required value={form.companyName} error={errors.companyName}
            onChange={(e) => updateField("companyName", e.target.value)}
            onBlur={(e) => {
              if (!validateCompanyName(e.target.value)) checkDuplicates(undefined, e.target.value);
            }}
            maxLength={200} />
          <Input label="Owner Name" required value={form.ownerName} error={errors.ownerName}
            onChange={(e) => updateField("ownerName", e.target.value)} maxLength={150} />
          <Input label="Username" required value={form.userName} error={errors.userName}
            onChange={(e) => updateField("userName", e.target.value)}
            onBlur={(e) => {
              if (!validateUserName(e.target.value)) checkDuplicates(undefined, undefined, e.target.value);
            }}
            maxLength={100} />
          <Input label="Password" type="text" required={!editing} value={form.password} error={errors.password}
            onChange={(e) => updateField("password", e.target.value)} maxLength={100} />
          <Input label="Email" type="email" value={form.email} error={errors.email}
            onChange={(e) => updateField("email", e.target.value)} maxLength={200} />
          <Input label="Phone" value={form.phone} error={errors.phone}
            onChange={(e) => updateField("phone", e.target.value)} maxLength={20} />
          <Input label="City" value={form.city} onChange={(e) => updateField("city", e.target.value)} />
          <Input label="State" value={form.state} onChange={(e) => updateField("state", e.target.value)} />
          <Input label="Country" value={form.country} onChange={(e) => updateField("country", e.target.value)} />
          <Input label="Zip Code" value={form.zipCode} onChange={(e) => updateField("zipCode", e.target.value)} />
          <Input label="Address" value={form.address} onChange={(e) => updateField("address", e.target.value)} className="sm:col-span-2" />
          <label className="flex items-center gap-2 sm:col-span-2">
            <input type="checkbox" checked={form.isActive} onChange={(e) => updateField("isActive", e.target.checked)} />
            <span className="text-sm">Active</span>
          </label>
        </div>
        <div className="mt-6 flex justify-end gap-3">
          <Button variant="secondary" onClick={() => setModalOpen(false)}>Cancel</Button>
          <Button onClick={handleSave} disabled={saving}>{saving ? "Saving..." : "Save"}</Button>
        </div>
      </Modal>

      <ConfirmModal open={deleteOpen} onClose={() => setDeleteOpen(false)} onConfirm={handleDeactivate}
        title="Inactive Tenant" message={`Are you sure you want to inactive tenant "${deleting?.companyName}"?`} loading={saving} />
    </div>
  );
}
