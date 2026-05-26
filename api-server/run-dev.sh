#!/bin/bash
cd "$(dirname "$0")"
exec dotnet run --project src/NaarNoor.API/NaarNoor.API.csproj
