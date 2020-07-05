@echo off
set VERSION=%~1

cd BackendApi
start /wait dotnet publish --configuration Release --output "../app-%VERSION%\BackendApi"
cd ../Frontend
start /wait dotnet publish --configuration Release --output "../app-%VERSION%\Frontend"
cd ../JobLogger
start /wait dotnet publish --configuration Release --output "../app-%VERSION%\JobLogger"
cd ../TextRankCalc
start /wait dotnet publish --configuration Release --output "../app-%VERSION%\TextRankCalc"

mkdir "../app-%VERSION%\config"
mkdir "../app-%VERSION%\Protos"
mkdir "../app-%VERSION%\nats"
mkdir "../app-%VERSION%\redis"

cd ..
copy "redis" "app-%VERSION%\redis"
copy "nats" "app-%VERSION%\nats"
copy "Protos" "app-%VERSION%\Protos"
copy "start.cmd" "app-%VERSION%\"
copy "stop.cmd" "app-%VERSION%\"
copy "config" "app-%VERSION%\config"