using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace QLThuVienApp.Forms
{
    public class FrmMain : Form
    {
        private readonly int _maThuThu;
        private readonly string _hoTen;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatus;

        public FrmMain(int maThuThu, string hoTen)
        {
            _maThuThu = maThuThu;
            _hoTen = hoTen;
            BuildUi();
        }

        private void BuildUi()
        {
            Text = "Phần mềm Quản lý Thư viện";
            WindowState = FormWindowState.Maximized;
            IsMdiContainer = true;
            Font = new Font("Segoe UI", 9F);

            BuildMenu();
            BuildStatusBar();
        }

        // ============ MENU ============
        private void BuildMenu()
        {
            var menu = new MenuStrip();

            // --- Danh mục ---
            var mnuDanhMuc = new ToolStripMenuItem("&Danh mục");
            mnuDanhMuc.DropDownItems.Add(NewItem("Quản lý &Sách", Keys.Control | Keys.S, () => Open<FrmSach>()));
            mnuDanhMuc.DropDownItems.Add(NewItem("Quản lý &Thể loại", Keys.Control | Keys.T, () => Open<FrmTheLoai>()));
            mnuDanhMuc.DropDownItems.Add(NewItem("Quản lý S&inh viên", Keys.Control | Keys.V, () => Open<FrmSinhVien>()));
            mnuDanhMuc.DropDownItems.Add(NewItem("Quản lý Thủ thư", Keys.None, () => Open<FrmThuThu>()));

            // --- Mượn / Trả ---
            var mnuMuonTra = new ToolStripMenuItem("&Mượn / Trả");
            mnuMuonTra.DropDownItems.Add(NewItem("Lập phiếu &mượn / trả sách", Keys.Control | Keys.M, OpenMuonTra));

            // --- Báo cáo ---
            var mnuBaoCao = new ToolStripMenuItem("&Báo cáo");
            mnuBaoCao.DropDownItems.Add(NewItem("Thống kê đầu sách / phiếu chưa trả", Keys.Control | Keys.B, () => Open<FrmBaoCao>()));

            menu.Items.AddRange(new ToolStripItem[]
            {
               mnuDanhMuc, mnuMuonTra, mnuBaoCao
            });

            MainMenuStrip = menu;
            Controls.Add(menu);
        }

        private ToolStripMenuItem NewItem(string text, Keys shortcut, Action onClick)
        {
            var item = new ToolStripMenuItem(text, null, (s, e) => onClick());
            if (shortcut != Keys.None) item.ShortcutKeys = shortcut;
            return item;
        }

        // ============ STATUS BAR ============
        private void BuildStatusBar()
        {
            statusStrip = new StatusStrip();
            lblStatus = new ToolStripStatusLabel($"Đăng nhập: {_hoTen}");
            statusStrip.Items.Add(lblStatus);
            Controls.Add(statusStrip);
        }

        // ============ MỞ FORM CON (mỗi loại chỉ 1 cửa sổ) ============
        private void Open<T>() where T : Form, new()
        {
            var existing = MdiChildren.FirstOrDefault(f => f is T);
            if (existing != null) { existing.Activate(); return; }

            var f = new T { MdiParent = this };
            f.Show();
        }

        private void OpenMuonTra()
        {
            var existing = MdiChildren.FirstOrDefault(f => f is FrmMuonTra);
            if (existing != null) { existing.Activate(); return; }

            // Form mượn/trả cần biết thủ thư đang đăng nhập để ghi vào phiếu
            var f = new FrmMuonTra(_maThuThu) { MdiParent = this };
            f.Show();
        }

        private void DangXuat()
        {
            if (MessageBox.Show("Đăng xuất và thoát chương trình?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
