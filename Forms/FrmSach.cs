using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QLThuVienApp.Database;

namespace QLThuVienApp.Forms
{
    /// <summary>
    /// FORM CRUD MẪU cho bảng Sách.
    /// Các form Thể loại / Sinh viên / Thủ thư được viết theo đúng khuôn này.
    /// </summary>
    public class FrmSach : Form
    {
        private DataGridView grid;
        private TextBox txtTen;
        private ComboBox cboLoai;
        private NumericUpDown numSoLuong;
        private TextBox txtTimKiem;
        private Button btnThem, btnSua, btnXoa, btnMoi;
        private int _maDangChon = 0;   // 0 = đang ở chế độ thêm mới

        public FrmSach()
        {
            BuildUi();
            LoadComboTheLoai();
            LoadGrid();
        }

        private void BuildUi()
        {
            Text = "Quản lý Sách";
            ClientSize = new Size(720, 460);
            Font = new Font("Segoe UI", 9F);

            // ----- Khu nhập liệu phía trên -----
            var pnl = new Panel { Dock = DockStyle.Top, Height = 150, Padding = new Padding(10) };

            var lblTen = new Label { Text = "&Tên sách:", Location = new Point(10, 15), Size = new Size(80, 23) };
            txtTen = new TextBox { Location = new Point(95, 12), Size = new Size(300, 23) };

            var lblLoai = new Label { Text = "Thể &loại:", Location = new Point(10, 50), Size = new Size(80, 23) };
            cboLoai = new ComboBox { Location = new Point(95, 47), Size = new Size(300, 23), DropDownStyle = ComboBoxStyle.DropDownList };

            var lblSL = new Label { Text = "&Số lượng:", Location = new Point(10, 85), Size = new Size(80, 23) };
            numSoLuong = new NumericUpDown { Location = new Point(95, 82), Size = new Size(120, 23), Minimum = 0, Maximum = 100000 };

            btnThem = new Button { Text = "Th&êm", Location = new Point(430, 12), Size = new Size(90, 30) };
            btnSua = new Button { Text = "&Sửa", Location = new Point(430, 48), Size = new Size(90, 30) };
            btnXoa = new Button { Text = "&Xóa", Location = new Point(430, 84), Size = new Size(90, 30) };
            btnMoi = new Button { Text = "&Làm mới", Location = new Point(530, 12), Size = new Size(90, 30) };

            btnThem.Click += (s, e) => Them();
            btnSua.Click += (s, e) => Sua();
            btnXoa.Click += (s, e) => Xoa();
            btnMoi.Click += (s, e) => LamMoi();

            // ----- Ô tìm kiếm -----
            var lblTim = new Label { Text = "Tìm theo tên:", Location = new Point(10, 120), Size = new Size(80, 23) };
            txtTimKiem = new TextBox { Location = new Point(95, 117), Size = new Size(300, 23) };
            txtTimKiem.TextChanged += (s, e) => LoadGrid(txtTimKiem.Text.Trim());

            pnl.Controls.AddRange(new Control[]
            {
                lblTen, txtTen, lblLoai, cboLoai, lblSL, numSoLuong,
                btnThem, btnSua, btnXoa, btnMoi, lblTim, txtTimKiem
            });

            // ----- Lưới dữ liệu -----
            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            grid.SelectionChanged += Grid_SelectionChanged;

            Controls.Add(grid);
            Controls.Add(pnl);
        }

        private void LoadComboTheLoai()
        {
            cboLoai.DataSource = DbHelper.LayTheLoai();
            cboLoai.DisplayMember = "TenLoai";
            cboLoai.ValueMember = "MaLoai";
            cboLoai.SelectedIndex = -1;
        }

        private void LoadGrid(string tuKhoa = "")
        {
            DataTable dt = string.IsNullOrWhiteSpace(tuKhoa)
                ? DbHelper.LayDanhSachSach()
                : DbHelper.TimSach(tuKhoa);
            grid.DataSource = dt;

            if (grid.Columns.Contains("MaSach")) grid.Columns["MaSach"].HeaderText = "Mã";
            if (grid.Columns.Contains("TenSach")) grid.Columns["TenSach"].HeaderText = "Tên sách";
            if (grid.Columns.Contains("TenLoai")) grid.Columns["TenLoai"].HeaderText = "Thể loại";
            if (grid.Columns.Contains("SoLuong")) grid.Columns["SoLuong"].HeaderText = "Số lượng";
            if (grid.Columns.Contains("MaLoai")) grid.Columns["MaLoai"].Visible = false;
        }

        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            if (grid.CurrentRow == null || grid.CurrentRow.DataBoundItem == null) return;
            var r = (DataRowView)grid.CurrentRow.DataBoundItem;

            _maDangChon = Convert.ToInt32(r["MaSach"]);
            txtTen.Text = r["TenSach"].ToString();
            cboLoai.SelectedValue = Convert.ToInt32(r["MaLoai"]);
            numSoLuong.Value = Convert.ToInt32(r["SoLuong"]);
        }

        // ----- Kiểm tra dữ liệu trước khi lưu -----
        private bool HopLe()
        {
            if (string.IsNullOrWhiteSpace(txtTen.Text))
            {
                MessageBox.Show("Tên sách không được để trống.", "Kiểm tra",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTen.Focus();
                return false;
            }
            if (cboLoai.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn thể loại.", "Kiểm tra",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboLoai.Focus();
                return false;
            }
            return true;
        }

        private void Them()
        {
            if (!HopLe()) return;
            try
            {
                DbHelper.ThemSach(txtTen.Text.Trim(),
                    Convert.ToInt32(cboLoai.SelectedValue), (int)numSoLuong.Value);
                LoadGrid();
                LamMoi();
            }
            catch (Exception ex) { BaoLoi(ex); }
        }

        private void Sua()
        {
            if (_maDangChon == 0)
            {
                MessageBox.Show("Hãy chọn 1 dòng trong danh sách để sửa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!HopLe()) return;
            try
            {
                DbHelper.SuaSach(_maDangChon, txtTen.Text.Trim(),
                    Convert.ToInt32(cboLoai.SelectedValue), (int)numSoLuong.Value);
                LoadGrid();
            }
            catch (Exception ex) { BaoLoi(ex); }
        }

        private void Xoa()
        {
            if (_maDangChon == 0)
            {
                MessageBox.Show("Hãy chọn 1 dòng để xóa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (MessageBox.Show("Bạn chắc chắn muốn xóa sách này?", "Xác nhận xóa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                DbHelper.XoaSach(_maDangChon);
                LoadGrid();
                LamMoi();
            }
            catch (Exception ex) { BaoLoi(ex); }
        }

        private void LamMoi()
        {
            _maDangChon = 0;
            txtTen.Clear();
            cboLoai.SelectedIndex = -1;
            numSoLuong.Value = 0;
            txtTen.Focus();
        }

        private void BaoLoi(Exception ex)
        {
            MessageBox.Show("Có lỗi xảy ra:\n" + ex.Message, "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
