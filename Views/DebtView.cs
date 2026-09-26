using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using QuanLyChiTieu;
using quanlycitieu.BLL;
using quanlycitieu.DTO;

namespace quanlycitieu.Views
{
    public class DebtView : UserControl
    {
        private DebtBLL debtBLL = new DebtBLL();

        // Filters
        private ComboBox cbFilterType;
        private ComboBox cbFilterStatus;
        private TextBox txtSearch;

        // Form inputs
        private RadioButton rbChoVay;
        private RadioButton rbDiVay;
        private TextBox txtPersonName;
        private TextBox txtAmount;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpDueDate;
        private TextBox txtNote;
        private Button btnAdd;
        private Button btnUpdate;
        private Button btnClear;

        // Grid & Actions
        private DataGridView dgvDebts;
        private Button btnSettle;
        private Button btnDelete;
        private int selectedDebtId = -1;
        private static readonly Font debtBoldFont = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        private static readonly Font debtRegularFont = new Font("Segoe UI", 9.5f);
        private static readonly Font debtSettledFont = new Font("Segoe UI", 9f);

        public DebtView()
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
                Text = "QUẢN LÝ KHOẢN VAY & NỢ",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(320, 18),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // 2. KHUNG NHẬP LIỆU BÊN TRÁI
            int leftX = 40;
            int inputWidth = 280;

            // Loại: Tôi cho vay / Tôi đi vay
            rbChoVay = new RadioButton() { Text = "Tôi Cho Vay", Checked = true, Location = new Point(leftX, 70), AutoSize = true, Font = new Font("Segoe UI", 10.5f, FontStyle.Bold), ForeColor = Color.FromArgb(46, 125, 50) };
            rbDiVay = new RadioButton() { Text = "Tôi Đi Vay", Location = new Point(leftX + 140, 70), AutoSize = true, Font = new Font("Segoe UI", 10.5f, FontStyle.Bold), ForeColor = Color.FromArgb(220, 38, 38) };
            this.Controls.Add(rbChoVay);
            this.Controls.Add(rbDiVay);

            Label CreateLabel(string text, int y) => new Label()
            {
                Text = text,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40),
                Location = new Point(leftX, y),
                AutoSize = true
            };

            // Người liên quan
            this.Controls.Add(CreateLabel("Người liên quan (*):", 110));
            txtPersonName = new TextBox() { Location = new Point(leftX, 135), Size = new Size(inputWidth, 30), Font = new Font("Segoe UI", 11), BorderStyle = BorderStyle.FixedSingle, PlaceholderText = "Ví dụ: Anh Nam, Bạn Linh..." };
            this.Controls.Add(txtPersonName);

            // Số tiền
            this.Controls.Add(CreateLabel("Số tiền (VNĐ) (*):", 175));
            txtAmount = new TextBox() { Location = new Point(leftX, 200), Size = new Size(inputWidth, 30), Font = new Font("Segoe UI", 11), BorderStyle = BorderStyle.FixedSingle, PlaceholderText = "Ví dụ: 5,000,000" };
            txtAmount.KeyPress += (s, e) => { if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true; };
            txtAmount.TextChanged += (s, e) => {
                if (string.IsNullOrWhiteSpace(txtAmount.Text)) return;
                string raw = txtAmount.Text.Replace(",", "");
                if (long.TryParse(raw, out long val)) {
                    string formatted = string.Format("{0:N0}", val);
                    if (txtAmount.Text != formatted)
                    {
                        txtAmount.Text = formatted;
                        txtAmount.SelectionStart = txtAmount.Text.Length;
                    }
                }
            };
            this.Controls.Add(txtAmount);

            // Ngày bắt đầu
            this.Controls.Add(CreateLabel("Ngày bắt đầu:", 240));
            dtpStartDate = new DateTimePicker() { Location = new Point(leftX, 265), Size = new Size(inputWidth, 30), Font = new Font("Segoe UI", 11), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };
            this.Controls.Add(dtpStartDate);

            // Hạn trả
            this.Controls.Add(CreateLabel("Hạn trả (*):", 305));
            dtpDueDate = new DateTimePicker() { Location = new Point(leftX, 330), Size = new Size(inputWidth, 30), Font = new Font("Segoe UI", 11), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy" };
            dtpDueDate.Value = DateTime.Today.AddMonths(1);
            this.Controls.Add(dtpDueDate);

            // Ghi chú
            this.Controls.Add(CreateLabel("Ghi chú:", 370));
            txtNote = new TextBox() { Location = new Point(leftX, 395), Size = new Size(inputWidth, 30), Font = new Font("Segoe UI", 11), BorderStyle = BorderStyle.FixedSingle, PlaceholderText = "Mục đích vay, hẹn trả..." };
            this.Controls.Add(txtNote);

            // Nút bấm form (Theme White Flat giống Danh mục)
            btnAdd = new Button() { Text = "THÊM", Location = new Point(leftX, 445), Size = new Size(85, 34), Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), BackColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnUpdate = new Button() { Text = "SỬA", Location = new Point(leftX + 95, 445), Size = new Size(85, 34), Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), BackColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Enabled = false };
            btnClear = new Button() { Text = "LÀM MỚI", Location = new Point(leftX + 190, 445), Size = new Size(90, 34), Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), BackColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnClear.Click += (s, e) => ClearForm();

            this.Controls.Add(btnAdd);
            this.Controls.Add(btnUpdate);
            this.Controls.Add(btnClear);

            // 3. KHU VỰC BÊN PHẢI (BỘ LỌC + GRID + NÚT TẤT TOÁN)
            int rightX = 360;

            // Bộ lọc: Loại
            Label lblFType = new Label() { Text = "Loại:", Location = new Point(rightX, 70), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            cbFilterType = new ComboBox() { Location = new Point(rightX + 45, 67), Size = new Size(95, 28), Font = new Font("Segoe UI", 9.5f), DropDownStyle = ComboBoxStyle.DropDownList };
            cbFilterType.Items.AddRange(new object[] { "Tất cả", "Cho vay", "Đi vay" });
            cbFilterType.SelectedIndex = 0;
            cbFilterType.SelectedIndexChanged += (s, e) => LoadData();

            // Bộ lọc: Trạng thái
            Label lblFStatus = new Label() { Text = "Trạng thái:", Location = new Point(rightX + 150, 70), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            cbFilterStatus = new ComboBox() { Location = new Point(rightX + 230, 67), Size = new Size(105, 28), Font = new Font("Segoe UI", 9.5f), DropDownStyle = ComboBoxStyle.DropDownList };
            cbFilterStatus.Items.AddRange(new object[] { "Tất cả", "Chưa trả", "Đã tất toán" });
            cbFilterStatus.SelectedIndex = 0;
            cbFilterStatus.SelectedIndexChanged += (s, e) => LoadData();

            // Tìm kiếm
            Label lblSearch = new Label() { Text = "Tìm:", Location = new Point(rightX + 345, 70), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            txtSearch = new TextBox() { Location = new Point(rightX + 380, 67), Size = new Size(200, 28), Font = new Font("Segoe UI", 9.5f), BorderStyle = BorderStyle.FixedSingle, PlaceholderText = "Tên người / ghi chú..." };
            txtSearch.TextChanged += (s, e) => LoadData();

            this.Controls.Add(lblFType);
            this.Controls.Add(cbFilterType);
            this.Controls.Add(lblFStatus);
            this.Controls.Add(cbFilterStatus);
            this.Controls.Add(lblSearch);
            this.Controls.Add(txtSearch);

            // Bảng danh sách DataGridView
            dgvDebts = new DataGridView()
            {
                Location = new Point(rightX, 105),
                Size = new Size(580, 480),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackgroundColor = Color.White,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                RowTemplate = { Height = 36 }
            };
            dgvDebts.SelectionChanged += DgvDebts_SelectionChanged;
            dgvDebts.CellFormatting += DgvDebts_CellFormatting;
            this.Controls.Add(dgvDebts);

            // Nút Xác Nhận Tất Toán (Dưới Grid)
            btnSettle = new Button()
            {
                Text = "✔ XÁC NHẬN TẤT TOÁN",
                Location = new Point(rightX, 600),
                Size = new Size(220, 36),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(46, 125, 50),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnSettle.Click += BtnSettle_Click;
            this.Controls.Add(btnSettle);

            // Nút Xóa Khoản Nợ (Dưới Grid bên phải)
            btnDelete = new Button()
            {
                Text = "XÓA KHOẢN NỢ",
                Location = new Point(rightX + 430, 600),
                Size = new Size(150, 36),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(220, 38, 38),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(btnDelete);
        }

        public void LoadData()
        {
            try
            {
                int? dtype = cbFilterType.SelectedIndex == 1 ? 1 : (cbFilterType.SelectedIndex == 2 ? 2 : null);
                int? status = cbFilterStatus.SelectedIndex == 1 ? 0 : (cbFilterStatus.SelectedIndex == 2 ? 1 : null);
                string search = txtSearch.Text.Trim();

                DataTable dt = debtBLL.GetDebts(AuthForm.CurrentUserId, dtype, status, search);

                dgvDebts.SelectionChanged -= DgvDebts_SelectionChanged;
                dgvDebts.DataSource = dt;

                // Ẩn các cột kỹ thuật
                if (dgvDebts.Columns.Contains("Id")) dgvDebts.Columns["Id"].Visible = false;
                if (dgvDebts.Columns.Contains("DebtType")) dgvDebts.Columns["DebtType"].Visible = false;
                if (dgvDebts.Columns.Contains("Status")) dgvDebts.Columns["Status"].Visible = false;
                if (dgvDebts.Columns.Contains("TransactionId")) dgvDebts.Columns["TransactionId"].Visible = false;

                // Tên cột hiển thị
                if (dgvDebts.Columns.Contains("Loai")) dgvDebts.Columns["Loai"].HeaderText = "Phân Loại";
                if (dgvDebts.Columns.Contains("NguoiLienQuan")) dgvDebts.Columns["NguoiLienQuan"].HeaderText = "Người Liên Quan";
                if (dgvDebts.Columns.Contains("SoTien"))
                {
                    dgvDebts.Columns["SoTien"].HeaderText = "Số Tiền";
                    dgvDebts.Columns["SoTien"].DefaultCellStyle.Format = "N0";
                    dgvDebts.Columns["SoTien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
                if (dgvDebts.Columns.Contains("NgayVay"))
                {
                    dgvDebts.Columns["NgayVay"].HeaderText = "Ngày Vay";
                    dgvDebts.Columns["NgayVay"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    dgvDebts.Columns["NgayVay"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                if (dgvDebts.Columns.Contains("HanTra"))
                {
                    dgvDebts.Columns["HanTra"].HeaderText = "Hạn Trả";
                    dgvDebts.Columns["HanTra"].DefaultCellStyle.Format = "dd/MM/yyyy";
                    dgvDebts.Columns["HanTra"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                if (dgvDebts.Columns.Contains("TrangThai"))
                {
                    dgvDebts.Columns["TrangThai"].HeaderText = "Trạng Thái";
                    dgvDebts.Columns["TrangThai"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
                if (dgvDebts.Columns.Contains("GhiChu")) dgvDebts.Columns["GhiChu"].HeaderText = "Ghi Chú";

                dgvDebts.ClearSelection();
                ClearForm();
                dgvDebts.SelectionChanged += DgvDebts_SelectionChanged;
            }
            catch { }
        }

        private static DateTime ToDateTime(object? val)
        {
            if (val == null || val == DBNull.Value) return DateTime.Today;
            if (val is DateTime dt) return dt;
            if (val is DateOnly d) return d.ToDateTime(TimeOnly.MinValue);
            if (DateTime.TryParse(val.ToString(), out DateTime parsed)) return parsed;
            return DateTime.Today;
        }

        private void DgvDebts_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvDebts.Rows.Count) return;

            var row = dgvDebts.Rows[e.RowIndex];
            int status = Convert.ToInt32(row.Cells["Status"].Value);
            DateTime dueDate = ToDateTime(row.Cells["HanTra"].Value);

            // Nổi bật hàng quá hạn chưa trả
            if (status == 0 && dueDate.Date < DateTime.Today)
            {
                e.CellStyle.BackColor = Color.FromArgb(254, 226, 226); // Nền đỏ nhạt
                e.CellStyle.ForeColor = Color.FromArgb(185, 28, 28);   // Chữ đỏ đậm
                e.CellStyle.Font = debtBoldFont;
            }
            else if (status == 1)
            {
                // Đã tất toán
                e.CellStyle.BackColor = Color.FromArgb(245, 245, 245);
                e.CellStyle.ForeColor = Color.FromArgb(100, 116, 139);
                e.CellStyle.Font = debtSettledFont;
            }
            else
            {
                e.CellStyle.BackColor = Color.White;
                e.CellStyle.ForeColor = Color.FromArgb(30, 30, 30);
                e.CellStyle.Font = debtRegularFont;
            }
        }

        private void DgvDebts_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvDebts.SelectedRows.Count > 0)
            {
                var row = dgvDebts.SelectedRows[0];
                selectedDebtId = Convert.ToInt32(row.Cells["Id"].Value);
                int dtype = Convert.ToInt32(row.Cells["DebtType"].Value);
                int status = Convert.ToInt32(row.Cells["Status"].Value);

                if (dtype == 1) rbChoVay.Checked = true; else rbDiVay.Checked = true;
                txtPersonName.Text = row.Cells["NguoiLienQuan"].Value?.ToString();
                decimal amt = Convert.ToDecimal(row.Cells["SoTien"].Value);
                txtAmount.Text = string.Format("{0:N0}", amt);
                dtpStartDate.Value = ToDateTime(row.Cells["NgayVay"].Value);
                dtpDueDate.Value = ToDateTime(row.Cells["HanTra"].Value);
                txtNote.Text = row.Cells["GhiChu"].Value?.ToString();

                btnSettle.Enabled = (status == 0);
                btnUpdate.Enabled = (status == 0);
                btnDelete.Enabled = true;
            }
        }

        private void ClearForm()
        {
            selectedDebtId = -1;
            rbChoVay.Checked = true;
            txtPersonName.Clear();
            txtAmount.Clear();
            dtpStartDate.Value = DateTime.Today;
            dtpDueDate.Value = DateTime.Today.AddMonths(1);
            txtNote.Clear();

            btnSettle.Enabled = false;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPersonName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên người liên quan!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPersonName.Focus();
                return;
            }

            string raw = txtAmount.Text.Replace(",", "");
            if (!decimal.TryParse(raw, out decimal amt) || amt <= 0)
            {
                MessageBox.Show("Vui lòng nhập số tiền hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAmount.Focus();
                return;
            }

            if (dtpDueDate.Value.Date < dtpStartDate.Value.Date)
            {
                MessageBox.Show("Hạn trả không thể trước ngày bắt đầu!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int dtype = rbChoVay.Checked ? 1 : 2;
                debtBLL.AddDebt(AuthForm.CurrentUserId, dtype, txtPersonName.Text, amt, dtpStartDate.Value, dtpDueDate.Value, txtNote.Text);
                MessageBox.Show("Đã thêm khoản vay/nợ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUpdate_Click(object? sender, EventArgs e)
        {
            if (selectedDebtId <= 0) return;

            if (string.IsNullOrWhiteSpace(txtPersonName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên người liên quan!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string raw = txtAmount.Text.Replace(",", "");
            if (!decimal.TryParse(raw, out decimal amt) || amt <= 0)
            {
                MessageBox.Show("Vui lòng nhập số tiền hợp lệ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                int dtype = rbChoVay.Checked ? 1 : 2;
                debtBLL.UpdateDebt(AuthForm.CurrentUserId, selectedDebtId, dtype, txtPersonName.Text, amt, dtpStartDate.Value, dtpDueDate.Value, txtNote.Text);
                MessageBox.Show("Cập nhật khoản nợ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (selectedDebtId <= 0) return;

            if (MessageBox.Show(this, "Bạn có chắc chắn muốn xóa bản ghi khoản vay/nợ này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    debtBLL.DeleteDebt(AuthForm.CurrentUserId, selectedDebtId);
                    MessageBox.Show(this, "Đã xóa bản ghi thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnSettle_Click(object? sender, EventArgs e)
        {
            if (selectedDebtId <= 0) return;

            string person = txtPersonName.Text;
            string amtStr = txtAmount.Text;
            bool isChoVay = rbChoVay.Checked;

            string confirmMsg = isChoVay
                ? $"Xác nhận đã THU ĐỦ số tiền {amtStr} đ từ {person}?\n\n➔ Hệ thống sẽ tự động tạo một khoản 'Thu Nhập' ({amtStr} đ) vào sổ giao dịch."
                : $"Xác nhận đã HOÀN TRẢ số tiền {amtStr} đ cho {person}?\n\n➔ Hệ thống sẽ tự động trừ một khoản 'Chi Tiêu' ({amtStr} đ) vào sổ giao dịch.";

            var result = MessageBox.Show(this, confirmMsg, "Xác nhận tất toán khoản nợ", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    string msg = debtBLL.SettleDebt(AuthForm.CurrentUserId, selectedDebtId);
                    MessageBox.Show(this, msg, "Tất toán thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Lỗi tất toán", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
