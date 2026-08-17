import type { SubscriptionPlan } from "./types";

export function formatDateISO(date: Date): string {
  return date.toISOString().split("T")[0];
}

export function todayISO(): string {
  return formatDateISO(new Date());
}

export function isFreePlan(plan: SubscriptionPlan | undefined): boolean {
  if (!plan) return false;
  return plan.price === 0 || /free/i.test(plan.planName);
}

export function addDays(date: Date, days: number): Date {
  const result = new Date(date);
  result.setDate(result.getDate() + days);
  return result;
}

export interface SubscriptionDates {
  startDate: string;
  endDate: string;
  nextBillingDate: string;
  amount: number;
  paymentStatus: string;
}

export function computeSubscriptionFromPlan(
  plan: SubscriptionPlan,
  startDateStr?: string
): SubscriptionDates {
  const start = startDateStr ? new Date(startDateStr) : new Date();
  const startDate = formatDateISO(start);

  if (isFreePlan(plan)) {
    return {
      startDate,
      endDate: startDate,
      nextBillingDate: "",
      amount: 0,
      paymentStatus: "Pending",
    };
  }

  const end = addDays(start, plan.durationDays);
  const nextBilling = addDays(end, 1);

  return {
    startDate,
    endDate: formatDateISO(end),
    nextBillingDate: formatDateISO(nextBilling),
    amount: plan.price,
    paymentStatus: "Pending",
  };
}
