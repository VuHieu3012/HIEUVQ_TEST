import { apiClient } from '../utils/api';
import { API_CONFIG } from '../constants/api';
import { authStorage } from '../utils/storage';
import { LoginModel, RegisterModel, AuthResponse, ApiResponse, User } from '../types';

export const authService = {
  async login(loginData: LoginModel): Promise<AuthResponse> {
    try {
      const response = await apiClient.post<AuthResponse>(API_CONFIG.ENDPOINTS.AUTH.LOGIN, loginData);
      if (response.success && response.token) {
        if (response.refreshToken) {
          authStorage.setTokens(response.token, response.refreshToken);
        } else {
          authStorage.setToken(response.token);
        }
      }
      return response;
    } catch (error: any) {
      return {
        success: false,
        message: error.response?.data?.message || 'Login failed'
      };
    }
  },

  async register(registerData: RegisterModel): Promise<AuthResponse> {
    try {
      const response = await apiClient.post<AuthResponse>(API_CONFIG.ENDPOINTS.AUTH.REGISTER, registerData);
      // Don't auto-login after registration - user should login manually
      return response;
    } catch (error: any) {
      return {
        success: false,
        message: error.response?.data?.message || 'Registration failed'
      };
    }
  },

  async refreshToken(): Promise<AuthResponse> {
    try {
      const refreshToken = authStorage.getRefreshToken();
      if (!refreshToken) {
        return { success: false, message: 'No refresh token available' };
      }

      const response = await apiClient.post<AuthResponse>(API_CONFIG.ENDPOINTS.AUTH.REFRESH, refreshToken);
      if (response.success && response.token) {
        if (response.refreshToken) {
          authStorage.setTokens(response.token, response.refreshToken);
        } else {
          authStorage.setToken(response.token);
        }
      }
      return response;
    } catch (error: any) {
      return {
        success: false,
        message: error.response?.data?.message || 'Token refresh failed'
      };
    }
  },

  async logout(): Promise<void> {
    try {
      const token = authStorage.getToken();
      if (token) {
        await apiClient.post(API_CONFIG.ENDPOINTS.AUTH.LOGOUT, token);
      }
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      authStorage.removeTokens();
    }
  },

  async validateToken(): Promise<AuthResponse> {
    try {
      const token = authStorage.getToken();
      if (!token) {
        return { success: false, message: 'No token found' };
      }
      
      const response = await apiClient.post<AuthResponse>(API_CONFIG.ENDPOINTS.AUTH.VALIDATE, token);
      return response;
    } catch (error: any) {
      return {
        success: false,
        message: error.response?.data?.message || 'Token validation failed'
      };
    }
  },

  async getUsers(): Promise<ApiResponse<User[]>> {
    try {
      const response = await apiClient.get<ApiResponse<User[]>>(API_CONFIG.ENDPOINTS.DATA.USERS);
      return response;
    } catch (error: any) {
      return {
        success: false,
        error: error.response?.data?.message || 'Failed to get users'
      };
    }
  },

};
