<template>
  <div class="space-y-6">
    <div class="flex justify-between items-center">
      <h1 class="text-3xl font-bold text-gray-900 dark:text-white">Dashboard</h1>
      <button
        @click="showCreateModal = true"
        class="btn btn-primary"
      >
        Create Project
      </button>
    </div>

    <div v-if="loading" class="flex justify-center py-8">
      <svg class="animate-spin h-8 w-8 text-primary-600" fill="none" viewBox="0 0 24 24">
        <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
        <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
      </svg>
    </div>

    <div v-else-if="projects.length === 0" class="text-center py-12">
      <svg class="mx-auto h-12 w-12 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
      </svg>
      <h3 class="mt-2 text-sm font-medium text-gray-900 dark:text-white">No projects</h3>
      <p class="mt-1 text-sm text-gray-500 dark:text-gray-400">Get started by creating a new project.</p>
    </div>

    <div v-else class="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
      <div
        v-for="project in projects"
        :key="project.id"
        @click="openProject(project.id)"
        class="card cursor-pointer hover:shadow-lg transition-shadow"
      >
        <div class="flex items-start justify-between">
          <div class="flex-1">
            <h3 class="text-lg font-semibold text-gray-900 dark:text-white mb-2">{{ project.name }}</h3>
            <p class="text-gray-600 dark:text-gray-400 text-sm mb-4">{{ project.description }}</p>
            <div class="flex items-center justify-between text-sm text-gray-500 dark:text-gray-400">
              <span>{{ project.taskCount }} tasks</span>
              <span>{{ project.members.length }} members</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Create Project Modal -->
    <div v-if="showCreateModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-gray-800 rounded-lg p-6 w-full max-w-md">
        <h2 class="text-xl font-bold text-gray-900 dark:text-white mb-4">Create New Project</h2>

        <form @submit.prevent="createProject">
          <div class="mb-4">
            <label for="projectName" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Project Name
            </label>
            <input
              id="projectName"
              v-model="newProject.name"
              type="text"
              required
              class="input"
              placeholder="Enter project name"
            />
          </div>

          <div class="mb-6">
            <label for="projectDescription" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Description
            </label>
            <textarea
              id="projectDescription"
              v-model="newProject.description"
              rows="3"
              class="input"
              placeholder="Enter project description (optional)"
            ></textarea>
          </div>

          <div class="flex justify-end space-x-3">
            <button
              type="button"
              @click="showCreateModal = false"
              class="btn btn-secondary"
            >
              Cancel
            </button>
            <button
              type="submit"
              :disabled="creating"
              class="btn btn-primary"
            >
              <span v-if="creating" class="mr-2">
                <svg class="animate-spin h-4 w-4" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
              </span>
              Create
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useProjectsStore } from '@/store/projects'
import { onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'

const projectsStore = useProjectsStore()
const router = useRouter()

const loading = ref(false)
const creating = ref(false)
const showCreateModal = ref(false)
const newProject = reactive({
  name: '',
  description: ''
})

const projects = projectsStore.projects

const openProject = (projectId: number) => {
  router.push(`/projects/${projectId}`)
}

const createProject = async () => {
  try {
    creating.value = true
    await projectsStore.createProject(newProject)
    showCreateModal.value = false
    newProject.name = ''
    newProject.description = ''
  } catch (error: any) {
    console.error('Failed to create project:', error)
    if (window.$toast) {
      window.$toast('error', 'Create Failed', error.response?.data?.message || 'Failed to create project')
    }
  } finally {
    creating.value = false
  }
}

onMounted(async () => {
  try {
    loading.value = true
    await projectsStore.getProjects()
  } catch (error) {
    console.error('Failed to load projects:', error)
    if (window.$toast) {
      window.$toast('error', 'Load Failed', 'Failed to load projects')
    }
  } finally {
    loading.value = false
  }
})
</script>
