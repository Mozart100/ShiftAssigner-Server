# ShiftAssigner Server

## PostgreSQL Docker Setup

### Quick Start - Rebuild PostgreSQL Container

To completely remove and recreate the PostgreSQL container from scratch:

```powershell
docker stop postgres14 2>$null; docker rm postgres14 2>$null; docker run --name postgres14 -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=shiftassigner -p 5432:5432 -d postgres:14
```

### Database Configuration

- **Container Name**: `postgres14`
- **Database**: `shiftassigner`
- **Username**: `postgres`
- **Password**: `postgres`
- **Port**: `5432`
- **Host**: `localhost`

### Connection String

```
Host=localhost;Database=shiftassigner;Username=postgres;Password=postgres;Port=5432
```

### Useful Commands

```powershell
# Check if container is running
docker ps | findstr postgres

# Connect to database
docker exec -it postgres14 psql -U postgres -d shiftassigner

# List all schemas
docker exec postgres14 psql -U postgres -d shiftassigner -c "\dn"

# List all tables in public schema
docker exec postgres14 psql -U postgres -d shiftassigner -c "\dt"

# List all tables in specific tenant schema
docker exec postgres14 psql -U postgres -d shiftassigner -c "\dt testcompany_1.*"


docker stop postgres14 2>$null; docker rm postgres14 2>$null; docker run --name postgres14 -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=shiftassigner -p 5432:5432 -d postgres:14
```