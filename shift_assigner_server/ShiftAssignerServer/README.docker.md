# Docker — ShiftAssignerServer

This file contains quick instructions to build and run the ShiftAssignerServer API container locally.

Prerequisites
- Docker Desktop (or another Docker runtime) installed and running.

Build the image

```powershell
cd 'C:\MyDevelopment\ShiftAssigner-Server\ShiftAssigner\ShiftAssignerServer'
docker build -t shiftassignerserver:local .
```

Run the container

```powershell
docker run --rm -p 8080:80 --name shiftassigner shiftassignerserver:local
```

Open the API
- Visit http://localhost:8080/ (or http://localhost:8080/swagger if you have Swagger enabled) to exercise endpoints.

Use docker-compose (build + run)

```powershell
cd 'C:\MyDevelopment\ShiftAssigner-Server\ShiftAssigner\ShiftAssignerServer'
docker-compose up --build
```

Stopping the container
- If you used `docker run`:
  - Ctrl+C in the terminal will stop the container if attached, or `docker stop shiftassigner` from a new shell.
- If you used docker-compose:
  - Ctrl+C stops the compose run. Use `docker-compose down` to remove containers.

Notes
- The image runs the app on port 80 inside the container and the compose file maps it to host port 8080.
- To pass configuration or secrets to the container, use environment variables in the `docker run` command or in `docker-compose.yml`.
- For HTTPS in-container you must add certificates and expose 443; for local testing HTTP is simpler.

Debugging the server inside a container
--------------------------------------

There are two pragmatic approaches to debug the API while running in a container:

1) Run the app inside an SDK container (mount your source) and use VS Code to attach to the process.

- Start the SDK container with your project mounted and run `dotnet watch run` so code changes pick up automatically. Replace the path below with your local absolute path when running from PowerShell:

```powershell
docker run --rm -it -p 8080:80 \
  -v C:\MyDevelopment\ShiftAssigner-Server\ShiftAssigner:/src \
  -w /src/ShiftAssignerServer \
  mcr.microsoft.com/dotnet/sdk:8.0 \
  bash -c "dotnet watch run --urls http://+:80"
```

- In VS Code open the Debug view and use "Attach to Process" and pick the `dotnet` process (you may need the ".NET Core Debugger" extension installed). Alternatively use the Docker extension and run the command "Docker: Attach to .NET Core" which will copy the debugger into the container and attach automatically.

2) Run the published runtime image (faster, closer to production) and attach with the Docker extension.

- Build and run the runtime image as shown earlier. Then in VS Code use the Docker extension command palette entry "Docker: Attach to .NET Core" and choose the running container and process to attach.

Notes on debugging
- When you attach to the testhost (if running tests) or the `dotnet` process inside the container, breakpoints in controllers and services will be hit. If you do code edits in the mounted-source approach the container running `dotnet watch` will restart and you may need to re-attach.

Passing JWT configuration into the container
-----------------------------------------

The server reads JWT settings from configuration (e.g., `appsettings.json`). To override them at runtime, pass environment variables when starting the container. Use the double-underscore (`__`) form to set nested configuration keys with environment variables (ASP.NET Core configuration binding).

Example: generate a secure random 32-byte key in PowerShell and run the container with JWT env vars:

```powershell
# generate a base64 key
$bytes = New-Object byte[] 32; [System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes); $key = [Convert]::ToBase64String($bytes); $key

docker run --rm -p 8080:80 \
  -e "Jwt__Key=$key" \
  -e "Jwt__Issuer=ShiftAssigner" \
  -e "Jwt__Audience=ShiftAssignerClients" \
  --name shiftassigner shiftassignerserver:local
```

Notes about the key:
- The `Jwt__Key` value should match what your `JwtService` expects. For symmetric signing, a 32-byte (256-bit) key is appropriate. We encode it as Base64 in the example to allow passing bytes via an environment variable safely.

Example: passing environment variables with docker-compose

Add the environment vars under the `shiftassigner` service in `docker-compose.yml`:

```yaml
services:
  shiftassigner:
    image: shiftassignerserver:local
    ports:
      - "8080:80"
    environment:
      - Jwt__Key=REPLACE_WITH_BASE64_KEY
      - Jwt__Issuer=ShiftAssigner
      - Jwt__Audience=ShiftAssignerClients
```

Security note
- Don't hardcode secrets in source or checked-in compose files. For local dev use environment variables or a `.env` file excluded from source control. For production use a secret store (Azure Key Vault, AWS Secrets Manager, etc.).


If you want, I can add a small healthcheck to `docker-compose.yml` or a script that waits for a ready endpoint before starting additional services.
# ShiftAssignerServer — Docker usage

This file shows how to build and run the API container and a simple `bdd-tests` service which runs the test project inside a container.

Build & run the API image:

```powershell
cd 'C:\MyDevelopment\ShiftAssigner-Server\ShiftAssigner\ShiftAssignerServer'
docker build -t shiftassignerserver:local .
docker run --rm -p 8080:80 --name shiftassigner shiftassignerserver:local
```

Use docker-compose to build and run both the API and the test runner (the test runner will run once and exit):

```powershell
cd 'C:\MyDevelopment\ShiftAssigner-Server\ShiftAssigner\ShiftAssignerServer'
docker-compose up --build
```

Notes and troubleshooting
- The `bdd-tests` service mounts the test project from `..\ShiftAssignerServerTests` and runs `dotnet test` inside the container after a short sleep. This is a pragmatic wait; if the API requires more time to start, increase the `sleep` duration in `docker-compose.yml`.
- The test runner currently runs `dotnet test` against the test project as-is. If your tests use `WebApplicationFactory<Program>` (in-process test host), they will not call the running `shiftassigner` service. To run tests against the live container you must change tests to target the live URL (http://shiftassigner:80) or create alternative test configuration.
- If tests or build fail due to missing packages, run `dotnet restore` locally or ensure the container can access nuget (network, proxies).
- For more robust orchestration use a healthcheck on `shiftassigner` and wait for `service_healthy` before running tests — but the base ASP.NET runtime image may not include `curl` or `wget` used by common healthchecks. You can add a tiny health probe utility or a startup script to expose readiness.

Debugging inside container
- To debug the API with Visual Studio Code you can either:
  - Run the API locally with F5 (recommended), or
  - Run the container with additional debug tooling (more setup required).
