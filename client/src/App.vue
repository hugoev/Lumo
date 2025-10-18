<template>
  <div id="app" class="min-h-screen bg-gray-50 dark:bg-gray-900">
    <Navbar v-if="authStore.isAuthenticated" />
    <main class="container mx-auto px-4 py-8">
      <RouterView />
    </main>
    <ToastContainer />
  </div>
</template>

<script setup lang="ts">
import Navbar from '@/components/Navbar.vue'
import ToastContainer from '@/components/ToastContainer.vue'
import { useAuthStore } from '@/store/auth'
import { onMounted } from 'vue'
import { RouterView } from 'vue-router'

const authStore = useAuthStore()

onMounted(() => {
  authStore.initializeAuth()
})
</script>

<style>
@import '@/styles/index.css';
</style>
