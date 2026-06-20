using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QLThuVienApp.Database;

namespace QLThuVienApp.Forms
{
    /// <summary>Form đăng nhập. Kiểm tra tài khoản trong bảng tblThuThu.</summary>
    public class FrmLogin : Form
    {
        private TextBox txtUser;
        private TextBox txtPass;
        private Button btnDangNhap;
        private Button btnThoat;

        // Thông tin trả ra cho Program sau khi đăng nhập thành công
        public string HoTenThuThu { get; private set; }
        public int MaThuThu { get; private set; }
        public string QuyenThuThu { get; private set; }

        public FrmLogin()
        {
            BuildUi();
        }

        private void BuildUi()
        {
            Text = "Đăng nhập - Quản lý Thư viện";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(360, 200);
            Font = new Font("Segoe UI", 9F);

            var lblTitle = new Label
            {
                Text = "PHẦN MỀM QUẢN LÝ THƯ VIỆN",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 15),
                Size = new Size(320, 30)
            };

            var lblUser = new Label { Text = "&Tên đăng nhập:", Location = new Point(20, 60), Size = new Size(110, 23) };
            txtUser = new TextBox { Text = "admin", Location = new Point(135, 57), Size = new Size(200, 23) };

            var lblPass = new Label { Text = "&Mật khẩu:", Location = new Point(20, 95), Size = new Size(110, 23) };
            txtPass = new TextBox { Text = "123456", Location = new Point(135, 92), Size = new Size(200, 23), UseSystemPasswordChar = true };

            btnDangNhap = new Button { Text = "Đăng &nhập", Location = new Point(135, 135), Size = new Size(95, 30) };
            btnThoat = new Button { Text = "T&hoát", Location = new Point(240, 135), Size = new Size(95, 30) };

            btnDangNhap.Click += BtnDangNhap_Click;
            btnThoat.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            // Enter = đăng nhập, Esc = thoát
            AcceptButton = btnDangNhap;
            CancelButton = btnThoat;

            Controls.AddRange(new Control[] { lblTitle, lblUser, txtUser, lblPass, txtPass, btnDangNhap, btnThoat });
        }

        private void BtnDangNhap_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUser.Text) || string.IsNullOrWhiteSpace(txtPass.Text))
            {
                MessageBox.Show("Vui lòng nhập đủ tên đăng nhập và mật khẩu.",
                    "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DataRow row = DbHelper.DangNhap(txtUser.Text.Trim(), txtPass.Text);
                if (row == null)
                {
                    MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu.",
                        "Đăng nhập thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtPass.Clear();
                    txtPass.Focus();
                    return;
                }

                MaThuThu = Convert.ToInt32(row["MaThuThu"]);
                HoTenThuThu = row["HoTen"].ToString();
                QuyenThuThu = row["Quyen"].ToString();
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không kết nối được CSDL.\n\nChi tiết: " + ex.Message,
                    "Lỗi kết nối", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
