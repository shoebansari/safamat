"use client";

import { useCallback, useEffect, useState } from "react";
import Link from "next/link";
import { Heart } from "lucide-react";
import { useAuth } from "@/context/AuthContext";
import { userAuthApi } from "@/lib/services";
import type { MemberPlan } from "@/lib/types";
import { Button } from "@/components/ui/Button";
import { Input } from "@/components/ui/Input";
import { Select } from "@/components/ui/Select";
import { Alert } from "@/components/ui/LoadingSpinner";

export default function RegisterPage() {
  const { register } = useAuth();
  const [form, setForm] = useState({
    tenantCode: "008",
    memberPlanId: "",
    userName: "",
    firstName: "",
    lastName: "",
    email: "",
    phone: "",
    password: "",
  });
  const [plans, setPlans] = useState<MemberPlan[]>([]);
  const [loadingPlans, setLoadingPlans] = useState(false);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  const loadPlans = useCallback(async (tenantCode: string) => {
    if (!tenantCode.trim()) {
      setPlans([]);
      setForm((f) => ({ ...f, memberPlanId: "" }));
      return;
    }
    setLoadingPlans(true);
    try {
      const data = await userAuthApi.getPlans(tenantCode.trim());
      setPlans(data);
      setForm((f) => ({
        ...f,
        memberPlanId: data.some((p) => p.memberPlanId === f.memberPlanId) ? f.memberPlanId : "",
      }));
    } catch {
      setPlans([]);
      setForm((f) => ({ ...f, memberPlanId: "" }));
    } finally {
      setLoadingPlans(false);
    }
  }, []);

  useEffect(() => {
    const timer = setTimeout(() => loadPlans(form.tenantCode), 400);
    return () => clearTimeout(timer);
  }, [form.tenantCode, loadPlans]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!form.memberPlanId) {
      setError("Please select a membership plan.");
      return;
    }
    setError("");
    setLoading(true);
    try {
      await register(form);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Registration failed");
    } finally {
      setLoading(false);
    }
  };

  const planOptions = [
    { value: "", label: loadingPlans ? "Loading plans..." : plans.length ? "Select a plan" : "No plans available for this tenant" },
    ...plans.map((p) => ({
      value: p.memberPlanId,
      label: `${p.planName} — ₹${p.price} / ${p.durationDays} days`,
    })),
  ];

  return (
    <div className="flex min-h-screen items-center justify-center bg-slate-50 p-4 sm:p-6">
      <div className="w-full max-w-lg rounded-2xl bg-white p-5 shadow-lg sm:p-8">
        <div className="mb-6 flex items-center gap-2 text-rose-600">
          <Heart size={24} fill="currentColor" />
          <span className="text-xl font-bold text-slate-900">Create Account</span>
        </div>
        {error && <Alert message={error} />}
        <form onSubmit={handleSubmit} className="space-y-4">
          <Input
            label="Tenant Code"
            value={form.tenantCode}
            onChange={(e) => setForm({ ...form, tenantCode: e.target.value, memberPlanId: "" })}
            required
          />
          <Select
            label="Membership Plan"
            options={planOptions}
            value={form.memberPlanId}
            onChange={(e) => setForm({ ...form, memberPlanId: e.target.value })}
            required
            disabled={loadingPlans || plans.length === 0}
          />
          {plans.length === 0 && form.tenantCode && !loadingPlans && (
            <p className="text-xs text-amber-600">No active plans found. Ask your tenant to create plans first.</p>
          )}
          <div className="grid grid-cols-2 gap-4">
            <Input label="First Name" value={form.firstName} onChange={(e) => setForm({ ...form, firstName: e.target.value })} required />
            <Input label="Last Name" value={form.lastName} onChange={(e) => setForm({ ...form, lastName: e.target.value })} required />
          </div>
          <Input label="Username" value={form.userName} onChange={(e) => setForm({ ...form, userName: e.target.value })} required />
          <Input label="Email" type="email" value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} required />
          <Input label="Phone" value={form.phone} onChange={(e) => setForm({ ...form, phone: e.target.value })} />
          <Input label="Password" type="password" value={form.password} onChange={(e) => setForm({ ...form, password: e.target.value })} required />
          <Button type="submit" className="w-full" disabled={loading || !form.memberPlanId}>
            {loading ? "Creating..." : "Register"}
          </Button>
        </form>
        <p className="mt-4 text-center text-sm text-slate-500">
          Already have an account? <Link href="/login" className="text-rose-600 hover:underline">Sign in</Link>
        </p>
      </div>
    </div>
  );
}
