import React, { createContext, useContext, useState, useEffect, useCallback, ReactNode } from 'react';
import { useNavigate } from 'react-router-dom';
import { User, AuthResponse } from '../types';
import { authService } from '../services/authService';
import { authStorage } from '../utils/storage';

interface AuthContextType {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  userType: string | null;
  isAdmin: boolean;
  login: (usernameOrEmail: string, password: string, rememberMe: boolean) => Promise<AuthResponse>;
  register: (username: string, email: string, password: string, confirmPassword: string, userType: string, acceptTerms: boolean, rememberMe: boolean) => Promise<AuthResponse>;
  logout: () => Promise<void>;
  validateToken: () => Promise<AuthResponse>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [userType, setUserType] = useState<string | null>(null);
  const navigate = useNavigate();

  const isAdmin = userType === 'Admin';

  const initAuth = useCallback(async () => {
    const token = authStorage.getToken();
    if (token) {
      const validation = await authService.validateToken();
      if (validation.success && validation.user) {
        setIsAuthenticated(true);
        setUser(validation.user);
        setUserType(validation.user.userType);
      } else {
        // Try to refresh token if validation fails
        const refreshResult = await authService.refreshToken();
        if (refreshResult.success && refreshResult.user) {
          setIsAuthenticated(true);
          setUser(refreshResult.user);
          setUserType(refreshResult.user.userType);
        } else {
          authStorage.removeTokens();
        }
      }
    }
    setIsLoading(false);
  }, []);

  useEffect(() => {
    initAuth();
  }, []); // Remove initAuth from dependency array to prevent infinite re-render

  const login = useCallback(async (
    usernameOrEmail: string,
    password: string,
    rememberMe: boolean
  ): Promise<AuthResponse> => {
    const result = await authService.login({ usernameOrEmail, password, rememberMe });
    if (result.success && result.user) {
      setIsAuthenticated(true);
      setUser(result.user);
      setUserType(result.user.userType);
      // Navigate based on user type
      if (result.user.userType === 'Admin') {
        navigate('/data');
      } else {
        navigate('/');
      }
    }
    return result;
  }, [navigate]);

  const register = useCallback(async (
    username: string,
    email: string,
    password: string,
    confirmPassword: string,
    userType: string,
    acceptTerms: boolean,
    rememberMe: boolean
  ): Promise<AuthResponse> => {
    const result = await authService.register({
      username,
      email,
      password,
      confirmPassword,
      userType: userType as any,
      acceptTerms,
    });
    if (result.success && result.user) {
      setIsAuthenticated(true);
      setUser(result.user);
      setUserType(result.user.userType);
    }
    return result;
  }, []);

  const logout = useCallback(async (): Promise<void> => {
    await authService.logout();
    setUser(null);
    setIsAuthenticated(false);
    setUserType(null);
    navigate('/login');
  }, [navigate]);

  const validateToken = useCallback(async (): Promise<AuthResponse> => {
    return await authService.validateToken();
  }, []);

  const value = {
    user,
    isAuthenticated,
    isLoading,
    userType,
    isAdmin,
    login,
    register,
    logout,
    validateToken,
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
