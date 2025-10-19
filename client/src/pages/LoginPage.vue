<template>
  <div class="auth-page">
    <div class="auth-container">
      <div class="auth-card card fade-in">
        <div class="card-body">
          <div class="auth-header">
            <h1 class="auth-title">Welcome back</h1>
            <p class="auth-subtitle">Sign in to your Lumo account</p>
          </div>

          <form @submit.prevent="handleLogin" class="auth-form">
            <div class="form-group">
              <label for="email" class="form-label">Email</label>
              <input
                id="email"
                v-model="form.email"
                type="email"
                class="form-input"
                placeholder="Enter your email"
                required
                autocomplete="email"
              />
            </div>

            <div class="form-group">
              <label for="password" class="form-label">Password</label>
              <input
                id="password"
                v-model="form.password"
                type="password"
                class="form-input"
                placeholder="Enter your password"
                required
                autocomplete="current-password"
              />
            </div>

            <button type="submit" class="btn btn-primary btn-lg auth-btn" :disabled="loading">
              <span v-if="loading" class="loading-spinner"></span>
              <span v-else>Sign In</span>
            </button>
          </form>

          <div class="auth-footer">
            <p class="auth-footer-text">
              Don't have an account?
              <router-link to="/register" class="auth-link">Sign up</router-link>
            </p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useToast } from '@/composables/useToast'
import { useAuthStore } from '@/store/auth'
import { reactive, ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const authStore = useAuthStore()
const { error: showError } = useToast()

const loading = ref(false)

const form = reactive({
  email: '',
  password: ''
})

const handleLogin = async () => {
  if (!form.email || !form.password) {
    showError('Validation Error', 'Please fill in all fields')
    return
  }

  loading.value = true

  try {
    await authStore.login({
      email: form.email,
      password: form.password
    })

    router.push('/dashboard')
  } catch (error: any) {
    const message = error.response?.data?.message || 'Login failed. Please try again.'
    showError('Login Failed', message)
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.auth-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 1rem;
}

.auth-container {
  width: 100%;
  max-width: 24rem;
}

.auth-card {
  background-color: white;
  box-shadow: 0 20px 25px rgba(0, 0, 0, 0.15), 0 10px 10px rgba(0, 0, 0, 0.05);
  border-radius: 1rem;
}

.auth-header {
  text-align: center;
  margin-bottom: 2rem;
}

.auth-title {
  font-size: 2rem;
  font-weight: 700;
  color: #2d3748;
  margin: 0 0 0.5rem 0;
}

.auth-subtitle {
  color: #4a5568;
  margin: 0;
  font-size: 0.875rem;
}

.auth-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.auth-btn {
  width: 100%;
  margin-top: 0.5rem;
}

.auth-footer {
  text-align: center;
  margin-top: 2rem;
  padding-top: 2rem;
  border-top: 1px solid #e2e8f0;
}

.auth-footer-text {
  color: #4a5568;
  font-size: 0.875rem;
  margin: 0;
}

.auth-link {
  color: #4299e1;
  text-decoration: none;
  font-weight: 500;
  transition: color 0.2s ease-in-out;
}

.auth-link:hover {
  color: #3182ce;
  text-decoration: underline;
}

/* Loading state */
.auth-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
  transform: none;
}

.auth-btn:disabled:hover {
  transform: none;
  box-shadow: none;
}

@media (max-width: 640px) {
  .auth-page {
    padding: 0.5rem;
  }

  .auth-container {
    max-width: none;
  }

  .auth-title {
    font-size: 1.75rem;
  }
}
</style>
