# 使用多阶段构建优化镜像大小

# 第一阶段：构建应用
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src/BackEnd/HailongConsulting.API

# 复制项目文件并还原依赖
COPY ["BackEnd/HailongConsulting.API/HailongConsulting.API.csproj", "./"]
RUN dotnet restore "HailongConsulting.API.csproj"

# 复制所有源代码
COPY BackEnd/HailongConsulting.API/ ./
COPY config/portal-defaults.json /src/config/portal-defaults.json

# 构建应用
RUN dotnet build "HailongConsulting.API.csproj" -c Release -o /app/build

# 发布应用
FROM build AS publish
RUN dotnet publish "HailongConsulting.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 第二阶段：运行时镜像
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# 安装curl用于健康检查
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# 复制发布的文件
COPY --from=publish /app/publish .

# 创建上传和日志目录
RUN mkdir -p /app/wwwroot/uploads /app/logs && \
    chmod -R 755 /app/wwwroot /app/logs

# 暴露端口
EXPOSE 5000

# 设置环境变量
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV TZ=Asia/Shanghai

# 启动应用
ENTRYPOINT ["dotnet", "HailongConsulting.API.dll"]
