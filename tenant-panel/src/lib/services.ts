import { api } from "./api";
import type { Member, MemberPlan, TenantLoginResponse, TenantMemberDetail } from "./types";

export const tenantAuthApi = {
  login: (userName: string, password: string) =>
    api.post<TenantLoginResponse>("/api/tenant/auth/login", { userName, password }),
};

export const memberPlansApi = {
  list: () => api.get<MemberPlan[]>("/api/tenant/plans"),
  create: (body: {
    planName: string;
    description: string;
    price: number;
    durationDays: number;
    isActive: boolean;
  }) => api.post<MemberPlan>("/api/tenant/plans", body),
  update: (
    id: string,
    body: Partial<{
      planName: string;
      description: string;
      price: number;
      durationDays: number;
      isActive: boolean;
    }>
  ) => api.put<MemberPlan>(`/api/tenant/plans/${id}`, body),
  delete: (id: string) => api.delete(`/api/tenant/plans/${id}`),
};

export const membersApi = {
  getByUserCode: (userCode: string) =>
    api.get<Member>(`/api/tenant/users/${encodeURIComponent(userCode)}`),
  updatePlan: (userCode: string, body: { memberPlanId: string; paymentStatus: string }) =>
    api.put<Member>(`/api/tenant/users/${encodeURIComponent(userCode)}/plan`, body),
  getPendingApprovals: () => api.get<Member[]>("/api/tenant/users/pending-approvals"),
  getDetail: (userId: string) => api.get<TenantMemberDetail>(`/api/tenant/users/detail/${userId}`),
  updateProfileApproval: (memberId: string, status: string) =>
    api.put<Member>(`/api/tenant/users/${memberId}/profile-approval`, { status }),
  updatePhotoApproval: (photoId: string, status: string) =>
    api.put<Member>(`/api/tenant/users/photos/${photoId}/approval`, { status }),
};
