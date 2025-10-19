<template>
  <article class="task-card card" draggable="true" @dragstart="handleDragStart" role="article" :aria-label="`Task: ${task.title}`">
    <div class="card-body">
      <header class="task-header">
        <h3 class="task-title">{{ task.title }}</h3>
        <div class="task-actions" role="toolbar" aria-label="Task actions">
          <button @click="editTask" class="btn btn-ghost btn-sm" aria-label="Edit task" title="Edit task">
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true">
              <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/>
              <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/>
            </svg>
          </button>
          <button @click="deleteTask" class="btn btn-ghost btn-sm btn-danger" aria-label="Delete task" title="Delete task">
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" aria-hidden="true">
              <polyline points="3,6 5,6 21,6"/>
              <path d="M19,6v14a2,2 0 0,1-2,2H7a2,2 0 0,1-2-2V6m3,0V4a2,2 0 0,1,2-2h4a2,2 0 0,1,2,2v2"/>
            </svg>
          </button>
        </div>
      </header>

      <p class="task-description">{{ task.description }}</p>

      <div class="task-meta">
        <div class="task-status">
          <span :class="`status-badge ${getStatusClass(task.status)}`">
            {{ getStatusText(task.status) }}
          </span>
        </div>

        <div class="task-assignee" v-if="task.assignee">
          <span class="assignee-label">Assigned to:</span>
          <span class="assignee-name">{{ task.assignee.fullName }}</span>
        </div>

        <div class="task-due-date" v-if="task.dueDate">
          <span class="due-label">Due:</span>
          <span class="due-date">{{ formatDate(task.dueDate) }}</span>
        </div>
      </div>
    </div>
  </article>
</template>

<script setup lang="ts">
import type { Task } from '@/types';

interface Props {
  task: Task
}

const props = defineProps<Props>()

const emit = defineEmits<{
  edit: [task: Task]
  delete: [task: Task]
  dragstart: [event: DragEvent, task: Task]
}>()

const editTask = () => {
  emit('edit', props.task)
}

const deleteTask = () => {
  emit('delete', props.task)
}

const handleDragStart = (event: DragEvent) => {
  if (event.dataTransfer) {
    event.dataTransfer.effectAllowed = 'move'
    event.dataTransfer.setData('text/plain', props.task.id.toString())
  }
  emit('dragstart', event, props.task)
}

const formatDate = (dateString: string) => {
  const date = new Date(dateString)
  return date.toLocaleDateString('en-US', {
    month: 'short',
    day: 'numeric',
    year: 'numeric'
  })
}

const getStatusClass = (status: string | number) => {
  // Convert status to string for CSS class
  const statusStr = typeof status === 'number'
    ? (status === 0 ? 'Todo' : status === 1 ? 'InProgress' : status === 2 ? 'Done' : 'Todo')
    : status
  return `status-${statusStr.toLowerCase()}`
}

const getStatusText = (status: string | number) => {
  // Convert status to display text
  return typeof status === 'number'
    ? (status === 0 ? 'Todo' : status === 1 ? 'InProgress' : status === 2 ? 'Done' : 'Todo')
    : status
}
</script>

<style scoped>
.task-card {
  margin-bottom: 1rem;
  background-color: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 0.5rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  transition: box-shadow 0.2s ease, transform 0.2s ease;
}

.task-card:hover {
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.task-card[draggable="true"]:hover {
  cursor: grab;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.15);
}

.task-card[draggable="true"]:active {
  cursor: grabbing;
}

.task-card.dragging {
  opacity: 0.8;
  transform: rotate(2deg);
  z-index: 1000;
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
}

.task-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  margin-bottom: 0.75rem;
}

.task-title {
  font-size: 1rem;
  font-weight: 600;
  color: #2d3748;
  margin: 0;
  flex: 1;
  margin-right: 0.75rem;
  line-height: 1.3;
}

.task-actions {
  display: flex;
  gap: 0.25rem;
  opacity: 1;
}

.task-description {
  color: #4a5568;
  font-size: 0.875rem;
  line-height: 1.4;
  margin-bottom: 1rem;
}

.task-meta {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.task-status {
  align-self: flex-start;
}

.task-assignee,
.task-due-date {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  font-size: 0.75rem;
}

.assignee-label,
.due-label {
  color: #718096;
  font-weight: 500;
}

.assignee-name {
  color: #4a5568;
  font-weight: 500;
}

.due-date {
  color: #4a5568;
  font-weight: 500;
}

@media (max-width: 640px) {
  .task-header {
    flex-direction: column;
    gap: 0.5rem;
  }

  .task-title {
    margin-right: 0;
  }

  .task-actions {
    align-self: flex-end;
  }
}
</style>