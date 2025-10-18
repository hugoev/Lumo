import { tasksApi } from '@/api/tasks'
import type { CreateTaskRequest, Task, TaskStatus, UpdateTaskRequest } from '@/types'
import { defineStore } from 'pinia'
import { computed, ref } from 'vue'

export const useTasksStore = defineStore('tasks', () => {
  const tasks = ref<Task[]>([])
  const loading = ref(false)

  const tasksByStatus = computed(() => {
    const grouped: Record<TaskStatus, Task[]> = {
      [TaskStatus.Todo]: [],
      [TaskStatus.InProgress]: [],
      [TaskStatus.Done]: []
    }

    tasks.value.forEach(task => {
      grouped[task.status].push(task)
    })

    // Sort each column by order
    Object.keys(grouped).forEach(status => {
      grouped[status as TaskStatus].sort((a, b) => a.order - b.order)
    })

    return grouped
  })

  const getTasksForProject = async (projectId: number) => {
    try {
      loading.value = true
      tasks.value = await tasksApi.getTasksForProject(projectId)
    } catch (error) {
      throw error
    } finally {
      loading.value = false
    }
  }

  const createTask = async (projectId: number, taskData: CreateTaskRequest) => {
    try {
      const newTask = await tasksApi.createTask(projectId, taskData)
      tasks.value.push(newTask)
      return newTask
    } catch (error) {
      throw error
    }
  }

  const updateTask = async (taskId: number, taskData: UpdateTaskRequest) => {
    try {
      const updatedTask = await tasksApi.updateTask(taskId, taskData)

      // Update in tasks array
      const index = tasks.value.findIndex(t => t.id === taskId)
      if (index !== -1) {
        tasks.value[index] = updatedTask
      }

      return updatedTask
    } catch (error) {
      throw error
    }
  }

  const deleteTask = async (taskId: number) => {
    try {
      await tasksApi.deleteTask(taskId)
      tasks.value = tasks.value.filter(t => t.id !== taskId)
    } catch (error) {
      throw error
    }
  }

  const moveTask = async (taskId: number, newStatus: TaskStatus, newOrder: number) => {
    try {
      // Optimistically update the UI
      const task = tasks.value.find(t => t.id === taskId)
      if (task) {
        task.status = newStatus
        task.order = newOrder
      }

      // Update on server
      const updates = [{
        taskId,
        status: newStatus,
        order: newOrder
      }]

      await tasksApi.updateTaskStatuses(updates)
    } catch (error) {
      // Revert optimistic update on error
      await getTasksForProject(task?.projectId || 0)
      throw error
    }
  }

  const reorderTasks = async (projectId: number, updates: Array<{ taskId: number; status: TaskStatus; order: number }>) => {
    try {
      // Optimistically update the UI
      updates.forEach(update => {
        const task = tasks.value.find(t => t.id === update.taskId)
        if (task) {
          task.status = update.status
          task.order = update.order
        }
      })

      // Update on server
      await tasksApi.updateTaskStatuses(updates)
    } catch (error) {
      // Revert optimistic update on error
      await getTasksForProject(projectId)
      throw error
    }
  }

  return {
    tasks,
    loading,
    tasksByStatus,
    getTasksForProject,
    createTask,
    updateTask,
    deleteTask,
    moveTask,
    reorderTasks
  }
})
