echo +++++ [Tutorial] Pre Build

set SOLUTION_DIR=%1
set AZOS_DIR=%2contentFiles\any\any\azos\run-netf\
set PROJECT_DIR=%3
set CONFIG=%4
set OUT_DIR=%5
set TOOL_DIR=%OUT_DIR%azos\run-netf\

echo +++ Solution: %SOLUTION_DIR%
echo +++ Project:  %PROJECT_DIR%
echo +++ Config:   %CONFIG%
echo +++ Out:      %OUT_DIR%
echo +++ Tool:     %TOOL_DIR%

"%AZOS_DIR%buildinfo.exe" > "%PROJECT_DIR%BUILD_INFO.txt"
echo Created BUILD_INFO.txt:
echo -----------------------------
type "%PROJECT_DIR%BUILD_INFO.txt"
echo -----------------------------

echo +++++ [Tutorial] Pre Build