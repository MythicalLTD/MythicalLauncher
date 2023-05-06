@echo off
set outputDirectory="# Path to output directory #"
cd %outputDirectory%
del /Q /F *.xml
del /Q /F *.dll
del /Q /F *.exe.config
del /Q /F *.pdb
start MythicalLauncher.exe 
exit