using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QLThuVienApp.Database;

namespace QLThuVienApp.Forms
{
    public class FrmThuThu : Form
    {
        private DataGridView grid;
        private TextBox txtHoTen, txtTenDN, txtMatKhau;
        private ComboBox cboQuyen;
        private Button btnThem, btnSua, btnXoa, btnMoi;
        private int _maDangChon = 0;

        public FrmThuThu()
        {
            BuildUi();
            LoadGrid();
        }

        private void BuildUi()
        {
            Text = "Quan ly Thu thu";
            ClientSize = new Size(740, 460);
            Font = new Font("Segoe UI", 9F);

            var pnl = new Panel { Dock = DockStyle.Top, Height = 145, Padding = new Padding(10) };

            var lblHoTen = new Label { Text = "&Ho ten:", Location = new Point(10, 15), Size = new Size(95, 23) };
            txtHoTen = new TextBox { Location = new Point(110, 12), Size = new Size(240, 23) };

            var lblDN = new Label { Text = "&Ten dang nhap:", Location = new Point(10, 48), Size = new Size(95, 23) };
            txtTenDN = new TextBox { Location = new Point(110, 45), Size = new Size(240, 23) };

            var lblMK = new Label { Text = "&Mat khau:", Location = new Point(10, 81), Size = new Size(95, 23) };
            txtMatKhau = new TextBox { Location = new Point(110, 78), Size = new Size(240, 23), UseSystemPasswordChar = true };

            var lblQuyen = new Label { Text = "&Quyen:", Location = new Point(10, 114), Size = new Size(95, 23) };
            cboQuyen = new ComboBox
            {
                Location = new Point(110, 111),
                Size = new Size(240, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cboQuyen.Items.AddRange(new object[] { "ThuThu", "Admin" });
            cboQuyen.SelectedIndex = 0;

            btnThem = new Button { Text = "Th&em", Location = new Point(375, 45), Size = new Size(85, 28) };
            btnSua = new Button { Text = "&Sua", Location = new Point(470, 45), Size = new Size(85, 28) };
            btnXoa = new Button { Text = "&Xoa", Location = new Point(375, 81), Size = new Size(85, 28) };
            btnMoi = new Button { Text = "&Lam moi", Location = new Point(470, 81), Size = new Size(85, 28) };

            btnThem.Click += (s, e) => Them();
            btnSua.Click += (s, e) => Sua();
            btnXoa.Click += (s, e) => Xoa();
            btnMoi.Click += (s, e) => LamMoi();

            pnl.Controls.AddRange(new Control[]
            {
                lblHoTen, txtHoTen, lblDN, txtTenDN, lblMK, txtMatKhau,
                lblQuyen, cboQuyen, btnThem, btnSua, btnXoa, btnMoi
            });

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

        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            if (grid.CurrentRow?.DataBoundItem == null) return;
            var r = (DataRowView)grid.CurrentRow.DataBoundItem;
            _maDangChon = Convert.ToInt32(r["MaThuThu"]);
            txtHoTen.Text = r["HoTen"].ToString();
            txtTenDN.Text = r["TenDangNhap"].ToString();
            cboQuyen.SelectedItem = r["Quyen"].ToString();
            txtMatKhau.Clear();
        }

        private void LoadGrid()
        {
            grid.DataSource = DbHelper.LayThuThu();
            if (grid.Columns.Contains("MaThuThu")) grid.Columns["MaThuThu"].HeaderText = "Ma";
            if (grid.Columns.Contains("HoTen")) grid.Columns["HoTen"].HeaderText = "Ho ten";
            if (grid.Columns.Contains("TenDangNhap")) grid.Columns["TenDangNhap"].HeaderText = "Ten dang nhap";
            if (grid.Columns.Contains("Quyen")) grid.Columns["Quyen"].HeaderText = "Quyen";
        }

        private bool HopLe(bool laThem)
        {
            if (string.IsNullOrWhiteSpace(txtHoTen.Text) || string.IsNullOrWhiteSpace(txtTenDN.Text))
            {
                MessageBox.Show("Ho ten va ten dang nhap khong duoc de trong.", "Kiem tra",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (laThem && string.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                MessageBox.Show("Khi them moi phai nhap mat khau.", "Kiem tra",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMatKhau.Focus();
                return false;
            }
            if (cboQuyen.SelectedItem == null)
            {
                MessageBox.Show("Hay chon quyen cho thu thu.", "Kiem tra",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboQuyen.Focus();
                return false;
            }
            return true;
        }

        private void Them()
        {
            if (!HopLe(true)) return;
            try
            {
                DbHelper.ThemThuThu(
                    txtHoTen.Text.Trim(),
                    txtTenDN.Text.Trim(),
                    txtMatKhau.Text,
                    cboQuyen.SelectedItem.ToString());
                LoadGrid();
                LamMoi();
            }
            catch (Exception ex) { BaoLoi(ex); }
        }

        private void Sua()
        {
            if (_maDangChon == 0) { ChonDi(); return; }
            if (!HopLe(false)) return;
            try
            {
                DbHelper.SuaThuThu(
                    _maDangChon,
                    txtHoTen.Text.Trim(),
                    txtTenDN.Text.Trim(),
                    txtMatKhau.Text,
                    cboQuyen.SelectedItem.ToString());
                LoadGrid();
            }
            catch (Exception ex) { BaoLoi(ex); }
        }

        private void Xoa()
        {
            if (_maDangChon == 0) { ChonDi(); return; }
            if (MessageBox.Show("Xoa thu thu nay?", "Xac nhan",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                DbHelper.XoaThuThu(_maDangChon);
                LoadGrid();
                LamMoi();
            }
            catch (Exception ex) { BaoLoi(ex); }
        }

        private void LamMoi()
        {
            _maDangChon = 0;
            txtHoTen.Clear();
            txtTenDN.Clear();
            txtMatKhau.Clear();
            cboQuyen.SelectedIndex = 0;
            txtHoTen.Focus();
        }

        private void ChonDi() => MessageBox.Show("Hay chon 1 dong trong danh sach.", "Thong bao",
            MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void BaoLoi(Exception ex) => MessageBox.Show("Co loi:\n" + ex.Message, "Loi",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
