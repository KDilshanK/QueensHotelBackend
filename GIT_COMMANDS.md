# Queens Hotel API - Quick Git Commands Reference

## Initial Setup (First Time Only)

```bash
# Navigate to project root
cd "D:\Esoft\ADSE\Queen Hotel System\Documents\Project File\QueensHotelAPI (1)\QueensHotelAPI"

# Option 1: Use automated script (Windows)
.\git-setup.bat

# Option 2: Use PowerShell script
.\git-setup.ps1

# Option 3: Manual commands
git init
git add .
git commit -m "Initial commit: Queens Hotel API"
git branch -M main
git remote add origin https://github.com/KDilshanK/QueensHotelBackend.git
git push -u origin main
```

## Daily Development Workflow

### Check Status
```bash
git status                    # See what changed
git diff                      # See detailed changes
```

### Save Changes
```bash
git add .                     # Stage all changes
git add SpecificFile.cs       # Stage specific file
git commit -m "Your message"  # Commit changes
```

### Push to GitHub
```bash
git push                      # Push committed changes
git push origin main          # Explicit push to main branch
```

### Pull Latest Changes
```bash
git pull                      # Get latest from GitHub
git pull origin main          # Explicit pull from main
```

## Common Scenarios

### Add New Feature
```bash
# Create feature branch
git checkout -b feature/billing-enhancements

# Make changes, then commit
git add .
git commit -m "Add: Enhanced billing service charge calculations"

# Push feature branch
git push -u origin feature/billing-enhancements

# Merge to main (after review)
git checkout main
git merge feature/billing-enhancements
git push
```

### Fix a Bug
```bash
git checkout -b bugfix/customer-search-error
# Fix the bug
git add .
git commit -m "Fix: Customer search null reference error"
git push -u origin bugfix/customer-search-error
```

### Undo Changes

```bash
# Undo uncommitted changes to a file
git checkout -- FileName.cs

# Undo all uncommitted changes
git reset --hard

# Undo last commit (keep changes)
git reset --soft HEAD~1

# Undo last commit (discard changes)
git reset --hard HEAD~1
```

### View History
```bash
git log                       # Full history
git log --oneline            # Compact history
git log --graph --oneline    # Visual history
```

## Good Commit Messages

### Format
```
Type: Short description (50 chars max)

Detailed explanation if needed (wrap at 72 chars)
```

### Types
- **Add:** New feature
- **Fix:** Bug fix
- **Update:** Modify existing feature
- **Refactor:** Code restructuring
- **Docs:** Documentation changes
- **Test:** Add or modify tests
- **Style:** Formatting changes

### Examples
```bash
git commit -m "Add: Customer invoice PDF generation"
git commit -m "Fix: Billing total amount calculation rounding error"
git commit -m "Update: Reservation cancellation email template"
git commit -m "Refactor: Extract billing service charge logic to separate service"
git commit -m "Docs: Add API endpoint documentation for suite management"
```

## Branch Naming Conventions

```
feature/short-description    # New features
bugfix/short-description     # Bug fixes
hotfix/short-description     # Urgent production fixes
refactor/short-description   # Code improvements
docs/short-description       # Documentation updates
```

Examples:
```
feature/automated-checkout
bugfix/payment-card-encryption
hotfix/critical-db-connection
refactor/repository-pattern
docs/api-authentication-guide
```

## Ignore Files After Commit

If you committed sensitive files by mistake:

```bash
# Stop tracking the file
git rm --cached appsettings.Development.json

# Commit the removal
git commit -m "Remove: Sensitive configuration file"

# Add to .gitignore if not already there
echo "appsettings.Development.json" >> .gitignore

# Commit .gitignore update
git add .gitignore
git commit -m "Update: Add appsettings.Development.json to .gitignore"

# Push changes
git push
```

## Useful Aliases (Optional)

Add to `.gitconfig`:

```ini
[alias]
    st = status
    co = checkout
    br = branch
    ci = commit
    unstage = reset HEAD --
    last = log -1 HEAD
    visual = log --graph --oneline --all
```

Usage:
```bash
git st              # Instead of: git status
git co main         # Instead of: git checkout main
git br              # Instead of: git branch
```

## GitHub Authentication

### Using Personal Access Token (PAT)

1. Go to GitHub ? Settings ? Developer settings ? Personal access tokens
2. Generate new token with 'repo' permissions
3. Copy the token
4. When prompted for password, use the PAT instead

### Using SSH (Recommended)

```bash
# Generate SSH key
ssh-keygen -t ed25519 -C "your-email@example.com"

# Add to SSH agent
ssh-add ~/.ssh/id_ed25519

# Copy public key
cat ~/.ssh/id_ed25519.pub

# Add to GitHub: Settings ? SSH and GPG keys ? New SSH key

# Change remote to SSH
git remote set-url origin git@github.com:KDilshanK/QueensHotelBackend.git
```

## Troubleshooting

### "Repository not found"
- Ensure repository exists on GitHub
- Check repository name and username
- Verify authentication

### "Permission denied"
- Check GitHub authentication
- Use Personal Access Token or SSH key
- Verify repository access rights

### "Merge conflict"
```bash
# See conflicted files
git status

# Edit files to resolve conflicts
# Remove markers: <<<<<<<, =======, >>>>>>>

# After resolving
git add .
git commit -m "Resolve: Merge conflicts"
git push
```

### "Detached HEAD state"
```bash
# Return to main branch
git checkout main
```

## Resources

- Git Documentation: https://git-scm.com/doc
- GitHub Guides: https://guides.github.com/
- Pro Git Book: https://git-scm.com/book/en/v2
- GitHub Skills: https://skills.github.com/

## Quick Help

```bash
git help                  # General help
git help commit          # Help for specific command
git command --help       # Detailed help
```
