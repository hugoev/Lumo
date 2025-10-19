<template>
  <div id="app" class="min-h-screen">
    <a href="#main-content" class="skip-link">Skip to main content</a>
    <Navbar v-if="authStore.isAuthenticated" />
    <main id="main-content" class="container" role="main">
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
