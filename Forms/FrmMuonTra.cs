using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QLThuVienApp.Database;

namespace QLThuVienApp.Forms
{
    /// <summary>
    /// Form nghiệp vụ MƯỢN / TRẢ - phần phức tạp nhất (master-detail).
    /// Tab "Mượn": chọn sinh viên -> lập phiếu -> thêm từng sách vào phiếu.
    /// Tab "Trả" : xem các sách đang mượn chưa trả -> chọn -> trả.
    /// </summary>
    public class FrmMuonTra : Form
    {
        private readonly int _maThuThu;      // thủ thư đang đăng nhập
        private int _maPhieuHienTai = 0;     // phiếu đang thao tác (0 = chưa lập)

        // Tab Mượn
        private ComboBox cboSinhVien, cboSach;
        private Button btnLapPhieu, btnMuon;
        private Label lblPhieu;
        private DataGridView gridChiTiet;

        // Tab Trả
        private DataGridView gridChuaTra;
        private Button btnTra, btnTaiLai;

        public FrmMuonTra(int maThuThu)
        {
            _maThuThu = maThuThu;
            BuildUi();
            LoadCombos();
        }

        private void BuildUi()
        {
            Text = "Mượn / Trả sách";
            ClientSize = new Size(760, 520);
            Font = new Font("Segoe UI", 9F);

            var tabs = new TabControl { Dock = DockStyle.Fill };
            tabs.TabPages.Add(BuildTabMuon());
            tabs.TabPages.Add(BuildTabTra());
            Controls.Add(tabs);
        }

        // ================= TAB MƯỢN =================
        private TabPage BuildTabMuon()
        {
            var tab = new TabPage("Lập phiếu &mượn");

            var lblSV = new Label { Text = "&Sinh viên:", Location = new Point(15, 20), Size = new Size(75, 23) };
            cboSinhVien = new ComboBox { Location = new Point(95, 17), Size = new Size(260, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            btnLapPhieu = new Button { Text = "&Lập phiếu mới", Location = new Point(370, 16), Size = new Size(130, 26) };
            btnLapPhieu.Click += (s, e) => LapPhieu();

            lblPhieu = new Label
            {
                Text = "Chưa lập phiếu",
                Location = new Point(515, 20),
                Size = new Size(220, 23),
                ForeColor = Color.DarkRed,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            var lblSach = new Label { Text = "Chọn sác&h:", Location = new Point(15, 60), Size = new Size(75, 23) };
            cboSach = new ComboBox { Location = new Point(95, 57), Size = new Size(260, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            btnMuon = new Button { Text = "Thêm vào phiếu (Mượ&n)", Location = new Point(370, 56), Size = new Size(165, 26), Enabled = false };
            btnMuon.Click += (s, e) => Muon();

            var lblCt = new Label { Text = "Sách trong phiếu:", Location = new Point(15, 95), Size = new Size(150, 23) };

            gridChiTiet = new DataGridView
            {
                Location = new Point(15, 120),
                Size = new Size(715, 360),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            tab.Controls.AddRange(new Control[]
            {
                lblSV, cboSinhVien, btnLapPhieu, lblPhieu,
                lblSach, cboSach, btnMuon, lblCt, gridChiTiet
            });
            return tab;
        }

        private void LapPhieu()
        {
            if (cboSinhVien.SelectedValue == null)
            {
                MessageBox.Show("Hãy chọn sinh viên mượn.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                int maSV = Convert.ToInt32(cboSinhVien.SelectedValue);
                _maPhieuHienTai = DbHelper.LapPhieuMuon(maSV, _maThuThu);
                lblPhieu.Text = "Phiếu số: " + _maPhieuHienTai;
                lblPhieu.ForeColor = Color.Green;
                btnMuon.Enabled = true;
                LoadChiTiet();
                cboSach.Focus();
            }
            catch (Exception ex) { BaoLoi(ex); }
        }

        private void Muon()
        {
            if (_maPhieuHienTai == 0)
            {
                MessageBox.Show("Hãy lập phiếu trước.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (cboSach.SelectedValue == null)
            {
                MessageBox.Show("Hãy chọn sách.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                int maSach = Convert.ToInt32(cboSach.SelectedValue);
                DbHelper.MuonSach(_maPhieuHienTai, maSach);
                LoadChiTiet();
            }
            catch (Exception ex)
            {
                // SP ném lỗi khi sách đã hết (SIGNAL) hoặc khi mượn trùng sách trong cùng phiếu
                MessageBox.Show(ex.Message, "Không thể mượn",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void LoadChiTiet()
        {
            if (_maPhieuHienTai == 0) return;
            gridChiTiet.DataSource = DbHelper.ChiTietPhieu(_maPhieuHienTai);
            if (gridChiTiet.Columns.Contains("MaSach")) gridChiTiet.Columns["MaSach"].HeaderText = "Mã sách";
            if (gridChiTiet.Columns.Contains("TenSach")) gridChiTiet.Columns["TenSach"].HeaderText = "Tên sách";
            if (gridChiTiet.Columns.Contains("TrangThai")) gridChiTiet.Columns["TrangThai"].HeaderText = "Trạng thái";
        }

        // ================= TAB TRẢ =================
        private TabPage BuildTabTra()
        {
            var tab = new TabPage("&Trả sách");

            btnTaiLai = new Button { Text = "Tải &lại danh sách", Location = new Point(15, 15), Size = new Size(140, 28) };
            btnTaiLai.Click += (s, e) => LoadChuaTra();

            btnTra = new Button { Text = "&Trả sách đã chọn", Location = new Point(165, 15), Size = new Size(150, 28) };
            btnTra.Click += (s, e) => Tra();

            var lbl = new Label
            {
                Text = "Danh sách sách đang được mượn (chưa trả):",
                Location = new Point(15, 52),
                Size = new Size(400, 23)
            };

            gridChuaTra = new DataGridView
            {
                Location = new Point(15, 78),
                Size = new Size(715, 400),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                ReadOnly = true,
                AllowUserToAddRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            tab.Controls.AddRange(new Control[] { btnTaiLai, btnTra, lbl, gridChuaTra });
            tab.Enter += (s, e) => LoadChuaTra();   // tự tải khi mở tab
            return tab;
        }

        private void LoadChuaTra()
        {
            gridChuaTra.DataSource = DbHelper.PhieuMuonChuaTra();
            if (gridChuaTra.Columns.Contains("MaPhieu")) gridChuaTra.Columns["MaPhieu"].HeaderText = "Phiếu";
            if (gridChuaTra.Columns.Contains("MaSach")) gridChuaTra.Columns["MaSach"].HeaderText = "Mã sách";
            if (gridChuaTra.Columns.Contains("SinhVien")) gridChuaTra.Columns["SinhVien"].HeaderText = "Sinh viên";
            if (gridChuaTra.Columns.Contains("TenSach")) gridChuaTra.Columns["TenSach"].HeaderText = "Tên sách";
            if (gridChuaTra.Columns.Contains("NgayMuon")) gridChuaTra.Columns["NgayMuon"].HeaderText = "Ngày mượn";
        }

        private void Tra()
        {
            if (gridChuaTra.CurrentRow?.DataBoundItem == null)
            {
                MessageBox.Show("Hãy chọn 1 dòng để trả.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var r = (DataRowView)gridChuaTra.CurrentRow.DataBoundItem;
            int maPhieu = Convert.ToInt32(r["MaPhieu"]);
            int maSach = Convert.ToInt32(r["MaSach"]);
            try
            {
                DbHelper.TraSach(maPhieu, maSach);
                LoadChuaTra();
                MessageBox.Show("Đã trả sách.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { BaoLoi(ex); }
        }

        // ================= CHUNG =================
        private void LoadCombos()
        {
            cboSinhVien.DataSource = DbHelper.LaySinhVien();
            cboSinhVien.DisplayMember = "HoTen";
            cboSinhVien.ValueMember = "MaSV";
            cboSinhVien.SelectedIndex = -1;

            cboSach.DataSource = DbHelper.LayDanhSachSach();
            cboSach.DisplayMember = "TenSach";
            cboSach.ValueMember = "MaSach";
            cboSach.SelectedIndex = -1;
        }

        private void BaoLoi(Exception ex) => MessageBox.Show("Có lỗi:\n" + ex.Message, "Lỗi",
            MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
