# Lexicon

A Blog/CMS application built with .NET 8 (Clean Architecture) and Next.js.

## Features

- **Posts** - Create, update, publish, and manage blog posts
- **Categories** - Hierarchical category system
- **Tags** - Flexible tagging for posts
- **Authors** - Author profiles and management
- **Comments** - Comment system with moderation
- **Media** - File upload and management

## Architecture

### Backend (.NET 8 - Clean Architecture)

```
src/
├── Lexicon.Domain/          # Entities, Value Objects, Interfaces
├── Lexicon.Application/     # Use Cases, DTOs, Services
├── Lexicon.Infrastructure/  # Database, Repositories
└── Lexicon.Api/             # Controllers, Middleware
```

### Frontend (Next.js 16)

```
frontend/
├── app/                     # App Router pages
├── components/              # UI components
├── lib/                     # API client, utilities
└── types/                   # TypeScript definitions
```

## Tech Stack

### Backend
- **.NET 8** - Latest LTS framework
- **Entity Framework Core 8** - ORM with PostgreSQL
- **FluentValidation** - Input validation
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - API documentation

### Frontend
- **Next.js 16** - React framework with App Router
- **TypeScript** - Type safety
- **Tailwind CSS** - Utility-first styling
- **Axios** - HTTP client

### Infrastructure
- **Docker** - Containerization
- **PostgreSQL** - Database

## Getting Started

### Prerequisites

- Docker & Docker Compose

### Running with Docker

```bash
# Start the application
docker-compose up -d

# View logs
docker-compose logs -f

# Stop
docker-compose down
```

The application will be available at:
- **Frontend**: http://localhost:10181
- **API**: http://localhost:10180
- **Swagger UI**: http://localhost:10180/swagger
- **Database**: localhost:15432

### Development Mode

```bash
# Backend only with hot reload
docker-compose -f docker-compose.dev.yml up -d

# Frontend local development
cd frontend
npm install
npm run dev
```

## API Endpoints

### Posts
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/posts` | List posts (paginated) |
| GET | `/api/posts/{slug}` | Get post by slug |
| POST | `/api/posts` | Create post |
| PUT | `/api/posts/{id}` | Update post |
| DELETE | `/api/posts/{id}` | Delete post |
| POST | `/api/posts/{id}/publish` | Publish post |

### Categories
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/categories` | List categories |
| GET | `/api/categories/tree` | Get category tree |
| GET | `/api/categories/{slug}` | Get category |
| POST | `/api/categories` | Create category |
| PUT | `/api/categories/{id}` | Update category |
| DELETE | `/api/categories/{id}` | Delete category |

### Tags
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/tags` | List tags |
| GET | `/api/tags/{slug}` | Get tag |
| POST | `/api/tags` | Create tag |
| DELETE | `/api/tags/{id}` | Delete tag |

### Authors
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/authors` | List authors |
| GET | `/api/authors/{id}` | Get author |
| POST | `/api/authors` | Create author |
| PUT | `/api/authors/{id}` | Update author |

### Comments
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/posts/{postId}/comments` | Get post comments |
| POST | `/api/posts/{postId}/comments` | Add comment |
| PUT | `/api/comments/{id}/approve` | Approve comment |
| DELETE | `/api/comments/{id}` | Delete comment |

## Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string | See docker-compose.yml |
| `ASPNETCORE_ENVIRONMENT` | Environment (Development/Production) | Development |

## License

This project is licensed under the Apache License 2.0 - see the [LICENSE](LICENSE) file for details.
