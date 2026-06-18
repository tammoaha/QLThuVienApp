using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QLThuVienApp.Database;

namespace QLThuVienApp.Forms
{
    /// <summary>CRUD Sinh viên (đăng ký làm thẻ mượn).</summary>
    public class FrmSinhVien : Form
    {
        private DataGridView grid;
        private TextBox txtHoTen, txtLop;
        private Button btnThem, btnSua, btnXoa, btnMoi;
        private int _maDangChon = 0;

        public FrmSinhVien()
        {
            BuildUi();
            LoadGrid();
        }

        private void BuildUi()
        {
            Text = "Quản lý Sinh viên";
            ClientSize = new Size(600, 420);
            Font = new Font("Segoe UI", 9F);

            var pnl = new Panel { Dock = DockStyle.Top, Height = 100, Padding = new Padding(10) };
            var lblTen = new Label { Text = "&Họ tên:", Location = new Point(10, 18), Size = new Size(70, 23) };
            txtHoTen = new TextBox { Location = new Point(85, 15), Size = new Size(250, 23) };
            var lblLop = new Label { Text = "&Lớp:", Location = new Point(10, 52), Size = new Size(70, 23) };
            txtLop = new TextBox { Location = new Point(85, 49), Size = new Size(250, 23) };

            btnThem = new Button { Text = "Th&êm", Location = new Point(360, 13), Size = new Size(85, 28) };
            btnSua = new Button { Text = "&Sửa", Location = new Point(450, 13), Size = new Size(85, 28) };
            btnXoa = new Button { Text = "&Xóa", Location = new Point(360, 48), Size = new Size(85, 28) };
            btnMoi = new Button { Text = "&Làm mới", Location = new Point(450, 48), Size = new Size(85, 28) };

            btnThem.Click += (s, e) => Them();
            btnSua.Click += (s, e) => Sua();
            btnXoa.Click += (s, e) => Xoa();
            btnMoi.Click += (s, e) => LamMoi();

            pnl.Controls.AddRange(new Control[] { lblTen, txtHoTen, lblLop, txtLop, btnThem, btnSua, btnXoa, btnMoi });

            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            grid.SelectionChanged += (s, e) =>
            {
                if (grid.CurrentRow?.DataBoundItem == null) return;
                var r = (DataRowView)grid.CurrentRow.DataBoundItem;
                _maDangChon = Convert.ToInt32(r["MaSV"]);
                txtHoTen.Text = r["HoTen"].ToString();
                txtLop.Text = r["Lop"].ToString();
            };

            Controls.Add(grid);
            Controls.Add(pnl);
        }

        private void LoadGrid()
        {
            grid.DataSource = DbHelper.LaySinhVien();
            if (grid.Columns.Contains("MaSV")) grid.Columns["MaSV"].HeaderText = "Mã SV";
            if (grid.Columns.Contains("HoTen")) grid.Columns["HoTen"].HeaderText = "Họ tên";
            if (grid.Columns.Contains("Lop")) grid.Columns["Lop"].HeaderText = "Lớp";
        }

        private bool HopLe()
        {
            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                MessageBox.Show("Họ tên không được để trống.", "Kiểm tra",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHoTen.Focus();
                return false;
            }
            return true;
        }

        private void Them()
        {
            if (!HopLe()) return;
            try { DbHelper.ThemSinhVien(txtHoTen.Text.Trim(), txtLop.Text.Trim()); LoadGrid(); LamMoi(); }
            catch (Exception ex) { BaoLoi(ex); }
        }

        private void Sua()
        {
            if (_maDangChon == 0) { ChonDi(); return; }
            if (!HopLe()) return;
            try { DbHelper.SuaSinhVien(_maDangChon, txtHoTen.Text.Trim(), txtLop.Text.Trim()); LoadGrid(); }
            catch (Exception ex) { BaoLoi(ex); }
        }

        private void Xoa()
        {
            if (_maDangChon == 0) { ChonDi(); return; }
            if (MessageBox.Show("Xóa sinh viên này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try { DbHelper.XoaSinhVien(_maDangChon); LoadGrid(); LamMoi(); }
            catch (Exception ex) { BaoLoi(ex); }
        }

        private void LamMoi() { _maDangChon = 0; txtHoTen.Clear(); txtLop.Clear(); txtHoTen.Focus(); }
        private void ChonDi() => MessageBox.Show("Hãy chọn 1 dòng trong danh sách.", "Thông báo",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        private void BaoLoi(Exception ex) => MessageBox.Show("Có lỗi:\n" + ex.Message, "Lỗi",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
