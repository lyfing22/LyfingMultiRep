@echo off
chcp 65001 >nul
echo === [1/5] 清理旧产物 ===
if exist build rmdir /s /q build
if exist dist  rmdir /s /q dist

echo === [2/5] PyInstaller 打包 ===
pyinstaller ^
  --noconfirm ^
  --clean ^
  --onedir ^
  --name "dat2pdf" ^
  --add-data "dat2Img;dat2Img" ^
  --add-data "config.json;." ^
  --hidden-import img2pdf ^
  --hidden-import openpyxl ^
  --console ^
  dat2pdf.py
if errorlevel 1 (
    echo [错误] 打包失败
    pause
    exit /b 1
)

echo === [3/5] 拷贝 config.json 到产物根 ===
copy /Y config.json dist\dat2pdf\ >nul

echo === [4/5] 拷贝 dat2Img 到产物根 (供 EXE 加载 DLL) ===
xcopy /E /I /Y /Q dat2Img dist\dat2pdf\dat2Img >nul

echo === [5/5] 完成 ===
echo 产物路径: %CD%\dist\dat2pdf
echo.
dir dist\dat2pdf
echo.
echo 总大小:
powershell -NoProfile -Command "$size = (Get-ChildItem -LiteralPath 'dist\dat2pdf' -Recurse -File | Measure-Object Length -Sum).Sum; Write-Host ('{0:N1} MB' -f ($size / 1MB))"
pause
