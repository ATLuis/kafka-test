FROM mcr.microsoft.com/vscode/devcontainers/dotnet:6.0

WORKDIR /workspace

COPY . .

CMD ["/bin/sh", "-c", "while sleep 1000; do :; done"]