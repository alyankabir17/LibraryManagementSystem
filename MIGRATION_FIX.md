# IMPORTANT: How to Fix the "Invalid object name 'AspNetUsers'" Error

## The Problem
The database doesn't have the ASP.NET Identity tables yet because we added authentication features but haven't created the migration.

## Solution Steps

### Step 1: Stop the Running Application
**VERY IMPORTANT**: You must stop the application first!

- In Visual Studio: Click the **Stop** button (red square) or press **Shift+F5**
- In Terminal: Press **Ctrl+C**
- In Browser: Close all browser windows showing the application

Wait 5-10 seconds to ensure the process is fully stopped.

### Step 2: Create the Migration

Open **Package Manager Console** in Visual Studio (Tools ? NuGet Package Manager ? Package Manager Console) and run:

```powershell
Add-Migration AddAuthenticationAndFineManagement
```

**OR** in the Terminal/Command Prompt (in the project directory):

```bash
dotnet ef migrations add AddAuthenticationAndFineManagement
```

### Step 3: Update the Database

In **Package Manager Console**:
```powershell
Update-Database
```

**OR** in Terminal:
```bash
dotnet ef database update
```

### Step 4: Run the Application Again

Press **F5** in Visual Studio or run:
```bash
dotnet run
```

## What This Migration Will Create

The migration will add these Identity tables to your database:
- **AspNetUsers** - User accounts
- **AspNetRoles** - User roles
- **AspNetUserRoles** - User-role relationships
- **AspNetUserClaims** - User claims
- **AspNetUserLogins** - External login providers
- **AspNetUserTokens** - Authentication tokens
- **AspNetRoleClaims** - Role claims

It will also:
- Add **DueDate** column to IssueRecords
- Add **FineAmount** column to IssueRecords
- Add **IsFinePaid** column to IssueRecords

## Troubleshooting

### If migration fails with "file is locked":
1. Make sure the application is completely stopped
2. Close all browser windows
3. Wait 10 seconds
4. Try again

### If you get "No migrations configuration type found":
Make sure you're in the correct directory (where the .csproj file is):
```bash
cd LibraryManagementSystem
dotnet ef migrations add AddAuthenticationAndFineManagement
```

### If you want to start fresh:
1. Delete all files in the `Migrations` folder (if it exists)
2. Delete the database:
   - Open SQL Server Management Studio
   - Connect to `.\SQLEXPRESS`
   - Delete `LibraryManagementDB` database
3. Run:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

## After Successful Migration

1. **Run the application** - It will open at the Login page
2. **Click "Create Account"** to register your first user
3. **Fill in the form**:
   - Full Name
   - Email
   - Password (min 6 characters)
   - Confirm Password
4. **Click Register** - You'll be automatically logged in
5. **Start using the library system!**

## Quick Command Reference

```bash
# Stop everything first!

# Create migration
dotnet ef migrations add AddAuthenticationAndFineManagement

# Apply migration to database
dotnet ef database update

# Run the app
dotnet run
```

---

**Remember**: Always stop the application before running migrations!
