IF DB_ID(N'QLThuVien') IS NULL
BEGIN
    CREATE DATABASE QLThuVien;
END
GO

USE QLThuVien;
GO

IF OBJECT_ID(N'dbo.tblPhieuMuonChiTiet', N'U') IS NOT NULL DROP TABLE dbo.tblPhieuMuonChiTiet;
IF OBJECT_ID(N'dbo.tblPhieuMuon', N'U') IS NOT NULL DROP TABLE dbo.tblPhieuMuon;
IF OBJECT_ID(N'dbo.tblSach', N'U') IS NOT NULL DROP TABLE dbo.tblSach;
IF OBJECT_ID(N'dbo.tblTheLoai', N'U') IS NOT NULL DROP TABLE dbo.tblTheLoai;
IF OBJECT_ID(N'dbo.tblSinhVien', N'U') IS NOT NULL DROP TABLE dbo.tblSinhVien;
IF OBJECT_ID(N'dbo.tblThuThu', N'U') IS NOT NULL DROP TABLE dbo.tblThuThu;
GO

CREATE TABLE dbo.tblTheLoai
(
    MaLoai  INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_tblTheLoai PRIMARY KEY,
    TenLoai NVARCHAR(100) NOT NULL CONSTRAINT UQ_tblTheLoai_TenLoai UNIQUE
);

CREATE TABLE dbo.tblSach
(
    MaSach  INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_tblSach PRIMARY KEY,
    TenSach NVARCHAR(200) NOT NULL,
    SoLuong INT NOT NULL CONSTRAINT CK_tblSach_SoLuong CHECK (SoLuong >= 0),
    MaLoai  INT NOT NULL,
    CONSTRAINT FK_tblSach_tblTheLoai FOREIGN KEY (MaLoai)
        REFERENCES dbo.tblTheLoai(MaLoai)
);

CREATE TABLE dbo.tblSinhVien
(
    MaSV  INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_tblSinhVien PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    Lop   NVARCHAR(50) NULL
);

CREATE TABLE dbo.tblThuThu
(
    MaThuThu    INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_tblThuThu PRIMARY KEY,
    HoTen       NVARCHAR(100) NOT NULL,
    TenDangNhap NVARCHAR(50) NOT NULL CONSTRAINT UQ_tblThuThu_TenDangNhap UNIQUE,
    MatKhau     NVARCHAR(100) NOT NULL,
    Quyen       NVARCHAR(20) NOT NULL CONSTRAINT DF_tblThuThu_Quyen DEFAULT N'ThuThu',
    CONSTRAINT CK_tblThuThu_Quyen CHECK (Quyen IN (N'Admin', N'ThuThu'))
);

CREATE TABLE dbo.tblPhieuMuon
(
    MaPhieu  INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_tblPhieuMuon PRIMARY KEY,
    MaSV     INT NOT NULL,
    MaThuThu INT NOT NULL,
    NgayMuon DATE NOT NULL CONSTRAINT DF_tblPhieuMuon_NgayMuon DEFAULT CONVERT(date, GETDATE()),
    CONSTRAINT FK_tblPhieuMuon_tblSinhVien FOREIGN KEY (MaSV)
        REFERENCES dbo.tblSinhVien(MaSV),
    CONSTRAINT FK_tblPhieuMuon_tblThuThu FOREIGN KEY (MaThuThu)
        REFERENCES dbo.tblThuThu(MaThuThu)
);

CREATE TABLE dbo.tblPhieuMuonChiTiet
(
    MaPhieu  INT NOT NULL,
    MaSach   INT NOT NULL,
    NgayMuon DATE NOT NULL CONSTRAINT DF_tblPhieuMuonChiTiet_NgayMuon DEFAULT CONVERT(date, GETDATE()),
    NgayTra  DATE NULL,
    CONSTRAINT PK_tblPhieuMuonChiTiet PRIMARY KEY (MaPhieu, MaSach),
    CONSTRAINT FK_tblPhieuMuonChiTiet_tblPhieuMuon FOREIGN KEY (MaPhieu)
        REFERENCES dbo.tblPhieuMuon(MaPhieu),
    CONSTRAINT FK_tblPhieuMuonChiTiet_tblSach FOREIGN KEY (MaSach)
        REFERENCES dbo.tblSach(MaSach)
);
GO

CREATE OR ALTER PROCEDURE dbo.sp_TheLoai_Them
    @p_TenLoai NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.tblTheLoai(TenLoai) VALUES (@p_TenLoai);
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_TheLoai_Sua
    @p_MaLoai INT,
    @p_TenLoai NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.tblTheLoai SET TenLoai = @p_TenLoai WHERE MaLoai = @p_MaLoai;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_TheLoai_Xoa
    @p_MaLoai INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.tblTheLoai WHERE MaLoai = @p_MaLoai;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Sach_Them
    @p_TenSach NVARCHAR(200),
    @p_MaLoai INT,
    @p_SoLuong INT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.tblSach(TenSach, MaLoai, SoLuong)
    VALUES (@p_TenSach, @p_MaLoai, @p_SoLuong);
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Sach_Sua
    @p_MaSach INT,
    @p_TenSach NVARCHAR(200),
    @p_MaLoai INT,
    @p_SoLuong INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.tblSach
    SET TenSach = @p_TenSach,
        MaLoai = @p_MaLoai,
        SoLuong = @p_SoLuong
    WHERE MaSach = @p_MaSach;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Sach_Xoa
    @p_MaSach INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.tblSach WHERE MaSach = @p_MaSach;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_SinhVien_Them
    @p_HoTen NVARCHAR(100),
    @p_Lop NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.tblSinhVien(HoTen, Lop) VALUES (@p_HoTen, @p_Lop);
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_SinhVien_Sua
    @p_MaSV INT,
    @p_HoTen NVARCHAR(100),
    @p_Lop NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.tblSinhVien SET HoTen = @p_HoTen, Lop = @p_Lop WHERE MaSV = @p_MaSV;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_SinhVien_Xoa
    @p_MaSV INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.tblSinhVien WHERE MaSV = @p_MaSV;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_ThuThu_Them
    @p_HoTen NVARCHAR(100),
    @p_TenDangNhap NVARCHAR(50),
    @p_MatKhau NVARCHAR(100),
    @p_Quyen NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.tblThuThu(HoTen, TenDangNhap, MatKhau, Quyen)
    VALUES (@p_HoTen, @p_TenDangNhap, @p_MatKhau, @p_Quyen);
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_ThuThu_Sua
    @p_MaThuThu INT,
    @p_HoTen NVARCHAR(100),
    @p_TenDangNhap NVARCHAR(50),
    @p_MatKhau NVARCHAR(100) = NULL,
    @p_Quyen NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.tblThuThu
    SET HoTen = @p_HoTen,
        TenDangNhap = @p_TenDangNhap,
        MatKhau = ISNULL(NULLIF(@p_MatKhau, N''), MatKhau),
        Quyen = @p_Quyen
    WHERE MaThuThu = @p_MaThuThu;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_ThuThu_Xoa
    @p_MaThuThu INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.tblThuThu WHERE MaThuThu = @p_MaThuThu;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_LapPhieuMuon
    @p_MaSV INT,
    @p_MaThuThu INT,
    @p_MaPhieuMoi INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.tblPhieuMuon(MaSV, MaThuThu, NgayMuon)
    VALUES (@p_MaSV, @p_MaThuThu, CONVERT(date, GETDATE()));

    SET @p_MaPhieuMoi = CONVERT(INT, SCOPE_IDENTITY());
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_MuonSach
    @p_MaPhieu INT,
    @p_MaSach INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @v_Tong INT;
    DECLARE @v_DangMuon INT;
    DECLARE @v_ConLai INT;

    SELECT @v_Tong = SoLuong FROM dbo.tblSach WHERE MaSach = @p_MaSach;

    IF @v_Tong IS NULL
    BEGIN
        RAISERROR(N'Sach khong ton tai.', 16, 1);
        RETURN;
    END

    SELECT @v_DangMuon = COUNT(*)
    FROM dbo.tblPhieuMuonChiTiet
    WHERE MaSach = @p_MaSach AND NgayTra IS NULL;

    SET @v_ConLai = @v_Tong - @v_DangMuon;

    IF @v_ConLai <= 0
    BEGIN
        RAISERROR(N'Sach da het, khong the cho muon.', 16, 1);
        RETURN;
    END

    IF EXISTS (
        SELECT 1
        FROM dbo.tblPhieuMuonChiTiet
        WHERE MaPhieu = @p_MaPhieu AND MaSach = @p_MaSach
    )
    BEGIN
        RAISERROR(N'Sach nay da co trong phieu muon.', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.tblPhieuMuonChiTiet(MaPhieu, MaSach, NgayMuon, NgayTra)
    VALUES (@p_MaPhieu, @p_MaSach, CONVERT(date, GETDATE()), NULL);
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_TraSach
    @p_MaPhieu INT,
    @p_MaSach INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.tblPhieuMuonChiTiet
    SET NgayTra = CONVERT(date, GETDATE())
    WHERE MaPhieu = @p_MaPhieu
      AND MaSach = @p_MaSach
      AND NgayTra IS NULL;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_ThongKeSachConLai
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        s.MaSach,
        s.TenSach,
        tl.TenLoai,
        s.SoLuong AS TongSoLuong,
        s.SoLuong - ISNULL(dm.DangMuon, 0) AS ConLai
    FROM dbo.tblSach s
    INNER JOIN dbo.tblTheLoai tl ON s.MaLoai = tl.MaLoai
    LEFT JOIN (
        SELECT MaSach, COUNT(*) AS DangMuon
        FROM dbo.tblPhieuMuonChiTiet
        WHERE NgayTra IS NULL
        GROUP BY MaSach
    ) dm ON s.MaSach = dm.MaSach
    ORDER BY s.MaSach;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_PhieuMuonChuaTra
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        pm.MaPhieu,
        s.MaSach,
        sv.HoTen AS SinhVien,
        s.TenSach,
        ct.NgayMuon
    FROM dbo.tblPhieuMuonChiTiet ct
    INNER JOIN dbo.tblPhieuMuon pm ON ct.MaPhieu = pm.MaPhieu
    INNER JOIN dbo.tblSinhVien sv ON pm.MaSV = sv.MaSV
    INNER JOIN dbo.tblSach s ON ct.MaSach = s.MaSach
    WHERE ct.NgayTra IS NULL
    ORDER BY ct.NgayMuon, pm.MaPhieu;
END
GO

INSERT INTO dbo.tblTheLoai(TenLoai) VALUES
(N'Cong nghe thong tin'),
(N'Van hoc'),
(N'Kinh te');

INSERT INTO dbo.tblSach(TenSach, MaLoai, SoLuong) VALUES
(N'Lap trinh C# co ban', 1, 3),
(N'Co so du lieu SQL Server', 1, 2),
(N'Nha gia kim', 2, 5);

INSERT INTO dbo.tblSinhVien(HoTen, Lop) VALUES
(N'Nguyen Van An', N'CNTT01'),
(N'Tran Thi Binh', N'CNTT02');

INSERT INTO dbo.tblThuThu(HoTen, TenDangNhap, MatKhau, Quyen) VALUES
(N'Le Quan Tri', N'admin', N'123456', N'Admin');
GO
