<template>
  <div
    draggable="true"
    @dragstart="onDragStart"
    class="bg-white dark:bg-gray-800 rounded-lg p-4 shadow-sm hover:shadow-md transition-shadow cursor-move border border-gray-200 dark:border-gray-700"
  >
    <div class="space-y-3">
      <!-- Task Title -->
      <h3 class="font-medium text-gray-900 dark:text-white">{{ task.title }}</h3>

      <!-- Task Description -->
      <p v-if="task.description" class="text-sm text-gray-600 dark:text-gray-400 line-clamp-2">
        {{ task.description }}
      </p>

      <!-- Due Date -->
      <div v-if="task.dueDate" class="flex items-center text-sm">
        <svg class="w-4 h-4 text-gray-400 mr-1" fill="currentColor" viewBox="0 0 20 20">
          <path fill-rule="evenodd" d="M6 2a1 1 0 00-1 1v1H4a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V6a2 2 0 00-2-2h-1V3a1 1 0 10-2 0v1H7V3a1 1 0 00-1-1zm0 5a1 1 0 000 2h8a1 1 0 100-2H6z" clip-rule="evenodd" />
        </svg>
        <span :class="isOverdue ? 'text-red-600 dark:text-red-400' : 'text-gray-600 dark:text-gray-400'">
          {{ formatDate(task.dueDate) }}
        </span>
      </div>

      <!-- Assignee -->
      <div v-if="task.assignee" class="flex items-center text-sm">
        <div class="w-6 h-6 bg-primary-100 dark:bg-primary-900 rounded-full flex items-center justify-center mr-2">
          <span class="text-xs font-medium text-primary-700 dark:text-primary-300">
            {{ getInitials(task.assignee.fullName) }}
          </span>
        </div>
        <span class="text-gray-600 dark:text-gray-400">{{ task.assignee.fullName }}</span>
      </div>

      <!-- Actions -->
      <div class="flex justify-between items-center pt-2 border-t border-gray-100 dark:border-gray-700">
        <div class="flex space-x-2">
          <button
            @click="emit('edit', task.id)"
            class="text-gray-400 hover:text-gray-600 dark:text-gray-500 dark:hover:text-gray-300"
            title="Edit task"
          >
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
            </svg>
          </button>
          <button
            @click="emit('delete', task.id)"
            class="text-gray-400 hover:text-red-600 dark:text-gray-500 dark:hover:text-red-400"
            title="Delete task"
          >
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
            </svg>
          </button>
        </div>

        <!-- Status indicator -->
        <div class="flex items-center">
          <div
            :class="[
              'w-3 h-3 rounded-full',
              task.status === 'Todo' ? 'bg-gray-400' :
              task.status === 'InProgress' ? 'bg-blue-500' :
              'bg-green-500'
            ]"
          ></div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Task, User } from '@/types'
import { computed } from 'vue'

interface Props {
  task: Task
  projectMembers: User[]
}

interface Emits {
  (e: 'edit', taskId: number): void
  (e: 'delete', taskId: number): void
  (e: 'dragstart', event: DragEvent): void
}

const props = defineProps<Props>()
const emit = defineEmits<Emits>()

const isOverdue = computed(() => {
  if (!props.task.dueDate) return false
  return new Date(props.task.dueDate) < new Date()
})

const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString()
}

const getInitials = (name: string) => {
  return name
    .split(' ')
    .map(word => word.charAt(0))
    .join('')
    .toUpperCase()
    .substring(0, 2)
}

const onDragStart = (event: DragEvent) => {
  emit('dragstart', event)
}
</script>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
