import { ref } from 'vue'

// Toast interface definition (moved from ToastContainer.vue to avoid circular imports)
export interface Toast {
  id: string
  type: 'success' | 'error' | 'warning' | 'info'
  title: string
  message: string
  duration?: number
}

let toastCounter = 0

export interface ToastOptions {
  type: 'success' | 'error' | 'warning' | 'info'
  title: string
  message: string
  duration?: number
}

class ToastManager {
  private toasts = ref<Toast[]>([])
  private listeners: ((toasts: Toast[]) => void)[] = []

  addToast(options: ToastOptions) {
    const id = `toast-${++toastCounter}`
    const toast: Toast = {
      id,
      duration: options.duration || 5000,
      ...options
    }

    this.toasts.value.push(toast)

    if (toast.duration && toast.duration > 0) {
      setTimeout(() => {
        this.removeToast(id)
      }, toast.duration)
    }

    this.notifyListeners()
    return id
  }

  removeToast(id: string) {
    const index = this.toasts.value.findIndex(toast => toast.id === id)
    if (index > -1) {
      this.toasts.value.splice(index, 1)
      this.notifyListeners()
    }
  }

  getToasts() {
    return this.toasts.value
  }

  subscribe(listener: (toasts: Toast[]) => void) {
    this.listeners.push(listener)
    return () => {
      const index = this.listeners.indexOf(listener)
      if (index > -1) {
        this.listeners.splice(index, 1)
      }
    }
  }

  private notifyListeners() {
    this.listeners.forEach(listener => listener(this.toasts.value))
  }
}

// Create a singleton instance
const toastManager = new ToastManager()

export const useToast = () => {
  const addToast = (options: ToastOptions) => {
    return toastManager.addToast(options)
  }

  const removeToast = (id: string) => {
    toastManager.removeToast(id)
  }

  const success = (title: string, message?: string) => {
    return addToast({ type: 'success', title, message: message || '' })
  }

  const error = (title: string, message?: string) => {
    return addToast({ type: 'error', title, message: message || '' })
  }

  const warning = (title: string, message?: string) => {
    return addToast({ type: 'warning', title, message: message || '' })
  }

  const info = (title: string, message?: string) => {
    return addToast({ type: 'info', title, message: message || '' })
  }

  return {
    addToast,
    removeToast,
    success,
    error,
    warning,
    info
  }
}

// Export the toast manager for the ToastContainer component to use
export { toastManager }
