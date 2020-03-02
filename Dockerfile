FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS build
WORKDIR /app

COPY *.sln .
COPY Base/Base.csproj ./Base/
COPY BLL/BLL.csproj ./BLL/
COPY DAL/DAL.csproj ./DAL/
COPY UnitTestBLL/UnitTestBLL.csproj ./UnitTestBLL/
COPY WebApplication1/WebApplication1.csproj ./WebApplication1/
RUN dotnet restore 

COPY . ./
RUN dotnet publish -c release -o /out  --no-restore

FROM mcr.microsoft.com/dotnet/core/aspnet:2.1
WORKDIR /out
COPY --from=build /out .

EXPOSE 80 5000 5001

ENTRYPOINT ["dotnet", "WebApplication1.dll"]

