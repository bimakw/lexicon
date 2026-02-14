# Technical Standards

Standar teknis buat project Lexicon. Blog/CMS REST API, pake .NET 9 + Clean Architecture.

Koe wajib baca ini sebelum ngoding. Lek ada yang bingung, tanya aja.

## Architecture

```
src/
├── Lexicon.Domain/          # Entity, Interface, Enum
├── Lexicon.Application/     # Service, DTO, Validator
├── Lexicon.Infrastructure/  # EF Core, Repository implementation
└── Lexicon.Api/             # Controller, Middleware, Program.cs
tests/
└── Lexicon.Application.Tests/
```

Dependency direction e: Domain ← Application ← Infrastructure ← Api. Layer dalam ga boleh reference layer luar.

### Domain

- Entity harus punya behavior sendiri — misal `Post.Publish()`, `Post.Unpublish()`. Jangan taruh logic di service.
- Interface repository di-define di sini, implementation e di Infrastructure.
- Ga boleh ada dependency ke EF Core atau ASP.NET di layer ini.

### Application

- Service itu orchestrator, bukan tempat business logic. Logic e di entity.
- DTO buat input/output API, pisah dari entity biar ga bocor.
- Semua Create/Update DTO wajib ada FluentValidation validator.
- Pake `Result<T>` buat return value — jangan return null, jangan throw exception buat expected error.

### Infrastructure

- Repository implementation pake EF Core.
- Migration pake EF Core CLI.
- Service eksternal (storage, email dll) masuknya di sini.

### Api

- Controller tipis — cuma mapping HTTP ke service call.
- Authorization pake `[Authorize]` policy-based.
- Global exception handler buat error yang ga ke-handle.

## Git Flow — Trunk-Based

Satu branch utama: `main`. Semua kerjaan lewat branch pendek.

### Branch Naming

| Prefix | Kapan |
|--------|-------|
| `feature/<issue>-<desc>` | fitur baru |
| `fix/<issue>-<desc>` | bug fix |
| `chore/<desc>` | maintenance, update deps, CI |

### Rules

- Branch hidup **max 2-3 hari**. Jangan lama-lama.
- Selalu branch dari `main` terbaru.
- **Squash merge** PR biar history rapi.
- Hapus branch abis merge.
- Ga boleh push langsung ke `main`.
- PR harus passing CI sebelum merge.

### Commit Messages

```
feat: add PostService unit tests
fix: strip special chars in slug generation
refactor: move publish logic to Post entity
test: add validators for CreatePostDto
chore: bump EF Core to 9.0.1
docs: update API section in README
```

Squash merge title e: `feat: description (#PR)`.

## Coding Conventions

### C#

- PascalCase buat public member, `_camelCase` buat private field.
- Semua I/O harus async. Selalu propagate `CancellationToken`.
- Nullable reference types enabled — jangan ada warning.
- Pake record buat DTO dan value object.
- Pake `Result<T>` buat expected failure, jangan exception.
- Comment cuma buat WHY, bukan WHAT. Yang jelas dari code e ga usah dicomment.
- Satu class per file.

### TypeScript / Next.js

- App Router doang (jangan Pages Router).
- Functional component. Server Component kalo bisa.
- Tailwind CSS 4, pake `clsx` + `tailwind-merge` buat conditional class.
- API call lewat centralized client di `lib/api.ts`.
- Strict TypeScript — ga boleh `any`.
- Shared types di folder `types/`.

### SQL / PostgreSQL

- snake_case buat table dan column.
- Tambahin index buat FK dan column yang sering di-filter.
- Seed data di `Program.cs`, jangan di migration.
- Nama migration yang deskriptif.

## Testing

**Stack**: xUnit + Moq + FluentAssertions

```
tests/
└── Lexicon.Application.Tests/
    ├── Services/
    │   ├── PostServiceTests.cs
    │   └── CategoryServiceTests.cs
    ├── Validators/
    │   └── CreatePostDtoValidatorTests.cs
    └── Identity/
        └── AuthValidatorsTests.cs
```

**Naming**: `Should_ExpectedBehavior_When_Condition`

Contoh: `Should_ReturnNull_When_PostNotFound`, `Should_CreatePost_When_ValidDto`.

Yang di-test: service method, validator, domain entity behavior.

Run: `dotnet test --configuration Release --verbosity normal`

## CI/CD

GitHub Actions, jalan di push ke `main` dan di PR:

| Job | Steps |
|-----|-------|
| Backend | Restore → Build → Test |
| Frontend | Install → Lint → Build |
| Docker | Build API + Frontend image |

## Docker

- Multi-stage build: SDK buat build, ASP.NET runtime buat final.
- Non-root user di production image.
- `.dockerignore` biar image ga gede.
- `docker-compose.yml` buat full stack.

## Security

Yang udah ada:
- JWT auth + refresh token
- RBAC (Admin, Editor, Author, Reader)
- Rate limiting (AspNetCoreRateLimit)
- Security headers, CORS, HTML sanitization
- BCrypt password hashing
