import { tasksApi } from '@/api/tasks'
import { CreateTaskRequest, Task, TaskStatus, UpdateTaskRequest } from '@/types'
import { defineStore } from 'pinia'
import { computed, ref } from 'vue'

export const useTasksStore = defineStore('tasks', () => {
  const tasks = ref<Task[]>([])
  const loading = ref(false)

  const tasksByStatus = computed(() => {
    const grouped: Record<string, Task[]> = {
      'Todo': [],
      'InProgress': [],
      'Done': []
    }

    // Safety check - ensure tasks exist before processing
    if (!tasks.value || !Array.isArray(tasks.value)) {
      return grouped
    }

    tasks.value.forEach(task => {
      // Safety check - ensure task has required properties
      if (!task || (typeof task.status !== 'number' && typeof task.status !== 'string')) {
        console.warn('Invalid task object:', task)
        return
      }

      // Convert backend status (integer or string) to frontend string
      let status: string = 'Todo' // Default fallback
      if (typeof task.status === 'string' && task.status) {
        // Already a string enum value
        status = task.status
      } else if (typeof task.status === 'number') {
        // Convert backend status (integer) to frontend string
        switch (task.status) {
          case 0:
            status = 'Todo'
            break
          case 1:
            status = 'InProgress'
            break
          case 2:
            status = 'Done'
            break
          default:
            console.warn(`Unknown task status: ${task.status}`, task)
            status = 'Todo' // Default fallback
        }
      } else {
        console.warn(`Invalid task status type: ${typeof task.status}, value: ${task.status}`, task)
        status = 'Todo' // Default fallback
      }

      // Ensure status is valid and grouped[status] exists
      if (status && typeof status === 'string' && status in grouped && Array.isArray(grouped[status])) {
        grouped[status].push(task)
      } else {
        console.warn(`Cannot group task - invalid status: ${status} for task:`, task)
      }
    })

    // Sort each column by order
    Object.keys(grouped).forEach(status => {
      if (grouped[status] && Array.isArray(grouped[status])) {
        grouped[status].sort((a, b) => (a?.order || 0) - (b?.order || 0))
      }
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
      // Get the task being moved
      const task = tasks.value.find(t => t.id === taskId)
      if (!task) return

      // Convert statuses to comparable format (string enum)
      const oldStatusStr = typeof task.status === 'number'
        ? (task.status === 0 ? 'Todo' : task.status === 1 ? 'InProgress' : task.status === 2 ? 'Done' : 'Todo')
        : task.status
      const newStatusStr = newStatus

      // Optimistically update the UI
      task.status = newStatus
      task.order = newOrder

      // If moving to a different column, reorder other tasks in the target column
      if (oldStatusStr !== newStatusStr) {
        // Get tasks in the target column and reorder them
        const tasksInTargetColumn = tasks.value
          .filter(t => {
            const tStatusStr = typeof t.status === 'number'
              ? (t.status === 0 ? 'Todo' : t.status === 1 ? 'InProgress' : t.status === 2 ? 'Done' : 'Todo')
              : t.status
            return tStatusStr === newStatusStr
          })
          .sort((a, b) => (a.order || 0) - (b.order || 0))

        // Update orders for all tasks in the target column
        tasksInTargetColumn.forEach((t, index) => {
          t.order = index + 1
        })
      }

      // Convert TaskStatus enum to backend integer format
      const statusValue = newStatus === TaskStatus.Todo ? 0 :
                         newStatus === TaskStatus.InProgress ? 1 :
                         newStatus === TaskStatus.Done ? 2 : 0

      // Prepare updates for all affected tasks
      const updates = []

      // Update the moved task
      updates.push({
        taskId,
        status: statusValue, // Send as number, not string
        order: newOrder
      })

      // Update other tasks in the target column if status changed
      if (oldStatusStr !== newStatusStr) {
        const tasksInTargetColumn = tasks.value
          .filter(t => {
            const tStatusStr = typeof t.status === 'number'
              ? (t.status === 0 ? 'Todo' : t.status === 1 ? 'InProgress' : t.status === 2 ? 'Done' : 'Todo')
              : t.status
            return tStatusStr === newStatusStr && t.id !== taskId
          })
          .sort((a, b) => (a.order || 0) - (b.order || 0))

        tasksInTargetColumn.forEach((t, index) => {
          const newOrderForTask = index + 1
          if (t.order !== newOrderForTask) {
            updates.push({
              taskId: t.id,
              status: statusValue, // Send as number, not string
              order: newOrderForTask
            })
          }
        })
      }

      await tasksApi.updateTaskStatuses(updates)
    } catch (error) {
      // Revert optimistic update on error - need to get current project ID from tasks
      const currentTask = tasks.value.find(t => t.id === taskId)
      if (currentTask?.projectId) {
        await getTasksForProject(currentTask.projectId)
      }
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

      // Convert TaskStatus enums to backend integer format
      const convertedUpdates = updates.map(update => ({
        taskId: update.taskId,
        status: update.status === TaskStatus.Todo ? 0 :
               update.status === TaskStatus.InProgress ? 1 :
               update.status === TaskStatus.Done ? 2 : 0,
        order: update.order
      }))

      // Update on server
      await tasksApi.updateTaskStatuses(convertedUpdates)
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
