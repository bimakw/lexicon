# Technical Standards

Standar teknis buat project Lexicon — Blog/CMS REST API, .NET 9 + Clean Architecture + PostgreSQL 16 + Next.js.

Koe wajib baca ini sebelum ngoding. Lek ada yang bingung, tanya aja.

---

## Architecture — Clean Architecture

```
src/
├── Lexicon.Domain/          # Entity, Value Object, Interface, Enum
├── Lexicon.Application/     # Service, DTO, Validator, Mapping
├── Lexicon.Infrastructure/  # EF Core, Repository implementation
└── Lexicon.Api/             # Controller, Middleware, Program.cs
tests/
└── Lexicon.Application.Tests/
```

Dependency direction e: **Domain ← Application ← Infrastructure ← Api**. Layer dalam ga boleh reference layer luar.

### Domain Layer

Domain itu core e project — entity, value object, enum, sama interface repository.

**Rules:**
- Entity harus punya behavior — `Post.Publish()`, `Post.Unpublish()`, `Comment.Approve()`. Jangan bikin entity jadi data bag (anemic model).
- State transition pake guard clause. Contoh: `Publish()` cuma boleh kalo status e `Draft`.
- Value Object buat concept tanpa identity — `Slug`, `Email`, dsb. Harus immutable.
- Repository interface di-define di sini. Implementation e di Infrastructure.
- **Zero framework dependency** — ga boleh ada EF Core, ASP.NET, atau library infra apapun di layer ini.
- Domain Event buat capture perubahan state kalau relevan.

```csharp
// Contoh entity dengan behavior
public class Post : BaseEntity
{
    public void Publish()
    {
        if (Status != PostStatus.Draft)
            throw new DomainException("Only draft posts can be published");

        Status = PostStatus.Published;
        PublishedAt = DateTime.UtcNow;
    }

    public void Unpublish()
    {
        if (Status != PostStatus.Published)
            throw new DomainException("Only published posts can be unpublished");

        Status = PostStatus.Draft;
        PublishedAt = null;
    }
}
```

### Application Layer

Orchestrator — koordinasi antara domain dan infrastructure. Ga boleh ada business rule di sini.

**Rules:**
- Service method itu orchestration: validasi input → panggil domain → panggil repo → return result.
- DTO buat input/output API, pisah dari entity. Pake record biar immutable.
- Semua Create/Update DTO wajib ada FluentValidation validator.
- Pake `Result<T>` buat return value — jangan return null, jangan throw exception buat expected failure.
- AutoMapper profile buat mapping Entity ↔ DTO.

```csharp
// DTO pake record
public record CreatePostDto(string Title, string Content, int CategoryId, List<int> TagIds);

// Result pattern
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, error);
}
```

### Infrastructure Layer

Implementation detail — EF Core, external API, file storage.

**Rules:**
- Repository implementation pake EF Core. Selalu pake `AsNoTracking()` buat read-only query.
- Migration pake EF Core CLI. Nama migration harus deskriptif.
- Konfigurasi entity di `OnModelCreating()` — column length, index, relation, semua di sini.
- External service (storage, email) wrapper e di sini, interface e di Domain/Application.

### Api Layer

Entry point — thin controller, middleware, DI registration.

**Rules:**
- Controller tipis — cuma mapping HTTP ke service call, ga ada logic.
- Authorization pake `[Authorize]` policy-based, bukan role string literals.
- Global exception handler buat error yang ga ke-handle — return proper ProblemDetails.
- Versioning pake URL path prefix (`/api/v1/`).
- Health check endpoint (`/health`) buat container orchestration.

---

## C# Conventions

### Naming

| Element | Convention | Contoh |
|---------|-----------|--------|
| Public member | PascalCase | `GetPostById` |
| Private field | `_camelCase` | `_postRepository` |
| Parameter | camelCase | `postId` |
| Constant | PascalCase | `MaxTitleLength` |
| Interface | `I` prefix | `IPostRepository` |
| DTO | PascalCase + Dto suffix | `CreatePostDto` |

### Modern C# (.NET 9)

- **Primary constructor** — pake buat service class yang butuh DI injection.
- **Records** — buat DTO dan value object. Immutable by default.
- **Pattern matching** — pake switch expression buat conditional logic.
- **Collection expressions** — `int[] ids = [1, 2, 3]` instead of `new int[] { 1, 2, 3 }`.
- **Nullable reference types** enabled — zero nullable warning.

```csharp
// Primary constructor buat service
public class PostService(
    IPostRepository postRepository,
    ICategoryRepository categoryRepository,
    IMapper mapper,
    ILogger<PostService> logger) : IPostService
{
    public async Task<Result<PostDto>> GetByIdAsync(int id, CancellationToken ct)
    {
        var post = await postRepository.GetByIdAsync(id, ct);
        if (post is null)
            return Result<PostDto>.Failure("Post not found");

        return Result<PostDto>.Success(mapper.Map<PostDto>(post));
    }
}
```

### Async Rules

- Semua I/O wajib async. Ga boleh `.Result` atau `.Wait()` — deadlock.
- Selalu propagate `CancellationToken` dari controller sampai ke repository.
- `ConfigureAwait(false)` di library code, ga perlu di ASP.NET controller.
- Ga boleh `async void` — selalu return `Task` atau `Task<T>`.
- Parallel execution buat call yang independent:

```csharp
var (posts, categories) = await (
    postRepository.GetAllAsync(ct),
    categoryRepository.GetAllAsync(ct)
);
```

### Error Handling

- Domain exception buat invariant violation — `DomainException`, `NotFoundException`.
- `Result<T>` buat expected business failure — "post not found", "slug already exists".
- Global exception middleware buat unhandled error → return `ProblemDetails` (RFC 7807).
- **Jangan** catch generic `Exception` tanpa logging.
- **Jangan** swallow error — selalu log atau propagate.

### Comment Rules

- Comment cuma buat **WHY**, bukan WHAT. Code yang jelas ga butuh comment.
- Ga boleh `// Get post by id` di atas `GetByIdAsync()` — obvious.
- XML doc comment cuma buat public API yang complex.
- TODO pake format `// TODO(#issue): description`.
- **Hapus** commented-out code — pake git buat history.

---

## TypeScript / Next.js Conventions

### Struktur Frontend

```
frontend/
├── app/                    # App Router pages
│   ├── (auth)/            # Auth-related routes
│   ├── (dashboard)/       # Dashboard routes
│   └── layout.tsx
├── components/            # Reusable components
│   ├── ui/               # Base UI components
│   └── forms/            # Form components
├── lib/                   # Utilities, API client
│   ├── api.ts            # Centralized API client
│   └── utils.ts
├── types/                 # Shared TypeScript types
└── hooks/                 # Custom hooks
```

### Rules

- **App Router** doang — jangan pake Pages Router.
- **Server Component** by default. Tambah `'use client'` cuma kalo butuh interactivity.
- **Strict TypeScript** — ga boleh `any`. Pake `unknown` + type guard kalo ga tau type e.
- API call lewat centralized client di `lib/api.ts`.
- Shared types di folder `types/` — sync sama backend DTO.
- Styling pake Tailwind CSS 4, conditional class pake `clsx` + `tailwind-merge`.
- State management: Zustand buat client state, TanStack Query buat server state.
- Core Web Vitals target: LCP < 2.5s, INP < 200ms.

---

## Database — PostgreSQL 16

### Naming

- Table: `snake_case`, plural — `posts`, `categories`, `post_tags`.
- Column: `snake_case` — `created_at`, `author_id`, `is_published`.
- Index: `ix_[table]_[columns]` — `ix_posts_slug`, `ix_posts_category_id`.
- FK: `fk_[table]_[ref_table]` — `fk_posts_categories`.

### Rules

- Tambahin index buat FK dan column yang sering di-filter/sort.
- Seed data di `Program.cs` atau separate seeder class, bukan di migration.
- Migration nama e deskriptif — `AddSlugIndexToPosts`, bukan `Migration20240214`.
- Pake `uuid` buat PK kalo butuh distributed ID, `int` kalo simple auto-increment cukup.
- `timestamp with time zone` buat semua datetime column — jangan `timestamp`.

---

## Testing

**Stack**: xUnit + Moq + FluentAssertions

### Struktur Test

```
tests/
└── Lexicon.Application.Tests/
    ├── Services/
    │   ├── PostServiceTests.cs
    │   ├── CategoryServiceTests.cs
    │   └── AuthorServiceTests.cs
    ├── Validators/
    │   ├── CreatePostDtoValidatorTests.cs
    │   └── CreateCategoryDtoValidatorTests.cs
    ├── Domain/
    │   ├── PostEntityTests.cs
    │   └── SlugValueObjectTests.cs
    └── Identity/
        └── AuthValidatorsTests.cs
```

### Test Naming

Format: `Should_ExpectedBehavior_When_Condition`

```
Should_ReturnFailure_When_PostNotFound
Should_CreatePost_When_ValidDto
Should_ThrowDomainException_When_PublishingNonDraftPost
Should_HaveError_When_TitleIsEmpty
```

### Coverage Target per Layer

| Layer | Target | Kenapa |
|-------|--------|--------|
| Domain Entity | 100% | Pure logic, ga ada dependency |
| Service / UseCase | 100% | Business rules, harus bulletproof |
| Validator | 95%+ | Input validation, critical path |
| Controller | 80%+ | Routing, integration |
| Utility / Helper | 95%+ | Dipake di mana-mana |

### Apa yang Di-test

**Prioritas (urutan):**
1. **Business rules** — entity behavior, service orchestration
2. **Error handling** — apa yang terjadi kalo gagal
3. **Edge case** — null, empty, boundary value, duplicate
4. **Validation** — semua rule di validator, happy + sad path
5. **Happy path** — flow normal (ini justru prioritas lebih rendah dari error path)

**Yang ga perlu di-test:**
- AutoMapper profile — tested implicitly lewat service test
- EF Core configuration — tested via integration/migration test
- Third-party library behavior

### Contoh Test

```csharp
public class PostServiceTests
{
    private readonly Mock<IPostRepository> _postRepoMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly PostService _sut;

    public PostServiceTests()
    {
        _sut = new PostService(
            _postRepoMock.Object,
            Mock.Of<ICategoryRepository>(),
            _mapperMock.Object,
            Mock.Of<ILogger<PostService>>());
    }

    [Fact]
    public async Task Should_ReturnFailure_When_PostNotFound()
    {
        _postRepoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Post?)null);

        var result = await _sut.GetByIdAsync(99, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Post not found");
    }

    [Fact]
    public async Task Should_ReturnPost_When_Found()
    {
        var post = new Post { Id = 1, Title = "Test Post" };
        var dto = new PostDto { Id = 1, Title = "Test Post" };

        _postRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(post);
        _mapperMock.Setup(m => m.Map<PostDto>(post)).Returns(dto);

        var result = await _sut.GetByIdAsync(1, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Title.Should().Be("Test Post");
    }
}
```

### Run Tests

```bash
dotnet test --configuration Release --verbosity normal
dotnet test --collect:"XPlat Code Coverage"   # coverage report
```

---

## Git Flow — Trunk-Based

Single trunk: `main`. Semua kerjaan lewat short-lived branch.

### Branch Naming

| Prefix | Kapan |
|--------|-------|
| `feature/<issue>-<desc>` | fitur baru |
| `fix/<issue>-<desc>` | bug fix |
| `chore/<desc>` | maintenance, deps, CI |

Contoh: `feature/13-service-unit-tests`, `fix/15-slug-special-chars`.

### Rules

- Branch hidup **max 2-3 hari**. Kalo udah lama, pecah jadi PR lebih kecil.
- Selalu branch dari `main` terbaru.
- **Squash merge** PR biar history rapi, satu PR = satu commit di main.
- Hapus branch abis merge.
- Ga boleh push langsung ke `main` — harus lewat PR.
- PR harus passing CI sebelum merge.
- PR size target: **< 400 baris** changed. Kalo lebih, pecah.

### Commit Messages

```
feat: add PostService unit tests
fix: strip special chars in slug generation
refactor: move publish logic to Post entity
test: add validators for CreatePostDto
chore: bump EF Core to 9.0.1
docs: update API section in README
```

- Imperative mood, lowercase, tanpa titik di akhir.
- Max 72 karakter buat subject line.
- Reference issue kalo ada: `feat: add slug validation (#15)`
- Squash merge title: `feat: description (#PR)`

### Code Review

- Review dalam 24 jam.
- PR description harus jelas — apa yang diubah, kenapa, gimana test e.
- Approve cuma kalo udah bener. Ga usah approve kalo masih ragu.
- Feedback yang spesifik ke code, jangan generic "looks good".

---

## CI/CD — GitHub Actions

Jalan di push ke `main` dan di semua PR.

### Pipeline

| Stage | Steps | Blocking |
|-------|-------|----------|
| **Build** | `dotnet restore` → `dotnet build` | Yes |
| **Test** | `dotnet test` + coverage report | Yes |
| **Lint** | Code style check | Yes |
| **Security** | Dependency vulnerability scan | Yes (critical/high) |
| **Docker** | Build API + Frontend image | Yes |
| **Deploy** | Push to registry / deploy | Manual trigger |

### Quality Gate

| Check | Threshold |
|-------|-----------|
| Build | Pass |
| All tests | Pass |
| Coverage (service layer) | ≥ 90% |
| Lint error | 0 |
| Critical/High vulnerability | 0 |

---

## Docker

### Dockerfile — .NET Multi-Stage

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY *.sln .
COPY src/*/*.csproj ./
RUN for f in *.csproj; do mkdir -p src/$(basename $f .csproj) && mv $f src/$(basename $f .csproj)/; done
RUN dotnet restore

COPY src/ src/
RUN dotnet publish src/Lexicon.Api/Lexicon.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
RUN addgroup --gid 1001 appgroup && adduser --uid 1001 --gid 1001 --disabled-password appuser

WORKDIR /app
COPY --from=build --chown=appuser:appgroup /app/publish .

USER appuser
EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Lexicon.Api.dll"]
```

### Rules

- Multi-stage build wajib — SDK buat build, runtime buat final.
- **Non-root user** di production image. Ga boleh run sebagai root.
- `.dockerignore` biar image ga gede — exclude `bin/`, `obj/`, `.git`, `node_modules`.
- HEALTHCHECK wajib ada buat container orchestration.
- Image size target: < 250MB buat .NET API.
- Pake specific tag, jangan `:latest` — `mcr.microsoft.com/dotnet/aspnet:9.0`.
- Layer ordering: dependency dulu (jarang berubah), source code terakhir (sering berubah) → biar cache efektif.

### Compose — Local Dev

```yaml
services:
  api:
    build:
      context: .
      target: build
    ports:
      - "5000:8080"
    depends_on:
      db:
        condition: service_healthy
    environment:
      - ConnectionStrings__DefaultConnection=Host=db;Database=lexicon;Username=lexicon;Password=secret

  db:
    image: postgres:16-alpine
    volumes:
      - pgdata:/var/lib/postgresql/data
    environment:
      POSTGRES_DB: lexicon
      POSTGRES_USER: lexicon
      POSTGRES_PASSWORD: secret
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U lexicon"]
      interval: 5s
      timeout: 3s
      retries: 5

volumes:
  pgdata:
```

---

## Security Baseline

### Yang Udah Ada

- JWT auth + refresh token
- RBAC role: Admin, Editor, Author, Reader
- Rate limiting (AspNetCoreRateLimit)
- Security headers, CORS, HTML sanitization
- BCrypt password hashing

### Rules Tambahan

- **Secrets management** — connection string, JWT secret di User Secrets (dev) atau env var / secret manager (prod). Ga boleh hardcode.
- **Input validation** — semua input dari user di-validasi via FluentValidation sebelum masuk service layer.
- **SQL injection** — EF Core sudah safe by default. Kalo raw query, WAJIB parameterized.
- **XSS** — HTML sanitize user content sebelum simpan. Razor auto-encode output.
- **Auth middleware** — semua endpoint protected by default, explicit `[AllowAnonymous]` buat yang public.
- **Dependency scanning** — `dotnet list package --vulnerable` di CI pipeline.

---

## DI Registration

### Lifetime Guidelines

| Lifetime | Kapan |
|----------|-------|
| **Scoped** | Service, Repository — per-request |
| **Singleton** | Stateless helper, HttpClientFactory, Options |
| **Transient** | Validator, lightweight object |

Jangan register semua nya Scoped — itu lazy thinking. Pake lifetime yang bener.

### Registration Order di Program.cs

1. Framework services (EF Core, Identity, Authentication)
2. Application services (IPostService → PostService)
3. Infrastructure services (IPostRepository → PostRepository)
4. Validators
5. AutoMapper
6. Rate limiting, CORS, health checks

---

## File/Folder Rules

- Satu class per file. Nama file = nama class.
- Function max 50 baris (prefer < 25).
- File max 500 baris (prefer < 300). Kalo lebih, pecah by responsibility.
- Import order: System → External package → Internal project.
- Hapus unused import.
