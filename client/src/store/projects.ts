import { projectsApi } from '@/api/projects'
import type { CreateProjectRequest, Project, UpdateProjectRequest } from '@/types'
import { defineStore } from 'pinia'
import { computed, ref } from 'vue'

export const useProjectsStore = defineStore('projects', () => {
  const projects = ref<Project[]>([])
  const currentProject = ref<Project | null>(null)
  const loading = ref(false)

  const projectsById = computed(() => {
    const map = new Map<number, Project>()
    projects.value.forEach(project => {
      map.set(project.id, project)
    })
    return map
  })

  const getProjects = async () => {
    try {
      loading.value = true
      projects.value = await projectsApi.getProjects()
    } catch (error) {
      throw error
    } finally {
      loading.value = false
    }
  }

  const getProject = async (id: number) => {
    try {
      currentProject.value = await projectsApi.getProject(id)
      return currentProject.value
    } catch (error) {
      throw error
    }
  }

  const createProject = async (projectData: CreateProjectRequest) => {
    try {
      const newProject = await projectsApi.createProject(projectData)
      projects.value.push(newProject)
      return newProject
    } catch (error) {
      throw error
    }
  }

  const updateProject = async (id: number, projectData: UpdateProjectRequest) => {
    try {
      const updatedProject = await projectsApi.updateProject(id, projectData)

      // Update in projects array
      const index = projects.value.findIndex(p => p.id === id)
      if (index !== -1) {
        projects.value[index] = updatedProject
      }

      // Update current project if it's the same
      if (currentProject.value?.id === id) {
        currentProject.value = updatedProject
      }

      return updatedProject
    } catch (error) {
      throw error
    }
  }

  const deleteProject = async (id: number) => {
    try {
      await projectsApi.deleteProject(id)

      // Remove from projects array
      projects.value = projects.value.filter(p => p.id !== id)

      // Clear current project if it's the deleted one
      if (currentProject.value?.id === id) {
        currentProject.value = null
      }
    } catch (error) {
      throw error
    }
  }

  const inviteMember = async (projectId: number, email: string) => {
    try {
      const member = await projectsApi.inviteMember(projectId, { email })

      // Update project members if we have the project loaded
      const project = projects.value.find(p => p.id === projectId)
      if (project) {
        project.members.push(member.user)
      }

      if (currentProject.value?.id === projectId) {
        currentProject.value.members.push(member.user)
      }

      return member
    } catch (error) {
      throw error
    }
  }

  const getProjectMembers = async (projectId: number) => {
    try {
      return await projectsApi.getProjectMembers(projectId)
    } catch (error) {
      throw error
    }
  }

  return {
    projects,
    currentProject,
    projectsById,
    getProjects,
    getProject,
    createProject,
    updateProject,
    deleteProject,
    inviteMember,
    getProjectMembers
  }
})
