@echo off

cd BackendApi
start /b dotnet BackendApi.dll
cd ../Frontend
start /b dotnet Frontend.dll
cd ../JobLogger
start /b dotnet JobLogger.dll
cd ../TextRankCalc
start /b dotnet TextRankCalc.dll
cd ../nats
start nats-server.exe
cd ../redis
start redis-server.exe