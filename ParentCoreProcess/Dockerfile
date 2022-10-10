FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ParentCoreProcess/ParentCoreProcess.csproj", "ParentCoreProcess/"]
RUN dotnet restore "ParentCoreProcess/ParentCoreProcess.csproj"
COPY . .
WORKDIR "/src/ParentCoreProcess"
RUN dotnet build "ParentCoreProcess.csproj" -c Debug -o /app/build

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build_child
WORKDIR /src
COPY ["ChildProcess/ChildProcess.csproj", "ChildProcess/"]
RUN dotnet restore "ChildProcess/ChildProcess.csproj"
COPY . .
WORKDIR "/src/ChildProcess"
RUN dotnet build "ChildProcess.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "ParentCoreProcess.csproj" -c Debug -o /app/publish

FROM build_child AS publish_child
RUN dotnet publish "ChildProcess.csproj" -c Debug -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish_child /app/publish .

RUN apt-get update  \
    && apt-get install -y openssh-server net-tools wget unzip \
    && mkdir -p /var/run/sshd

COPY entrypoint.sh entrypoint.sh
COPY ssh_config /etc/ssh/sshd_config
RUN service ssh restart
RUN echo "root:docker"|chpasswd

RUN chmod +x /app/entrypoint.sh

EXPOSE 22

ENTRYPOINT ["/app/entrypoint.sh"]