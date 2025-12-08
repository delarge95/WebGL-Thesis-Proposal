@echo off
echo ========================================
echo Installing npm dependencies...
echo ========================================
cd /d "E:\WebGL_tesis\docs"

if exist node_modules (
    echo Removing existing node_modules...
    rmdir /s /q node_modules
)

call npm install

if %errorlevel% equ 0 (
    echo.
    echo ========================================
    echo Dependencies installed successfully!
    echo ========================================
    echo.
    echo Installed packages:
    call npm list --depth=0
    
    echo.
    echo ========================================
    echo Testing build...
    echo ========================================
    call npm run build
    
    if %errorlevel% equ 0 (
        echo.
        echo ========================================
        echo Build successful! Output in dist/
        echo ========================================
    ) else (
        echo.
        echo ========================================
        echo Build failed!
        echo ========================================
    )
) else (
    echo.
    echo ========================================
    echo Installation failed!
    echo ========================================
)

pause
