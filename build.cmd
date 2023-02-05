@echo off

set URL=https://download.visualstudio.microsoft.com/download/pr/24e5c5e1-0ce5-460b-98d9-9f5580df2339/f7d6511bccba8892b48f06b0c723fceb/vs_BuildTools.exe
set TEMP=%TEMP%\vs_buildtools.exe

echo Downloading Microsoft Build Tools...
curl -LJO %URL% -o %TEMP%

echo Installing Microsoft Build Tools...
%TEMP% --quiet --wait --norestart --nocache --installPath "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\BuildTools"

echo Adding MSBuild to PATH...
setx /M PATH "%PATH%;%ProgramFiles(x86)%\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin"

echo Building project...
MSBuild MythicalLauncher.sln /p:Configuration=Release

echo Build completed.
