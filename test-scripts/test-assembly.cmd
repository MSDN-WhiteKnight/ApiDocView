@echo off
cd ..
dotnet build
ApiDocView\bin\Debug\netcoreapp3.1\ApiDocView.exe "..\dotnet-docs\docs\standard\assembly"
