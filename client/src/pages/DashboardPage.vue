<template>
  <div class="dashboard-page fade-in">
    <div class="dashboard-header">
      <div class="dashboard-title">
        <h1>Your Projects</h1>
        <p>Manage and collaborate on your projects</p>
      </div>
      <button @click="showCreateModal = true" class="btn btn-primary">
        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" style="margin-right: 0.5rem;">
          <line x1="12" y1="5" x2="12" y2="19"/>
          <line x1="5" y1="12" x2="19" y2="12"/>
        </svg>
        New Project
      </button>
    </div>

    <div v-if="loading" class="loading-container">
      <div class="loading-spinner"></div>
      <p>Loading projects...</p>
    </div>

    <div v-else-if="projects.length === 0" class="empty-state">
      <div class="empty-state-icon">
        <svg width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
          <path d="M12 2L2 7l10 5 10-5-10-5z"/>
          <path d="M2 17l10 5 10-5"/>
          <path d="M2 12l10 5 10-5"/>
        </svg>
      </div>
      <h2>No projects yet</h2>
      <p>Create your first project to get started with Lumo</p>
      <button @click="showCreateModal = true" class="btn btn-primary">
        Create Your First Project
      </button>
    </div>

    <div v-else class="projects-grid">
      <ProjectCard
        v-for="project in projects"
        :key="project.id"
        :project="project"
        @edit="handleEditProject"
        @delete="handleDeleteProject"
        @open="handleOpenProject"
        @invite="handleInviteMember"
      />
    </div>

    <!-- Create Project Modal -->
    <Modal v-model:isOpen="showCreateModal" title="Create New Project" large>
      <form @submit.prevent="handleCreateProject" class="project-form">
        <div class="form-group">
          <label for="projectName" class="form-label">Project Name</label>
          <input
            id="projectName"
            v-model="createForm.name"
            type="text"
            class="form-input"
            placeholder="Enter project name"
            required
            maxlength="100"
          />
        </div>

        <div class="form-group">
          <label for="projectDescription" class="form-label">Description</label>
          <textarea
            id="projectDescription"
            v-model="createForm.description"
            class="form-textarea"
            placeholder="Describe your project..."
            rows="4"
            maxlength="500"
          />
        </div>
      </form>

      <template #footer>
        <button @click="showCreateModal = false" class="btn btn-secondary">
          Cancel
        </button>
        <button @click="handleCreateProject" class="btn btn-primary" :disabled="!canCreate">
          Create Project
        </button>
      </template>
    </Modal>

    <!-- Edit Project Modal -->
    <Modal v-model:isOpen="showEditModal" title="Edit Project" large>
      <form @submit.prevent="handleUpdateProject" class="project-form">
        <div class="form-group">
          <label for="editProjectName" class="form-label">Project Name</label>
          <input
            id="editProjectName"
            v-model="editForm.name"
            type="text"
            class="form-input"
            placeholder="Enter project name"
            required
            maxlength="100"
          />
        </div>

        <div class="form-group">
          <label for="editProjectDescription" class="form-label">Description</label>
          <textarea
            id="editProjectDescription"
            v-model="editForm.description"
            class="form-textarea"
            placeholder="Describe your project..."
            rows="4"
            maxlength="500"
          />
        </div>
      </form>

      <template #footer>
        <button @click="showEditModal = false" class="btn btn-secondary">
          Cancel
        </button>
        <button @click="handleUpdateProject" class="btn btn-primary" :disabled="!canEdit">
          Update Project
        </button>
      </template>
    </Modal>

    <!-- Delete Confirmation Modal -->
    <Modal v-model:isOpen="showDeleteModal" title="Delete Project" :large="false">
      <div class="delete-confirmation">
        <p>Are you sure you want to delete <strong>{{ projectToDelete?.name }}</strong>?</p>
        <p class="delete-warning">This action cannot be undone and will permanently delete the project and all its tasks.</p>
      </div>

      <template #footer>
        <button @click="showDeleteModal = false" class="btn btn-secondary">
          Cancel
        </button>
        <button @click="confirmDeleteProject" class="btn btn-danger">
          Delete Project
        </button>
      </template>
    </Modal>

    <!-- Invite Member Modal -->
    <Modal v-model:isOpen="showInviteModal" title="Invite Member" :large="false">
      <form @submit.prevent="handleInviteMemberSubmit" class="invite-form">
        <div class="form-group">
          <label for="inviteEmail" class="form-label">Email Address</label>
          <input
            id="inviteEmail"
            v-model="inviteForm.email"
            type="email"
            class="form-input"
            placeholder="Enter email address"
            required
          />
        </div>
      </form>

      <template #footer>
        <button @click="showInviteModal = false" class="btn btn-secondary">
          Cancel
        </button>
        <button type="submit" class="btn btn-primary" :disabled="!canInvite">
          Send Invitation
        </button>
      </template>
    </Modal>
  </div>
</template>

<script setup lang="ts">
import Modal from '@/components/Modal.vue'
import ProjectCard from '@/components/ProjectCard.vue'
import { useToast } from '@/composables/useToast'
import { useProjectsStore } from '@/store/projects'
import type { Project } from '@/types'
import { computed, onMounted, reactive, ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const projectsStore = useProjectsStore()
const { success: showSuccess, error: showError } = useToast()

const showCreateModal = ref(false)
const showEditModal = ref(false)
const showDeleteModal = ref(false)
const showInviteModal = ref(false)
const projectToDelete = ref<Project | null>(null)
const projectToInvite = ref<Project | null>(null)

const createForm = reactive({
  name: '',
  description: ''
})

const editForm = reactive({
  name: '',
  description: ''
})

const inviteForm = reactive({
  email: ''
})

const editingProject = ref<Project | null>(null)

const projects = computed(() => projectsStore.projects)
const loading = computed(() => projectsStore.loading)

const canCreate = computed(() => createForm.name.trim().length > 0)

const canEdit = computed(() => editForm.name.trim().length > 0)

const canInvite = computed(() => inviteForm.email.trim().length > 0 && /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(inviteForm.email.trim()))

const handleCreateProject = async () => {
  if (!canCreate.value) return

  try {
    await projectsStore.createProject({
      name: createForm.name.trim(),
      description: createForm.description.trim()
    })

    showSuccess('Project Created', 'Your project has been created successfully!')
    showCreateModal.value = false
    resetCreateForm()
  } catch (error: any) {
    const message = error.response?.data?.message || 'Failed to create project. Please try again.'
    showError('Create Failed', message)
  }
}

const handleEditProject = (project: Project) => {
  editingProject.value = project
  editForm.name = project.name
  editForm.description = project.description
  showEditModal.value = true
}

const handleUpdateProject = async () => {
  if (!canEdit.value || !editingProject.value) return

  try {
    await projectsStore.updateProject(editingProject.value.id, {
      name: editForm.name.trim(),
      description: editForm.description.trim()
    })

    showSuccess('Project Updated', 'Your project has been updated successfully!')
    showEditModal.value = false
    editingProject.value = null
    resetEditForm()
  } catch (error: any) {
    const message = error.response?.data?.message || 'Failed to update project. Please try again.'
    showError('Update Failed', message)
  }
}

const handleDeleteProject = (project: Project) => {
  projectToDelete.value = project
  showDeleteModal.value = true
}

const confirmDeleteProject = async () => {
  if (!projectToDelete.value) return

  try {
    await projectsStore.deleteProject(projectToDelete.value.id)
    showSuccess('Project Deleted', 'The project has been deleted successfully!')
    showDeleteModal.value = false
    projectToDelete.value = null
  } catch (error: any) {
    const message = error.response?.data?.message || 'Failed to delete project. Please try again.'
    showError('Delete Failed', message)
  }
}

const handleOpenProject = (project: Project) => {
  router.push(`/projects/${project.id}`)
}

const handleInviteMember = (project: Project) => {
  projectToInvite.value = project
  inviteForm.email = ''
  showInviteModal.value = true
}

const resetCreateForm = () => {
  createForm.name = ''
  createForm.description = ''
}

const resetEditForm = () => {
  editForm.name = ''
  editForm.description = ''
}

const handleInviteMemberSubmit = async () => {
  if (!canInvite.value || !projectToInvite.value) return

  try {
    await projectsStore.inviteMember(projectToInvite.value.id, inviteForm.email.trim())

    showSuccess('Invitation Sent', `An invitation has been sent to ${inviteForm.email.trim()}`)
    showInviteModal.value = false
    resetInviteForm()
  } catch (error: any) {
    const message = error.response?.data?.message || 'Failed to send invitation. Please try again.'
    showError('Invitation Failed', message)
  }
}

const resetInviteForm = () => {
  inviteForm.email = ''
}

const loadProjects = async () => {
  try {
    await projectsStore.getProjects()
  } catch (error: any) {
    const message = error.response?.data?.message || 'Failed to load projects. Please try again.'
    showError('Load Failed', message)
  }
}

onMounted(() => {
  loadProjects()
})
</script>

<style scoped>
.dashboard-page {
  max-width: 1200px;
  margin: 0 auto;
  padding: 2rem 1rem;
}

.dashboard-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 3rem;
  padding-bottom: 1.5rem;
  border-bottom: 1px solid #e2e8f0;
}

.dashboard-title h1 {
  font-size: 2.5rem;
  font-weight: 700;
  color: #2d3748;
  margin: 0 0 0.5rem 0;
}

.dashboard-title p {
  color: #4a5568;
  font-size: 1.125rem;
  margin: 0;
}

.projects-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 2rem;
}

.loading-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4rem 2rem;
  color: #4a5568;
}

.loading-container p {
  margin-top: 1rem;
  font-size: 1.125rem;
}

.empty-state {
  text-align: center;
  padding: 4rem 2rem;
  color: #4a5568;
}

.empty-state-icon {
  margin-bottom: 2rem;
  color: #a0aec0;
}

.empty-state h2 {
  font-size: 1.875rem;
  font-weight: 600;
  margin-bottom: 1rem;
  color: #2d3748;
}

.empty-state p {
  font-size: 1.125rem;
  margin-bottom: 2rem;
  max-width: 32rem;
  margin-left: auto;
  margin-right: auto;
}

.project-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.delete-confirmation {
  text-align: center;
  padding: 1rem 0;
}

.delete-confirmation p {
  margin-bottom: 1rem;
  font-size: 1rem;
  color: #4a5568;
}

.delete-warning {
  background-color: #fed7d7;
  color: #742a2a;
  padding: 1rem;
  border-radius: 0.5rem;
  font-size: 0.875rem;
  margin: 0 !important;
}

@media (max-width: 768px) {
  .dashboard-page {
    padding: 1rem 0.5rem;
  }

  .dashboard-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 1.5rem;
    margin-bottom: 2rem;
  }

  .dashboard-title h1 {
    font-size: 2rem;
  }

  .dashboard-title p {
    font-size: 1rem;
  }

  .projects-grid {
    grid-template-columns: 1fr;
    gap: 1.5rem;
  }
}

@media (max-width: 640px) {
  .dashboard-header {
    margin-bottom: 1.5rem;
    padding-bottom: 1rem;
  }

  .dashboard-title h1 {
    font-size: 1.75rem;
  }

  .empty-state {
    padding: 2rem 1rem;
  }

  .empty-state h2 {
    font-size: 1.5rem;
  }

  .empty-state p {
    font-size: 1rem;
  }
}
</style>
