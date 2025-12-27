import axios from 'axios';

const API_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:10180/api';

const authApi = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true, // Important for cookies
});

export interface User {
  id: string;
  username: string;
  email: string;
  firstName?: string;
  lastName?: string;
  avatarUrl?: string;
  role: string;
  permissions: string[];
}

export interface AuthResponse {
  accessToken: string;
  user: User;
}

export interface RegisterData {
  username: string;
  email: string;
  password: string;
  firstName?: string;
  lastName?: string;
}

export interface LoginData {
  usernameOrEmail: string;
  password: string;
}

export interface AuthError {
  message: string;
  code: string;
}

let accessToken: string | null = null;

export const setAccessToken = (token: string | null) => {
  accessToken = token;
};

export const getAccessToken = () => accessToken;

// Add auth header to requests
authApi.interceptors.request.use((config) => {
  if (accessToken) {
    config.headers.Authorization = `Bearer ${accessToken}`;
  }
  return config;
});

// Handle token refresh on 401
authApi.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const { data } = await authApi.post<AuthResponse>('/auth/refresh');
        setAccessToken(data.accessToken);
        originalRequest.headers.Authorization = `Bearer ${data.accessToken}`;
        return authApi(originalRequest);
      } catch (refreshError) {
        setAccessToken(null);
        if (typeof window !== 'undefined') {
          window.location.href = '/login';
        }
        return Promise.reject(refreshError);
      }
    }

    return Promise.reject(error);
  }
);

export const auth = {
  register: async (data: RegisterData): Promise<AuthResponse> => {
    const response = await authApi.post<AuthResponse>('/auth/register', data);
    setAccessToken(response.data.accessToken);
    return response.data;
  },

  login: async (data: LoginData): Promise<AuthResponse> => {
    const response = await authApi.post<AuthResponse>('/auth/login', data);
    setAccessToken(response.data.accessToken);
    return response.data;
  },

  logout: async (): Promise<void> => {
    try {
      await authApi.post('/auth/logout');
    } finally {
      setAccessToken(null);
    }
  },

  refresh: async (): Promise<AuthResponse> => {
    const response = await authApi.post<AuthResponse>('/auth/refresh');
    setAccessToken(response.data.accessToken);
    return response.data;
  },

  me: async (): Promise<User> => {
    const response = await authApi.get<User>('/auth/me');
    return response.data;
  },

  revokeAll: async (): Promise<void> => {
    await authApi.post('/auth/revoke-all');
    setAccessToken(null);
  },
};

export default authApi;
