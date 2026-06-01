# University Tasks — Database First REST API

ASP.NET Core Web API (`UniversityTasksDbFirstApi`) over an existing SQL Server database using EF Core Database First (no migrations).

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (10.x)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (optional, for SQL Server in Docker)
- `dotnet-ef` global tool (for re-scaffolding)

## 1. Start SQL Server

From the repository root:

```bash
docker compose up -d
```

Wait until SQL Server is ready (~20–30 seconds on first start).

## 2. Create and seed the database

```bash
docker cp TASK/zadanie_1_db_first_university_tasks_setup.sql apbd_lecture9_db_first:/tmp/setup.sql

docker exec apbd_lecture9_db_first /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "YourStrongPassword123!" -C \
  -i /tmp/setup.sql
```

## 3. (Optional) Re-scaffold EF Core model

If you change the database schema, regenerate entities from the database (overwrites `Models/` and `Data/UniversityTasksDbContext.cs`):

```bash
cd UniversityTasksDbFirstApi

dotnet ef dbcontext scaffold \
  "Server=localhost,14333;Database=ApbdLecture9DbFirstTask;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True" \
  Microsoft.EntityFrameworkCore.SqlServer \
  --context UniversityTasksDbContext \
  --context-dir Data \
  --output-dir Models \
  --no-onconfiguring
```

Custom code lives outside scaffolded files: `ModelExtensions/`, `DTOs/`, `Services/`, `Controllers/`.

## 4. Run the API

```bash
cd UniversityTasksDbFirstApi
dotnet run
```

Open Swagger UI: `https://localhost:7xxx/swagger` (port from console output).

## API endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/courses?activeOnly=true` | Courses with assignment counts |
| GET | `/api/courses/{id}/assignments?publishedOnly=true` | Assignments for a course |
| GET | `/api/students/{id}/dashboard` | Student dashboard |
| POST | `/api/submissions` | Create submission |
| PUT | `/api/submissions/{id}/grade` | Grade submission |
| DELETE | `/api/submissions/{id}` | Delete ungraded submission |

Connection string: `appsettings.json` → `DefaultConnection` (port `14333` for Docker).
