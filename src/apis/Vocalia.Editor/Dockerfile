#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM microsoft/dotnet:2.1-aspnetcore-runtime-nanoserver-1803 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 5002

FROM microsoft/dotnet:2.1-sdk-nanoserver-1803 AS build
WORKDIR /src
COPY ["src/apis/Vocalia.Editor/Vocalia.Editor.csproj", "src/apis/Vocalia.Editor/"]
RUN dotnet restore "src/apis/Vocalia.Editor/Vocalia.Editor.csproj"
COPY . .
WORKDIR "/src/src/apis/Vocalia.Editor"
RUN dotnet build "Vocalia.Editor.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Vocalia.Editor.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Vocalia.Editor.dll"]