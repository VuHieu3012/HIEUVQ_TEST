import axios, { AxiosInstance, AxiosRequestConfig, AxiosResponse } from 'axios';
import { API_CONFIG, HTTP_STATUS } from '../constants/api';
import { STORAGE_KEYS } from '../constants/app';
import { authStorage } from './storage';

class ApiClient {
  private client: AxiosInstance;

  constructor() {
    this.client = axios.create({
      baseURL: API_CONFIG.BASE_URL,
      headers: {
        'Content-Type': 'application/json',
      },
    });

    this.setupInterceptors();
  }

  private setupInterceptors(): void {
    // Request interceptor to add auth token
    this.client.interceptors.request.use((config) => {
      const token = authStorage.getToken();
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    });

    // Response interceptor for error handling and auto-refresh
    this.client.interceptors.response.use(
      (response) => response,
      async (error) => {
        const originalRequest = error.config;
        
        if (error.response?.status === HTTP_STATUS.UNAUTHORIZED && !originalRequest._retry) {
          originalRequest._retry = true;
          
          // Try to refresh token
          const refreshToken = authStorage.getRefreshToken();
          if (refreshToken) {
            try {
              const refreshResponse = await axios.post(
                `${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.AUTH.REFRESH}`,
                refreshToken,
                { headers: { 'Content-Type': 'application/json' } }
              );
              
              if (refreshResponse.data.success && refreshResponse.data.token && refreshResponse.data.refreshToken) {
                authStorage.setTokens(refreshResponse.data.token, refreshResponse.data.refreshToken);
                
                // Retry the original request with new token
                originalRequest.headers.Authorization = `Bearer ${refreshResponse.data.token}`;
                return this.client(originalRequest);
              }
            } catch (refreshError) {
              console.error('Token refresh failed:', refreshError);
            }
          }
          
          // If refresh failed, clear tokens
          authStorage.removeTokens();
        }
        
        return Promise.reject(error);
      }
    );
  }

  async get<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
    const response: AxiosResponse<T> = await this.client.get(url, config);
    return response.data;
  }

  async post<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
    const response: AxiosResponse<T> = await this.client.post(url, data, config);
    return response.data;
  }

  async put<T>(url: string, data?: any, config?: AxiosRequestConfig): Promise<T> {
    const response: AxiosResponse<T> = await this.client.put(url, data, config);
    return response.data;
  }

  async delete<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
    const response: AxiosResponse<T> = await this.client.delete(url, config);
    return response.data;
  }
}

export const apiClient = new ApiClient();
