export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface AdminUser {
  adminId: string;
  adminUserName: string;
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  isActive: boolean;
  lastLogin?: string;
  createdOn: string;
  updatedOn?: string;
}

export interface Tenant {
  tenantId: string;
  tenantCode: string;
  companyName: string;
  ownerName: string;
  userName: string;
  password: string;
  email: string;
  phone?: string;
  address?: string;
  city?: string;
  state?: string;
  country?: string;
  zipCode?: string;
  logoUrl?: string;
  databaseName?: string;
  databaseServer?: string;
  isActive: boolean;
  createdBy?: string;
  createdOn: string;
  updatedOn?: string;
}

export interface SubscriptionPlan {
  planId: string;
  planName: string;
  description?: string;
  price: number;
  durationDays: number;
  isActive: boolean;
  createdOn: string;
}

export interface TenantSubscription {
  tenantSubscriptionsId: string;
  tenantId: string;
  tenantName?: string;
  planId: string;
  planName?: string;
  startDate: string;
  endDate: string;
  nextBillingDate?: string;
  amount: number;
  paymentStatus: string;
  subscriptionStatus: string;
  createdOn: string;
}

export interface Payment {
  paymentId: string;
  subscriptionId: string;
  tenantId: string;
  tenantName?: string;
  amount: number;
  currency: string;
  paymentMethod?: string;
  transactionId?: string;
  invoiceNumber?: string;
  paymentGateway?: string;
  status: string;
  paidOn?: string;
}

export interface EmailTemplate {
  templateId: string;
  templateName: string;
  subject: string;
  body: string;
  isActive: boolean;
}

export interface SystemSetting {
  settingId: string;
  settingKey: string;
  settingValue: string;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
  admin: {
    adminId: string;
    adminUserName: string;
    firstName: string;
    lastName: string;
    email: string;
  };
}

export interface AuthUser {
  adminId: string;
  adminUserName: string;
  firstName: string;
  lastName: string;
  email: string;
}
