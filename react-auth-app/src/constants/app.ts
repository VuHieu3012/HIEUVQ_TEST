export const APP_CONFIG = {
  NAME: 'React Auth App',
  VERSION: '1.0.0',
  ROUTES: {
    HOME: '/',
    LOGIN: '/login',
    REGISTER: '/register',
    DATA: '/data',
  },
} as const;

export const STORAGE_KEYS = {
  AUTH_TOKEN: 'authToken',
  REFRESH_TOKEN: 'refreshToken',
} as const;

export const USER_TYPES = {
  END_USER: 'EndUser',
  ADMIN: 'Admin',
  PARTNER: 'Partner',
} as const;
