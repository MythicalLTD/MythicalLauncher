@echo off

rem Read the contents of the file into a variable
for /f "usebackq delims=" %%a in ("buildnumber") do set "contents=%%a"

rem Extract the number from the contents
set "number=%contents%"

rem Increment the number
set /a number+=1

rem Write the updated contents back to the file
echo.%number%>"buildnumber"
set solutionFile="C:\Users\MythicalDoggo\Documents\GitHub\MythicalLauncher\MythicalLauncher.sln"
set outputDirectory="C:\Users\MythicalDoggo\Desktop\MythicalLauncher Debug"
echo Building solution %solutionFile%...
msbuild %solutionFile% /p:OutputPath=%outputDirectory% /t:Rebuild
echo Build finished.
start open.bat
