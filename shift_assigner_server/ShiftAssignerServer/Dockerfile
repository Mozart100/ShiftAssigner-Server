# Multi-stage Dockerfile for ShiftAssignerServer
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# allow overriding build configuration at build time (Debug for dev)
ARG CONFIGURATION=Debug

# copy solution and project files for a layered restore

# copy the solution and project files (paths are relative to the build context)
COPY ShiftAssignerServer.csproj ./

# restore only the server project (avoid restoring sibling test projects outside the build context)
RUN dotnet restore "./ShiftAssignerServer.csproj"

# copy the rest of the source and publish (no-restore because we already restored the project)
COPY . ./
WORKDIR /src
RUN dotnet publish "ShiftAssignerServer.csproj" -c ${CONFIGURATION} -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Build-time toggles. These can be overridden at build time with
# --build-arg INSTALL_VSDBG=false --build-arg TARGET_ENV=Production
ARG INSTALL_VSDBG=true
ARG TARGET_ENV=Development

# Always expose the app on port 80. The runtime environment is set from
# TARGET_ENV (default Development) but can be changed for production builds.
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=${TARGET_ENV}

COPY --from=build /app/publish .

# Install small tools and optionally the VSDBG debugger so VS Code can attach
# reliably. Use a shell conditional so the same Dockerfile can produce lean
# production images by setting INSTALL_VSDBG=false at build time.
RUN apt-get update \
 && apt-get install -y --no-install-recommends \
	 curl \
	 unzip \
	 ca-certificates \
	 procps \
	 lsof \
	 iproute2 \
 && mkdir -p /remote/vsdbg \
 && if [ "${INSTALL_VSDBG}" = "true" ] ; then \
		echo "Installing vsdbg..." ; \
		curl -sSL https://aka.ms/getvsdbgsh | bash /dev/stdin -v latest -l /remote/vsdbg ; \
	 else \
		echo "Skipping vsdbg installation (INSTALL_VSDBG=${INSTALL_VSDBG})" ; \
	 fi \
 && rm -rf /var/lib/apt/lists/*

# Expose the web port. When the build used the default Debug configuration the
# published output includes PDB files so source-level debugging will work.
EXPOSE 80

ENTRYPOINT ["dotnet", "ShiftAssignerServer.dll"]
