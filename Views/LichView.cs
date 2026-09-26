using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using QuanLyChiTieu;

namespace quanlycitieu.Views
{
    public class LichView : UserControl
    {
        private DataGridView dgv;
        private TextBox txtLoai, txtGhiChu, txtSoTien, txtDanhMuc;
        private DateTimePicker dtpThoiGian;
        private Button btnUpdate, btnDelete, btnClear;
        
        // Filter controls
        private TextBox txtSearch;
        private ComboBox cbFilterLoai;

        private int selectedId = -1;

        public LichView()
        {
            this.Size = new Size(980, 700);
            this.BackColor = Color.FromArgb(209, 233, 255);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Label lblTitle = new Label() { Text = "BIẾN ĐỘNG SỐ DƯ", Font = new Font("Segoe UI", 16, FontStyle.Bold), Location = new Point(300, 10), AutoSize = true };
            this.Controls.Add(lblTitle);

            // Left panel: Details
            Label lblChiTiet = new Label() { Text = "Chi Tiết", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(20, 50), AutoSize = true };
            this.Controls.Add(lblChiTiet);

            int y = 90; int gap = 50;
            Label AddLabel(string text, int ypos) { var l = new Label() { Text = text, Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(40, ypos + 5), AutoSize = true }; this.Controls.Add(l); return l; }
            TextBox AddTextBox(int ypos) { var t = new TextBox() { Location = new Point(150, ypos), Size = new Size(220, 25), Font = new Font("Segoe UI", 10) }; this.Controls.Add(t); return t; }

            AddLabel("Loại", y); txtLoai = AddTextBox(y);
            AddLabel("Thời Gian", y += gap); 
            dtpThoiGian = new DateTimePicker() { Location = new Point(150, y), Size = new Size(220, 25), Font = new Font("Segoe UI", 10), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" }; this.Controls.Add(dtpThoiGian);
            AddLabel("Ghi Chú", y += gap); txtGhiChu = AddTextBox(y);
            AddLabel("Số Tiền", y += gap); txtSoTien = AddTextBox(y);
            AddLabel("Danh Mục", y += gap); txtDanhMuc = AddTextBox(y);

            txtLoai.ReadOnly = true; txtDanhMuc.ReadOnly = true;

            btnUpdate = new Button() { Text = "CẬP NHẬT", Location = new Point(40, y += 60), Size = new Size(100, 30), Font = new Font("Segoe UI", 9, FontStyle.Bold), BackColor = Color.White, FlatStyle = FlatStyle.Flat, Enabled = false };
            btnDelete = new Button() { Text = "XÓA", Location = new Point(150, y), Size = new Size(100, 30), Font = new Font("Segoe UI", 9, FontStyle.Bold), BackColor = Color.White, FlatStyle = FlatStyle.Flat, Enabled = false };
            btnClear = new Button() { Text = "LÀM MỚI", Location = new Point(260, y), Size = new Size(100, 30), Font = new Font("Segoe UI", 9, FontStyle.Bold), BackColor = Color.White, FlatStyle = FlatStyle.Flat };

            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += (s, e) => Clear();

            this.Controls.Add(btnUpdate); this.Controls.Add(btnDelete); this.Controls.Add(btnClear);

            // Right panel: Filters and Grid
            Label lblSearch = new Label() { Text = "Tìm kiếm:", Location = new Point(420, 55), AutoSize = true, Font = new Font("Segoe UI", 10) };
            txtSearch = new TextBox() { Location = new Point(500, 52), Size = new Size(160, 25), Font = new Font("Segoe UI", 10) };
            txtSearch.PlaceholderText = "Ghi chú, Tên DM...";
            txtSearch.TextChanged += (s, e) => LoadData();

            Label lblFilter = new Label() { Text = "Lọc:", Location = new Point(680, 55), AutoSize = true, Font = new Font("Segoe UI", 10) };
            cbFilterLoai = new ComboBox() { Location = new Point(720, 52), Size = new Size(110, 25), Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            cbFilterLoai.Items.AddRange(new object[] { "Tất cả", "Thu Nhập", "Chi Tiêu" });
            cbFilterLoai.SelectedIndex = 0;
            cbFilterLoai.SelectedIndexChanged += (s, e) => LoadData();

            this.Controls.Add(lblSearch); this.Controls.Add(txtSearch);
            this.Controls.Add(lblFilter); this.Controls.Add(cbFilterLoai);

            dgv = new DataGridView()
            {
                Location = new Point(420, 90),
                Size = new Size(530, 550),
                BackgroundColor = Color.White,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                RowTemplate = { Height = 34 }
            };
            dgv.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgv.SelectionChanged += Dgv_SelectionChanged;
            this.Controls.Add(dgv);
        }

        public void LoadData()
        {
            try {
                var transBLL = new quanlycitieu.BLL.TransactionBLL();
                var dt = transBLL.GetTransactionsByFilter(AuthForm.CurrentUserId, cbFilterLoai.SelectedIndex, txtSearch.Text);
                
                dgv.DataSource = dt;
                if (dgv.Columns.Contains("Mã")) dgv.Columns["Mã"].Visible = false;
                if (dgv.Columns.Contains("GhiChu")) dgv.Columns["GhiChu"].Visible = false;
                if (dgv.Columns.Contains("SoTien")) dgv.Columns["SoTien"].Visible = false;

                if (dgv.Columns.Contains("Loại"))
                {
                    dgv.Columns["Loại"].Visible = true;
                    dgv.Columns["Loại"].HeaderText = "Loại";
                    dgv.Columns["Loại"].DisplayIndex = 0;
                    dgv.Columns["Loại"].FillWeight = 24;
                    dgv.Columns["Loại"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                if (dgv.Columns.Contains("DanhMuc"))
                {
                    dgv.Columns["DanhMuc"].Visible = true;
                    dgv.Columns["DanhMuc"].HeaderText = "Danh Mục";
                    dgv.Columns["DanhMuc"].DisplayIndex = 1;
                    dgv.Columns["DanhMuc"].FillWeight = 40;
                }

                if (dgv.Columns.Contains("Thời Gian"))
                {
                    dgv.Columns["Thời Gian"].Visible = true;
                    dgv.Columns["Thời Gian"].HeaderText = "Thời Gian";
                    dgv.Columns["Thời Gian"].DisplayIndex = 2;
                    dgv.Columns["Thời Gian"].FillWeight = 36;
                    dgv.Columns["Thời Gian"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    dgv.Columns["Thời Gian"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                // Tô màu chữ theo Loại Thu Nhập / Chi Tiêu
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    string type = row.Cells["Loại"].Value?.ToString() ?? "";
                    if (type == "Thu Nhập")
                    {
                        row.Cells["Loại"].Style.ForeColor = Color.FromArgb(46, 125, 50);
                        row.Cells["Loại"].Style.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                    }
                    else
                    {
                        row.Cells["Loại"].Style.ForeColor = Color.FromArgb(220, 38, 38);
                        row.Cells["Loại"].Style.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                    }
                }

                Clear();
            } catch { }
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count > 0)
            {
                var row = dgv.SelectedRows[0];
                selectedId = Convert.ToInt32(row.Cells["Mã"].Value);
                txtLoai.Text = row.Cells["Loại"].Value?.ToString();
                if (DateTime.TryParse(row.Cells["Thời Gian"].Value?.ToString(), out DateTime dt)) dtpThoiGian.Value = dt;
                txtGhiChu.Text = row.Cells["GhiChu"].Value?.ToString();

                string rawAmt = row.Cells["SoTien"].Value?.ToString() ?? "0";
                if (decimal.TryParse(rawAmt, out decimal st))
                    txtSoTien.Text = string.Format("{0:N0}", st);
                else
                    txtSoTien.Text = rawAmt;

                txtDanhMuc.Text = row.Cells["DanhMuc"].Value?.ToString();
                
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
            }
        }

        private void Clear()
        {
            selectedId = -1;
            txtLoai.Clear(); txtGhiChu.Clear(); txtSoTien.Clear(); txtDanhMuc.Clear();
            dtpThoiGian.Value = DateTime.Now;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            string cleanAmt = txtSoTien.Text.Replace(",", "");
            if (selectedId == -1 || !decimal.TryParse(cleanAmt, out decimal amt)) return;
            try {
                var bll = new quanlycitieu.BLL.TransactionBLL();
                bll.UpdateTransaction(new quanlycitieu.DTO.TransactionDTO {
                    Id = selectedId,
                    UserId = AuthForm.CurrentUserId,
                    Amount = amt,
                    Date = dtpThoiGian.Value,
                    Description = txtGhiChu.Text,
                    Type = txtLoai.Text
                });
                LoadData();
                MessageBox.Show("Cập nhật thành công!");
            } catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (selectedId == -1) return;
            if (MessageBox.Show("Bạn có chắc chắn muốn xóa giao dịch này không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try {
                    var bll = new quanlycitieu.BLL.TransactionBLL();
                    bll.DeleteTransaction(AuthForm.CurrentUserId, selectedId);
                    LoadData();
                    MessageBox.Show("Xóa thành công!");
                } catch (Exception ex) { MessageBox.Show("Lỗi: " + ex.Message); }
            }
        }
    }
}
