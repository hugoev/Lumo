import { useAuthStore } from '@/store/auth'
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { pinia } from './store'

const app = createApp(App)

app.use(pinia)
app.use(router)

// Add navigation guards after plugins are installed
const authStore = useAuthStore()

router.beforeEach((to) => {
  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    return { name: 'Login' }
  }

  if (!to.meta.requiresAuth && authStore.isAuthenticated && (to.name === 'Login' || to.name === 'Register')) {
    return { name: 'Dashboard' }
  }
})

app.mount('#app')
