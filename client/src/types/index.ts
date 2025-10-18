export interface User {
  id: number;
  fullName: string;
  email: string;
  createdAt: string;
}

export interface Project {
  id: number;
  name: string;
  description: string;
  createdAt: string;
  owner: User;
  members: User[];
  taskCount: number;
}

export interface Task {
  id: number;
  projectId: number;
  title: string;
  description: string;
  status: TaskStatus | number; // Allow both string enum and backend integer values
  assignee?: User;
  dueDate?: string;
  order: number;
  createdAt: string;
  updatedAt: string;
}

export enum TaskStatus {
  Todo = 'Todo',
  InProgress = 'InProgress',
  Done = 'Done'
}

// Keep for backward compatibility with existing frontend code
export type ProjectTaskStatus = TaskStatus

export interface CreateProjectRequest {
  name: string;
  description: string;
}

export interface UpdateProjectRequest {
  name?: string;
  description?: string;
}

export interface CreateTaskRequest {
  title: string;
  description: string;
  assigneeId?: number;
  dueDate?: string;
  status: TaskStatus | number; // Allow both string enum and backend integer values
  order: number;
}

export interface UpdateTaskRequest {
  title?: string;
  description?: string;
  assigneeId?: number;
  dueDate?: string;
  status?: TaskStatus | number; // Allow both string enum and backend integer values
  order?: number;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}

export interface InviteMemberRequest {
  email: string;
}

export interface ProjectMember {
  user: User;
  role: 'Owner' | 'Member';
  joinedAt: string;
}
