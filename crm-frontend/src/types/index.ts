// Customer status values
export const CustomerStatus = {
  Lead: 'Lead',
  Contacted: 'Contacted',
  NeedsAnalyzed: 'NeedsAnalyzed',
  Quoted: 'Quoted',
  Negotiating: 'Negotiating',
  Won: 'Won',
  Lost: 'Lost',
} as const;
export type CustomerStatus = (typeof CustomerStatus)[keyof typeof CustomerStatus];

// Customer source values
export const CustomerSource = {
  Website: 'Website',
  Referral: 'Referral',
  SocialMedia: 'SocialMedia',
  Event: 'Event',
  DirectContact: 'DirectContact',
  Other: 'Other',
} as const;
export type CustomerSource = (typeof CustomerSource)[keyof typeof CustomerSource];

// Interaction channel values
export const InteractionChannel = {
  Phone: 'Phone',
  Wechat: 'Wechat',
  Email: 'Email',
  Offline: 'Offline',
  Other: 'Other',
} as const;
export type InteractionChannel = (typeof InteractionChannel)[keyof typeof InteractionChannel];

export interface Customer {
  id: string;
  companyName: string;
  contactName: string;
  wechat?: string;
  phone?: string;
  email?: string;
  industry?: string;
  source?: CustomerSource;
  status: CustomerStatus;
  tags?: string[];
  score: number;
  lastInteractionAt?: string;
  createdAt: string;
  updatedAt: string;
}


export interface AttachmentInfo {
  url: string;
  fileName?: string;
  fileSize?: number;
}

export interface Interaction {
  id: string;
  customerId: string;
  happenedAt: string;
  channel: InteractionChannel;
  stage?: CustomerStatus;
  title: string;
  summary?: string;
  rawContent?: string;
  nextAction?: string;
  attachments?: AttachmentInfo[];
  createdAt: string;
  updatedAt: string;
}

export interface User {
  id: string;
  userName: string;
  role: string;
  createdAt: string;
  updatedAt: string;
  lastLoginAt?: string;
}

// API Response types
export interface ErrorDetail {
  field?: string;
  message: string;
}

export interface ApiResponse<T> {
  success: boolean;
  data: T | null;
  errors: ErrorDetail[];
}

export interface PagedResponse<T> {
  items: T[];
  total: number;
}

// Request types
export interface CreateCustomerRequest {
  companyName: string;
  contactName: string;
  wechat?: string;
  phone?: string;
  email?: string;
  industry?: string;
  source?: CustomerSource;
  status?: CustomerStatus;
  tags?: string[];
  score?: number;
}

export type UpdateCustomerRequest = CreateCustomerRequest;

export interface CustomerSearchRequest {
  page?: number;
  pageSize?: number;
  keyword?: string;
  status?: CustomerStatus;
  industry?: string;
  source?: CustomerSource;
  sortBy?: 'LastInteractionAt' | 'CreatedAt' | 'UpdatedAt';
  sortOrder?: 'asc' | 'desc';
}

export interface CreateInteractionRequest {
  happenedAt: string;
  channel: InteractionChannel;
  stage?: CustomerStatus;
  title: string;
  summary?: string;
  rawContent?: string;
  nextAction?: string;
  attachments?: AttachmentInfo[];
}

export type UpdateInteractionRequest = CreateInteractionRequest;

export interface LoginRequest {
  username: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
}
