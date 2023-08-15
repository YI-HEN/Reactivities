FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app
EXPOSE 8080
#建立環境及資料夾指定

#複製解決方案到Docker
COPY "Reactivities.sln" "Reactivities.sln"
COPY "API/API.csproj" "API/API.csproj"
COPY "Application/Application.csproj" "Application/Application.csproj"
COPY "Persistence/Persistence.csproj" "Persistence/Persistence.csproj"
COPY "Domain/Domain.csproj" "Domain/Domain.csproj"
COPY "Infrastructure/Infrastructure.csproj" "Infrastructure/Infrastructure.csproj"

#設定命令執行
RUN dotnet restore "Reactivities.sln"

#複製全部並建立
COPY . .
#指定工作目錄
WORKDIR /app

#運行命令
RUN dotnet publish -c Release -o out 

#建立運行鏡像檔(sdk的小包裝)
FROM mcr.microsoft.com/dotnet/aspnet:7.0
#指定工作目錄
WORKDIR /app

#複製並退出建立指令
COPY --from=build-env /app/out .

#建立入口點
ENTRYPOINT [ "dotnet", "API.dll" ]