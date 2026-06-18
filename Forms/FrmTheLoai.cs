using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QLThuVienApp.Database;

namespace QLThuVienApp.Forms
{
    /// <summary>CRUD Thể loại sách - cùng khuôn với FrmSach (đơn giản hơn).</summary>
    public class FrmTheLoai : Form
    {
        private DataGridView grid;
        private TextBox txtTen;
        private Button btnThem, btnSua, btnXoa, btnMoi;
        private int _maDangChon = 0;

        public FrmTheLoai()
        {
            BuildUi();
            LoadGrid();
        }

        private void BuildUi()
        {
            Text = "Quản lý Thể loại";
            ClientSize = new Size(560, 400);
            Font = new Font("Segoe UI", 9F);

            var pnl = new Panel { Dock = DockStyle.Top, Height = 90, Padding = new Padding(10) };
            var lblTen = new Label { Text = "&Tên thể loại:", Location = new Point(10, 18), Size = new Size(90, 23) };
            txtTen = new TextBox { Location = new Point(105, 15), Size = new Size(250, 23) };

            btnThem = new Button { Text = "Th&êm", Location = new Point(375, 13), Size = new Size(80, 28) };
            btnSua = new Button { Text = "&Sửa", Location = new Point(460, 13), Size = new Size(80, 28) };
            btnXoa = new Button { Text = "&Xóa", Location = new Point(375, 48), Size = new Size(80, 28) };
            btnMoi = new Button { Text = "&Làm mới", Location = new Point(460, 48), Size = new Size(80, 28) };

            btnThem.Click += (s, e) => Them();
            btnSua.Click += (s, e) => Sua();
            btnXoa.Click += (s, e) => Xoa();
            btnMoi.Click += (s, e) => LamMoi();

            pnl.Controls.AddRange(new Control[] { lblTen, txtTen, btnThem, btnSua, btnXoa, btnMoi });

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
                _maDangChon = Convert.ToInt32(r["MaLoai"]);
                txtTen.Text = r["TenLoai"].ToString();
            };

            Controls.Add(grid);
            Controls.Add(pnl);
        }

        private void LoadGrid()
        {
            grid.DataSource = DbHelper.LayTheLoai();
            if (grid.Columns.Contains("MaLoai")) grid.Columns["MaLoai"].HeaderText = "Mã";
            if (grid.Columns.Contains("TenLoai")) grid.Columns["TenLoai"].HeaderText = "Tên thể loại";
        }

        private bool HopLe()
        {
            if (string.IsNullOrWhiteSpace(txtTen.Text))
            {
                MessageBox.Show("Tên thể loại không được để trống.", "Kiểm tra",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTen.Focus();
                return false;
            }
            return true;
        }

        private void Them()
        {
            if (!HopLe()) return;
            try { DbHelper.ThemTheLoai(txtTen.Text.Trim()); LoadGrid(); LamMoi(); }
            catch (Exception ex) { BaoLoi(ex); }
        }

        private void Sua()
        {
            if (_maDangChon == 0) { ChonDi(); return; }
            if (!HopLe()) return;
            try { DbHelper.SuaTheLoai(_maDangChon, txtTen.Text.Trim()); LoadGrid(); }
            catch (Exception ex) { BaoLoi(ex); }
        }

        private void Xoa()
        {
            if (_maDangChon == 0) { ChonDi(); return; }
            if (MessageBox.Show("Xóa thể loại này?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try { DbHelper.XoaTheLoai(_maDangChon); LoadGrid(); LamMoi(); }
            catch (Exception ex) { BaoLoi(ex); }
        }

        private void LamMoi() { _maDangChon = 0; txtTen.Clear(); txtTen.Focus(); }
        private void ChonDi() => MessageBox.Show("Hãy chọn 1 dòng trong danh sách.", "Thông báo",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
        private void BaoLoi(Exception ex) => MessageBox.Show("Có lỗi:\n" + ex.Message, "Lỗi",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
