@echo off
set outputDirectory="C:\Users\MythicalDoggo\Desktop\MythicalLauncher Debug"
cd %outputDirectory%
del /Q /F *.xml
del /Q /F *.dll
del /Q /F *.exe.config
del /Q /F *.pdb
start MythicalLauncher.exe 
exit