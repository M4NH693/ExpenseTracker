using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyChiTieu;
using System.Data;

namespace quanlycitieu.Views
{
    public class NhapVaoView : UserControl
    {
        private Panel card;
        private TextBox txtVi;
        private TextBox txtSoTien;
        private TextBox txtGhiChu;
        private DateTimePicker dtpThoiGian;
        
        private Panel pnlThongTin;
        private RadioButton rbThuNhap;
        private RadioButton rbChiTieu;
        
        private ComboBox cbDanhMuc;
        private Button btnOk;

        public NhapVaoView()
        {
            this.Size = new Size(980, 700);
            this.BackColor = Color.FromArgb(209, 233, 255);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            card = new Panel() { Size = new Size(600, 450), BackColor = Color.White };
            card.Location = new Point((this.Width - card.Width) / 2, (this.Height - card.Height) / 2);
            card.Location = new Point(90, 75);
            card.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, card.Width, card.Height, 20, 20));

            Label lblHeader = new Label() { Text = "Nhập Vào", Font = new Font("Segoe UI", 16, FontStyle.Bold), Location = new Point(250, 20), AutoSize = true, ForeColor = Color.FromArgb(41, 40, 104) };
            Label lblVi = new Label() { Text = "Số dư Ví:", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(150, 70), AutoSize = true };
            txtVi = new TextBox() { Location = new Point(250, 70), Size = new Size(200, 30), Font = new Font("Segoe UI", 12), ReadOnly = true, BackColor = Color.WhiteSmoke, BorderStyle = BorderStyle.None };

            Label lblThongTin = new Label() { Text = "Loại Giao Dịch", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(50, 130), AutoSize = true };
            pnlThongTin = new Panel() { Location = new Point(50, 160), Size = new Size(200, 80), BackColor = Color.WhiteSmoke };
            rbThuNhap = new RadioButton() { Text = "Thu Nhập", Location = new Point(20, 10), Font = new Font("Segoe UI", 11) };
            rbChiTieu = new RadioButton() { Text = "Chi Tiêu", Location = new Point(20, 40), Font = new Font("Segoe UI", 11), Checked = true };
            rbThuNhap.CheckedChanged += Type_Changed;
            rbChiTieu.CheckedChanged += Type_Changed;
            pnlThongTin.Controls.Add(rbThuNhap);
            pnlThongTin.Controls.Add(rbChiTieu);

            Label lblDanhMuc = new Label() { Text = "Danh Mục", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(50, 260), AutoSize = true };
            cbDanhMuc = new ComboBox() { Location = new Point(50, 290), Size = new Size(200, 30), Font = new Font("Segoe UI", 12), DropDownStyle = ComboBoxStyle.DropDownList };

            Label lblSoTien = new Label() { Text = "Số Tiền", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(300, 130), AutoSize = true };
            txtSoTien = new TextBox() { Location = new Point(300, 155), Size = new Size(250, 30), Font = new Font("Segoe UI", 12) };
            txtSoTien.TextChanged += TxtSoTien_TextChanged;
            txtSoTien.KeyPress += TxtSoTien_KeyPress;

            Label lblThoiGian = new Label() { Text = "Thời Gian", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(300, 200), AutoSize = true };
            dtpThoiGian = new DateTimePicker() { Location = new Point(300, 225), Size = new Size(250, 30), Font = new Font("Segoe UI", 12), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };

            Label lblGhiChu = new Label() { Text = "Ghi Chú", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(300, 270), AutoSize = true };
            txtGhiChu = new TextBox() { Location = new Point(300, 295), Size = new Size(250, 30), Font = new Font("Segoe UI", 12) };

            btnOk = new Button() { Text = "LƯU GIAO DỊCH", Location = new Point(200, 380), Size = new Size(200, 45), Font = new Font("Segoe UI", 12, FontStyle.Bold), BackColor = Color.FromArgb(41, 40, 104), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.Click += BtnOk_Click;

            card.Controls.Add(lblHeader); card.Controls.Add(lblVi); card.Controls.Add(txtVi);
            card.Controls.Add(lblThongTin); card.Controls.Add(pnlThongTin);
            card.Controls.Add(lblDanhMuc); card.Controls.Add(cbDanhMuc);
            card.Controls.Add(lblSoTien); card.Controls.Add(txtSoTien);
            card.Controls.Add(lblThoiGian); card.Controls.Add(dtpThoiGian);
            card.Controls.Add(lblGhiChu); card.Controls.Add(txtGhiChu);
            card.Controls.Add(btnOk);

            this.Controls.Add(card);
            this.Resize += (s, e) => { card.Location = new Point((this.Width - card.Width) / 2, (this.Height - card.Height) / 2); };
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        private void TxtSoTien_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void TxtSoTien_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSoTien.Text)) return;
            string raw = txtSoTien.Text.Replace(",", "");
            if (long.TryParse(raw, out long val)) {
                txtSoTien.Text = string.Format("{0:N0}", val);
                txtSoTien.SelectionStart = txtSoTien.Text.Length;
            }
        }

        private void Type_Changed(object sender, EventArgs e)
        {
            LoadCategories(rbThuNhap.Checked ? "Thu Nhập" : "Chi Tiêu");
        }

        public void LoadData()
        {
            try
            {
                var bll = new quanlycitieu.BLL.TransactionBLL();
                decimal thu = bll.GetTotalIncome(AuthForm.CurrentUserId);
                decimal chi = bll.GetTotalExpense(AuthForm.CurrentUserId);
                txtVi.Text = (thu - chi).ToString("N0") + " đ";
                
                LoadCategories(rbThuNhap.Checked ? "Thu Nhập" : "Chi Tiêu");
            } catch { }
        }

        private void LoadCategories(string type)
        {
            try
            {
                var bll = new quanlycitieu.BLL.CategoryBLL();
                var dt = bll.GetCategoriesByType(AuthForm.CurrentUserId, type);
                cbDanhMuc.DataSource = dt;
                cbDanhMuc.DisplayMember = "Name";
                cbDanhMuc.ValueMember = "Id";
            }
            catch { }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (cbDanhMuc.SelectedValue == null) { MessageBox.Show("Vui lòng chọn danh mục!"); return; }
            string rawAmt = txtSoTien.Text.Replace(",", "");
            if (!decimal.TryParse(rawAmt, out decimal amt) || amt <= 0) { MessageBox.Show("Vui lòng nhập số tiền hợp lệ!"); return; }

            string type = rbThuNhap.Checked ? "Thu Nhập" : "Chi Tiêu";

            try
            {
                var bll = new quanlycitieu.BLL.TransactionBLL();
                bll.AddTransaction(new quanlycitieu.DTO.TransactionDTO {
                    UserId = AuthForm.CurrentUserId,
                    Type = type,
                    CategoryId = Convert.ToInt32(cbDanhMuc.SelectedValue),
                    Amount = amt,
                    Date = dtpThoiGian.Value,
                    Description = txtGhiChu.Text
                });
                MessageBox.Show("Đã thêm giao dịch thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSoTien.Clear(); txtGhiChu.Clear();
                LoadData();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}
