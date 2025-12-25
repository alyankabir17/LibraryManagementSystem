# Library Management System - Enhanced Version

ASP.NET Core MVC application with Entity Framework Code-First, featuring authentication, fine management, and modern UI/UX.

## ?? New Features

### 1. **User Authentication & Authorization**
- Secure login and registration system
- Session management with 20-minute timeout
- Automatic logout after inactivity
- User-friendly authentication pages

### 2. **Advanced Fine Management**
- 14-day borrowing period
- Automatic fine calculation (Rs. 5/day after due date)
- Fine payment tracking
- Early return option available
- Clear overdue warnings and notifications

### 3. **Enhanced UI/UX**
- Modern gradient design with purple theme
- Responsive layout for all devices
- Font Awesome icons throughout
- Interactive cards with hover effects
- Real-time session timeout warnings
- Beautiful login/register pages

### 4. **Session Management**
- 20-minute inactivity timeout
- Activity tracking (mouse, keyboard, clicks, scroll)
- 2-minute warning before expiration
- Automatic session renewal on user activity

## Technologies Used

- **Backend**: ASP.NET Core 8.0 MVC
- **ORM**: Entity Framework Core 8.0
- **Database**: SQL Server Express
- **Authentication**: ASP.NET Core Identity
- **Frontend**: Bootstrap 5, Font Awesome 6
- **Language**: C# 12.0

## Database Models

### ApplicationUser (New)
- Id (Primary Key)
- FullName (Required)
- Email (Required, Email format)
- Password (Hashed)
- RegisteredDate
- Role (Default: "User")

### Book
- BookId (Primary Key)
- Title (Required)
- Author (Required)
- ISBN
- Quantity (Required, >= 0)

### Member
- MemberId (Primary Key)
- Name (Required)
- Email (Required, Email format validation)
- Phone (Phone format validation)

### IssueRecord (Enhanced)
- IssueRecordId (Primary Key)
- BookId (Foreign Key)
- MemberId (Foreign Key)
- IssueDate (Required)
- **DueDate (Required) - NEW**
- ReturnDate (Nullable)
- **FineAmount (Decimal) - NEW**
- **IsFinePaid (Boolean) - NEW**

## Setup Instructions

### Prerequisites
- .NET 8.0 SDK
- SQL Server Express
- Visual Studio 2022 or VS Code

### Installation Steps

1. **Stop any running instance of the application**
   - Close all browser windows
   - Stop the debugger in Visual Studio if running

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Build the project**
   ```bash
   dotnet build
   ```

4. **Create new migration for authentication and fine management**
   ```bash
   dotnet ef migrations add AddAuthenticationAndFineManagement
   ```

5. **Update the database**
   ```bash
   dotnet ef database update
   ```

   **Alternative: Use Package Manager Console in Visual Studio**
   ```
   Add-Migration AddAuthenticationAndFineManagement
   Update-Database
   ```

6. **Run the application**
   ```bash
   dotnet run
   ```

   Or press F5 in Visual Studio.

7. **First-time setup**
   - Application will open at the Login page
   - Click "Create Account" to register
   - Fill in your details and create an account
   - You'll be automatically logged in

## Usage Guide

### Authentication

#### Registration
1. Click "Register" or "Create Account"
2. Enter Full Name, Email, and Password (minimum 6 characters)
3. Confirm password
4. Click "Register"

#### Login
1. Enter your email and password
2. Check "Remember Me" to stay logged in longer
3. Click "Login"

#### Session Management
- You have 20 minutes of inactivity before automatic logout
- A warning appears 2 minutes before expiration
- Any activity (mouse movement, clicks, etc.) resets the timer
- After logout, you must log in again to access the system

### Managing Books
1. Navigate to **Books** from the menu
2. Click **Add New Book** to add books
3. View availability status with color-coded badges:
   - Green: More than 5 copies available
   - Yellow: 1-5 copies left
   - Red: Out of stock
4. Use action buttons to Edit, View Details, or Delete

### Managing Members
1. Navigate to **Members** from the menu
2. Click **Add New Member** to register members
3. Each member has an avatar with their initial
4. Manage member information easily

### Issuing Books

#### Standard Issue Process
1. Navigate to **Issue Records**
2. Click **Issue New Book**
3. Select an available book (only books with quantity > 0 shown)
4. Select a member
5. System automatically sets:
   - Issue Date: Today
   - Due Date: 14 days from today
6. Click **Issue Book**

#### Important Issue Rules
- Books cannot be issued if quantity is 0
- Quantity decreases by 1 when issued
- Due date is automatically 14 days from issue date
- Member can return before due date (no fine)

### Returning Books

#### On-Time Return (No Fine)
1. Navigate to **Issue Records**
2. Find the book issue record
3. Click **Return** button
4. Confirm return date
5. Click **Confirm Return**
6. Book quantity automatically increases by 1

#### Late Return (With Fine)
1. If book is returned after due date:
   - System shows overdue warning
   - Calculates fine automatically (Rs. 5 per day)
   - Example: 3 days late = Rs. 15 fine
2. Librarian collects fine payment
3. Check "I confirm that the fine has been paid"
4. Click **Confirm Return**
5. System records fine amount and payment status

### Viewing Issue Records
- **Active**: Books currently issued (blue badge)
- **Overdue**: Books past due date (red badge with days overdue)
- **Returned**: Books returned (green badge)
- Fine amounts displayed prominently
- Payment status shown (Paid/Pending)

## Business Rules

### Book Issuance
- Books can only be issued if quantity > 0
- Quantity decreases by 1 on issue
- Due date is automatically set to 14 days

### Book Returns
- Books can be returned anytime
- Early returns: No fine
- Late returns: Rs. 5 per day fine
- Fine must be paid at return
- Quantity increases by 1 on return

### Fine Calculation
```
Days Late = Return Date - Due Date
If Days Late > 0:
    Fine = Days Late × Rs. 5
Else:
    Fine = Rs. 0
```

### Session Management
- 20-minute inactivity timeout
- Warning at 18 minutes
- Automatic logout at 20 minutes
- Activity resets timer

## Project Structure

```
LibraryManagementSystem/
??? Controllers/
?   ??? AccountController.cs (NEW)
?   ??? HomeController.cs
?   ??? BooksController.cs
?   ??? MembersController.cs
?   ??? IssueRecordsController.cs (ENHANCED)
??? Data/
?   ??? LibraryContext.cs (UPDATED)
??? Models/
?   ??? ApplicationUser.cs (NEW)
?   ??? ViewModels/
?   ?   ??? AuthViewModels.cs (NEW)
?   ??? Book.cs
?   ??? Member.cs
?   ??? IssueRecord.cs (ENHANCED)
?   ??? ErrorViewModel.cs
??? Views/
?   ??? Account/ (NEW)
?   ?   ??? Login.cshtml
?   ?   ??? Register.cshtml
?   ?   ??? AccessDenied.cshtml
?   ??? Books/ (ENHANCED)
?   ??? Members/ (ENHANCED)
?   ??? IssueRecords/ (ENHANCED)
?   ??? Home/
?   ??? Shared/
?       ??? _Layout.cshtml (ENHANCED)
??? wwwroot/
?   ??? css/
?   ?   ??? site.css
?   ?   ??? custom.css (NEW)
?   ??? js/
??? appsettings.json
??? Program.cs (ENHANCED)
```

## Security Features

- Password hashing using ASP.NET Core Identity
- Anti-forgery tokens on all forms
- Secure session cookies
- HTTPS redirection
- Authorization required for all library operations
- Protection against common web vulnerabilities

## UI/UX Improvements

### Design Elements
- Modern gradient backgrounds
- Smooth hover effects and animations
- Responsive cards with shadows
- Color-coded status indicators
- Floating form labels
- Icon integration throughout

### User Experience
- Intuitive navigation
- Clear visual feedback
- Confirmation dialogs
- Success/error notifications
- Loading states
- Mobile-friendly responsive design

## Connection String

Default connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=LibraryManagementDB;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

## Troubleshooting

### Migration Issues

**Error: Application is running**
```
Solution: Stop the application (Ctrl+C or stop debugger) before running migrations
```

**Error: Pending model changes**
```
dotnet ef migrations add [MigrationName]
dotnet ef database update
```

### Session Timeout

**Issue: Logged out unexpectedly**
```
Reason: 20 minutes of inactivity
Solution: Log in again
```

**Issue: Warning not showing**
```
Ensure JavaScript is enabled in your browser
```

### Fine Calculation

**Issue: Fine not calculating**
```
Check that due date is set correctly (14 days from issue date)
Return date must be later than due date for fine
```

### Build Errors

**Issue: Hot reload warning**
```
Restart the application to apply changes
Or stop debugging and rebuild
```

## Testing the Application

### Test Scenarios

1. **User Registration and Login**
   - Register a new user
   - Log out
   - Log in with same credentials

2. **Session Timeout**
   - Log in
   - Wait 18 minutes (warning should appear)
   - Move mouse (timer should reset)
   - Wait 20 minutes without activity (should logout)

3. **Book Management**
   - Add a book with quantity 5
   - Issue the book (quantity should become 4)
   - Return the book (quantity should become 5)

4. **Fine Calculation**
   - Issue a book with issue date 20 days ago
   - Due date will be 6 days ago
   - Return today
   - Fine should be 6 × Rs. 5 = Rs. 30

## License

This project is created for educational purposes.

## Support

For issues or questions:
1. Check this README
2. Review error messages carefully
3. Ensure all prerequisites are installed
4. Verify database connection string

## Version History

### Version 2.0 (Current)
- Added user authentication and authorization
- Implemented fine management system
- Enhanced UI/UX with modern design
- Added session timeout management
- Improved all views with better styling

### Version 1.0
- Basic CRUD operations
- Simple book issue/return
- Basic views

---

**Note**: Remember to stop the application before running migrations!
