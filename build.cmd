@echo off

echo Downloading Microsoft Build Tools...
powershell -Command "Invoke-WebRequest -Uri 'https://aka.ms/vs/16/release/vs_buildtools.exe' -OutFile 'vs_buildtools.exe'"

echo Installing Microsoft Build Tools...
vs_buildtools.exe --quiet --wait --norestart --nocache --installPath "C:\BuildTools"

echo Adding MSBuild to PATH...
setx PATH "%PATH%;C:\BuildTools\MSBuild\Current\Bin"

echo Building project...
msbuild MythicalLauncher.sln /p:Configuration=Release

echo Build completed.
