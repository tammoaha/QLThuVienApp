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
        private readonly string _quyen;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel lblStatus;

        public FrmMain(int maThuThu, string hoTen, string quyen)
        {
            _maThuThu = maThuThu;
            _hoTen = hoTen;
            _quyen = quyen;
            BuildUi();
        }

        private void BuildUi()
        {
            Text = "Phan mem Quan ly Thu vien";
            WindowState = FormWindowState.Maximized;
            IsMdiContainer = true;
            Font = new Font("Segoe UI", 9F);

            BuildMenu();
            BuildStatusBar();
        }

        private void BuildMenu()
        {
            var menu = new MenuStrip();

            var mnuDanhMuc = new ToolStripMenuItem("&Danh muc");
            mnuDanhMuc.DropDownItems.Add(NewItem("Quan ly &Sach", Keys.Control | Keys.S, () => Open<FrmSach>()));
            mnuDanhMuc.DropDownItems.Add(NewItem("Quan ly &The loai", Keys.Control | Keys.T, () => Open<FrmTheLoai>()));
            mnuDanhMuc.DropDownItems.Add(NewItem("Quan ly S&inh vien", Keys.Control | Keys.V, () => Open<FrmSinhVien>()));
            mnuDanhMuc.DropDownItems.Add(NewItem("Quan ly Thu thu", Keys.None, OpenThuThu));

            var mnuMuonTra = new ToolStripMenuItem("&Muon / Tra");
            mnuMuonTra.DropDownItems.Add(NewItem("Lap phieu &muon / tra sach", Keys.Control | Keys.M, OpenMuonTra));

            var mnuBaoCao = new ToolStripMenuItem("&Bao cao");
            mnuBaoCao.DropDownItems.Add(NewItem("Thong ke dau sach / phieu chua tra", Keys.Control | Keys.B, () => Open<FrmBaoCao>()));

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

        private void BuildStatusBar()
        {
            statusStrip = new StatusStrip();
            lblStatus = new ToolStripStatusLabel($"Dang nhap: {_hoTen} - Quyen: {_quyen}");
            statusStrip.Items.Add(lblStatus);
            Controls.Add(statusStrip);
        }

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

            var f = new FrmMuonTra(_maThuThu) { MdiParent = this };
            f.Show();
        }

        private void OpenThuThu()
        {
            if (string.Equals(_quyen, "TroLy", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Tro ly thu thu khong co quyen tao, them, sua tai khoan.",
                    "Khong du quyen", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Open<FrmThuThu>();
        }

        private void DangXuat()
        {
            if (MessageBox.Show("Dang xuat va thoat chuong trinh?", "Xac nhan",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
