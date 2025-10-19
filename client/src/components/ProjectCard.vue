<template>
  <div class="project-card card">
    <div class="card-body">
      <div class="project-header">
        <h3 class="project-title">{{ project.name }}</h3>
        <div class="project-actions">
          <button @click="editProject" class="btn btn-ghost btn-sm">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"/>
              <path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"/>
            </svg>
          </button>
          <button @click="deleteProject" class="btn btn-ghost btn-sm btn-danger">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <polyline points="3,6 5,6 21,6"/>
              <path d="M19,6v14a2,2 0 0,1-2,2H7a2,2 0 0,1-2-2V6m3,0V4a2,2 0 0,1,2-2h4a2,2 0 0,1,2,2v2"/>
            </svg>
          </button>
        </div>
      </div>

      <p class="project-description">{{ project.description }}</p>

      <div class="project-meta">
        <div class="project-members">
          <span class="members-label">Members:</span>
          <div class="members-list">
            <span v-for="member in project.members.slice(0, 3)" :key="member.id" class="member-avatar">
              {{ member.fullName.charAt(0).toUpperCase() }}
            </span>
            <span v-if="project.members.length > 3" class="member-more">
              +{{ project.members.length - 3 }}
            </span>
          </div>
        </div>

        <div class="project-stats">
          <span class="tasks-count">{{ project.taskCount }} tasks</span>
        </div>
      </div>
    </div>

    <div class="card-footer">
      <button @click="openProject" class="btn btn-primary btn-sm">
        Open Project
      </button>
      <button @click="inviteMember" class="btn btn-secondary btn-sm">
        Invite Member
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { Project } from '@/types';

interface Props {
  project: Project
}

const props = defineProps<Props>()

const emit = defineEmits<{
  edit: [project: Project]
  delete: [project: Project]
  open: [project: Project]
  invite: [project: Project]
}>()

const editProject = () => {
  emit('edit', props.project)
}

const deleteProject = () => {
  emit('delete', props.project)
}

const openProject = () => {
  emit('open', props.project)
}

const inviteMember = () => {
  emit('invite', props.project)
}
</script>

<style scoped>
.project-card {
  cursor: pointer;
  background-color: #ffffff;
  border: 1px solid #e2e8f0;
  border-radius: 0.75rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  transition: box-shadow 0.2s ease, transform 0.2s ease;
}

.project-card:hover {
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  transform: translateY(-2px);
}

.project-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  margin-bottom: 1rem;
}

.project-title {
  font-size: 1.25rem;
  font-weight: 600;
  color: #2d3748;
  margin: 0;
  flex: 1;
  margin-right: 1rem;
}

.project-actions {
  display: flex;
  gap: 0.5rem;
  opacity: 1;
}

.project-description {
  color: #4a5568;
  margin-bottom: 1.5rem;
  line-height: 1.5;
}

.project-meta {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin-bottom: 1rem;
}

.project-members {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.members-label {
  font-size: 0.875rem;
  color: #4a5568;
  font-weight: 500;
}

.members-list {
  display: flex;
  align-items: center;
  gap: 0.25rem;
}

.member-avatar {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 2rem;
  height: 2rem;
  background-color: #4299e1;
  color: white;
  border-radius: 50%;
  font-size: 0.75rem;
  font-weight: 500;
}

.member-more {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 2rem;
  height: 2rem;
  background-color: #e2e8f0;
  color: #4a5568;
  border-radius: 50%;
  font-size: 0.75rem;
  font-weight: 500;
}

.project-stats {
  display: flex;
  align-items: center;
}

.tasks-count {
  font-size: 0.875rem;
  color: #4a5568;
  font-weight: 500;
}

@media (max-width: 640px) {
  .project-header {
    flex-direction: column;
    gap: 0.75rem;
  }

  .project-title {
    margin-right: 0;
  }

  .project-actions {
    align-self: flex-end;
  }

  .project-meta {
    flex-direction: column;
    align-items: flex-start;
    gap: 0.75rem;
  }
}
</style>
