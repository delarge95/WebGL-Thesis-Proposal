Write-Host "Installing dependencies..." -ForegroundColor Cyan
Set-Location "E:\WebGL_tesis\docs"

if (Test-Path "node_modules") {
    Write-Host "Removing existing node_modules..." -ForegroundColor Yellow
    Remove-Item -Recurse -Force node_modules
}

npm install

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nDependencies installed successfully!" -ForegroundColor Green
    Write-Host "`nInstalled packages:" -ForegroundColor Cyan
    npm list --depth=0
    
    Write-Host "`n`nTesting build..." -ForegroundColor Cyan
    npm run build
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "`nBuild successful! Output in dist/" -ForegroundColor Green
    } else {
        Write-Host "`nBuild failed!" -ForegroundColor Red
    }
} else {
    Write-Host "`nInstallation failed!" -ForegroundColor Red
}
