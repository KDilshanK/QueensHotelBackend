# Queens Hotel API - Git Setup Script (PowerShell)
# Author: dilshan-jolanka
# Date: 2025-01-07

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Queens Hotel API - Git Setup Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if Git is installed
try {
    $gitVersion = git --version
    Write-Host "? Git detected: $gitVersion" -ForegroundColor Green
} catch {
    Write-Host "? ERROR: Git is not installed or not in PATH" -ForegroundColor Red
    Write-Host "Please install Git from https://git-scm.com/" -ForegroundColor Yellow
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""
Write-Host "[1/7] Initializing Git repository..." -ForegroundColor Yellow
try {
    git init
    Write-Host "? SUCCESS: Git repository initialized" -ForegroundColor Green
} catch {
    Write-Host "? ERROR: Failed to initialize Git repository" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""
Write-Host "[2/7] Adding all files..." -ForegroundColor Yellow
try {
    git add .
    Write-Host "? SUCCESS: Files added" -ForegroundColor Green
} catch {
    Write-Host "? ERROR: Failed to add files" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""
Write-Host "[3/7] Checking status..." -ForegroundColor Yellow
git status

Write-Host ""
Write-Host "[4/7] Creating initial commit..." -ForegroundColor Yellow
try {
    git commit -m "Initial commit: Queens Hotel API with customer, reservation, billing, suite management, and automated services"
    Write-Host "? SUCCESS: Initial commit created" -ForegroundColor Green
} catch {
    Write-Host "? WARNING: Commit may have failed or files already committed" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "[5/7] Setting branch to main..." -ForegroundColor Yellow
try {
    git branch -M main
    Write-Host "? SUCCESS: Branch set to main" -ForegroundColor Green
} catch {
    Write-Host "? ERROR: Failed to set branch name" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""
Write-Host "[6/7] Adding remote repository..." -ForegroundColor Yellow
try {
    git remote add origin https://github.com/KDilshanK/QueensHotelBackend.git
    Write-Host "? SUCCESS: Remote repository added" -ForegroundColor Green
} catch {
    Write-Host "? WARNING: Remote 'origin' may already exist" -ForegroundColor Yellow
    Write-Host "Removing existing remote and adding again..." -ForegroundColor Yellow
    git remote remove origin
    git remote add origin https://github.com/KDilshanK/QueensHotelBackend.git
    Write-Host "? SUCCESS: Remote repository updated" -ForegroundColor Green
}

Write-Host ""
Write-Host "[7/7] Pushing to GitHub..." -ForegroundColor Yellow
Write-Host "NOTE: You may be prompted for GitHub credentials" -ForegroundColor Cyan
try {
    git push -u origin main
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "SUCCESS! Project pushed to GitHub" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
} catch {
    Write-Host ""
    Write-Host "? ERROR: Failed to push to GitHub" -ForegroundColor Red
    Write-Host ""
    Write-Host "Possible issues:" -ForegroundColor Yellow
    Write-Host "- GitHub repository doesn't exist yet - create it first at https://github.com/new" -ForegroundColor Yellow
    Write-Host "- Authentication failed - you may need to use a Personal Access Token" -ForegroundColor Yellow
    Write-Host "- Remote already exists - try: git push -f origin main" -ForegroundColor Yellow
    Write-Host ""
    Read-Host "Press Enter to exit"
    exit 1
}

Write-Host ""
Write-Host "Repository: https://github.com/KDilshanK/QueensHotelBackend" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Visit your repository on GitHub" -ForegroundColor White
Write-Host "2. Add a description and topics" -ForegroundColor White
Write-Host "3. Configure branch protection rules (optional)" -ForegroundColor White
Write-Host "4. Set up CI/CD (optional)" -ForegroundColor White
Write-Host ""
Write-Host "? IMPORTANT: appsettings.Development.json is NOT in Git" -ForegroundColor Yellow
Write-Host "  Configure it locally with your connection string" -ForegroundColor Yellow
Write-Host ""
Read-Host "Press Enter to exit"
