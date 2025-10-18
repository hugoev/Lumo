<template>
  <div class="space-y-6">
    <!-- Header -->
    <div class="flex justify-between items-center">
      <div v-if="project">
        <h1 class="text-3xl font-bold text-gray-900 dark:text-white">{{ project.name }}</h1>
        <p class="text-gray-600 dark:text-gray-400 mt-1">{{ project.description }}</p>
      </div>
      <div class="flex space-x-3">
        <button
          @click="showInviteModal = true"
          class="btn btn-secondary"
        >
          Invite Member
        </button>
        <button
          @click="showCreateTaskModal = true"
          class="btn btn-primary"
        >
          Add Task
        </button>
      </div>
    </div>

    <!-- Loading state -->
    <div v-if="loading" class="flex justify-center py-12">
      <svg class="animate-spin h-8 w-8 text-primary-600" fill="none" viewBox="0 0 24 24">
        <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
        <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
      </svg>
    </div>

    <!-- Kanban Board -->
    <div v-else-if="project" class="grid grid-cols-1 md:grid-cols-3 gap-6">
      <!-- Todo Column -->
      <div class="space-y-4">
        <div class="flex items-center justify-between">
          <h2 class="text-lg font-semibold text-gray-900 dark:text-white">Todo</h2>
          <span class="bg-gray-200 dark:bg-gray-700 text-gray-600 dark:text-gray-400 px-2 py-1 rounded-full text-sm">
            {{ tasksByStatus[TaskStatus.Todo]?.length || 0 }}
          </span>
        </div>

        <div
          ref="todoColumn"
          class="min-h-[400px] bg-gray-100 dark:bg-gray-800 rounded-lg p-4"
          @drop="onDrop($event, TaskStatus.Todo)"
          @dragover.prevent
          @dragenter.prevent
        >
          <TaskCard
            v-for="task in tasksByStatus[TaskStatus.Todo]"
            :key="task.id"
            :task="task"
            :project-members="project.members"
            @edit="editTask"
            @delete="deleteTask"
            @dragstart="onDragStart($event, task)"
          />
        </div>
      </div>

      <!-- In Progress Column -->
      <div class="space-y-4">
        <div class="flex items-center justify-between">
          <h2 class="text-lg font-semibold text-gray-900 dark:text-white">In Progress</h2>
          <span class="bg-blue-200 dark:bg-blue-800 text-blue-600 dark:text-blue-400 px-2 py-1 rounded-full text-sm">
            {{ tasksByStatus[TaskStatus.InProgress]?.length || 0 }}
          </span>
        </div>

        <div
          ref="inProgressColumn"
          class="min-h-[400px] bg-blue-50 dark:bg-blue-900/20 rounded-lg p-4"
          @drop="onDrop($event, TaskStatus.InProgress)"
          @dragover.prevent
          @dragenter.prevent
        >
          <TaskCard
            v-for="task in tasksByStatus[TaskStatus.InProgress]"
            :key="task.id"
            :task="task"
            :project-members="project.members"
            @edit="editTask"
            @delete="deleteTask"
            @dragstart="onDragStart($event, task)"
          />
        </div>
      </div>

      <!-- Done Column -->
      <div class="space-y-4">
        <div class="flex items-center justify-between">
          <h2 class="text-lg font-semibold text-gray-900 dark:text-white">Done</h2>
          <span class="bg-green-200 dark:bg-green-800 text-green-600 dark:text-green-400 px-2 py-1 rounded-full text-sm">
            {{ tasksByStatus[TaskStatus.Done]?.length || 0 }}
          </span>
        </div>

        <div
          ref="doneColumn"
          class="min-h-[400px] bg-green-50 dark:bg-green-900/20 rounded-lg p-4"
          @drop="onDrop($event, TaskStatus.Done)"
          @dragover.prevent
          @dragenter.prevent
        >
          <TaskCard
            v-for="task in tasksByStatus[TaskStatus.Done]"
            :key="task.id"
            :task="task"
            :project-members="project.members"
            @edit="editTask"
            @delete="deleteTask"
            @dragstart="onDragStart($event, task)"
          />
        </div>
      </div>
    </div>

    <!-- Create Task Modal -->
    <div v-if="showCreateTaskModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-gray-800 rounded-lg p-6 w-full max-w-md">
        <h2 class="text-xl font-bold text-gray-900 dark:text-white mb-4">Create New Task</h2>

        <form @submit.prevent="createTask">
          <div class="mb-4">
            <label for="taskTitle" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Title
            </label>
            <input
              id="taskTitle"
              v-model="newTask.title"
              type="text"
              required
              class="input"
              placeholder="Enter task title"
            />
          </div>

          <div class="mb-4">
            <label for="taskDescription" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Description
            </label>
            <textarea
              id="taskDescription"
              v-model="newTask.description"
              rows="3"
              class="input"
              placeholder="Enter task description (optional)"
            ></textarea>
          </div>

          <div class="mb-4">
            <label for="taskAssignee" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Assignee
            </label>
            <select
              id="taskAssignee"
              v-model="newTask.assigneeId"
              class="input"
            >
              <option value="">Unassigned</option>
              <option v-for="member in project?.members" :key="member.id" :value="member.id">
                {{ member.fullName }}
              </option>
            </select>
          </div>

          <div class="mb-4">
            <label for="taskDueDate" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Due Date
            </label>
            <input
              id="taskDueDate"
              v-model="newTask.dueDate"
              type="date"
              class="input"
            />
          </div>

          <div class="flex justify-end space-x-3">
            <button
              type="button"
              @click="showCreateTaskModal = false"
              class="btn btn-secondary"
            >
              Cancel
            </button>
            <button
              type="submit"
              :disabled="creatingTask"
              class="btn btn-primary"
            >
              <span v-if="creatingTask" class="mr-2">
                <svg class="animate-spin h-4 w-4" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
              </span>
              Create Task
            </button>
          </div>
        </form>
      </div>
    </div>

    <!-- Invite Member Modal -->
    <div v-if="showInviteModal" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div class="bg-white dark:bg-gray-800 rounded-lg p-6 w-full max-w-md">
        <h2 class="text-xl font-bold text-gray-900 dark:text-white mb-4">Invite Team Member</h2>

        <form @submit.prevent="inviteMember">
          <div class="mb-4">
            <label for="inviteEmail" class="block text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
              Email Address
            </label>
            <input
              id="inviteEmail"
              v-model="inviteEmail"
              type="email"
              required
              class="input"
              placeholder="Enter email address"
            />
          </div>

          <div class="flex justify-end space-x-3">
            <button
              type="button"
              @click="showInviteModal = false"
              class="btn btn-secondary"
            >
              Cancel
            </button>
            <button
              type="submit"
              :disabled="inviting"
              class="btn btn-primary"
            >
              <span v-if="inviting" class="mr-2">
                <svg class="animate-spin h-4 w-4" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
              </span>
              Invite
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import TaskCard from '@/components/TaskCard.vue'
import { useProjectsStore } from '@/store/projects'
import { useTasksStore } from '@/store/tasks'
import { TaskStatus } from '@/types'
import { computed, onMounted, reactive, ref } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()
const projectsStore = useProjectsStore()
const tasksStore = useTasksStore()

const loading = ref(false)
const showCreateTaskModal = ref(false)
const showInviteModal = ref(false)
const creatingTask = ref(false)
const inviting = ref(false)
const inviteEmail = ref('')

const project = computed(() => projectsStore.currentProject)
const tasksByStatus = computed(() => tasksStore.tasksByStatus)

const projectId = Number(route.params.id)

const newTask = reactive({
  title: '',
  description: '',
  assigneeId: '',
  dueDate: '',
  status: TaskStatus.Todo,
  order: 0
})

const onDragStart = (event: DragEvent, task: any) => {
  if (event.dataTransfer) {
    event.dataTransfer.effectAllowed = 'move'
    event.dataTransfer.setData('text/plain', task.id.toString())
  }
}

const onDrop = async (event: DragEvent, newStatus: TaskStatus) => {
  event.preventDefault()

  const taskId = Number(event.dataTransfer?.getData('text/plain'))
  if (!taskId) return

  const task = tasksStore.tasks.find(t => t.id === taskId)
  if (!task || task.status === newStatus) return

  // Calculate new order
  const targetColumnTasks = tasksByStatus.value[newStatus]
  const newOrder = targetColumnTasks.length > 0 ? Math.max(...targetColumnTasks.map(t => t.order)) + 1 : 0

  try {
    await tasksStore.moveTask(taskId, newStatus, newOrder)
  } catch (error) {
    console.error('Failed to move task:', error)
    if (window.$toast) {
      window.$toast('error', 'Move Failed', 'Failed to move task')
    }
  }
}

const createTask = async () => {
  try {
    creatingTask.value = true

    // Calculate order for new task
    const todoTasks = tasksByStatus.value[TaskStatus.Todo]
    newTask.order = todoTasks.length > 0 ? Math.max(...todoTasks.map(t => t.order)) + 1 : 0

    await tasksStore.createTask(projectId, newTask)

    showCreateTaskModal.value = false
    Object.assign(newTask, {
      title: '',
      description: '',
      assigneeId: '',
      dueDate: '',
      status: TaskStatus.Todo,
      order: 0
    })
  } catch (error: any) {
    console.error('Failed to create task:', error)
    if (window.$toast) {
      window.$toast('error', 'Create Failed', error.response?.data?.message || 'Failed to create task')
    }
  } finally {
    creatingTask.value = false
  }
}

const editTask = (taskId: number) => {
  // TODO: Implement edit task modal
  console.log('Edit task:', taskId)
}

const deleteTask = async (taskId: number) => {
  if (confirm('Are you sure you want to delete this task?')) {
    try {
      await tasksStore.deleteTask(taskId)
    } catch (error: any) {
      console.error('Failed to delete task:', error)
      if (window.$toast) {
        window.$toast('error', 'Delete Failed', error.response?.data?.message || 'Failed to delete task')
      }
    }
  }
}

const inviteMember = async () => {
  try {
    inviting.value = true
    await projectsStore.inviteMember(projectId, inviteEmail.value)
    showInviteModal.value = false
    inviteEmail.value = ''
  } catch (error: any) {
    console.error('Failed to invite member:', error)
    if (window.$toast) {
      window.$toast('error', 'Invite Failed', error.response?.data?.message || 'Failed to invite member')
    }
  } finally {
    inviting.value = false
  }
}

onMounted(async () => {
  try {
    loading.value = true
    await projectsStore.getProject(projectId)
    await tasksStore.getTasksForProject(projectId)
  } catch (error) {
    console.error('Failed to load project:', error)
    if (window.$toast) {
      window.$toast('error', 'Load Failed', 'Failed to load project')
    }
  } finally {
    loading.value = false
  }
})
</script>
