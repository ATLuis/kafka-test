FROM mcr.microsoft.com/vscode/devcontainers/dotnet:2.1

WORKDIR /workspace

COPY . .

CMD ["/bin/sh", "-c", "while sleep 1000; do :; done"]