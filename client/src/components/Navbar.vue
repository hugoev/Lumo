<template>
  <nav class="navbar" role="navigation" aria-label="Main navigation">
    <div class="container">
      <div class="navbar-content">
        <div class="navbar-brand">
          <router-link to="/dashboard" class="brand-link" aria-label="Lumo home">
            <h1 class="brand-title">Lumo</h1>
          </router-link>
        </div>

        <nav class="navbar-breadcrumbs" v-if="showBreadcrumbs" aria-label="Breadcrumb">
          <router-link to="/dashboard" class="breadcrumb-link" aria-label="Back to projects">Projects</router-link>
          <span v-if="currentProject" class="breadcrumb-separator" aria-hidden="true">/</span>
          <span v-if="currentProject" class="breadcrumb-current" aria-current="page">{{ currentProject.name }}</span>
        </nav>

        <div class="navbar-menu">
          <div class="navbar-user">
            <span class="user-name" aria-label="Current user">{{ authStore.user?.fullName }}</span>
            <button @click="handleLogout" class="btn btn-ghost btn-sm" aria-label="Logout">
              Logout
            </button>
          </div>
        </div>
      </div>
    </div>
  </nav>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/store/auth'
import { useProjectsStore } from '@/store/projects'
import { computed } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()
const authStore = useAuthStore()
const projectsStore = useProjectsStore()

const currentProject = computed(() => {
  const projectId = route.params.id
  if (projectId && projectsStore.currentProject) {
    return projectsStore.currentProject
  }
  return null
})

const showBreadcrumbs = computed(() => {
  return route.name === 'ProjectBoard' && currentProject.value
})

const handleLogout = () => {
  authStore.logout()
}
</script>

<style scoped>
.navbar {
  background-color: #ffffff;
  border-bottom: 1px solid #e2e8f0;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1), 0 1px 2px rgba(0, 0, 0, 0.05);
  position: sticky;
  top: 0;
  z-index: 40;
  backdrop-filter: blur(10px);
}

.navbar-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: 4rem;
  gap: 2rem;
  padding: 0 1rem;
}

.navbar-brand {
  display: flex;
  align-items: center;
  flex-shrink: 0;
}

.brand-link {
  text-decoration: none;
  color: inherit;
}

.brand-title {
  font-size: 1.5rem;
  font-weight: 700;
  color: #1a202c;
  margin: 0;
  transition: color 0.2s ease;
}

.brand-title:hover {
  color: #2d3748;
}

.navbar-breadcrumbs {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  flex: 1;
  justify-content: flex-start;
}

.breadcrumb-link {
  color: #3182ce;
  text-decoration: none;
  font-size: 0.875rem;
  font-weight: 500;
  padding: 0.25rem 0.5rem;
  border-radius: 0.25rem;
  transition: color 0.2s ease-in-out, background-color 0.2s ease-in-out;
}

.breadcrumb-link:hover {
  color: #2c5282;
  background-color: #f7fafc;
}

.breadcrumb-separator {
  color: #a0aec0;
  font-size: 0.875rem;
}

.breadcrumb-current {
  color: #4a5568;
  font-size: 0.875rem;
  font-weight: 500;
}

.navbar-menu {
  display: flex;
  align-items: center;
  flex-shrink: 0;
}

.navbar-user {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.user-name {
  font-size: 0.875rem;
  color: #2d3748;
  font-weight: 500;
}

@media (max-width: 768px) {
  .navbar-breadcrumbs {
    display: none;
  }

  .navbar-content {
    gap: 1rem;
  }
}

@media (max-width: 640px) {
  .navbar-content {
    height: 3.5rem;
  }

  .brand-title {
    font-size: 1.25rem;
  }

  .user-name {
    display: none;
  }
}
</style>