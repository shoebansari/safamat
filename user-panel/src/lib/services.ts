import { api, uploadFile } from "./api";
import type { Photo } from "./types";
import type {
  Conversation,
  DiscoverFilterOptions,
  DiscoverFilters,
  DiscoverProfile,
  Interest,
  LoginResponse,
  Match,
  Message,
  MemberPlan,
  Notification,
  Preference,
  UserProfile,
  UserSubscription,
} from "./types";

function toQuery(filters?: DiscoverFilters): string {
  if (!filters) return "";
  const params = new URLSearchParams();
  Object.entries(filters).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== "") {
      params.set(key, String(value));
    }
  });
  const qs = params.toString();
  return qs ? `?${qs}` : "";
}

export const userAuthApi = {
  getPlans: (tenantCode: string) =>
    api.get<MemberPlan[]>(`/api/user/auth/plans?tenantCode=${encodeURIComponent(tenantCode)}`),
  register: (body: {
    tenantCode: string;
    userName: string;
    firstName: string;
    lastName: string;
    email: string;
    phone?: string;
    password: string;
    memberPlanId: string;
  }) => api.post<LoginResponse>("/api/user/auth/register", body),
  login: (tenantCode: string, userName: string, password: string) =>
    api.post<LoginResponse>("/api/user/auth/login", { tenantCode, userName, password }),
};

export const subscriptionApi = {
  getPlans: () => api.get<MemberPlan[]>("/api/user/plans"),
  getMySubscription: () => api.get<UserSubscription | null>("/api/user/subscription"),
  changePlan: (memberPlanId: string) =>
    api.put<UserSubscription>("/api/user/subscription", { memberPlanId }),
};

export const profileApi = {
  getMe: () => api.get<UserProfile>("/api/user/profile/me"),
  getPublic: (userId: string) => api.get<UserProfile>(`/api/user/profile/${userId}`),
  saveBasic: (body: Record<string, unknown>) => api.put<UserProfile>("/api/user/profile/basic", body),
  saveEducation: (body: Record<string, unknown>) => api.put<UserProfile>("/api/user/profile/education", body),
  saveOccupation: (body: Record<string, unknown>) => api.put<UserProfile>("/api/user/profile/occupation", body),
  saveFamily: (body: Record<string, unknown>) => api.put<UserProfile>("/api/user/profile/family", body),
  saveLifestyle: (body: Record<string, unknown>) => api.put<UserProfile>("/api/user/profile/lifestyle", body),
  saveLocation: (body: Record<string, unknown>) => api.put<UserProfile>("/api/user/profile/location", body),
  getPreferences: () => api.get<Preference>("/api/user/profile/preferences"),
  savePreferences: (body: Preference) => api.put<Preference>("/api/user/profile/preferences", body),
  addPhoto: (photoUrl: string, isPrimary = false) =>
    api.post("/api/user/profile/photos", { photoUrl, isPrimary }),
  uploadPhoto: (file: File, isPrimary = false) =>
    uploadFile<Photo>("/api/user/profile/photos/upload", file, { isPrimary: String(isPrimary) }),
  deletePhoto: (photoId: string) => api.delete(`/api/user/profile/photos/${photoId}`),
};

export const socialApi = {
  discover: (filters?: DiscoverFilters) =>
    api.get<DiscoverProfile[]>(`/api/user/discover${toQuery(filters)}`),
  discoverFilterOptions: () => api.get<DiscoverFilterOptions>("/api/user/discover/filter-options"),
  interests: (type: "sent" | "received") => api.get<Interest[]>(`/api/user/interests?type=${type}`),
  sendInterest: (receiverId: string) => api.post<Interest>(`/api/user/interests/${receiverId}`, {}),
  respondInterest: (interestId: string, status: string) =>
    api.put<Interest>(`/api/user/interests/${interestId}`, { status }),
  matches: () => api.get<Match[]>("/api/user/matches"),
  favorites: () => api.get<DiscoverProfile[]>("/api/user/favorites"),
  toggleFavorite: (userId: string) => api.post(`/api/user/favorites/${userId}`, {}),
  block: (userId: string) => api.post(`/api/user/block/${userId}`, {}),
  report: (reportedUserId: string, reason: string, details?: string) =>
    api.post("/api/user/report", { reportedUserId, reason, details }),
  conversations: () => api.get<Conversation[]>("/api/user/conversations"),
  messages: (otherUserId: string) => api.get<Message[]>(`/api/user/messages/${otherUserId}`),
  sendMessage: (receiverUserId: string, message: string) =>
    api.post<Message>("/api/user/messages", { receiverUserId, message }),
  notifications: () => api.get<Notification[]>("/api/user/notifications"),
  unreadCount: () => api.get<number>("/api/user/notifications/unread-count"),
  markRead: (id: string) => api.put(`/api/user/notifications/${id}/read`, {}),
  markAllRead: () => api.put("/api/user/notifications/read-all", {}),
};
