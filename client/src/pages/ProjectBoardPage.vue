<template>
  <div class="project-board-page fade-in">
    <!-- Project Header -->
    <header class="project-header">
      <div class="project-info">
        <h1>{{ currentProject?.name }}</h1>
        <p>{{ currentProject?.description }}</p>
      </div>
      <div class="project-actions">
        <button @click="showMembersModal = true" class="btn btn-secondary" aria-label="View project members">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" style="margin-right: 0.5rem;" aria-hidden="true">
            <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/>
            <circle cx="9" cy="7" r="4"/>
            <path d="M23 21v-2a4 4 0 0 0-3-3.87"/>
            <path d="M16 3.13a4 4 0 0 1 0 7.75"/>
          </svg>
          Members
        </button>
        <button @click="showCreateTaskModal = true" class="btn btn-primary" aria-label="Create new task">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" style="margin-right: 0.5rem;" aria-hidden="true">
            <line x1="12" y1="5" x2="12" y2="19"/>
            <line x1="5" y1="12" x2="19" y2="12"/>
          </svg>
          Add Task
        </button>
      </div>
    </header>

    <!-- Loading State -->
    <div v-if="loading" class="loading-container">
      <div class="loading-spinner"></div>
      <p>Loading tasks...</p>
    </div>

    <!-- Kanban Board -->
    <div v-else class="kanban-board" role="region" aria-label="Task board">
      <div class="kanban-column" v-for="status in statuses" :key="status" role="region" :aria-label="`${getStatusTitle(status)} tasks (${getTasksForStatus(status)?.length || 0})`">
        <header class="column-header">
          <h2>{{ getStatusTitle(status) }}</h2>
          <span class="task-count" aria-label="Number of tasks">{{ getTasksForStatus(status)?.length || 0 }}</span>
        </header>

        <div
          class="column-content"
          :class="{ 'drag-over': dragOverColumn === status }"
          @dragover="handleDragOver($event, status)"
          @dragleave="handleDragLeave"
          @drop="handleDrop($event, status)"
        >
          <TaskCard
            v-for="task in getTasksForStatus(status)"
            :key="task.id"
            :task="task"
            @edit="handleEditTask"
            @delete="handleDeleteTask"
            @dragstart="(event) => handleDragStart(event, task)"
          />

          <!-- Empty state for column -->
          <div v-if="!getTasksForStatus(status) || getTasksForStatus(status).length === 0" class="empty-column">
            <p>No {{ status.toLowerCase() }} tasks</p>
            <p class="empty-hint">Drag tasks here or create new ones</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Create Task Modal -->
    <Modal v-model:isOpen="showCreateTaskModal" title="Create New Task" large>
      <form @submit.prevent="handleCreateTask" class="task-form">
        <div class="form-group">
          <label for="taskTitle" class="form-label">Task Title</label>
          <input
            id="taskTitle"
            v-model="createTaskForm.title"
            type="text"
            class="form-input"
            placeholder="Enter task title"
            required
            maxlength="200"
          />
        </div>

        <div class="form-group">
          <label for="taskDescription" class="form-label">Description</label>
          <textarea
            id="taskDescription"
            v-model="createTaskForm.description"
            class="form-textarea"
            placeholder="Describe the task..."
            rows="4"
            maxlength="1000"
          />
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="taskStatus" class="form-label">Status</label>
            <select id="taskStatus" v-model="createTaskForm.status" class="form-input">
              <option v-for="status in statuses" :key="status" :value="status">
                {{ getStatusTitle(status) }}
              </option>
            </select>
          </div>

          <div class="form-group">
            <label for="taskAssignee" class="form-label">Assignee</label>
            <select id="taskAssignee" v-model="createTaskForm.assigneeId" class="form-input">
              <option value="">Unassigned</option>
              <option v-for="member in projectMembers" :key="member.id" :value="member.id">
                {{ member.fullName }}
              </option>
            </select>
          </div>
        </div>

        <div class="form-group">
          <label for="taskDueDate" class="form-label">Due Date (Optional)</label>
          <input
            id="taskDueDate"
            v-model="createTaskForm.dueDate"
            type="date"
            class="form-input"
          />
        </div>
      </form>

      <template #footer>
        <button @click="showCreateTaskModal = false" class="btn btn-secondary">
          Cancel
        </button>
        <button @click="handleCreateTask" class="btn btn-primary" :disabled="!canCreateTask">
          Create Task
        </button>
      </template>
    </Modal>

    <!-- Edit Task Modal -->
    <Modal v-model:isOpen="showEditTaskModal" title="Edit Task" large>
      <form @submit.prevent="handleUpdateTask" class="task-form">
        <div class="form-group">
          <label for="editTaskTitle" class="form-label">Task Title</label>
          <input
            id="editTaskTitle"
            v-model="editTaskForm.title"
            type="text"
            class="form-input"
            placeholder="Enter task title"
            required
            maxlength="200"
          />
        </div>

        <div class="form-group">
          <label for="editTaskDescription" class="form-label">Description</label>
          <textarea
            id="editTaskDescription"
            v-model="editTaskForm.description"
            class="form-textarea"
            placeholder="Describe the task..."
            rows="4"
            maxlength="1000"
          />
        </div>

        <div class="form-row">
          <div class="form-group">
            <label for="editTaskStatus" class="form-label">Status</label>
            <select id="editTaskStatus" v-model="editTaskForm.status" class="form-input">
              <option v-for="status in statuses" :key="status" :value="status">
                {{ getStatusTitle(status) }}
              </option>
            </select>
          </div>

          <div class="form-group">
            <label for="editTaskAssignee" class="form-label">Assignee</label>
            <select id="editTaskAssignee" v-model="editTaskForm.assigneeId" class="form-input">
              <option value="">Unassigned</option>
              <option v-for="member in projectMembers" :key="member.id" :value="member.id">
                {{ member.fullName }}
              </option>
            </select>
          </div>
        </div>

        <div class="form-group">
          <label for="editTaskDueDate" class="form-label">Due Date (Optional)</label>
          <input
            id="editTaskDueDate"
            v-model="editTaskForm.dueDate"
            type="date"
            class="form-input"
          />
        </div>
      </form>

      <template #footer>
        <button @click="showEditTaskModal = false" class="btn btn-secondary">
          Cancel
        </button>
        <button @click="handleUpdateTask" class="btn btn-primary" :disabled="!canEditTask">
          Update Task
        </button>
      </template>
    </Modal>

    <!-- Delete Confirmation Modal -->
    <Modal v-model:isOpen="showDeleteModal" title="Delete Task" :large="false">
      <div class="delete-confirmation">
        <p>Are you sure you want to delete <strong>{{ taskToDelete?.title }}</strong>?</p>
        <p class="delete-warning">This action cannot be undone.</p>
      </div>

      <template #footer>
        <button @click="showDeleteModal = false" class="btn btn-secondary">
          Cancel
        </button>
        <button @click="confirmDeleteTask" class="btn btn-danger">
          Delete Task
        </button>
      </template>
    </Modal>

    <!-- Project Members Modal -->
    <Modal v-model:isOpen="showMembersModal" title="Project Members" large>
      <div class="members-section">
        <div class="members-header">
          <h3>Current Members</h3>
          <button @click="showInviteMemberModal = true" class="btn btn-primary btn-sm">
            Invite Member
          </button>
        </div>

        <div v-if="loadingMembers" class="members-loading">
          <div class="loading-spinner"></div>
          <p>Loading members...</p>
        </div>

        <div v-else-if="projectMembers.length === 0" class="members-empty">
          <p>No members yet. Invite team members to collaborate on this project.</p>
        </div>

        <div v-else class="members-list">
          <div v-for="member in projectMembers" :key="member.id" class="member-item">
            <div class="member-avatar">
              {{ member.fullName?.charAt(0)?.toUpperCase() || '?' }}
            </div>
            <div class="member-info">
              <div class="member-name">{{ member.fullName || 'Unknown User' }}</div>
              <div class="member-email">{{ member.email || 'No email' }}</div>
              <div class="member-role">Member</div>
            </div>
          </div>
        </div>
      </div>

      <template #footer>
        <button @click="showMembersModal = false" class="btn btn-secondary">
          Close
        </button>
      </template>
    </Modal>

    <!-- Invite Member Modal -->
    <Modal v-model:isOpen="showInviteMemberModal" title="Invite Member" :large="false">
      <form @submit.prevent="handleInviteToProject" class="invite-form">
        <div class="form-group">
          <label for="projectInviteEmail" class="form-label">Email Address</label>
          <input
            id="projectInviteEmail"
            v-model="projectInviteForm.email"
            type="email"
            class="form-input"
            placeholder="Enter email address"
            required
          />
        </div>
      </form>

      <template #footer>
        <button @click="showInviteMemberModal = false" class="btn btn-secondary">
          Cancel
        </button>
        <button @click="handleInviteToProject" class="btn btn-primary" :disabled="!canInviteToProject">
          Send Invitation
        </button>
      </template>
    </Modal>
  </div>
</template>

<script setup lang="ts">
import Modal from '@/components/Modal.vue'
import TaskCard from '@/components/TaskCard.vue'
import { useToast } from '@/composables/useToast'
import { useProjectsStore } from '@/store/projects'
import { useTasksStore } from '@/store/tasks'
import type { Task } from '@/types'
import { TaskStatus } from '@/types'
import { computed, onMounted, reactive, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'

const route = useRoute()
const router = useRouter()
const projectsStore = useProjectsStore()
const tasksStore = useTasksStore()
const { success: showSuccess, error: showError } = useToast()

const loading = ref(true)
const loadingMembers = ref(false)
const dragOverColumn = ref<TaskStatus | null>(null)
const showCreateTaskModal = ref(false)
const showEditTaskModal = ref(false)
const showDeleteModal = ref(false)
const showMembersModal = ref(false)
const showInviteMemberModal = ref(false)
const taskToDelete = ref<Task | null>(null)
const draggedTask = ref<Task | null>(null)

const projectInviteForm = reactive({
  email: ''
})

const createTaskForm = reactive({
  title: '',
  description: '',
  status: TaskStatus.Todo,
  assigneeId: '',
  dueDate: ''
})

const editTaskForm = reactive({
  title: '',
  description: '',
  status: TaskStatus.Todo,
  assigneeId: '',
  dueDate: ''
})

const editingTask = ref<Task | null>(null)

const projectId = computed(() => parseInt(route.params.id as string))
const currentProject = computed(() => projectsStore.currentProject)
// const tasks = computed(() => tasksStore.tasks) // Not used directly
const tasksByStatus = computed(() => tasksStore.tasksByStatus)
const projectMembers = computed(() => currentProject.value?.members || [])

const statuses = [TaskStatus.Todo, TaskStatus.InProgress, TaskStatus.Done]

const canCreateTask = computed(() => createTaskForm.title.trim().length > 0)

const canEditTask = computed(() => editTaskForm.title.trim().length > 0)

const canInviteToProject = computed(() => projectInviteForm.email.trim().length > 0 && /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(projectInviteForm.email.trim()))

const projectMembersData = ref<any[]>([])

const getStatusTitle = (status: TaskStatus) => {
  return status.replace(/([A-Z])/g, ' $1').trim()
}

const getTasksForStatus = (status: TaskStatus): Task[] => {
  const statusKey = status === TaskStatus.Todo ? 'Todo' :
                   status === TaskStatus.InProgress ? 'InProgress' :
                   status === TaskStatus.Done ? 'Done' : 'Todo'
  return tasksByStatus.value[statusKey] || []
}

const loadProjectAndTasks = async () => {
  try {
    loading.value = true

    // Load project
    await projectsStore.getProject(projectId.value)

    // Load tasks
    await tasksStore.getTasksForProject(projectId.value)
  } catch (error: any) {
    const message = error.response?.data?.message || 'Failed to load project. Please try again.'
    showError('Load Failed', message)
    router.push('/dashboard')
  } finally {
    loading.value = false
  }
}

const handleCreateTask = async () => {
  if (!canCreateTask.value || !projectId.value) return

  try {
    // Convert TaskStatus enum to backend-compatible format (integer)
    const statusValue = createTaskForm.status === TaskStatus.Todo ? 0 :
                       createTaskForm.status === TaskStatus.InProgress ? 1 :
                       createTaskForm.status === TaskStatus.Done ? 2 : 0

    await tasksStore.createTask(projectId.value, {
      title: createTaskForm.title.trim(),
      description: createTaskForm.description.trim(),
      status: statusValue,
      assigneeId: createTaskForm.assigneeId ? parseInt(createTaskForm.assigneeId) || undefined : undefined,
      dueDate: createTaskForm.dueDate || undefined,
      order: getNextOrderForStatus(createTaskForm.status)
    })

    showSuccess('Task Created', 'Your task has been created successfully!')
    showCreateTaskModal.value = false
    resetCreateTaskForm()
  } catch (error: any) {
    const message = error.response?.data?.message || 'Failed to create task. Please try again.'
    showError('Create Failed', message)
  }
}

const handleEditTask = (task: Task) => {
  editingTask.value = task
  editTaskForm.title = task.title
  editTaskForm.description = task.description
  // Convert status to string enum if it's a number
  editTaskForm.status = typeof task.status === 'number'
    ? (task.status === 0 ? TaskStatus.Todo : task.status === 1 ? TaskStatus.InProgress : TaskStatus.Done)
    : task.status
  editTaskForm.assigneeId = task.assignee?.id?.toString() || ''
  editTaskForm.dueDate = task.dueDate ? task.dueDate.split('T')[0] : ''
  showEditTaskModal.value = true
}

const handleUpdateTask = async () => {
  if (!canEditTask.value || !editingTask.value) return

  try {
    // Convert TaskStatus enum to backend-compatible format (integer)
    const statusValue = editTaskForm.status === TaskStatus.Todo ? 0 :
                       editTaskForm.status === TaskStatus.InProgress ? 1 :
                       editTaskForm.status === TaskStatus.Done ? 2 : 0

    await tasksStore.updateTask(editingTask.value.id, {
      title: editTaskForm.title.trim(),
      description: editTaskForm.description.trim(),
      status: statusValue,
      assigneeId: editTaskForm.assigneeId ? parseInt(editTaskForm.assigneeId) : undefined,
      dueDate: editTaskForm.dueDate || undefined,
      order: editingTask.value.order
    })

    showSuccess('Task Updated', 'Your task has been updated successfully!')
    showEditTaskModal.value = false
    editingTask.value = null
    resetEditTaskForm()
  } catch (error: any) {
    const message = error.response?.data?.message || 'Failed to update task. Please try again.'
    showError('Update Failed', message)
  }
}

const handleDeleteTask = (task: Task) => {
  taskToDelete.value = task
  showDeleteModal.value = true
}

const confirmDeleteTask = async () => {
  if (!taskToDelete.value) return

  try {
    await tasksStore.deleteTask(taskToDelete.value.id)
    showSuccess('Task Deleted', 'The task has been deleted successfully!')
    showDeleteModal.value = false
    taskToDelete.value = null
  } catch (error: any) {
    const message = error.response?.data?.message || 'Failed to delete task. Please try again.'
    showError('Delete Failed', message)
  }
}

// Drag and Drop functionality
const handleDragStart = (_event: DragEvent, task: Task) => {
  draggedTask.value = task
}

const handleDragOver = (event: DragEvent, status: TaskStatus) => {
  event.preventDefault()
  dragOverColumn.value = status
}

const handleDragLeave = () => {
  dragOverColumn.value = null
}

const handleDrop = async (event: DragEvent, newStatus: TaskStatus) => {
  event.preventDefault()
  dragOverColumn.value = null

  if (!draggedTask.value) return

  try {
    const newOrder = getNextOrderForStatus(newStatus)
    await tasksStore.moveTask(draggedTask.value.id, newStatus, newOrder)
    showSuccess('Task Moved', 'Task status has been updated!')
  } catch (error: any) {
    const message = error.response?.data?.message || 'Failed to update task status. Please try again.'
    showError('Update Failed', message)
    // Reload tasks to revert optimistic update
    await tasksStore.getTasksForProject(projectId.value)
  }

  draggedTask.value = null
}

const getNextOrderForStatus = (status: TaskStatus): number => {
  // Get tasks directly from the store to avoid stale computed values during drag operations
  const statusKey = status === TaskStatus.Todo ? 'Todo' :
                   status === TaskStatus.InProgress ? 'InProgress' :
                   status === TaskStatus.Done ? 'Done' : 'Todo'
  const tasksInStatus = tasksByStatus.value[statusKey] || []

  if (tasksInStatus.length === 0) return 1

  const orders = tasksInStatus
    .map(t => t.order)
    .filter(order => typeof order === 'number' && !isNaN(order) && order >= 0)
  return orders.length > 0 ? Math.max(...orders) + 1 : 1
}

const resetCreateTaskForm = () => {
  createTaskForm.title = ''
  createTaskForm.description = ''
  createTaskForm.status = TaskStatus.Todo
  createTaskForm.assigneeId = ''
  createTaskForm.dueDate = ''
}

const resetEditTaskForm = () => {
  editTaskForm.title = ''
  editTaskForm.description = ''
  editTaskForm.status = TaskStatus.Todo
  editTaskForm.assigneeId = ''
  editTaskForm.dueDate = ''
}

const loadProjectMembers = async () => {
  if (!currentProject.value?.id) return

  try {
    loadingMembers.value = true
    projectMembersData.value = await projectsStore.getProjectMembers(currentProject.value.id)
  } catch (error: any) {
    const message = error.response?.data?.message || 'Failed to load project members.'
    showError('Load Failed', message)
  } finally {
    loadingMembers.value = false
  }
}

const handleInviteToProject = async () => {
  if (!canInviteToProject.value || !currentProject.value) return

  try {
    await projectsStore.inviteMember(currentProject.value.id, projectInviteForm.email.trim())

    showSuccess('Invitation Sent', `An invitation has been sent to ${projectInviteForm.email.trim()}`)
    showInviteMemberModal.value = false
    resetProjectInviteForm()

    // Reload members to show the new invitation
    await loadProjectMembers()
  } catch (error: any) {
    const message = error.response?.data?.message || 'Failed to send invitation. Please try again.'
    showError('Invitation Failed', message)
  }
}

const resetProjectInviteForm = () => {
  projectInviteForm.email = ''
}

// Load data when project ID changes
watch(projectId, () => {
  if (projectId.value) {
    loadProjectAndTasks()
    loadProjectMembers()
  }
})

onMounted(() => {
  if (projectId.value) {
    loadProjectAndTasks()
    loadProjectMembers()
  }
})
</script>

<style scoped>
.project-board-page {
  min-height: calc(100vh - 4rem);
  padding: 2rem 1rem;
}

.project-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 3rem;
  padding-bottom: 1.5rem;
  border-bottom: 2px solid #e2e8f0;
  background-color: #ffffff;
  padding: 2rem;
  border-radius: 0.75rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.project-info h1 {
  font-size: 2.5rem;
  font-weight: 700;
  color: #1a202c;
  margin: 0 0 0.5rem 0;
}

.project-info p {
  color: #4a5568;
  font-size: 1.125rem;
  margin: 0;
  max-width: 32rem;
}

.kanban-board {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 2rem;
  height: calc(100vh - 12rem);
}

.kanban-column {
  display: flex;
  flex-direction: column;
  background-color: #ffffff;
  border-radius: 0.75rem;
  border: 1px solid #e2e8f0;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.column-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 1rem 1.5rem;
  border-bottom: 1px solid #e2e8f0;
  background-color: #f8fafc;
  border-radius: 0.75rem 0.75rem 0 0;
}

.column-header h2 {
  font-size: 1rem;
  font-weight: 600;
  color: #2d3748;
  margin: 0;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.task-count {
  background-color: #4299e1;
  color: white;
  font-size: 0.75rem;
  font-weight: 600;
  padding: 0.25rem 0.5rem;
  border-radius: 9999px;
  min-width: 1.5rem;
  text-align: center;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
}

.column-content {
  flex: 1;
  padding: 1rem;
  overflow-y: auto;
  transition: background-color 0.2s ease-in-out;
}

.column-content.drag-over {
  background-color: #ebf8ff;
  border: 2px dashed #3182ce;
  border-radius: 0.75rem;
  box-shadow: inset 0 0 0 2px rgba(49, 130, 206, 0.2);
}

.empty-column {
  text-align: center;
  padding: 2rem 1rem;
  color: #a0aec0;
}

.empty-column p {
  margin: 0.5rem 0;
  font-size: 0.875rem;
}

.empty-hint {
  font-style: italic;
}

.task-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
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

/* Members Section Styles */
.members-section {
  max-height: 60vh;
  overflow-y: auto;
}

.members-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1.5rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid #e2e8f0;
}

.members-header h3 {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 600;
  color: #2d3748;
}

.members-loading {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 2rem;
  color: #4a5568;
}

.members-loading p {
  margin-top: 1rem;
  font-size: 1rem;
}

.members-empty {
  text-align: center;
  padding: 2rem;
  color: #4a5568;
}

.members-empty p {
  margin: 0;
  font-size: 1rem;
}

.members-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.member-item {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  background-color: #f7fafc;
  border-radius: 0.5rem;
  border: 1px solid #e2e8f0;
}

.member-avatar {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 3rem;
  height: 3rem;
  background-color: #4299e1;
  color: white;
  border-radius: 50%;
  font-size: 1rem;
  font-weight: 600;
  flex-shrink: 0;
}

.member-info {
  flex: 1;
  min-width: 0;
}

.member-name {
  font-weight: 600;
  color: #2d3748;
  margin-bottom: 0.25rem;
  font-size: 1rem;
}

.member-email {
  color: #4a5568;
  font-size: 0.875rem;
  margin-bottom: 0.25rem;
}

.member-role {
  color: #718096;
  font-size: 0.75rem;
  text-transform: uppercase;
  font-weight: 500;
  letter-spacing: 0.05em;
}

@media (max-width: 1024px) {
  .kanban-board {
    grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
    gap: 1.5rem;
  }
}

@media (max-width: 768px) {
  .project-board-page {
    padding: 1rem 0.5rem;
  }

  .project-header {
    flex-direction: column;
    align-items: flex-start;
    gap: 1.5rem;
    margin-bottom: 2rem;
  }

  .project-info h1 {
    font-size: 2rem;
  }

  .project-info p {
    font-size: 1rem;
  }

  .kanban-board {
    grid-template-columns: 1fr;
    gap: 1.5rem;
    height: auto;
  }

  .form-row {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 640px) {
  .project-header {
    margin-bottom: 1.5rem;
    padding-bottom: 1rem;
  }

  .project-info h1 {
    font-size: 1.75rem;
  }

  .column-header {
    padding: 0.75rem 1rem;
  }

  .column-content {
    padding: 0.75rem;
  }
}
</style>
