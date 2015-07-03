@echo off
if [%1]==[] goto usage
if [%2]==[] goto usage

echo Creating content directory...
IF not exist %2 (mkdir %2)
if %errorlevel% neq 0 exit /b %errorlevel%

echo Copying content files to build output directory...
xcopy %1 %2 /s /e /y 1>NUL
if %errorlevel% neq 0 exit /b %errorlevel%

echo Finished executing post build script

goto :eof

:usage
@echo Usage: %0 path/to/content/dir path/to/output/content/dir
exit /B 1