# Lumo Project: Gemini Guidelines

This document provides a comprehensive guide for Gemini to understand and interact with the Lumo project.

## Project Overview

Lumo is a modern, full-stack project management web application. It features a .NET Core backend, a Vue.js 3 frontend, and a PostgreSQL database. The entire application is containerized using Docker for easy setup and deployment.

### Key Technologies

*   **Backend**: ASP.NET Core 8 Web API with Entity Framework Core
*   **Frontend**: Vue 3 + TypeScript + Vite + Pinia + Vue Router
*   **UI Framework**: Tailwind CSS with dark mode support
*   **Database**: PostgreSQL
*   **Authentication**: JWT (JSON Web Tokens)
*   **Deployment**: Docker containerized

## Building and Running the Project

The recommended way to run the project is by using Docker Compose.

### Using Docker Compose

1.  **Build and start the containers:**
    ```bash
    docker-compose up --build
    ```
2.  **Access the application:**
    *   **Frontend:** `http://localhost:5173`
    *   **Backend API:** `http://localhost:5000`
    *   **API Documentation:** `http://localhost:5000/swagger`

### Manual Setup

#### Backend

1.  **Navigate to the server directory:**
    ```bash
    cd server
    ```
2.  **Install dependencies:**
    ```bash
    dotnet restore
    ```
3.  **Update the database:**
    ```bash
    dotnet ef database update
    ```
4.  **Run the backend:**
    ```bash
    dotnet run
    ```

#### Frontend

1.  **Navigate to the client directory:**
    ```bash
    cd client
    ```
2.  **Install dependencies:**
    ```bash
    npm install
    ```
3.  **Start the development server:**
    ```bash
    npm run dev
    ```

## Development Conventions

### Backend

*   The backend is an ASP.NET Core Web API project.
*   Controllers are located in the `Controllers` directory.
*   Database models are in the `Models` directory.
*   Data Transfer Objects (DTOs) are in the `DTOs` directory.
*   Business logic is handled by services in the `Services` directory.
*   The database context is in the `Data` directory.

### Frontend

*   The frontend is a Vue.js 3 project using TypeScript and Vite.
*   Reusable components are in `src/components`.
*   Page components are in `src/pages`.
*   State management is handled by Pinia, with stores in `src/store`.
*   API client functions are in `src/api`.
*   TypeScript type definitions are in `src/types`.

### Testing

*   **Backend tests:**
    ```bash
    cd server
    dotnet test
    ```
*   **Frontend tests:**
    ```bash
    cd client
    npm run test
    ```
