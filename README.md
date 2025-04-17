# 🚗 Car service 🚗

## 🧑‍🔧 Authors
- **Michalczak Antoni**
- **Nabagło Dawid**

## 📜 Description
Web application designed specifically for car service management.

### ⭐ Features
- **Customer and vehicle management**
- **Service orders** with tasks and parts
- **Task assignment** to mechanics
- **Order commenting**
- **PDF report generation**
- **Vehicle photo uploads**
- **Repair filtering and reporting**

### 📝 Application Architecture Diagram
![appication diagram](./architecture_diagram.png)

### 🧩 System Components

| Component         | Technology         | Functionality |
|------------------|--------------------|---------------|
| **Frontend**      | Next.js (React, TypeScript) | - Multi-page web application<br>- Built with React and TypeScript |
| **Gateway**       | C#                 | - Entry point for the application<br>- Defines and handles all routes |
| **Auth Service**  | C#                 | - Manages user credentials<br>- Token generation and validation<br>- Handles authentication |
| **Workshop Service** | C#              | - Manages workers<br>- Provides all core workshop-related features |
| **Auth Database** | MSSQL              | - Stores user data<br>- Manages roles and permissions |
| **Main Database** | MSSQL              | - Stores workshop-related data<br>- Includes vehicles, repairs, parts, etc. |
| **Message Broker** | RabbitMQ          | - Queueing and processing report generation requests<br>- Receiving and distributing report status updates<br>- Triggering email notifications once reports are generated<br>- Ensures reliability and scalability by decoupling processes |
| **Report Service**| C#                 | - Manages and generates reports |
| **Email Service** | C#                 | - Sends emails (e.g., notifications after report generation) |
