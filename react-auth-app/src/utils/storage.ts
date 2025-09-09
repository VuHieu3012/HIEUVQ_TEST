import { STORAGE_KEYS } from '../constants/app';

export const storage = {
  get: <T>(key: string): T | null => {
    try {
      const item = localStorage.getItem(key);
      return item ? JSON.parse(item) : null;
    } catch {
      return null;
    }
  },

  set: <T>(key: string, value: T): void => {
    try {
      localStorage.setItem(key, JSON.stringify(value));
    } catch (error) {
      console.error('Error saving to localStorage:', error);
    }
  },

  remove: (key: string): void => {
    try {
      localStorage.removeItem(key);
    } catch (error) {
      console.error('Error removing from localStorage:', error);
    }
  },

  clear: (): void => {
    try {
      localStorage.clear();
    } catch (error) {
      console.error('Error clearing localStorage:', error);
    }
  },
};

export const authStorage = {
  getToken: (): string | null => {
    return localStorage.getItem(STORAGE_KEYS.AUTH_TOKEN);
  },

  getRefreshToken: (): string | null => {
    return localStorage.getItem(STORAGE_KEYS.REFRESH_TOKEN);
  },

  setToken: (token: string): void => {
    localStorage.setItem(STORAGE_KEYS.AUTH_TOKEN, token);
  },

  setRefreshToken: (refreshToken: string): void => {
    localStorage.setItem(STORAGE_KEYS.REFRESH_TOKEN, refreshToken);
  },

  setTokens: (token: string, refreshToken: string): void => {
    authStorage.setToken(token);
    authStorage.setRefreshToken(refreshToken);
  },

  removeToken: (): void => {
    localStorage.removeItem(STORAGE_KEYS.AUTH_TOKEN);
  },

  removeRefreshToken: (): void => {
    localStorage.removeItem(STORAGE_KEYS.REFRESH_TOKEN);
  },

  removeTokens: (): void => {
    authStorage.removeToken();
    authStorage.removeRefreshToken();
  },
};
