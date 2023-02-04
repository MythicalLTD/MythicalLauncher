@echo off

set solutionFile=MythicalLauncher.sln

"%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\MSBuild.exe" %solutionFile% /p:Configuration=Release
