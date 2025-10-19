<template>
  <div class="auth-page">
    <div class="auth-container">
      <div class="auth-card card fade-in">
        <div class="card-body">
          <div class="auth-header">
            <h1 class="auth-title">Create account</h1>
            <p class="auth-subtitle">Get started with Lumo today</p>
          </div>

          <form @submit.prevent="handleRegister" class="auth-form">
            <div class="form-group">
              <label for="fullName" class="form-label">Full Name</label>
              <input
                id="fullName"
                v-model="form.fullName"
                type="text"
                class="form-input"
                placeholder="Enter your full name"
                required
                autocomplete="name"
              />
            </div>

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
                placeholder="Create a password"
                required
                autocomplete="new-password"
                minlength="8"
              />
            </div>

            <div class="form-group">
              <label for="confirmPassword" class="form-label">Confirm Password</label>
              <input
                id="confirmPassword"
                v-model="form.confirmPassword"
                type="password"
                class="form-input"
                placeholder="Confirm your password"
                required
                autocomplete="new-password"
              />
            </div>

            <button type="submit" class="btn btn-primary btn-lg auth-btn" :disabled="loading">
              <span v-if="loading" class="loading-spinner"></span>
              <span v-else>Create Account</span>
            </button>
          </form>

          <div class="auth-footer">
            <p class="auth-footer-text">
              Already have an account?
              <router-link to="/login" class="auth-link">Sign in</router-link>
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
const { error: showError, success: showSuccess } = useToast()

const loading = ref(false)

const form = reactive({
  fullName: '',
  email: '',
  password: '',
  confirmPassword: ''
})

const handleRegister = async () => {
  if (!form.fullName || !form.email || !form.password || !form.confirmPassword) {
    showError('Validation Error', 'Please fill in all fields')
    return
  }

  if (form.password !== form.confirmPassword) {
    showError('Validation Error', 'Passwords do not match')
    return
  }

  if (form.password.length < 8) {
    showError('Validation Error', 'Password must be at least 8 characters long')
    return
  }

  loading.value = true

  try {
    await authStore.register({
      fullName: form.fullName,
      email: form.email,
      password: form.password
    })

    showSuccess('Account Created', 'Welcome to Lumo! Your account has been created successfully.')
    router.push('/dashboard')
  } catch (error: any) {
    const message = error.response?.data?.message || 'Registration failed. Please try again.'
    showError('Registration Failed', message)
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.auth-page {
  min-height: 100vh;
  width: 100vw;
  position: fixed;
  top: 0;
  left: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #ffffff;
  padding: 1rem;
  z-index: 999;
}

.auth-container {
  width: 100%;
  max-width: 28rem;
}

.auth-card {
  background-color: #fafbfc;
  box-shadow: 0 20px 25px rgba(0, 0, 0, 0.15), 0 10px 10px rgba(0, 0, 0, 0.05);
  border-radius: 1rem;
  border: 1px solid #e2e8f0;
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

/* Form validation styles */
.form-input:invalid {
  border-color: #f56565;
}

.form-input:invalid:focus {
  border-color: #f56565;
  box-shadow: 0 0 0 3px rgba(245, 101, 101, 0.1);
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

  .auth-form {
    gap: 1.25rem;
  }
}
</style>
