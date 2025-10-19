<template>
  <teleport to="body">
    <transition name="modal" appear>
      <div v-if="isOpen" class="modal-overlay" @click="handleBackdropClick">
        <div class="modal" :class="{ 'modal-large': large }" @click.stop>
          <div class="modal-header" v-if="$slots.header || title">
            <h2 v-if="title" class="modal-title">{{ title }}</h2>
            <button v-if="closable" @click="close" class="modal-close-btn" aria-label="Close modal">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <line x1="18" y1="6" x2="6" y2="18"/>
                <line x1="6" y1="6" x2="18" y2="18"/>
              </svg>
            </button>
            <div v-if="$slots.header" class="modal-header-content">
              <slot name="header" />
            </div>
          </div>

          <div class="modal-body">
            <slot />
          </div>

          <div class="modal-footer" v-if="$slots.footer">
            <slot name="footer" />
          </div>
        </div>
      </div>
    </transition>
  </teleport>
</template>

<script setup lang="ts">
import { watch } from 'vue'

interface Props {
  isOpen: boolean
  title?: string
  closable?: boolean
  large?: boolean
  closeOnBackdrop?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  closable: true,
  closeOnBackdrop: true
})

const emit = defineEmits<{
  close: []
  'update:isOpen': [value: boolean]
}>()

const close = () => {
  emit('update:isOpen', false)
  emit('close')
}

const handleBackdropClick = () => {
  if (props.closeOnBackdrop) {
    close()
  }
}

const handleEscapeKey = (event: KeyboardEvent) => {
  if (event.key === 'Escape' && props.closable) {
    close()
  }
}

// Add/remove escape key listener
watch(
  () => props.isOpen,
  (newValue) => {
    if (newValue) {
      document.addEventListener('keydown', handleEscapeKey)
      document.body.style.overflow = 'hidden'
    } else {
      document.removeEventListener('keydown', handleEscapeKey)
      document.body.style.overflow = ''
    }
  }
)
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 50;
  padding: 1rem;
  backdrop-filter: blur(4px);
}

.modal {
  background-color: white;
  border-radius: 0.75rem;
  box-shadow: 0 20px 25px rgba(0, 0, 0, 0.1), 0 10px 10px rgba(0, 0, 0, 0.04);
  max-width: 32rem;
  width: 100%;
  max-height: 90vh;
  overflow-y: auto;
  transform: scale(0.95);
  transition: transform 0.2s ease-out;
}

.modal-large {
  max-width: 48rem;
}

.modal-overlay .modal {
  transform: scale(1);
}

.modal-header {
  padding: 1.5rem;
  border-bottom: 1px solid #e2e8f0;
  position: relative;
}

.modal-title {
  font-size: 1.5rem;
  font-weight: 600;
  color: #2d3748;
  margin: 0;
}

.modal-close-btn {
  position: absolute;
  top: 1rem;
  right: 1rem;
  background: none;
  border: none;
  color: #718096;
  cursor: pointer;
  padding: 0.5rem;
  border-radius: 0.25rem;
  transition: color 0.2s ease-in-out, background-color 0.2s ease-in-out;
  display: flex;
  align-items: center;
  justify-content: center;
}

.modal-close-btn:hover {
  color: #4a5568;
  background-color: #fafbfc;
}

.modal-body {
  padding: 1.5rem;
}

.modal-footer {
  padding: 1rem 1.5rem;
  border-top: 1px solid #e2e8f0;
  display: flex;
  justify-content: flex-end;
  gap: 0.75rem;
}

/* Modal animations */
.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.2s ease-out;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-enter-active .modal,
.modal-leave-active .modal {
  transition: transform 0.2s ease-out;
}

.modal-enter-from .modal,
.modal-leave-to .modal {
  transform: scale(0.95);
}

/* Focus trap for accessibility */
.modal-overlay:focus {
  outline: none;
}

@media (max-width: 640px) {
  .modal-overlay {
    padding: 0.5rem;
  }

  .modal {
    max-width: calc(100vw - 1rem);
    max-height: calc(100vh - 1rem);
  }

  .modal-large {
    max-width: calc(100vw - 1rem);
  }
}
</style>
