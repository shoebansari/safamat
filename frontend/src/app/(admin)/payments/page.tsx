"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { Ban, Edit, Plus } from "lucide-react";
import { paymentsApi, tenantSubscriptionsApi } from "@/lib/services";
import type { Payment, TenantSubscription } from "@/lib/types";
import { todayISO } from "@/lib/subscription-utils";
import type { FieldErrors } from "@/lib/validation";
import { hasErrors, patchFieldError, positiveNumber, requiredSelect } from "@/lib/validation";
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
  subscriptionId: "", tenantId: "", amount: 0, currency: "INR",
  paymentMethod: "", transactionId: "", invoiceNumber: "", paymentGateway: "",
  status: "Pending", paidOn: todayISO(),
};

export default function PaymentsPage() {
  const [items, setItems] = useState<Payment[]>([]);
  const [subscriptions, setSubscriptions] = useState<TenantSubscription[]>([]);
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [modalOpen, setModalOpen] = useState(false);
  const [deleteOpen, setDeleteOpen] = useState(false);
  const [editing, setEditing] = useState<Payment | null>(null);
  const [deleting, setDeleting] = useState<Payment | null>(null);
  const [form, setForm] = useState(emptyForm);
  const [errors, setErrors] = useState<FieldErrors>({});
  const [saving, setSaving] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const result = await paymentsApi.list(page, 10);
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
    tenantSubscriptionsApi.list(1, 100).then((r) =>
      setSubscriptions(r.items.filter((s) => s.subscriptionStatus === "Active"))
    );
  }, [load]);

  /** Only tenants that have at least one active subscription */
  const tenantsWithSubscriptions = useMemo(() => {
    const map = new Map<string, string>();
    subscriptions.forEach((s) => {
      if (!map.has(s.tenantId)) map.set(s.tenantId, s.tenantName || "Unknown");
    });
    return Array.from(map.entries()).map(([tenantId, companyName]) => ({ tenantId, companyName }));
  }, [subscriptions]);

  const handleTenantChange = (tenantId: string) => {
    setForm((f) => ({ ...f, tenantId, subscriptionId: "", amount: 0 }));
    patchFieldError(setErrors, "tenantId", requiredSelect(tenantId, "Tenant"));
    patchFieldError(setErrors, "subscriptionId", undefined);
    patchFieldError(setErrors, "amount", undefined);
  };

  const handleSubscriptionChange = (subscriptionId: string) => {
    const sub = subscriptions.find((s) => s.tenantSubscriptionsId === subscriptionId);
    if (sub) {
      setForm((f) => ({
        ...f,
        subscriptionId,
        tenantId: sub.tenantId,
        amount: sub.amount,
        status: "Pending",
        paidOn: todayISO(),
      }));
      patchFieldError(setErrors, "subscriptionId", undefined);
      patchFieldError(setErrors, "amount", undefined);
      patchFieldError(setErrors, "paidOn", undefined);
    } else {
      setForm((f) => ({ ...f, subscriptionId, amount: 0 }));
      patchFieldError(setErrors, "subscriptionId", requiredSelect(subscriptionId, "Subscription"));
    }
  };

  const filteredSubscriptions = form.tenantId
    ? subscriptions.filter((s) => s.tenantId === form.tenantId)
    : subscriptions;

  const validate = (): boolean => {
    const e: FieldErrors = {};
    if (!editing) {
      e.tenantId = requiredSelect(form.tenantId, "Tenant");
      e.subscriptionId = requiredSelect(form.subscriptionId, "Subscription");
      e.amount = positiveNumber(form.amount, "Amount", true);
      e.status = requiredSelect(form.status, "Status");
      if (!form.paidOn) e.paidOn = "Paid on date is required";
    }
    Object.keys(e).forEach((k) => !e[k] && delete e[k]);
    setErrors(e);
    return !hasErrors(e);
  };

  const handleSave = async () => {
    if (!validate()) return;
    setSaving(true);
    try {
      if (editing) {
        await paymentsApi.update(editing.paymentId, {
          paymentMethod: form.paymentMethod, transactionId: form.transactionId,
          invoiceNumber: form.invoiceNumber, paymentGateway: form.paymentGateway,
          status: form.status, paidOn: form.paidOn || null,
        });
      } else {
        await paymentsApi.create({
          subscriptionId: form.subscriptionId,
          tenantId: form.tenantId,
          amount: form.amount,
          currency: form.currency,
          paymentMethod: form.paymentMethod || undefined,
          transactionId: form.transactionId || undefined,
          invoiceNumber: form.invoiceNumber || undefined,
          paymentGateway: form.paymentGateway || undefined,
          status: form.status,
          paidOn: form.paidOn,
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
      await paymentsApi.delete(deleting.paymentId);
      setDeleteOpen(false);
      load();
    } catch (err) {
      alert(err instanceof Error ? err.message : "Failed to cancel payment");
    } finally {
      setSaving(false);
    }
  };

  const statusVariant = (s: string) => s === "Paid" || s === "Completed" ? "success" : s === "Pending" ? "warning" : "danger";

  return (
    <div>
      <PageHeader title="Payments" description="Track tenant payment transactions"
        action={<Button onClick={() => { setEditing(null); setForm({ ...emptyForm, paidOn: todayISO() }); setErrors({}); setModalOpen(true); }}><Plus size={16} /> Add Payment</Button>} />
      {error && <Alert message={error} />}
      <p className="mb-4 text-xs text-slate-500"><span className="text-red-500">*</span> Required field</p>

      <Card>
        {loading ? <LoadingSpinner /> : items.length === 0 ? <EmptyState message="No payments found" /> : (
          <div className="overflow-x-auto">
            <table className="w-full text-sm">
              <thead>
                <tr className="border-b border-slate-100 text-left text-slate-500">
                  <th className="pb-3 font-medium">Tenant</th>
                  <th className="pb-3 font-medium">Amount</th>
                  <th className="pb-3 font-medium">Status</th>
                  <th className="pb-3 font-medium">Paid On</th>
                  <th className="pb-3 font-medium">Actions</th>
                </tr>
              </thead>
              <tbody>
                {items.map((item) => (
                  <tr key={item.paymentId} className="border-b border-slate-50 hover:bg-slate-50">
                    <td className="py-3 font-medium">{item.tenantName}</td>
                    <td className="py-3">{item.currency} {item.amount.toLocaleString()}</td>
                    <td className="py-3"><Badge variant={statusVariant(item.status)}>{item.status}</Badge></td>
                    <td className="py-3 text-slate-500">{item.paidOn ? new Date(item.paidOn).toLocaleDateString() : "-"}</td>
                    <td className="py-3">
                      <div className="flex gap-2">
                        <button onClick={() => { setEditing(item); setForm({ subscriptionId: item.subscriptionId, tenantId: item.tenantId, amount: item.amount, currency: item.currency, paymentMethod: item.paymentMethod || "", transactionId: item.transactionId || "", invoiceNumber: item.invoiceNumber || "", paymentGateway: item.paymentGateway || "", status: item.status, paidOn: item.paidOn?.split("T")[0] || todayISO() }); setErrors({}); setModalOpen(true); }} className="rounded p-1 text-slate-400 hover:text-blue-600"><Edit size={16} /></button>
                        {item.status !== "Cancelled" && (
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

      <Modal open={modalOpen} onClose={() => setModalOpen(false)} title={editing ? "Edit Payment" : "Add Payment"} size="lg">
        <div className="grid gap-4 sm:grid-cols-2">
          {!editing && (
            <>
              <Select label="Tenant" required value={form.tenantId} error={errors.tenantId}
                onChange={(e) => handleTenantChange(e.target.value)}
                options={[{ value: "", label: "Select tenant *" }, ...tenantsWithSubscriptions.map((t) => ({ value: t.tenantId, label: t.companyName }))]} />
              <Select label="Subscription" required value={form.subscriptionId} error={errors.subscriptionId}
                onChange={(e) => handleSubscriptionChange(e.target.value)}
                options={[{ value: "", label: "Select subscription *" }, ...filteredSubscriptions.map((s) => ({ value: s.tenantSubscriptionsId, label: `${s.tenantName} - ${s.planName}` }))]} />
              <Input label="Amount" type="number" required value={form.amount} error={errors.amount} readOnly className="bg-slate-50" />
              <Select label="Status" required value={form.status} error={errors.status}
                onChange={(e) => {
                  const value = e.target.value;
                  setForm({ ...form, status: value });
                  patchFieldError(setErrors, "status", requiredSelect(value, "Status"));
                }}
                options={["Pending", "Paid", "Failed", "Refunded"].map((s) => ({ value: s, label: s }))} />
              <Input label="Paid On" type="date" required value={form.paidOn} error={errors.paidOn}
                onChange={(e) => {
                  const value = e.target.value;
                  setForm({ ...form, paidOn: value });
                  patchFieldError(setErrors, "paidOn", value ? undefined : "Paid on date is required");
                }} />
            </>
          )}
          {editing && (
            <>
              <Input label="Payment Method" value={form.paymentMethod} onChange={(e) => setForm({ ...form, paymentMethod: e.target.value })} />
              <Input label="Transaction ID" value={form.transactionId} onChange={(e) => setForm({ ...form, transactionId: e.target.value })} />
              <Select label="Status" value={form.status} onChange={(e) => setForm({ ...form, status: e.target.value })}
                options={["Pending", "Paid", "Failed", "Refunded", "Cancelled"].map((s) => ({ value: s, label: s }))} />
              <Input label="Paid On" type="date" value={form.paidOn} onChange={(e) => setForm({ ...form, paidOn: e.target.value })} />
            </>
          )}
        </div>
        <div className="mt-6 flex justify-end gap-3">
          <Button variant="secondary" onClick={() => setModalOpen(false)}>Cancel</Button>
          <Button onClick={handleSave} disabled={saving}>{saving ? "Saving..." : "Save"}</Button>
        </div>
      </Modal>

      <ConfirmModal open={deleteOpen} onClose={() => setDeleteOpen(false)} onConfirm={handleDeactivate}
        title="Cancel Payment" message="Are you sure you want to inactive/cancel this payment record?" loading={saving} />
    </div>
  );
}
