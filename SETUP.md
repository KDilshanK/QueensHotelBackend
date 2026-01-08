# Queens Hotel API - Setup Guide

## Local Development Setup

### Step 1: Configure Connection String

**Option A: Using appsettings.Development.json (Recommended for local dev)**

Create `QueensHotelAPI/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=DILSHAN\\SQLEXPRESS;Database=latestQueenHotel;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true;Connection Timeout=60;Encrypt=false;"
  }
}
```

**Option B: Using User Secrets (Most secure for local dev)**

```bash
cd QueensHotelAPI
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=DILSHAN\\SQLEXPRESS;Database=latestQueenHotel;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true;Connection Timeout=60;Encrypt=false;"
```

**Option C: Using Environment Variables**

Windows:
```cmd
setx ConnectionStrings__DefaultConnection "Server=DILSHAN\\SQLEXPRESS;Database=latestQueenHotel;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true;Connection Timeout=60;Encrypt=false;"
```

### Step 2: Verify Setup

```bash
# Restore packages
dotnet restore

# Build project
dotnet build

# Run project
dotnet run --project QueensHotelAPI

# Test endpoints
curl http://localhost:5170/health
curl http://localhost:5170/health/database
```

## Git Commands for Initial Commit

```bash
# Navigate to project root
cd "D:\Esoft\ADSE\Queen Hotel System\Documents\Project File\QueensHotelAPI (1)\QueensHotelAPI"

# Initialize Git
git init

# Add all files (excluding .gitignore items)
git add .

# Verify what will be committed
git status

# Commit
git commit -m "Initial commit: Queens Hotel API with customer, reservation, billing, suite management, and automated services"

# Set main branch
git branch -M main

# Add remote repository
git remote add origin https://github.com/KDilshanK/QueensHotelBackend.git

# Push to GitHub
git push -u origin main
```

## Important Notes

?? **Security Checklist Before Pushing:**

- [x] `.gitignore` file created
- [x] Connection strings removed from `appsettings.json`
- [x] `appsettings.Development.json` added to `.gitignore`
- [x] No passwords or API keys in committed files
- [ ] Database backups stored securely (not in Git)

## After Cloning (For Team Members)

1. Clone the repository:
   ```bash
   git clone https://github.com/KDilshanK/QueensHotelBackend.git
   cd QueensHotelBackend
   ```

2. Create `appsettings.Development.json` with your connection string

3. Restore and run:
   ```bash
   dotnet restore
   dotnet run --project QueensHotelAPI
   ```

## Deployment to Azure

1. Create Azure App Service
2. Configure connection string in Azure Portal:
   - Go to App Service ? Configuration ? Connection strings
   - Add `DefaultConnection` with your Azure SQL connection string

3. Deploy from GitHub:
   - App Service ? Deployment Center
   - Connect to GitHub repository
   - Select branch: main
   - Save and deploy

## Troubleshooting

**Issue: Connection string not found**
- Solution: Create `appsettings.Development.json` or use user secrets

**Issue: Database connection failed**
- Verify SQL Server is running
- Check server name and database name
- Test with SQL Server Management Studio first

**Issue: Git push rejected**
- Solution: Run `git pull origin main` first, then push again

## Database Setup

Ensure your SQL Server has:
1. Database named `latestQueenHotel`
2. All required stored procedures installed:
   - GetCustomerData
   - GetReservationData
   - InsertBilling
   - GetBillingInfo
   - InsertBillingServiceCharge
   - GetBillingServiceCharges
   - GenerateCustomerInvoice
   - And others...

3. Tables properly configured with relationships

## Contact

For setup issues, contact: dilshan-jolanka@queenshotel.com
