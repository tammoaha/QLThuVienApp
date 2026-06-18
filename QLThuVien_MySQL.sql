
/* ---------- Tạo database ---------- */
CREATE DATABASE QLThuVien
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;
USE QLThuVien;

/* =========================================================
   1. TẠO CÁC BẢNG
   ========================================================= */

/* 1.1 Thể loại sách */
CREATE TABLE tblTheLoai (
    MaLoai      INT AUTO_INCREMENT PRIMARY KEY,
    TenLoai     VARCHAR(100) NOT NULL
);

/* 1.2 Sách */
CREATE TABLE tblSach (
    MaSach      INT AUTO_INCREMENT PRIMARY KEY,
    TenSach     VARCHAR(200) NOT NULL,
    MaLoai      INT NOT NULL,
    SoLuong     INT NOT NULL,
    CONSTRAINT CK_Sach_SoLuong CHECK (SoLuong >= 0),
    CONSTRAINT FK_Sach_TheLoai FOREIGN KEY (MaLoai)
        REFERENCES tblTheLoai(MaLoai)
);

/* 1.3 Sinh viên */
CREATE TABLE tblSinhVien (
    MaSV        INT AUTO_INCREMENT PRIMARY KEY,
    HoTen       VARCHAR(100) NOT NULL,
    Lop         VARCHAR(50)  NULL
);

/* 1.4 Thủ thư */
CREATE TABLE tblThuThu (
    MaThuThu    INT AUTO_INCREMENT PRIMARY KEY,
    HoTen       VARCHAR(100) NOT NULL,
    TenDangNhap VARCHAR(50)  NOT NULL UNIQUE,
    MatKhau     VARCHAR(100) NOT NULL
);

/* 1.5 Phiếu mượn  */
CREATE TABLE tblPhieuMuon (
    MaPhieu     INT AUTO_INCREMENT PRIMARY KEY,
    MaSV        INT NOT NULL,
    MaThuThu    INT NOT NULL,
    NgayMuon    DATE NOT NULL DEFAULT (CURRENT_DATE),
    CONSTRAINT FK_Phieu_SinhVien FOREIGN KEY (MaSV)
        REFERENCES tblSinhVien(MaSV),
    CONSTRAINT FK_Phieu_ThuThu FOREIGN KEY (MaThuThu)
        REFERENCES tblThuThu(MaThuThu)
);

/* 1.6 Chi tiết phiếu mượn  */
CREATE TABLE tblPhieuMuonChiTiet (
    MaPhieu     INT NOT NULL,
    MaSach      INT NOT NULL,
    NgayTra     DATE NULL,
    CONSTRAINT PK_ChiTiet PRIMARY KEY (MaPhieu, MaSach),
    CONSTRAINT FK_ChiTiet_Phieu FOREIGN KEY (MaPhieu)
        REFERENCES tblPhieuMuon(MaPhieu),
    CONSTRAINT FK_ChiTiet_Sach FOREIGN KEY (MaSach)
        REFERENCES tblSach(MaSach)
);

/* =========================================================
   2. STORED PROCEDURE 
   ========================================================= */
DELIMITER //

/* 2.1 Thêm sách */
CREATE PROCEDURE sp_Sach_Them(
    IN p_TenSach VARCHAR(200),
    IN p_MaLoai  INT,
    IN p_SoLuong INT
)
BEGIN
    INSERT INTO tblSach(TenSach, MaLoai, SoLuong)
    VALUES (p_TenSach, p_MaLoai, p_SoLuong);
END //

/* 2.2 Sửa sách */
CREATE PROCEDURE sp_Sach_Sua(
    IN p_MaSach  INT,
    IN p_TenSach VARCHAR(200),
    IN p_MaLoai  INT,
    IN p_SoLuong INT
)
BEGIN
    UPDATE tblSach
    SET TenSach = p_TenSach,
        MaLoai  = p_MaLoai,
        SoLuong = p_SoLuong
    WHERE MaSach = p_MaSach;
END //

/* 2.3 Xóa sách */
CREATE PROCEDURE sp_Sach_Xoa(
    IN p_MaSach INT
)
BEGIN
    DELETE FROM tblSach WHERE MaSach = p_MaSach;
END //

/* ----- Thể loại ----- */
CREATE PROCEDURE sp_TheLoai_Them(IN p_TenLoai VARCHAR(100))
BEGIN
    INSERT INTO tblTheLoai(TenLoai) VALUES (p_TenLoai);
END //

CREATE PROCEDURE sp_TheLoai_Sua(IN p_MaLoai INT, IN p_TenLoai VARCHAR(100))
BEGIN
    UPDATE tblTheLoai SET TenLoai = p_TenLoai WHERE MaLoai = p_MaLoai;
END //

CREATE PROCEDURE sp_TheLoai_Xoa(IN p_MaLoai INT)
BEGIN
    DELETE FROM tblTheLoai WHERE MaLoai = p_MaLoai;
END //

/* ----- Sinh viên ----- */
CREATE PROCEDURE sp_SinhVien_Them(IN p_HoTen VARCHAR(100), IN p_Lop VARCHAR(50))
BEGIN
    INSERT INTO tblSinhVien(HoTen, Lop) VALUES (p_HoTen, p_Lop);
END //

CREATE PROCEDURE sp_SinhVien_Sua(IN p_MaSV INT, IN p_HoTen VARCHAR(100), IN p_Lop VARCHAR(50))
BEGIN
    UPDATE tblSinhVien SET HoTen = p_HoTen, Lop = p_Lop WHERE MaSV = p_MaSV;
END //

CREATE PROCEDURE sp_SinhVien_Xoa(IN p_MaSV INT)
BEGIN
    DELETE FROM tblSinhVien WHERE MaSV = p_MaSV;
END //

/* ----- Thủ thư ----- */
CREATE PROCEDURE sp_ThuThu_Them(
    IN p_HoTen VARCHAR(100), IN p_TenDangNhap VARCHAR(50),
    IN p_MatKhau VARCHAR(100))
BEGIN
    INSERT INTO tblThuThu(HoTen, TenDangNhap, MatKhau)
    VALUES (p_HoTen, p_TenDangNhap, p_MatKhau);
END //

CREATE PROCEDURE sp_ThuThu_Sua(
    IN p_MaThuThu INT, IN p_HoTen VARCHAR(100), IN p_TenDangNhap VARCHAR(50),
    IN p_MatKhau VARCHAR(100))
BEGIN
    UPDATE tblThuThu
    SET HoTen = p_HoTen, TenDangNhap = p_TenDangNhap, MatKhau = p_MatKhau
    WHERE MaThuThu = p_MaThuThu;
END //

CREATE PROCEDURE sp_ThuThu_Xoa(IN p_MaThuThu INT)
BEGIN
    DELETE FROM tblThuThu WHERE MaThuThu = p_MaThuThu;
END //

/* =========================================================
   3. STORED PROCEDURE - NGHIỆP VỤ MƯỢN / TRẢ
   ========================================================= */

/* 3.1 Lập phiếu mượn mới (tạo phần đầu phiếu),
       trả về mã phiếu vừa tạo qua tham số OUT để dùng tiếp. */
CREATE PROCEDURE sp_LapPhieuMuon(
    IN  p_MaSV       INT,
    IN  p_MaThuThu   INT,
    OUT p_MaPhieuMoi INT
)
BEGIN
    INSERT INTO tblPhieuMuon(MaSV, MaThuThu, NgayMuon)
    VALUES (p_MaSV, p_MaThuThu, CURDATE());

    SET p_MaPhieuMoi = LAST_INSERT_ID();  -- lấy mã phiếu vừa sinh
END //

/* 3.2 Mượn 1 cuốn sách (thêm 1 dòng vào chi tiết phiếu).
       Có KIỂM TRA số lượng còn: nếu hết thì báo lỗi, không cho mượn. */
CREATE PROCEDURE sp_MuonSach(
    IN p_MaPhieu INT,
    IN p_MaSach  INT
)
BEGIN
    DECLARE v_Tong     INT;
    DECLARE v_DangMuon INT;
    DECLARE v_ConLai   INT;

    -- Tổng số bản của đầu sách
    SELECT SoLuong INTO v_Tong FROM tblSach WHERE MaSach = p_MaSach;

    -- Số bản đang được mượn mà chưa trả
    SELECT COUNT(*) INTO v_DangMuon
    FROM tblPhieuMuonChiTiet
    WHERE MaSach = p_MaSach AND NgayTra IS NULL;

    SET v_ConLai = v_Tong - v_DangMuon;

    IF v_ConLai <= 0 THEN
        SIGNAL SQLSTATE '45000'
            SET MESSAGE_TEXT = 'Sach da het, khong the cho muon.';
    END IF;

    INSERT INTO tblPhieuMuonChiTiet(MaPhieu, MaSach, NgayTra)
    VALUES (p_MaPhieu, p_MaSach, NULL);
END //

/* 3.3 Trả sách (đánh dấu ngày trả cho 1 cuốn trong 1 phiếu) */
CREATE PROCEDURE sp_TraSach(
    IN p_MaPhieu INT,
    IN p_MaSach  INT
)
BEGIN
    UPDATE tblPhieuMuonChiTiet
    SET NgayTra = CURDATE()
    WHERE MaPhieu = p_MaPhieu AND MaSach = p_MaSach;
END //

/* =========================================================
   4. STORED PROCEDURE - BÁO CÁO / THỐNG KÊ
   ========================================================= */

/* 4.1 Thống kê số lượng đầu sách còn lại trong thư viện */
CREATE PROCEDURE sp_ThongKeSachConLai()
BEGIN
    SELECT
        s.MaSach,
        s.TenSach,
        tl.TenLoai,
        s.SoLuong                          AS TongSoLuong,
        s.SoLuong - IFNULL(dm.DangMuon, 0) AS ConLai
    FROM tblSach s
    JOIN tblTheLoai tl ON s.MaLoai = tl.MaLoai
    LEFT JOIN (
        SELECT MaSach, COUNT(*) AS DangMuon
        FROM tblPhieuMuonChiTiet
        WHERE NgayTra IS NULL
        GROUP BY MaSach
    ) dm ON s.MaSach = dm.MaSach;
END //

/* 4.2 Danh sách các phiếu mượn chưa trả */
CREATE PROCEDURE sp_PhieuMuonChuaTra()
BEGIN
    SELECT
        pm.MaPhieu,
        s.MaSach,
        sv.HoTen   AS SinhVien,
        s.TenSach,
        pm.NgayMuon
    FROM tblPhieuMuonChiTiet ct
    JOIN tblPhieuMuon pm ON ct.MaPhieu = pm.MaPhieu
    JOIN tblSinhVien  sv ON pm.MaSV    = sv.MaSV
    JOIN tblSach      s  ON ct.MaSach  = s.MaSach
    WHERE ct.NgayTra IS NULL
    ORDER BY pm.NgayMuon;
END //

/* =========================================================
   5. DỮ LIỆU MẪU
   ========================================================= */

INSERT INTO tblTheLoai(TenLoai) VALUES
('Công nghệ thông tin'), ('Văn học'), ('Kinh tế');

INSERT INTO tblSach(TenSach, MaLoai, SoLuong) VALUES
('Lập trình C# cơ bản',        1, 3),
('Cơ sở dữ liệu SQL Server',   1, 2),
('Nhà giả kim',                2, 5);

INSERT INTO tblSinhVien(HoTen, Lop) VALUES
('Nguyễn Văn An',  'CNTT01'),
('Trần Thị Bình',  'CNTT02');

INSERT INTO tblThuThu(HoTen, TenDangNhap, MatKhau) VALUES
('Lê Quản Trị', 'admin', '123456');
