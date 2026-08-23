export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

export interface TenantUser {
  tenantId: string;
  tenantCode: string;
  companyName: string;
  userName: string;
  email: string;
}

export interface TenantLoginResponse {
  token: string;
  expiresAt: string;
  tenant: TenantUser;
}

export interface MemberPlan {
  memberPlanId: string;
  planName: string;
  description: string;
  price: number;
  durationDays: number;
  isActive: boolean;
  createdOn: string;
}

export interface MemberSubscription {
  memberSubscriptionId: string;
  memberPlanId: string;
  planName: string;
  planPrice: number;
  paymentStatus: string;
  assignedOn: string;
}

export interface Member {
  memberId: string;
  userId?: string;
  userCode: string;
  fullName: string;
  email?: string;
  phone?: string;
  bio?: string;
  profilePhotoUrl?: string;
  primaryPhotoUrl?: string;
  pendingPhotoUrl?: string;
  photos?: MemberPhoto[];
  profileStatus: string;
  photoStatus?: string;
  hasPendingPhoto?: boolean;
  pendingPhotoId?: string;
  isActive?: boolean;
  createdOn: string;
  currentSubscription?: MemberSubscription | null;
}

export interface MemberPhoto {
  photoId: string;
  photoUrl: string;
  isApproved: boolean;
  isPrimary: boolean;
}

export interface TenantMemberDetail {
  userId: string;
  userCode: string;
  firstName: string;
  lastName: string;
  email?: string;
  phone?: string;
  gender?: string;
  age?: number;
  height?: number;
  religion?: string;
  caste?: string;
  motherTongue?: string;
  maritalStatus?: string;
  aboutMe?: string;
  profileStatus: string;
  photos: MemberPhoto[];
  education?: { qualification?: string; college?: string };
  occupation?: { occupation?: string; workLocation?: string };
  location?: { city?: string; state?: string; country?: string };
  family?: { familyType?: string; fatherName?: string; motherName?: string };
  lifestyle?: { diet?: string; hobbies?: string };
}
