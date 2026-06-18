using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace QLThuVienApp.Database
{
    public static class DbHelper
    {
        private static string ConnStr =>
            ConfigurationManager.ConnectionStrings["QLThuVien"].ConnectionString;

        private static SqlConnection GetConn() => new SqlConnection(ConnStr);

        public static DataTable QueryProc(string proc, params SqlParameter[] ps)
        {
            using (var conn = GetConn())
            using (var cmd = new SqlCommand(proc, conn) { CommandType = CommandType.StoredProcedure })
            {
                if (ps != null) cmd.Parameters.AddRange(ps);
                var dt = new DataTable();
                using (var da = new SqlDataAdapter(cmd)) da.Fill(dt);
                return dt;
            }
        }

        public static int ExecProc(string proc, params SqlParameter[] ps)
        {
            using (var conn = GetConn())
            using (var cmd = new SqlCommand(proc, conn) { CommandType = CommandType.StoredProcedure })
            {
                if (ps != null) cmd.Parameters.AddRange(ps);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static DataTable QueryText(string sql, params SqlParameter[] ps)
        {
            using (var conn = GetConn())
            using (var cmd = new SqlCommand(sql, conn))
            {
                if (ps != null) cmd.Parameters.AddRange(ps);
                var dt = new DataTable();
                using (var da = new SqlDataAdapter(cmd)) da.Fill(dt);
                return dt;
            }
        }

        private static SqlParameter P(string name, object value) =>
            new SqlParameter(name, value ?? DBNull.Value);

        public static DataRow DangNhap(string tenDangNhap, string matKhau)
        {
            var dt = QueryText(
                "SELECT MaThuThu, HoTen, Quyen FROM tblThuThu " +
                "WHERE TenDangNhap = @u AND MatKhau = @p",
                P("@u", tenDangNhap), P("@p", matKhau));
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        public static DataTable LayTheLoai() =>
            QueryText("SELECT MaLoai, TenLoai FROM tblTheLoai ORDER BY TenLoai");

        public static void ThemTheLoai(string ten) =>
            ExecProc("sp_TheLoai_Them", P("@p_TenLoai", ten));

        public static void SuaTheLoai(int ma, string ten) =>
            ExecProc("sp_TheLoai_Sua", P("@p_MaLoai", ma), P("@p_TenLoai", ten));

        public static void XoaTheLoai(int ma) =>
            ExecProc("sp_TheLoai_Xoa", P("@p_MaLoai", ma));

        public static DataTable LayDanhSachSach() =>
            QueryText(
                "SELECT s.MaSach, s.TenSach, s.MaLoai, tl.TenLoai, s.SoLuong " +
                "FROM tblSach s JOIN tblTheLoai tl ON s.MaLoai = tl.MaLoai " +
                "ORDER BY s.MaSach");

        public static DataTable TimSach(string tuKhoa) =>
            QueryText(
                "SELECT s.MaSach, s.TenSach, s.MaLoai, tl.TenLoai, s.SoLuong " +
                "FROM tblSach s JOIN tblTheLoai tl ON s.MaLoai = tl.MaLoai " +
                "WHERE s.TenSach LIKE @kw ORDER BY s.MaSach",
                P("@kw", "%" + tuKhoa + "%"));

        public static void ThemSach(string ten, int maLoai, int soLuong) =>
            ExecProc("sp_Sach_Them",
                P("@p_TenSach", ten), P("@p_MaLoai", maLoai), P("@p_SoLuong", soLuong));

        public static void SuaSach(int ma, string ten, int maLoai, int soLuong) =>
            ExecProc("sp_Sach_Sua",
                P("@p_MaSach", ma), P("@p_TenSach", ten),
                P("@p_MaLoai", maLoai), P("@p_SoLuong", soLuong));

        public static void XoaSach(int ma) =>
            ExecProc("sp_Sach_Xoa", P("@p_MaSach", ma));

        public static DataTable LaySinhVien() =>
            QueryText("SELECT MaSV, HoTen, Lop FROM tblSinhVien ORDER BY MaSV");

        public static void ThemSinhVien(string hoTen, string lop) =>
            ExecProc("sp_SinhVien_Them", P("@p_HoTen", hoTen), P("@p_Lop", lop));

        public static void SuaSinhVien(int ma, string hoTen, string lop) =>
            ExecProc("sp_SinhVien_Sua", P("@p_MaSV", ma), P("@p_HoTen", hoTen), P("@p_Lop", lop));

        public static void XoaSinhVien(int ma) =>
            ExecProc("sp_SinhVien_Xoa", P("@p_MaSV", ma));

        public static DataTable LayThuThu() =>
            QueryText("SELECT MaThuThu, HoTen, TenDangNhap, Quyen FROM tblThuThu ORDER BY MaThuThu");

        public static void ThemThuThu(string hoTen, string tenDN, string matKhau, string quyen) =>
            ExecProc("sp_ThuThu_Them",
                P("@p_HoTen", hoTen), P("@p_TenDangNhap", tenDN),
                P("@p_MatKhau", matKhau), P("@p_Quyen", quyen));

        public static void SuaThuThu(int ma, string hoTen, string tenDN, string matKhau, string quyen) =>
            ExecProc("sp_ThuThu_Sua",
                P("@p_MaThuThu", ma), P("@p_HoTen", hoTen), P("@p_TenDangNhap", tenDN),
                P("@p_MatKhau", string.IsNullOrWhiteSpace(matKhau) ? (object)DBNull.Value : matKhau),
                P("@p_Quyen", quyen));

        public static void XoaThuThu(int ma) =>
            ExecProc("sp_ThuThu_Xoa", P("@p_MaThuThu", ma));

        public static int LapPhieuMuon(int maSV, int maThuThu)
        {
            using (var conn = GetConn())
            using (var cmd = new SqlCommand("sp_LapPhieuMuon", conn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@p_MaSV", maSV);
                cmd.Parameters.AddWithValue("@p_MaThuThu", maThuThu);
                var outP = new SqlParameter("@p_MaPhieuMoi", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outP);
                conn.Open();
                cmd.ExecuteNonQuery();
                return Convert.ToInt32(outP.Value);
            }
        }

        public static void MuonSach(int maPhieu, int maSach) =>
            ExecProc("sp_MuonSach", P("@p_MaPhieu", maPhieu), P("@p_MaSach", maSach));

        public static void TraSach(int maPhieu, int maSach) =>
            ExecProc("sp_TraSach", P("@p_MaPhieu", maPhieu), P("@p_MaSach", maSach));

        public static DataTable ChiTietPhieu(int maPhieu) =>
            QueryText(
                "SELECT ct.MaSach, s.TenSach, " +
                "       CASE WHEN ct.NgayTra IS NULL THEN N'Dang muon' ELSE N'Da tra' END AS TrangThai " +
                "FROM tblPhieuMuonChiTiet ct JOIN tblSach s ON ct.MaSach = s.MaSach " +
                "WHERE ct.MaPhieu = @mp ORDER BY ct.MaSach",
                P("@mp", maPhieu));

        public static DataTable ThongKeSachConLai() => QueryProc("sp_ThongKeSachConLai");
        public static DataTable PhieuMuonChuaTra() => QueryProc("sp_PhieuMuonChuaTra");
    }
}
