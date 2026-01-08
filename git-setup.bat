@echo off
echo ========================================
echo Queens Hotel API - Git Setup Script
echo ========================================
echo.

REM Check if Git is installed
git --version >nul 2>&1
if errorlevel 1 (
    echo ERROR: Git is not installed or not in PATH
    echo Please install Git from https://git-scm.com/
    pause
    exit /b 1
)

echo [1/7] Initializing Git repository...
git init
if errorlevel 1 (
    echo ERROR: Failed to initialize Git repository
    pause
    exit /b 1
)
echo SUCCESS: Git repository initialized
echo.

echo [2/7] Adding all files...
git add .
if errorlevel 1 (
    echo ERROR: Failed to add files
    pause
    exit /b 1
)
echo SUCCESS: Files added
echo.

echo [3/7] Checking status...
git status
echo.

echo [4/7] Creating initial commit...
git commit -m "Initial commit: Queens Hotel API with customer, reservation, billing, suite management, and automated services"
if errorlevel 1 (
    echo ERROR: Failed to create commit
    echo Note: If all files are already committed, this is normal
)
echo.

echo [5/7] Setting branch to main...
git branch -M main
if errorlevel 1 (
    echo ERROR: Failed to set branch name
    pause
    exit /b 1
)
echo SUCCESS: Branch set to main
echo.

echo [6/7] Adding remote repository...
git remote add origin https://github.com/KDilshanK/QueensHotelBackend.git
if errorlevel 1 (
    echo WARNING: Remote 'origin' may already exist
    echo Removing existing remote and adding again...
    git remote remove origin
    git remote add origin https://github.com/KDilshanK/QueensHotelBackend.git
)
echo SUCCESS: Remote repository added
echo.

echo [7/7] Pushing to GitHub...
echo NOTE: You may be prompted for GitHub credentials
git push -u origin main
if errorlevel 1 (
    echo ERROR: Failed to push to GitHub
    echo.
    echo Possible issues:
    echo - GitHub repository doesn't exist yet - create it first at https://github.com/new
    echo - Authentication failed - you may need to use a Personal Access Token
    echo - Remote already exists - try: git push -f origin main
    echo.
    pause
    exit /b 1
)

echo.
echo ========================================
echo SUCCESS! Project pushed to GitHub
echo ========================================
echo.
echo Repository: https://github.com/KDilshanK/QueensHotelBackend
echo.
echo Next steps:
echo 1. Visit your repository on GitHub
echo 2. Add a description and topics
echo 3. Configure branch protection rules (optional)
echo 4. Set up CI/CD (optional)
echo.
echo Remember: appsettings.Development.json is NOT in Git
echo           Configure it locally with your connection string
echo.
pause
