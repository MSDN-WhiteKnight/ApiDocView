@echo off
dotnet build
ApiDocView\bin\Debug\netcoreapp3.1\ApiDocView.exe "..\dotnet-api-docs"
pause
