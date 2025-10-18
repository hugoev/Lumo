import type { AuthResponse, LoginRequest, RegisterRequest } from '@/types'
import axios from 'axios'

const api = axios.create({
  baseURL: '/api/auth',
  headers: {
    'Content-Type': 'application/json'
  }
})

// Request interceptor to add auth token (skip for login/register)
api.interceptors.request.use((config) => {
  // Don't add auth token for login and register requests
  if (config.url === '/login' || config.url === '/register') {
    return config
  }

  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

export const authApi = {
  async login(credentials: LoginRequest): Promise<AuthResponse> {
    const response = await api.post<AuthResponse>('/login', credentials)
    return response.data
  },

  async register(userData: RegisterRequest): Promise<AuthResponse> {
    const response = await api.post<AuthResponse>('/register', userData)
    return response.data
  }
}
