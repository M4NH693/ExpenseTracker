# ExpenseTracker — Hệ thống quản lý chi tiêu và tài chính cá nhân

[English](README.md) | [Tiếng Việt](README_VI.md)

![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16.0-4169E1?logo=postgresql)
![Platform](https://img.shields.io/badge/Platform-Windows-0078D6?logo=windows)
![Architecture](https://img.shields.io/badge/Architecture-3--Tier%20Layered-0F4761)

ExpenseTracker (Quản lý chi tiêu) là ứng dụng desktop Windows Forms xây dựng trên nền tảng .NET 10, phục vụ quản lý toàn diện tài chính cá nhân, phân loại thu chi, thiết lập hạn mức ngân sách tháng kèm cảnh báo tự động, theo dõi sổ vay nợ đối ứng và phân tích báo cáo bằng biểu đồ đa chiều. Ứng dụng được thiết kế chuẩn mực theo mô hình kiến trúc 3 lớp (3-Tier Architecture: DTO – DAL – BLL – GUI), toàn bộ dữ liệu vận hành được lưu trữ an toàn trong cơ sở dữ liệu quan hệ PostgreSQL với các truy vấn tham số hóa (Parameterized Queries) và ràng buộc toàn vẹn nghiêm ngặt.

---

## Tính năng

### 1. Quản lý tài khoản và bảo mật

- Đăng ký và đăng nhập tài khoản bằng email, họ và tên, ngày sinh, giới tính và địa chỉ.
- Mã hóa mật khẩu bảo mật và cơ chế xác thực phiên làm việc.
- **Quên mật khẩu qua mã OTP gửi về Email:**
  - Tự động sinh mã OTP ngẫu nhiên 6 chữ số kèm bộ đếm thời gian hết hạn sau 5 phút.
  - Gửi email HTML bất đồng bộ qua máy chủ SMTP của Google (`smtp.gmail.com:587`, mã hóa SSL/TLS) sử dụng `System.Net.Mail` kèm nút đếm ngược 60 giây chống spam.
  - Xác thực OTP 2 bước và chuyển sang form đặt lại mật khẩu mới an toàn.
- Quản lý hồ sơ cá nhân: cập nhật thông tin người dùng và quy trình đổi mật khẩu trực tiếp trong ứng dụng.
- Lưu trữ ngữ cảnh phiên người dùng toàn cục qua biến tĩnh an toàn (`AuthForm.CurrentUserId`).

### 2. Quản lý giao dịch thu và chi

- Ghi nhận chi tiết các khoản thu nhập và chi tiêu phát sinh với số tiền, ngày giao dịch, ghi chú và danh mục tương ứng.
- Tự động định dạng dấu phân cách hàng nghìn theo thời gian thực (`N0`, ví dụ: `1,500,000 đ`) ngay khi gõ số tiền.
- Hộp chọn danh mục linh hoạt: tự động lọc danh mục tương ứng theo loại giao dịch (*Thu Nhập* hoặc *Chi Tiêu*).
- **Lịch sử giao dịch & Biến động số dư (Màn hình Lịch):**
  - Tìm kiếm nhanh theo nội dung ghi chú giao dịch.
  - Bộ lọc giao dịch theo loại (*Tất cả*, *Thu Nhập*, *Chi Tiêu*).
  - Thứ tự hiển thị cột rõ ràng: `[Loại] | [Danh Mục] | [Thời Gian]`.
  - Tô màu trực quan: Màu xanh lá (Emerald Green) cho khoản Thu Nhập, Màu đỏ (Crimson Red) cho khoản Chi Tiêu.
  - Hỗ trợ chỉnh sửa thông tin hoặc xóa giao dịch kèm hộp thoại xác nhận.

### 3. Quản lý danh mục thu chi

- Phân loại rõ ràng giữa danh mục Thu Nhập (Lương, Thưởng, Tiền phụ cấp, Đầu tư,...) và danh mục Chi Tiêu (Ăn uống, Nhà ở, Đi lại, Mua sắm, Y tế, Giải trí,...).
- Đầy đủ thao tác CRUD: Thêm danh mục mới, chỉnh sửa tên/loại danh mục và xóa danh mục không còn nhu cầu sử dụng.
- Tách biệt dữ liệu theo từng người dùng: Mỗi tài khoản tự quản lý cây danh mục độc lập của riêng mình.

### 4. Thiết lập ngân sách và cảnh báo chi tiêu

- Đặt hạn mức chi tiêu tối đa cho từng danh mục chi tiêu theo tháng và năm (ví dụ: *Ăn uống* tối đa `3,000,000 đ` trong tháng 10/2026).
- Theo dõi tiến độ sử dụng ngân sách qua bảng dữ liệu và thanh tiến độ ProgressBar có màu sắc nhận diện:
  - **An toàn (< 80%):** Hiển thị thanh tiến độ bình thường.
  - **Cảnh báo vàng (80% – 100%):** Nhắc nhở người dùng sắp chạm hạn mức chi tiêu.
  - **Cảnh báo đỏ vượt mức (> 100%):** Xuất hiện hộp thoại MessageBox cảnh báo khẩn cấp, yêu cầu người dùng xác nhận trước khi lưu bất kỳ giao dịch chi tiêu nào làm vượt quá hạn mức ngân sách tháng.
- Tự động tính toán số tiền còn lại có thể chi tiêu trong chu kỳ ngân sách hiện hành.

### 5. Quản lý khoản vay và sổ nợ (Debts & Loans)

- Theo dõi danh sách công nợ cá nhân: *Tôi Nợ* (khoản nợ phải trả) và *Cho Vay* (khoản nợ phải thu).
- Quản lý tên đối tác, số tiền, ngày hẹn trả, ghi chú mục đích và trạng thái (*Đang Nợ* / *Đã Trả*).
- **Tất toán nợ nguyên tử (`SettleDebtWithTransaction`):**
  - Đổi trạng thái bản ghi nợ sang *Đã Trả*.
  - Tự động sinh và chèn một giao dịch đối ứng vào bảng `Transactions` trong cùng một giao dịch cơ sở dữ liệu, đảm bảo số dư ví và lịch sử dòng tiền luôn đồng bộ và chính xác.
- Lọc danh sách nợ theo phân loại (*Tôi Nợ* / *Cho Vay*) và trạng thái thanh toán.

### 6. Màn hình Tổng quan Dashboard theo tháng

- Thanh điều hướng Tháng/Năm trực quan: `[◀] Tháng MM/yyyy [▶] [Tháng Này]`.
- Tự động tải lại tức thì 3 thẻ tài chính quan trọng khi chuyển đổi tháng:
  - **TỔNG THU:** Tổng thu nhập tích lũy trong tháng đang chọn.
  - **TỔNG CHI:** Tổng chi tiêu tích lũy trong tháng đang chọn.
  - **CÒN LẠI:** Số dư ròng của tháng (`Tổng Thu - Tổng Chi`).
- Bảng danh sách các giao dịch gần đây trong tháng, sắp xếp giảm dần theo thời gian.
- **Cột "Nhìn Lại" (Monthly Review):** Phân tích chi tiết từng danh mục có phát sinh thu chi trong tháng với phân loại màu sắc:
  - *Thu Nhập:* Màu xanh lá kèm dấu `+` phía trước (`+15,000,000 đ`).
  - *Chi Tiêu:* Màu đỏ theo định dạng tiền tệ chuẩn (`2,000,000 đ`).

### 7. Báo cáo thống kê biểu đồ tài chính

- Tích hợp thư viện `WinForms.DataVisualization` với 3 chế độ xem biểu đồ chuyên nghiệp:
  - **Chế độ 1 — So sánh theo ngày:** Biểu đồ cột so sánh hai luồng Thu Nhập và Chi Tiêu của từng ngày trong tháng. Xử lý triệt để các điểm $0$ đồng, không gây rối mắt bởi các mũi tên callout.
  - **Chế độ 2 — Xu hướng 12 tháng:** Biểu đồ cột tổng thể cả 12 tháng trong năm (`Tháng 1` đến `Tháng 12`) với cơ chế đánh chỉ mục cố định (`IsXValueIndexed = true`), hiển thị thông thoáng và không bị co cụm dữ liệu.
  - **Chế độ 3 — Cơ cấu chi tiêu:** Biểu đồ hình tròn (Pie Chart) thể hiện tỷ trọng phần trăm chi tiêu của từng danh mục kèm lát cắt màu sắc và chú giải (Legend).

---

## Công nghệ sử dụng

- **Ngôn ngữ & Nền tảng:** C# 12, .NET 10 (Windows Desktop SDK)
- **Giao diện người dùng:** Windows Forms, phong cách Flat Design, GDI+
- **Cơ sở dữ liệu:** PostgreSQL 16+ kết nối qua `Npgsql 10.0.3` (ADO.NET Data Provider)
- **Thư viện đồ họa biểu đồ:** `WinForms.DataVisualization 1.10.2` (System.Windows.Forms.DataVisualization.Charting)
- **Dịch vụ thư điện tử:** `System.Net.Mail` (SMTP Client mã hóa bảo mật SSL/TLS)
- **Mô hình kiến trúc:** Kiến trúc 3 lớp (Presentation – BLL – DAL – DTO)

---

## Kiến trúc và Cấu trúc thư mục dự án

Dự án được tổ chức chặt chẽ theo mô hình 3 lớp (3-Tier Architecture) giúp phân tách rõ ràng trách nhiệm giữa giao diện, nghiệp vụ và truy xuất dữ liệu:

```text
quanlychitieu/
├── AuthForm.cs                 # Form đăng nhập và đăng ký tài khoản
├── Form1.cs                    # Form khung chính chứa sidebar điều hướng
├── Program.cs                  # Điểm khởi chạy ứng dụng
│
├── DTO/                        # Data Transfer Objects (Đối tượng truyền dữ liệu)
│   ├── UserDTO.cs              # Thực thể người dùng
│   ├── TransactionDTO.cs       # Thực thể giao dịch thu chi
│   ├── CategoryDTO.cs          # Thực thể danh mục
│   ├── BudgetDTO.cs            # Thực thể hạn mức ngân sách
│   └── DebtDTO.cs              # Thực thể khoản nợ và cho vay
│
├── DAL/                        # Data Access Layer (Tầng truy cập dữ liệu PostgreSQL)
│   ├── DbConnection.cs         # Quản lý kết nối Npgsql và thực thi SQL Parameterized
│   ├── UserDAL.cs              # Truy vấn xác thực và thông tin người dùng
│   ├── TransactionDAL.cs       # Thao tác CRUD và lọc lịch sử giao dịch
│   ├── CategoryDAL.cs          # Thao tác CRUD bảng danh mục
│   ├── BudgetDAL.cs            # Truy vấn lưu và kiểm tra hạn mức ngân sách
│   ├── DebtDAL.cs              # Thao tác CRUD khoản nợ và tất toán nợ
│   ├── StatisticDAL.cs         # Truy vấn tổng hợp số liệu biểu đồ ngày, năm, tròn
│   └── DashboardDAL.cs         # Truy vấn số liệu thẻ tổng quan và cột Nhìn Lại
│
├── BLL/                        # Business Logic Layer (Tầng xử lý logic nghiệp vụ)
│   ├── UserBLL.cs              # Logic xác thực, sinh và kiểm tra OTP, hồ sơ
│   ├── TransactionBLL.cs       # Logic kiểm tra dữ liệu và phân luồng giao dịch
│   ├── CategoryBLL.cs          # Logic nghiệp vụ danh mục
│   ├── BudgetBLL.cs            # Thuật toán kiểm tra ngưỡng cảnh báo ngân sách 80%/100%
│   ├── DebtBLL.cs              # Nghiệp vụ xử lý công nợ và tất toán dòng tiền
│   ├── StatisticBLL.cs         # Logic tổng hợp ma trận thống kê 12 tháng và theo ngày
│   └── DashboardBLL.cs         # Tính toán tổng hợp KPI tháng và số dư
│
├── Views/                      # Presentation Layer (Tầng giao diện người dùng)
│   ├── TrangChuView.cs         # Màn hình Tổng quan Dashboard và cột Nhìn Lại
│   ├── NhapVaoView.cs          # Màn hình nhập giao dịch kèm cảnh báo ngân sách
│   ├── LichView.cs             # Màn hình lịch sử biến động số dư
│   ├── ThongKeView.cs          # Màn hình báo cáo biểu đồ 3 chế độ
│   ├── CategoriesView.cs       # Màn hình quản lý danh mục thu chi
│   ├── BudgetView.cs           # Màn hình thiết lập ngân sách và tiến độ
│   ├── DebtView.cs             # Màn hình quản lý sổ nợ và cho vay
│   ├── UserProfileForm.cs      # Form xem hồ sơ cá nhân và đổi mật khẩu
│   ├── FrmForgotPassword.cs    # Form nhập email gửi OTP khôi phục mật khẩu
│   └── FrmResetPassword.cs     # Form đặt lại mật khẩu mới
│
└── Services/                   # Dịch vụ hạ tầng dùng chung
    └── EmailService.cs         # Service gửi email OTP qua giao thức Google SMTP
```

---

## Thiết kế cơ sở dữ liệu

Cơ sở dữ liệu gồm 5 bảng quan hệ được chuẩn hóa trong PostgreSQL:

| Tên bảng | Khóa chính | Khóa ngoại | Mô tả nghiệp vụ |
| :--- | :--- | :--- | :--- |
| **`Users`** | `Id` (SERIAL) | *Không có* | Lưu thông tin định danh, tài khoản và mật khẩu người dùng. |
| **`Categories`** | `Id` (SERIAL) | `UserId -> Users(Id)` | Phân loại danh mục Thu Nhập và Chi Tiêu theo từng người dùng. |
| **`Transactions`** | `Id` (SERIAL) | `UserId -> Users(Id)`, `CategoryId -> Categories(Id)` | Lưu trữ toàn bộ các giao dịch dòng tiền thực tế phát sinh. |
| **`Budgets`** | `BudgetId` (SERIAL) | `UserId -> Users(Id)`, `CategoryId -> Categories(Id)` | Lưu hạn mức chi tiêu tối đa theo danh mục cho từng tháng/năm. |
| **`Debts`** | `DebtId` (SERIAL) | `UserId -> Users(Id)` | Quản lý các hợp đồng vay nợ và cho vay cá nhân. |

---

## Yêu cầu môi trường

- **Hệ điều hành:** Windows 10 hoặc Windows 11 (64-bit)
- **Môi trường phát triển / Runtime:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (hoặc .NET Desktop Runtime 10)
- **Hệ quản trị CSDL:** [PostgreSQL 14+](https://www.postgresql.org/download/) chạy trên `localhost:5432` hoặc máy chủ từ xa.

---

## Hướng dẫn cấu hình

### 1. Cấu hình kết nối cơ sở dữ liệu

Mở file [`DAL/DbConnection.cs`](file:///d:/APPLICATIONS/HTML/quanlychitieu/DAL/DbConnection.cs) và cập nhật thông tin tài khoản PostgreSQL của bạn:

```csharp
private static string connectionString = 
    "Host=localhost;Port=5432;Database=quan_ly_chi_tieu;Username=postgres;Password=mật_khẩu_của_bạn;";
```

### 2. Cấu hình gửi thư điện tử SMTP (Quên mật khẩu)

Để sử dụng tính năng **Quên mật khẩu qua OTP Gmail**, thiết lập Mật khẩu ứng dụng (App Password) trong [`Services/EmailService.cs`](file:///d:/APPLICATIONS/HTML/quanlychitieu/Services/EmailService.cs):

```csharp
private static readonly string SmtpHost = "smtp.gmail.com";
private static readonly int SmtpPort = 587;
private static readonly string SenderEmail = "email_cua_ban@gmail.com";
private static readonly string SenderPassword = "mat_khau_ung_dung_16_ky_tu"; // Tạo tại Google Account -> App Passwords
```

---

## Build và chạy ứng dụng

Tại thư mục gốc của repository, mở PowerShell hoặc Command Prompt và chạy:

```powershell
# Khôi phục các gói thư viện và biên dịch dự án
dotnet build

# Khởi chạy ứng dụng
dotnet run --project quanlycitieu.csproj
```


---

## Giấy phép (License)

Dự án được thực hiện phục vụ mục đích học tập và nghiên cứu trong khuôn khổ học phần Lập trình .NET 1 tại HUMG.
