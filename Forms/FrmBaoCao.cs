using System;
using System.Drawing;
using System.Windows.Forms;
using QLThuVienApp.Database;

namespace QLThuVienApp.Forms
{
    /// <summary>
    /// Form báo cáo / thống kê theo yêu cầu đề:
    ///  - Thống kê số lượng đầu sách còn lại
    ///  - Thống kê các phiếu mượn chưa trả
    /// Ở đây hiển thị bằng lưới (DataGridView). Để nộp đúng yêu cầu "Crystal Report",
    /// xem hướng dẫn chuyển sang Crystal Report trong file README.md.
    /// </summary>
    public class FrmBaoCao : Form
    {
        private DataGridView grid;
        private Button btnSachConLai, btnChuaTra;

        public FrmBaoCao()
        {
            BuildUi();
            XemSachConLai();   // mặc định hiện thống kê đầu sách
        }

        private void BuildUi()
        {
            Text = "Báo cáo - Thống kê";
            ClientSize = new Size(700, 460);
            Font = new Font("Segoe UI", 9F);

            var pnl = new Panel { Dock = DockStyle.Top, Height = 50, Padding = new Padding(10) };
            btnSachConLai = new Button { Text = "Thống kê đầu &sách còn lại", Location = new Point(10, 8), Size = new Size(200, 30) };
            btnChuaTra = new Button { Text = "Phiếu mượn &chưa trả", Location = new Point(220, 8), Size = new Size(180, 30) };

            btnSachConLai.Click += (s, e) => XemSachConLai();
            btnChuaTra.Click += (s, e) => XemChuaTra();

            pnl.Controls.AddRange(new Control[] { btnSachConLai, btnChuaTra });

            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            Controls.Add(grid);
            Controls.Add(pnl);
        }

        private void XemSachConLai()
        {
            try
            {
                grid.DataSource = DbHelper.ThongKeSachConLai();
                if (grid.Columns.Contains("MaSach")) grid.Columns["MaSach"].HeaderText = "Mã";
                if (grid.Columns.Contains("TenSach")) grid.Columns["TenSach"].HeaderText = "Tên sách";
                if (grid.Columns.Contains("TenLoai")) grid.Columns["TenLoai"].HeaderText = "Thể loại";
                if (grid.Columns.Contains("TongSoLuong")) grid.Columns["TongSoLuong"].HeaderText = "Tổng SL";
                if (grid.Columns.Contains("ConLai")) grid.Columns["ConLai"].HeaderText = "Còn lại";
            }
            catch (Exception ex) { BaoLoi(ex); }
        }

        private void XemChuaTra()
        {
            try
            {
                grid.DataSource = DbHelper.PhieuMuonChuaTra();
                if (grid.Columns.Contains("MaPhieu")) grid.Columns["MaPhieu"].HeaderText = "Phiếu";
                if (grid.Columns.Contains("MaSach")) grid.Columns["MaSach"].Visible = false;
                if (grid.Columns.Contains("SinhVien")) grid.Columns["SinhVien"].HeaderText = "Sinh viên";
                if (grid.Columns.Contains("TenSach")) grid.Columns["TenSach"].HeaderText = "Tên sách";
                if (grid.Columns.Contains("NgayMuon")) grid.Columns["NgayMuon"].HeaderText = "Ngày mượn";
            }
            catch (Exception ex) { BaoLoi(ex); }
        }

        private void BaoLoi(Exception ex) => MessageBox.Show("Có lỗi:\n" + ex.Message, "Lỗi",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
