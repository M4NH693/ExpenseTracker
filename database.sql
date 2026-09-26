-- ============================================================
-- QUẢN LÝ CHI TIÊU - Database Schema v2.0
-- PostgreSQL >= 12
-- Thiết kế: Ràng buộc chuyên nghiệp, chuẩn doanh nghiệp
-- ============================================================

-- Xóa bảng cũ theo thứ tự phụ thuộc (con trước, cha sau)
DROP TABLE IF EXISTS Transactions CASCADE;
DROP TABLE IF EXISTS Categories  CASCADE;
DROP TABLE IF EXISTS Users       CASCADE;

-- ============================================================
-- 1. BẢNG USERS (Người dùng)
-- ============================================================
CREATE TABLE Users (
    Id          SERIAL PRIMARY KEY,
    FullName    VARCHAR(100) NOT NULL,
    Gender      VARCHAR(10)  NOT NULL CHECK (Gender IN ('Nam', 'Nữ', 'Khác')),
    Dob         DATE         NOT NULL CHECK (Dob <= CURRENT_DATE),
    Email       VARCHAR(100) UNIQUE NOT NULL,
    Address     TEXT         DEFAULT '',
    Password    VARCHAR(255) NOT NULL,
    CreatedAt   TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Index tăng tốc tìm kiếm theo Email khi đăng nhập
CREATE INDEX idx_users_email ON Users (Email);

-- ============================================================
-- 2. BẢNG CATEGORIES (Danh mục)
-- ============================================================
-- UserId = NULL → Danh mục mặc định hệ thống (mọi user đều thấy)
-- UserId = N   → Danh mục riêng của user N
CREATE TABLE Categories (
    Id      SERIAL PRIMARY KEY,
    UserId  INT          REFERENCES Users(Id) ON DELETE CASCADE,
    Name    VARCHAR(100) NOT NULL,
    Type    VARCHAR(20)  NOT NULL CHECK (Type IN ('Thu Nhập', 'Chi Tiêu')),

    -- Mỗi user không được tạo 2 danh mục trùng tên cùng loại
    CONSTRAINT uq_category_per_user UNIQUE (UserId, Name, Type)
);

-- Index tăng tốc truy vấn danh mục theo user + loại
CREATE INDEX idx_categories_user_type ON Categories (UserId, Type);

-- ============================================================
-- 3. BẢNG TRANSACTIONS (Giao dịch)
-- ============================================================
CREATE TABLE Transactions (
    Id          SERIAL PRIMARY KEY,
    UserId      INT          NOT NULL REFERENCES Users(Id)      ON DELETE CASCADE,
    CategoryId  INT          REFERENCES Categories(Id)          ON DELETE CASCADE,
    Type        VARCHAR(20)  NOT NULL CHECK (Type IN ('Thu Nhập', 'Chi Tiêu')),
    Amount      DECIMAL(18,2) NOT NULL CHECK (Amount <> 0),
    Date        TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description TEXT         DEFAULT '',
    CreatedAt   TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Index tăng tốc dashboard (lọc theo user + ngày, sắp xếp mới nhất)
CREATE INDEX idx_transactions_user_date ON Transactions (UserId, Date DESC);
-- Index tăng tốc thống kê theo danh mục
CREATE INDEX idx_transactions_category  ON Transactions (CategoryId);
-- Index tăng tốc tính tổng thu/chi
CREATE INDEX idx_transactions_user_type ON Transactions (UserId, Type);

-- ============================================================
-- 4. DỮ LIỆU KHỞI TẠO - Danh mục mặc định hệ thống
-- ============================================================
INSERT INTO Categories (Name, Type, UserId) VALUES
    ('Lương',        'Thu Nhập', NULL),
    ('Lãi Ngân Hàng','Thu Nhập', NULL),
    ('Thưởng',       'Thu Nhập', NULL),
    ('Đi Lại',       'Chi Tiêu', NULL),
    ('Ăn Uống',      'Chi Tiêu', NULL),
    ('Bạn Bè',       'Chi Tiêu', NULL),
    ('Mua Sắm',      'Chi Tiêu', NULL),
    ('Tiền Phòng',   'Chi Tiêu', NULL),
    ('Y Tế',         'Chi Tiêu', NULL),
    ('Giáo Dục',     'Chi Tiêu', NULL);
