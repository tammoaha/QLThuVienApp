using System;
using System.Windows.Forms;
using QLThuVienApp.Forms;

namespace QLThuVienApp
{
    internal static class Program
    {
        /// <summary>Điểm khởi chạy ứng dụng.</summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Hiện đăng nhập trước. Đăng nhập thành công mới vào form chính.
            using (var login = new FrmLogin())
            {
                if (login.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new FrmMain(login.MaThuThu, login.HoTenThuThu));
                }
                // Nếu đóng/huỷ đăng nhập -> chương trình kết thúc.
            }
        }
    }
}
