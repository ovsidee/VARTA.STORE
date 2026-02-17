# 🛒 VARTA.STORE

> **A modern, high-performance game store platform for DayZ servers.**  
> *Built with .NET 9, Blazor WebAssembly, and Docker.*

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)
![Status](https://img.shields.io/badge/status-production-green.svg)
![Users](https://img.shields.io/badge/users-100%2B-success.svg)

---

## 🌟 Overview

**VARTA.STORE** is a fully automated e-commerce solution designed specifically for game server monetization. It provides a seamless experience for players to purchase in-game items, currency, or VIP status, with instant delivery to the game server.

Currently deployed in production and serving **100+ active users**.

## ✨ Key Features

- **🛍️ Modern Storefront:** Responsive, fast, and beautiful UI built with **Blazor WebAssembly** and **MudBlazor**.
- **🔑 Seamless Authentication:** Secure login via **Steam OpenID** (no registration required).
- **💳 Real-time Payments:** Integrated **Donatik** payment gateway for instant transaction processing.
- **🎮 Game Server Integration:** Custom API that communicates directly with DayZ servers to spawn items or update player stats in real-time.
- **⚡ High Performance:** Backend powered by **ASP.NET Core (.NET 9)** and **PostgreSQL** for maximum speed and reliability.
- **🔒 Secure & Production Ready:** Dockerized architecture with automatic HTTPS (Caddy) and secure JWT authentication.

## 🛠️ Technology Stack

### Frontend
- **Framework:** Blazor WebAssembly (.NET 9)
- **UI Library:** MudBlazor
- **State Management:** Blazored.LocalStorage

### Backend
- **API:** ASP.NET Core Web API (.NET 9)
- **Database:** PostgreSQL 15 (Entity Framework Core)
- **Auth:** JWT (JSON Web Tokens) + Steam OpenID

### DevOps & Deployment
- **Containerization:** Docker & Docker Compose
- **Reverse Proxy:** Caddy (Automatic SSL/HTTPS)
- **Hosting:** VPS (Ubuntu 24.04)

---

## 🚀 Getting Started

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (Optional, for local dev)

### 1. Clone the Repository
```bash
git clone https://github.com/ovsidee/VARTA.STORE.git
cd VARTA.STORE
```

### 2. Configure Environment
Create a `.env` file in the root directory:
```ini
# Database
DB_HOST=db
DB_NAME=VartaStoreDb
DB_USER=varta
DB_PASSWORD=your_secure_password

# Authentication
STEAM_API_KEY=your_steam_api_key
JWT_KEY=your_super_secret_jwt_key

# Payments
DONATIK_TOKEN=your_donatik_token

# App Config
PUBLIC_URL=http://localhost
```

### 3. Run with Docker
```bash
docker compose up -d --build
```
The application will be available at:
- **Client:** `http://localhost`
- **API:** `http://localhost/api`

---

## 📦 Deployment

This project includes a production-ready `docker-compose.yml` with **Caddy** for automatic HTTPS.

1.  **Change `PUBLIC_URL`** in `.env` to your domain (e.g., `https://varta-shop.com`).
2.  **Update `docker-compose.yml`** (ensure ports 80/443 are exposed).
3.  **Run:**
    ```bash
    docker compose up -d
    ```

## 🔌 API Documentation

The backend exposes a RESTful API for game servers and the frontend client.

- `POST /api/auth/login` - Steam OpenID Login
- `GET /api/products` - List available products
- `POST /api/orders` - Create a new order
- `POST /api/donations/webhook` - Handle payment notifications

---

## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

Made with ❤️ by the VARTA Development Team.
