FROM microsoft/aspnetcore-build:2.0
ADD . /app
WORKDIR /app
RUN dotnet restore ./RedpointDefaultBackend.sln && dotnet publish ./RedpointDefaultBackend.sln -c Release -o ./obj/Docker/publish
WORKDIR /app/obj/Docker/publish
EXPOSE 80
ENTRYPOINT ["dotnet", "RedpointDefaultBackend.dll"]
