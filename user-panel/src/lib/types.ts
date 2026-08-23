export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

export interface UserSession {
  userId: string;
  tenantId: string;
  tenantCode: string;
  userCode: string;
  userName: string;
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  primaryPhotoUrl?: string;
  isProfileCompleted: boolean;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
  user: UserSession;
}

export interface UserProfile {
  profileId: string;
  userId: string;
  userCode: string;
  firstName: string;
  lastName: string;
  email?: string;
  phone?: string;
  gender?: string;
  dateOfBirth?: string;
  age?: number;
  height?: number;
  weight?: number;
  maritalStatus?: string;
  religion?: string;
  caste?: string;
  subCaste?: string;
  motherTongue?: string;
  bloodGroup?: string;
  aboutMe?: string;
  isProfileCompleted: boolean;
  profileStatus: string;
  primaryPhotoUrl?: string;
  education?: Education;
  occupation?: Occupation;
  family?: Family;
  lifestyle?: Lifestyle;
  location?: Location;
  photos: Photo[];
}

export interface Education {
  educationId: string;
  qualification?: string;
  college?: string;
  university?: string;
  passingYear?: number;
  educationType?: string;
}

export interface Occupation {
  occupationId: string;
  occupation?: string;
  companyName?: string;
  designation?: string;
  annualIncome?: number;
  workLocation?: string;
}

export interface Family {
  familyId: string;
  familyType?: string;
  familyStatus?: string;
  fatherName?: string;
  fatherOccupation?: string;
  motherName?: string;
  motherOccupation?: string;
  brothers?: number;
  sisters?: number;
}

export interface Lifestyle {
  lifestyleId: string;
  diet?: string;
  smoking: boolean;
  drinking: boolean;
  hobbies?: string;
  languagesKnown?: string;
}

export interface Location {
  locationId: string;
  country?: string;
  state?: string;
  city?: string;
  address?: string;
  pincode?: string;
}

export interface Photo {
  photoId: string;
  photoUrl: string;
  isPrimary: boolean;
  displayOrder: number;
  isApproved: boolean;
  uploadedOn: string;
}

export interface Preference {
  preferenceId?: string;
  minAge?: number;
  maxAge?: number;
  minHeight?: number;
  maxHeight?: number;
  religion?: string;
  caste?: string;
  education?: string;
  occupation?: string;
  country?: string;
  state?: string;
  city?: string;
}

export interface DiscoverProfile {
  userId: string;
  userCode: string;
  firstName: string;
  lastName: string;
  age?: number;
  gender?: string;
  city?: string;
  state?: string;
  religion?: string;
  caste?: string;
  motherTongue?: string;
  maritalStatus?: string;
  occupation?: string;
  education?: string;
  height?: number;
  primaryPhotoUrl?: string;
  photoUrls?: string[];
  matchPercentage: number;
}

export interface DiscoverFilters {
  minAge?: number;
  maxAge?: number;
  city?: string;
  state?: string;
  religion?: string;
  caste?: string;
  motherTongue?: string;
  maritalStatus?: string;
  occupation?: string;
  education?: string;
  gender?: string;
  minHeight?: number;
  maxHeight?: number;
}

export interface DiscoverFilterOptions {
  cities: string[];
  states: string[];
  religions: string[];
  motherTongues: string[];
  maritalStatuses: string[];
  occupations: string[];
  educations: string[];
  genders: string[];
}

export interface Interest {
  interestId: string;
  senderUserId: string;
  receiverUserId: string;
  senderName: string;
  receiverName: string;
  senderPhotoUrl?: string;
  receiverPhotoUrl?: string;
  status: string;
  sentOn: string;
}

export interface Match {
  matchId: string;
  userId: string;
  name: string;
  photoUrl?: string;
  matchPercentage: number;
  matchedOn: string;
}

export interface Message {
  messageId: string;
  senderUserId: string;
  receiverUserId: string;
  senderName: string;
  message: string;
  isRead: boolean;
  sentOn: string;
}

export interface Conversation {
  userId: string;
  name: string;
  photoUrl?: string;
  lastMessage?: string;
  lastMessageOn?: string;
  unreadCount: number;
}

export interface Notification {
  notificationId: string;
  title: string;
  message: string;
  relatedUserId?: string;
  isRead: boolean;
  createdOn: string;
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

export interface UserSubscription {
  userSubscriptionId: string;
  memberPlanId: string;
  planName: string;
  planPrice: number;
  durationDays: number;
  paymentStatus: string;
  assignedOn: string;
}
