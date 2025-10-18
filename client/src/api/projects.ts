import type { CreateProjectRequest, InviteMemberRequest, Project, ProjectMember, UpdateProjectRequest } from '@/types'
import axios from 'axios'

const api = axios.create({
  baseURL: '/api/projects',
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

export const projectsApi = {
  async getProjects(): Promise<Project[]> {
    const response = await api.get<Project[]>('')
    return response.data
  },

  async getProject(id: number): Promise<Project> {
    const response = await api.get<Project>(`/${id}`)
    return response.data
  },

  async createProject(projectData: CreateProjectRequest): Promise<Project> {
    const response = await api.post<Project>('', projectData)
    return response.data
  },

  async updateProject(id: number, projectData: UpdateProjectRequest): Promise<Project> {
    const response = await api.put<Project>(`/${id}`, projectData)
    return response.data
  },

  async deleteProject(id: number): Promise<void> {
    await api.delete(`/${id}`)
  },

  async inviteMember(projectId: number, inviteData: InviteMemberRequest): Promise<ProjectMember> {
    const response = await api.post<ProjectMember>(`/${projectId}/invite`, inviteData)
    return response.data
  },

  async getProjectMembers(projectId: number): Promise<ProjectMember[]> {
    const response = await api.get<ProjectMember[]>(`/${projectId}/members`)
    return response.data
  }
}
