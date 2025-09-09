export interface User {
  id: number;
  username: string;
  email: string;
  userType: 'EndUser' | 'Admin' | 'Partner';
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface LoginModel {
  usernameOrEmail: string;
  password: string;
  rememberMe: boolean;
}

export interface RegisterModel {
  username: string;
  email: string;
  password: string;
  confirmPassword: string;
  userType: 'EndUser' | 'Admin' | 'Partner';
  acceptTerms: boolean;
}

export interface AuthResponse {
  success: boolean;
  message?: string;
  token?: string;
  refreshToken?: string;
  expiresAt?: string;
  user?: User;
}

export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
  error?: string;
}

