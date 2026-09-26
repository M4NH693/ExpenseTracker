using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyChiTieu;
using quanlycitieu.BLL;
using quanlycitieu.DTO;

namespace quanlycitieu.Views
{
    public class BudgetView : UserControl
    {
        private BudgetBLL budgetBLL = new BudgetBLL();
        private CategoryBLL categoryBLL = new CategoryBLL();

        // Top Filter & Summary
        private ComboBox cbMonth;
        private NumericUpDown numYear;
        private Label lblTotalBudget;
        private Label lblTotalSpent;
        private Label lblTotalRemaining;

        // Input Form
        private ComboBox cbCategory;
        private TextBox txtAmountLimit;
        private Button btnSave;
        private Button btnDelete;
        private int selectedBudgetId = -1;
        private static readonly Font budgetBoldFont = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        private static readonly Font budgetRegularFont = new Font("Segoe UI", 9f);

        // Grid
        private DataGridView dgvBudgets;

        public BudgetView()
        {
            this.Size = new Size(980, 700);
            this.BackColor = Color.FromArgb(209, 233, 255); // Nền xanh giống Danh Mục
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // 1. TIÊU ĐỀ
            Label lblTitle = new Label()
            {
                Text = "QUẢN LÝ NGÂN SÁCH CHI TIÊU",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(320, 18),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // 2. CHỌN KỲ (THÁNG / NĂM)
            int leftX = 40;
            int inputWidth = 280;

            Label lblChonThang = new Label() { Text = "Tháng:", Location = new Point(leftX, 70), AutoSize = true, Font = new Font("Segoe UI", 10.5f, FontStyle.Bold) };
            cbMonth = new ComboBox() { Location = new Point(leftX + 60, 67), Size = new Size(75, 28), Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            for (int m = 1; m <= 12; m++) cbMonth.Items.Add(m);
            cbMonth.SelectedItem = DateTime.Today.Month;
            cbMonth.SelectedIndexChanged += (s, e) => LoadData();

            Label lblChonNam = new Label() { Text = "Năm:", Location = new Point(leftX + 150, 70), AutoSize = true, Font = new Font("Segoe UI", 10.5f, FontStyle.Bold) };
            numYear = new NumericUpDown() { Location = new Point(leftX + 195, 67), Size = new Size(85, 28), Font = new Font("Segoe UI", 10), Minimum = 2000, Maximum = 2100, Value = DateTime.Today.Year };
            numYear.ValueChanged += (s, e) => LoadData();

            this.Controls.Add(lblChonThang);
            this.Controls.Add(cbMonth);
            this.Controls.Add(lblChonNam);
            this.Controls.Add(numYear);

            // 3. KHUNG NHẬP LIỆU
            Label CreateLabel(string text, int y) => new Label()
            {
                Text = text,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                Location = new Point(leftX, y),
                AutoSize = true
            };

            // Danh mục
            this.Controls.Add(CreateLabel("Danh mục chi tiêu (*):", 115));
            cbCategory = new ComboBox()
            {
                Location = new Point(leftX, 140),
                Size = new Size(inputWidth, 30),
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(cbCategory);

            // Hạn mức
            this.Controls.Add(CreateLabel("Hạn mức tối đa (VNĐ) (*):", 185));
            txtAmountLimit = new TextBox()
            {
                Location = new Point(leftX, 210),
                Size = new Size(inputWidth, 30),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                PlaceholderText = "Ví dụ: 3,000,000"
            };
            txtAmountLimit.KeyPress += (s, e) => {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
            };
            txtAmountLimit.TextChanged += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtAmountLimit.Text)) return;
                string raw = txtAmountLimit.Text.Replace(",", "");
                if (long.TryParse(raw, out long val)) {
                    string formatted = string.Format("{0:N0}", val);
                    if (txtAmountLimit.Text != formatted)
                    {
                        txtAmountLimit.Text = formatted;
                        txtAmountLimit.SelectionStart = txtAmountLimit.Text.Length;
                    }
                }
            };
            this.Controls.Add(txtAmountLimit);

            // Nút Lưu và Xóa (Theme White Flat)
            btnSave = new Button()
            {
                Text = "LƯU HẠN MỨC",
                Location = new Point(leftX, 260),
                Size = new Size(135, 36),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnDelete = new Button()
            {
                Text = "XÓA HẠN MỨC",
                Location = new Point(leftX + 145, 260),
                Size = new Size(135, 36),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(220, 38, 38),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);

            // 4. CÁC THẺ THỐNG KÊ TỔNG QUAN (Phía dưới form nhập liệu)
            Panel CreateSummaryBox(string title, Color titleColor, int top, out Label valLabel)
            {
                Panel box = new Panel()
                {
                    Location = new Point(leftX, top),
                    Size = new Size(inputWidth, 65),
                    BackColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };
                Label lTitle = new Label() { Text = title, ForeColor = titleColor, Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(12, 8), AutoSize = true };
                valLabel = new Label() { Text = "0 đ", ForeColor = Color.FromArgb(30, 30, 30), Font = new Font("Segoe UI", 12.5f, FontStyle.Bold), Location = new Point(12, 30), AutoSize = true };
                box.Controls.Add(lTitle);
                box.Controls.Add(valLabel);
                return box;
            }

            this.Controls.Add(CreateSummaryBox("TỔNG NGÂN SÁCH THÁNG", Color.FromArgb(41, 40, 104), 320, out lblTotalBudget));
            this.Controls.Add(CreateSummaryBox("TỔNG ĐÃ CHI TIÊU", Color.FromArgb(220, 38, 38), 395, out lblTotalSpent));
            this.Controls.Add(CreateSummaryBox("NGÂN SÁCH CÒN LẠI", Color.FromArgb(46, 125, 50), 470, out lblTotalRemaining));

            // 5. BẢNG DANH SÁCH NGÂN SÁCH (Bên phải)
            int rightX = 360;
            dgvBudgets = new DataGridView()
            {
                Location = new Point(rightX, 67),
                Size = new Size(580, 560),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackgroundColor = Color.White,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                RowTemplate = { Height = 36 }
            };
            dgvBudgets.SelectionChanged += DgvBudgets_SelectionChanged;
            dgvBudgets.CellFormatting += DgvBudgets_CellFormatting;
            this.Controls.Add(dgvBudgets);
        }

        public void LoadData()
        {
            if (cbMonth.SelectedItem == null) return;
            int month = Convert.ToInt32(cbMonth.SelectedItem);
            int year = Convert.ToInt32(numYear.Value);

            // Tải danh mục loại Chi Tiêu
            try
            {
                DataTable catDt = categoryBLL.GetCategoriesByType(AuthForm.CurrentUserId, "Chi Tiêu");
                cbCategory.DataSource = catDt;
                cbCategory.DisplayMember = "Name";
                cbCategory.ValueMember = "Id";
            }
            catch { }

            // Tải danh sách ngân sách tháng
            try
            {
                DataTable dt = budgetBLL.GetBudgets(AuthForm.CurrentUserId, month, year);

                dgvBudgets.SelectionChanged -= DgvBudgets_SelectionChanged;
                dgvBudgets.DataSource = dt;

                // Ẩn cột kỹ thuật
                if (dgvBudgets.Columns.Contains("Id")) dgvBudgets.Columns["Id"].Visible = false;
                if (dgvBudgets.Columns.Contains("CategoryId")) dgvBudgets.Columns["CategoryId"].Visible = false;
                if (dgvBudgets.Columns.Contains("Month")) dgvBudgets.Columns["Month"].Visible = false;
                if (dgvBudgets.Columns.Contains("Year")) dgvBudgets.Columns["Year"].Visible = false;

                // Tên cột hiển thị
                if (dgvBudgets.Columns.Contains("DanhMuc")) dgvBudgets.Columns["DanhMuc"].HeaderText = "Danh Mục";
                if (dgvBudgets.Columns.Contains("HanMuc"))
                {
                    dgvBudgets.Columns["HanMuc"].HeaderText = "Hạn Mức";
                    dgvBudgets.Columns["HanMuc"].DefaultCellStyle.Format = "N0";
                    dgvBudgets.Columns["HanMuc"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                if (dgvBudgets.Columns.Contains("DaChi"))
                {
                    dgvBudgets.Columns["DaChi"].HeaderText = "Đã Chi";
                    dgvBudgets.Columns["DaChi"].DefaultCellStyle.Format = "N0";
                    dgvBudgets.Columns["DaChi"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                if (dgvBudgets.Columns.Contains("ConLai"))
                {
                    dgvBudgets.Columns["ConLai"].HeaderText = "Còn Lại";
                    dgvBudgets.Columns["ConLai"].DefaultCellStyle.Format = "N0";
                    dgvBudgets.Columns["ConLai"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                if (dgvBudgets.Columns.Contains("PhanTram"))
                {
                    dgvBudgets.Columns["PhanTram"].HeaderText = "% Đã Dùng";
                    dgvBudgets.Columns["PhanTram"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                // Tính toán thống kê tổng quan
                decimal totalBudget = 0;
                decimal totalSpent = 0;
                foreach (DataRow r in dt.Rows)
                {
                    totalBudget += Convert.ToDecimal(r["HanMuc"]);
                    totalSpent += Convert.ToDecimal(r["DaChi"]);
                }

                lblTotalBudget.Text = totalBudget.ToString("N0") + " đ";
                lblTotalSpent.Text = totalSpent.ToString("N0") + " đ";
                decimal rem = totalBudget - totalSpent;
                lblTotalRemaining.Text = rem.ToString("N0") + " đ";
                lblTotalRemaining.ForeColor = rem >= 0 ? Color.FromArgb(46, 125, 50) : Color.FromArgb(220, 38, 38);

                dgvBudgets.ClearSelection();
                ClearForm();
                dgvBudgets.SelectionChanged += DgvBudgets_SelectionChanged;
            }
            catch { }
        }

        private void DgvBudgets_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvBudgets.Rows.Count) return;

            var row = dgvBudgets.Rows[e.RowIndex];
            if (row.Cells["PhanTram"].Value != null && double.TryParse(row.Cells["PhanTram"].Value.ToString(), out double pct))
            {
                if (pct > 100)
                {
                    // Vượt 100%: Nền đỏ nhạt, chữ đỏ đậm
                    e.CellStyle.BackColor = Color.FromArgb(254, 226, 226);
                    e.CellStyle.ForeColor = Color.FromArgb(185, 28, 28);
                    e.CellStyle.Font = budgetBoldFont;
                }
                else if (pct >= 80)
                {
                    // Từ 80% - 100%: Nền vàng cam nhạt
                    e.CellStyle.BackColor = Color.FromArgb(255, 251, 235);
                    e.CellStyle.ForeColor = Color.FromArgb(180, 83, 9);
                    e.CellStyle.Font = budgetBoldFont;
                }
                else
                {
                    e.CellStyle.BackColor = Color.White;
                    e.CellStyle.ForeColor = Color.FromArgb(30, 30, 30);
                    e.CellStyle.Font = budgetRegularFont;
                }

                if (dgvBudgets.Columns[e.ColumnIndex].Name == "PhanTram" && e.Value != null)
                {
                    e.Value = $"{pct:F1}%";
                    e.FormattingApplied = true;
                }
            }
        }

        private void DgvBudgets_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvBudgets.SelectedRows.Count > 0)
            {
                var row = dgvBudgets.SelectedRows[0];
                selectedBudgetId = Convert.ToInt32(row.Cells["Id"].Value);
                cbCategory.SelectedValue = Convert.ToInt32(row.Cells["CategoryId"].Value);
                decimal limit = Convert.ToDecimal(row.Cells["HanMuc"].Value);
                txtAmountLimit.Text = string.Format("{0:N0}", limit);
                btnDelete.Enabled = true;
            }
        }

        private void ClearForm()
        {
            selectedBudgetId = -1;
            txtAmountLimit.Clear();
            btnDelete.Enabled = false;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (cbCategory.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn danh mục chi tiêu!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string raw = txtAmountLimit.Text.Replace(",", "");
            if (!decimal.TryParse(raw, out decimal limit) || limit <= 0)
            {
                MessageBox.Show("Vui lòng nhập hạn mức hợp lệ (lớn hơn 0đ)!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAmountLimit.Focus();
                return;
            }

            int catId = Convert.ToInt32(cbCategory.SelectedValue);
            int month = Convert.ToInt32(cbMonth.SelectedItem);
            int year = Convert.ToInt32(numYear.Value);

            try
            {
                budgetBLL.SetBudget(AuthForm.CurrentUserId, catId, limit, month, year);
                MessageBox.Show(this, "Đã lưu hạn mức ngân sách thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (selectedBudgetId <= 0) return;

            if (MessageBox.Show(this, "Bạn có chắc chắn muốn xóa hạn mức ngân sách này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    budgetBLL.DeleteBudget(AuthForm.CurrentUserId, selectedBudgetId);
                    MessageBox.Show(this, "Đã xóa hạn mức thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
