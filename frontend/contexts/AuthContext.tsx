'use client';

import { createContext, useContext, useEffect, useState, useCallback, ReactNode } from 'react';
import { auth, User, LoginData, RegisterData, setAccessToken, getAccessToken } from '@/lib/auth';

interface AuthContextType {
  user: User | null;
  isLoading: boolean;
  isAuthenticated: boolean;
  login: (data: LoginData) => Promise<void>;
  register: (data: RegisterData) => Promise<void>;
  logout: () => Promise<void>;
  hasPermission: (permission: string) => boolean;
  hasRole: (role: string) => boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  // Try to restore session on mount
  useEffect(() => {
    const initAuth = async () => {
      try {
        // Try to refresh token (will use httpOnly cookie)
        const response = await auth.refresh();
        setUser(response.user);
      } catch {
        // No valid session
        setUser(null);
        setAccessToken(null);
      } finally {
        setIsLoading(false);
      }
    };

    initAuth();
  }, []);

  const login = useCallback(async (data: LoginData) => {
    const response = await auth.login(data);
    setUser(response.user);
  }, []);

  const register = useCallback(async (data: RegisterData) => {
    const response = await auth.register(data);
    setUser(response.user);
  }, []);

  const logout = useCallback(async () => {
    await auth.logout();
    setUser(null);
  }, []);

  const hasPermission = useCallback((permission: string) => {
    if (!user) return false;
    return user.permissions.includes(permission);
  }, [user]);

  const hasRole = useCallback((role: string) => {
    if (!user) return false;
    return user.role.toLowerCase() === role.toLowerCase();
  }, [user]);

  const value: AuthContextType = {
    user,
    isLoading,
    isAuthenticated: !!user && !!getAccessToken(),
    login,
    register,
    logout,
    hasPermission,
    hasRole,
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}

// HOC for protected routes
export function withAuth<P extends object>(
  Component: React.ComponentType<P>,
  requiredPermission?: string
) {
  return function ProtectedComponent(props: P) {
    const { isAuthenticated, isLoading, hasPermission } = useAuth();

    if (isLoading) {
      return <div>Loading...</div>;
    }

    if (!isAuthenticated) {
      if (typeof window !== 'undefined') {
        window.location.href = '/login';
      }
      return null;
    }

    if (requiredPermission && !hasPermission(requiredPermission)) {
      return <div>Access Denied</div>;
    }

    return <Component {...props} />;
  };
}
