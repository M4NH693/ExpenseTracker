using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyChiTieu;
using System.Data;

namespace quanlycitieu.Views
{
    public class CategoriesView : UserControl
    {
        private DataGridView dgvCategories;
        private TextBox txtName;
        private ComboBox cbType;
        private Button btnAdd, btnUpdate, btnDelete, btnClear;
        private int selectedId = -1;

        public CategoriesView()
        {
            this.Size = new Size(980, 700);
            this.BackColor = Color.FromArgb(209, 233, 255);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Label lblTitle = new Label() { Text = "QUẢN LÝ DANH MỤC", Font = new Font("Segoe UI", 16, FontStyle.Bold), Location = new Point(300, 20), AutoSize = true };
            this.Controls.Add(lblTitle);

            Label lblType = new Label() { Text = "Loại:", Font = new Font("Segoe UI", 12), Location = new Point(40, 100), AutoSize = true };
            cbType = new ComboBox() { Location = new Point(120, 100), Size = new Size(200, 30), Font = new Font("Segoe UI", 12), DropDownStyle = ComboBoxStyle.DropDownList };
            cbType.Items.AddRange(new object[] { "Thu Nhập", "Chi Tiêu" });
            this.Controls.Add(lblType); this.Controls.Add(cbType);

            Label lblName = new Label() { Text = "Tên DM:", Font = new Font("Segoe UI", 12), Location = new Point(40, 150), AutoSize = true };
            txtName = new TextBox() { Location = new Point(120, 150), Size = new Size(200, 30), Font = new Font("Segoe UI", 12) };
            this.Controls.Add(lblName); this.Controls.Add(txtName);

            btnAdd = new Button() { Text = "THÊM", Location = new Point(40, 200), Size = new Size(70, 30), Font = new Font("Segoe UI", 9, FontStyle.Bold), BackColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnUpdate = new Button() { Text = "SỬA", Location = new Point(120, 200), Size = new Size(70, 30), Font = new Font("Segoe UI", 9, FontStyle.Bold), BackColor = Color.White, FlatStyle = FlatStyle.Flat, Enabled = false };
            btnDelete = new Button() { Text = "XÓA", Location = new Point(200, 200), Size = new Size(70, 30), Font = new Font("Segoe UI", 9, FontStyle.Bold), BackColor = Color.White, FlatStyle = FlatStyle.Flat, Enabled = false };
            btnClear = new Button() { Text = "LÀM MỚI", Location = new Point(280, 200), Size = new Size(80, 30), Font = new Font("Segoe UI", 9, FontStyle.Bold), BackColor = Color.White, FlatStyle = FlatStyle.Flat };

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += (s, e) => Clear();

            this.Controls.Add(btnAdd); this.Controls.Add(btnUpdate); this.Controls.Add(btnDelete); this.Controls.Add(btnClear);

            dgvCategories = new DataGridView() { Location = new Point(400, 80), Size = new Size(540, 550), BackgroundColor = Color.White, SelectionMode = DataGridViewSelectionMode.FullRowSelect, ReadOnly = true, AllowUserToAddRows = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            dgvCategories.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvCategories.SelectionChanged += Dgv_SelectionChanged;
            this.Controls.Add(dgvCategories);
        }

        public void LoadData()
        {
            try {
                var bll = new quanlycitieu.BLL.CategoryBLL();
                dgvCategories.DataSource = bll.GetAllCategories(AuthForm.CurrentUserId);
                if (dgvCategories.Columns.Contains("Id")) dgvCategories.Columns["Id"].Visible = false;
                Clear();
            } catch { }
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCategories.SelectedRows.Count > 0)
            {
                var row = dgvCategories.SelectedRows[0];
                selectedId = Convert.ToInt32(row.Cells["Id"].Value);
                txtName.Text = row.Cells["Tên"].Value?.ToString();
                cbType.SelectedItem = row.Cells["Loại"].Value?.ToString();
                
                btnUpdate.Enabled = true;
                btnDelete.Enabled = true;
            }
        }

        private void Clear()
        {
            selectedId = -1;
            txtName.Clear();
            cbType.SelectedIndex = -1;
            btnUpdate.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try {
                var bll = new quanlycitieu.BLL.CategoryBLL();
                bll.AddCategory(AuthForm.CurrentUserId, txtName.Text, cbType.SelectedItem?.ToString());
                LoadData();
                MessageBox.Show("Thêm thành công!");
            } catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedId > 0)
            {
                try {
                    var bll = new quanlycitieu.BLL.CategoryBLL();
                    bll.UpdateCategory(AuthForm.CurrentUserId, selectedId, txtName.Text, cbType.SelectedItem?.ToString());
                    LoadData();
                    MessageBox.Show("Cập nhật thành công!");
                } catch (Exception ex) { MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (selectedId > 0 && MessageBox.Show(
                "Xóa danh mục sẽ xóa luôn tất cả giao dịch liên quan!\nBạn có chắc chắn muốn xóa?", 
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try {
                    var bll = new quanlycitieu.BLL.CategoryBLL();
                    bll.DeleteCategory(AuthForm.CurrentUserId, selectedId);
                    LoadData();
                    MessageBox.Show("Xóa danh mục và các giao dịch liên quan thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                } catch (Exception ex) { 
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
