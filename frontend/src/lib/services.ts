import type {
  AdminUser,
  ApiResponse,
  EmailTemplate,
  LoginResponse,
  PagedResult,
  Payment,
  SubscriptionPlan,
  SystemSetting,
  Tenant,
  TenantSubscription,
} from "./types";
import { api } from "./api";
import { getApiUrl } from "./config";

type QueryParams = Record<string, string | number | boolean | undefined>;

function buildQuery(params: QueryParams): string {
  const search = new URLSearchParams();
  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== "") search.append(key, String(value));
  });
  const qs = search.toString();
  return qs ? `?${qs}` : "";
}

export const authApi = {
  login: (adminUserName: string, password: string) =>
    fetch(`${getApiUrl()}/api/auth/login`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ adminUserName, password }),
    }).then(async (res) => {
      const json = (await res.json()) as ApiResponse<LoginResponse>;
      if (!res.ok || !json.success) throw new Error(json.message || "Login failed");
      return json.data;
    }),
};

export const adminUsersApi = {
  list: (page = 1, pageSize = 10, search = "") =>
    api.get<PagedResult<AdminUser>>(`/api/adminusers${buildQuery({ page, pageSize, search })}`),
  get: (id: string) => api.get<AdminUser>(`/api/adminusers/${id}`),
  create: (data: Partial<AdminUser> & { password: string }) =>
    api.post<AdminUser>("/api/adminusers", data),
  update: (id: string, data: Partial<AdminUser> & { password?: string }) =>
    api.put<AdminUser>(`/api/adminusers/${id}`, data),
  delete: (id: string) => api.delete<object>(`/api/adminusers/${id}`),
  usernameExists: (username: string) =>
    api.get<boolean>(`/api/adminusers/exists?username=${encodeURIComponent(username)}`),
};

export const tenantsApi = {
  list: (page = 1, pageSize = 10, search = "", isActive?: boolean) =>
    api.get<PagedResult<Tenant>>(`/api/tenants${buildQuery({ page, pageSize, search, isActive })}`),
  get: (id: string) => api.get<Tenant>(`/api/tenants/${id}`),
  create: (data: Record<string, unknown>) => api.post<Tenant>("/api/tenants", data),
  update: (id: string, data: Record<string, unknown>) => api.put<Tenant>(`/api/tenants/${id}`, data),
  delete: (id: string) => api.delete<object>(`/api/tenants/${id}`),
  exists: (tenantCode?: string, companyName?: string, userName?: string, excludeTenantId?: string) =>
    api.get<{ tenantCodeExists: boolean; companyNameExists: boolean; userNameExists: boolean }>(
      `/api/tenants/exists${buildQuery({ tenantCode, companyName, userName, excludeTenantId })}`
    ),
};

export const subscriptionPlansApi = {
  list: (page = 1, pageSize = 10, isActive?: boolean) =>
    api.get<PagedResult<SubscriptionPlan>>(`/api/subscriptionplans${buildQuery({ page, pageSize, isActive })}`),
  get: (id: string) => api.get<SubscriptionPlan>(`/api/subscriptionplans/${id}`),
  create: (data: Record<string, unknown>) => api.post<SubscriptionPlan>("/api/subscriptionplans", data),
  update: (id: string, data: Record<string, unknown>) =>
    api.put<SubscriptionPlan>(`/api/subscriptionplans/${id}`, data),
  delete: (id: string) => api.delete<object>(`/api/subscriptionplans/${id}`),
};

export const tenantSubscriptionsApi = {
  list: (page = 1, pageSize = 10, tenantId?: string, status?: string) =>
    api.get<PagedResult<TenantSubscription>>(
      `/api/tenantsubscriptions${buildQuery({ page, pageSize, tenantId, status })}`
    ),
  get: (id: string) => api.get<TenantSubscription>(`/api/tenantsubscriptions/${id}`),
  create: (data: Record<string, unknown>) =>
    api.post<TenantSubscription>("/api/tenantsubscriptions", data),
  update: (id: string, data: Record<string, unknown>) =>
    api.put<TenantSubscription>(`/api/tenantsubscriptions/${id}`, data),
  delete: (id: string) => api.delete<object>(`/api/tenantsubscriptions/${id}`),
};

export const paymentsApi = {
  list: (page = 1, pageSize = 10, tenantId?: string, status?: string) =>
    api.get<PagedResult<Payment>>(`/api/payments${buildQuery({ page, pageSize, tenantId, status })}`),
  get: (id: string) => api.get<Payment>(`/api/payments/${id}`),
  create: (data: Record<string, unknown>) => api.post<Payment>("/api/payments", data),
  update: (id: string, data: Record<string, unknown>) => api.put<Payment>(`/api/payments/${id}`, data),
  delete: (id: string) => api.delete<object>(`/api/payments/${id}`),
};

export const emailTemplatesApi = {
  list: (page = 1, pageSize = 10, isActive?: boolean) =>
    api.get<PagedResult<EmailTemplate>>(`/api/emailtemplates${buildQuery({ page, pageSize, isActive })}`),
  get: (id: string) => api.get<EmailTemplate>(`/api/emailtemplates/${id}`),
  create: (data: Record<string, unknown>) => api.post<EmailTemplate>("/api/emailtemplates", data),
  update: (id: string, data: Record<string, unknown>) =>
    api.put<EmailTemplate>(`/api/emailtemplates/${id}`, data),
  delete: (id: string) => api.delete<object>(`/api/emailtemplates/${id}`),
};

export const systemSettingsApi = {
  list: (page = 1, pageSize = 10, search = "") =>
    api.get<PagedResult<SystemSetting>>(`/api/systemsettings${buildQuery({ page, pageSize, search })}`),
  get: (id: string) => api.get<SystemSetting>(`/api/systemsettings/${id}`),
  create: (data: Record<string, unknown>) => api.post<SystemSetting>("/api/systemsettings", data),
  update: (id: string, data: Record<string, unknown>) =>
    api.put<SystemSetting>(`/api/systemsettings/${id}`, data),
  delete: (id: string) => api.delete<object>(`/api/systemsettings/${id}`),
};
