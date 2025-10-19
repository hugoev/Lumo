import type { CreateTaskRequest, Task, UpdateTaskRequest } from '@/types'
import axios from 'axios'

const api = axios.create({
  baseURL: '/api/tasks',
  headers: {
    'Content-Type': 'application/json'
  }
})

// Request interceptor to add auth token
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

export const tasksApi = {
  async getTasksForProject(projectId: number): Promise<Task[]> {
    const response = await api.get<Task[]>(`/project/${projectId}`)
    return response.data
  },

  async getTask(id: number): Promise<Task> {
    const response = await api.get<Task>(`/${id}`)
    return response.data
  },

  async createTask(projectId: number, taskData: CreateTaskRequest): Promise<Task> {
    const response = await api.post<Task>(`/project/${projectId}`, taskData)
    return response.data
  },

  async updateTask(id: number, taskData: UpdateTaskRequest): Promise<Task> {
    const response = await api.put<Task>(`/${id}`, taskData)
    return response.data
  },

  async deleteTask(id: number): Promise<void> {
    await api.delete(`/${id}`)
  },

  async updateTaskStatuses(updates: Array<{ taskId: number; status: number; order: number }>): Promise<Task[]> {
    const response = await api.put<Task[]>('/update-statuses', updates)
    return response.data
  }
}
