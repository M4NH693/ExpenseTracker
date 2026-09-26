# ExpenseTracker — Personal Finance and Expense Management System

[English](README.md) | [Tiếng Việt](README_VI.md)

![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16.0-4169E1?logo=postgresql)
![Platform](https://img.shields.io/badge/Platform-Windows-0078D6?logo=windows)
![Architecture](https://img.shields.io/badge/Architecture-3--Tier%20Layered-0F4761)

ExpenseTracker (Quản lý chi tiêu) is a modern .NET 10 Windows Forms desktop application designed for comprehensive personal financial tracking, income and expense categorization, monthly budget limits with proactive threshold warnings, debts and loans management with automatic cash-flow synchronization, and multi-dimensional financial chart analytics. Built with a clean 3-Tier Layered Architecture (DTO – DAL – BLL – GUI), runtime data is persisted in a PostgreSQL relational database with parameterized queries and strict referential integrity.

---

## Features

### 1. User Authentication and Account Security

- User registration and login with email identity, full name, date of birth, gender, and address.
- Password hashing and secure credential validation.
- **Forgot Password via Email OTP:**
  - Random 6-digit OTP code generation with a 5-minute transient memory cache.
  - Asynchronous email dispatch via Google SMTP (`smtp.gmail.com:587`, TLS/SSL) using `System.Net.Mail` with a built-in 60-second cooldown timer.
  - Two-step verification and dedicated password reset form.
- User profile management with editable personal details and an in-app password change workflow.
- Single global session tracking via static user identity context (`AuthForm.CurrentUserId`).

### 2. Income and Expense Tracking

- Record daily income and expenditure with monetary amount, transaction date, custom notes, and associated category.
- Real-time numerical formatting with thousands separators (`N0`, e.g., `1,500,000 đ`) on text entry.
- Dynamic category picker that automatically filters by transaction type (*Thu Nhập* or *Chi Tiêu*).
- Built-in transaction history (Financial Calendar / Balance Variance view):
  - Keyword search across transaction descriptions.
  - Filter transactions by type (*All*, *Thu Nhập*, *Chi Tiêu*).
  - Explicit column layout (`[Loại] | [Danh Mục] | [Thời Gian]`) with color-coded typography (Emerald Green for income, Crimson Red for expenses).
  - Update and delete transactions with confirmation prompts.

### 3. Category Management

- Separate classification for Income categories (Salary, Bonus, Allowance, Investment, etc.) and Expense categories (Food & Dining, Housing, Transportation, Shopping, Healthcare, Entertainment, etc.).
- Complete CRUD operations: Add new category, edit name/type, and delete unused categories.
- User-scoped isolation: Each account manages its own independent category taxonomy.

### 4. Budget Limits and Spending Alerts

- Set monthly and yearly spending caps per expense category (e.g., maximum `3,000,000 đ` for *Food & Dining* in October 2026).
- Track real-time budget utilization via data grids and visual percentage indicators:
  - **Safe (< 80%):** Standard progress indication.
  - **Warning (80% – 100%):** Amber threshold alert signaling approaching limit.
  - **Exceeded (> 100%):** Prominent Red alert with a modal dialog warning the user before confirming any new transaction that breaches the allocated budget.
- Automatic calculation of remaining spending allowance for the selected billing cycle.

### 5. Debts and Loans Management (Sổ Vay Nợ)

- Track personal liabilities (*Tôi Nợ* / Borrowed) and receivables (*Cho Vay* / Lent).
- Store borrower/lender name, loan amount, due date, notes, and lifecycle status (*Đang Nợ* / Active vs. *Đã Trả* / Settled).
- **Atomic Debt Settlement (`SettleDebtWithTransaction`):**
  - Marks the debt record as *Đã Trả*.
  - Automatically generates and inserts a corresponding counter-transaction into the `Transactions` ledger within an atomic database operation, ensuring wallet balance and historical cash flow stay in sync.
- Filter debt records by debt type and settlement status.

### 6. Interactive Monthly Dashboard

- Seamless month/year navigation bar (`[◀] Tháng MM/yyyy [▶] [Tháng Này]`).
- Instant recalculation of financial summary cards upon month switching:
  - **TỔNG THU (Total Income):** Aggregated earnings for the selected month.
  - **TỔNG CHI (Total Expense):** Aggregated spending for the selected month.
  - **CÒN LẠI (Net Balance):** Real-time monthly surplus/deficit (`Total Income - Total Expense`).
- Monthly recent transactions grid sorted descending by timestamp.
- **"Nhìn Lại" (Monthly Review) Panel:** Breakdown of active categories in the selected month with dual-color indicators:
  - *Income:* Emerald Green with leading `+` prefix (`+15,000,000 đ`).
  - *Expense:* Crimson Red with standard currency formatting (`2,000,000 đ`).

### 7. Financial Chart Analytics and Reports

- Powered by `WinForms.DataVisualization` with 3 analytical display modes:
  - **Mode 1 — Daily Comparison:** Dual-column bar chart comparing daily Income vs. Expense across all days in the selected month. Zero-value days are rendered cleanly without cluttering callout arrows.
  - **Mode 2 — 12-Month Yearly Trend:** Annual overview displaying Income and Expense columns for all 12 calendar months (`Tháng 1` to `Tháng 12`) with fixed category indexing (`IsXValueIndexed = true`).
  - **Mode 3 — Category Expense Share:** Interactive Pie Chart illustrating proportional spending distribution across categories with percentage slices and legends.
- Data export utility and on-demand refresh.

---

## Technology Stack

- **Language & Runtime:** C# 12, .NET 10 (Windows Desktop SDK)
- **UI Framework:** Windows Forms, Flat Modern Design, GDI+
- **Database:** PostgreSQL 16+ via `Npgsql 10.0.3` (ADO.NET Data Provider)
- **Visualization:** `WinForms.DataVisualization 1.10.2` (System.Windows.Forms.DataVisualization.Charting)
- **Email Service:** `System.Net.Mail` (SMTP Client with SSL/TLS encryption)
- **Architecture Pattern:** 3-Tier Layered Architecture (Presentation – BLL – DAL – DTO)

---

## Architecture and Project Structure

The solution strictly adheres to the 3-Tier architecture to decouple presentation, business logic, and data access concerns:

```text
quanlychitieu/
├── AuthForm.cs                 # Login and Register presentation form
├── Form1.cs                    # Main application shell with sidebar navigation
├── Program.cs                  # Application bootstrap entry point
│
├── DTO/                        # Data Transfer Objects
│   ├── UserDTO.cs              # User entity representation
│   ├── TransactionDTO.cs       # Transaction data model
│   ├── CategoryDTO.cs          # Category data model
│   ├── BudgetDTO.cs            # Budget threshold model
│   └── DebtDTO.cs              # Debt and loan entity model
│
├── DAL/                        # Data Access Layer (PostgreSQL parameterized SQL)
│   ├── DbConnection.cs         # Npgsql connection management and query execution
│   ├── UserDAL.cs              # User authentication and profile queries
│   ├── TransactionDAL.cs       # Transaction CRUD and filtering queries
│   ├── CategoryDAL.cs          # Category storage queries
│   ├── BudgetDAL.cs            # Budget upsert and threshold queries
│   ├── DebtDAL.cs              # Debt records and atomic settlement queries
│   ├── StatisticDAL.cs         # Aggregate queries for daily, yearly, and pie charts
│   └── DashboardDAL.cs         # Monthly overview cards and review queries
│
├── BLL/                        # Business Logic Layer (Validation & Rules)
│   ├── UserBLL.cs              # Auth logic, OTP generation & verification, profile
│   ├── TransactionBLL.cs       # Transaction business rules and filter routing
│   ├── CategoryBLL.cs          # Category business validation
│   ├── BudgetBLL.cs            # Budget limit calculation and 80%/100% alert engine
│   ├── DebtBLL.cs              # Loan validation and settlement workflows
│   ├── StatisticBLL.cs         # 12-month calendar and daily matrix aggregation
│   └── DashboardBLL.cs         # Monthly KPI calculation and category balance rollup
│
├── Views/                      # Presentation Layer (UserControls & Dialogs)
│   ├── TrangChuView.cs         # Overview dashboard with month picker & review panel
│   ├── NhapVaoView.cs          # Income/Expense entry with budget limit verification
│   ├── LichView.cs             # Financial transaction history and variance table
│   ├── ThongKeView.cs          # 3-Mode financial charts and reporting
│   ├── CategoriesView.cs       # Category management interface
│   ├── BudgetView.cs           # Monthly budget allocation and progress indicators
│   ├── DebtView.cs             # Debt and loan tracking interface
│   ├── UserProfileForm.cs      # User profile edit & password change dialog
│   ├── FrmForgotPassword.cs    # Email submission and OTP verification modal
│   └── FrmResetPassword.cs     # New password configuration modal
│
└── Services/                   # Cross-cutting Infrastructure Services
    └── EmailService.cs         # Asynchronous SMTP client for HTML OTP emails
```

---

## Database Schema

The database consists of 5 normalized relational tables in PostgreSQL:

| Table Name | Primary Key | Foreign Keys | Description |
| :--- | :--- | :--- | :--- |
| **`Users`** | `Id` (SERIAL) | *None* | User accounts, credentials, and profile attributes. |
| **`Categories`** | `Id` (SERIAL) | `UserId -> Users(Id)` | User-scoped income and expense category taxonomy. |
| **`Transactions`** | `Id` (SERIAL) | `UserId -> Users(Id)`, `CategoryId -> Categories(Id)` | Core ledger of income and expenditure entries. |
| **`Budgets`** | `BudgetId` (SERIAL) | `UserId -> Users(Id)`, `CategoryId -> Categories(Id)` | Category-specific monthly budget caps. |
| **`Debts`** | `DebtId` (SERIAL) | `UserId -> Users(Id)` | Personal debt and loan contracts with lifecycle status. |

---

## Requirements

- **Operating System:** Windows 10 or Windows 11 (64-bit)
- **Runtime / SDK:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (or .NET Desktop Runtime 10)
- **Database Server:** [PostgreSQL 14+](https://www.postgresql.org/download/) running on `localhost:5432` or accessible network host.

---

## Configuration

### 1. Database Connection

Configure your PostgreSQL credentials in [`DAL/DbConnection.cs`](file:///d:/APPLICATIONS/HTML/quanlychitieu/DAL/DbConnection.cs):

```csharp
private static string connectionString = 
    "Host=localhost;Port=5432;Database=quan_ly_chi_tieu;Username=postgres;Password=your_password;";
```

### 2. SMTP Email Configuration

For the **Forgot Password OTP** feature, set your Google App Password in [`Services/EmailService.cs`](file:///d:/APPLICATIONS/HTML/quanlychitieu/Services/EmailService.cs):

```csharp
private static readonly string SmtpHost = "smtp.gmail.com";
private static readonly int SmtpPort = 587;
private static readonly string SenderEmail = "your_email@gmail.com";
private static readonly string SenderPassword = "your_app_password"; // 16-character Google App Password
```

---

## Build and Run

From the repository root:

```powershell
# Restore dependencies and build the solution
dotnet build

# Launch the desktop application
dotnet run --project quanlycitieu.csproj
```

---

## Authors & Academic Details

- **Student:** Nguyễn Văn Mạnh
- **Student ID:** 2321050012
- **Class:** DCCTCT68_05B
- **Institution:** Hanoi University of Mining and Geology (HUMG) — Faculty of Information Technology, Department of Software Engineering
- **Course:** Lập trình .NET 1 + BTL
- **Instructor:** ThS. Ngô Hùng Long

---

## License

This project is developed for educational and academic purposes under the .NET Programming curriculum at HUMG.
