using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using QuanLyChiTieu;

namespace quanlycitieu.Views
{
    public class TrangChuView : UserControl
    {
        private Label txtTongThu, txtTongChi, txtConLai;
        private Panel pnlRight;
        private DataGridView dgvRecent;

        public TrangChuView()
        {
            this.Size = new Size(980, 700);
            this.BackColor = Color.FromArgb(209, 233, 255);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Label lblTitle = new Label() { Text = "TỔNG QUAN", Font = new Font("Segoe UI", 16, FontStyle.Bold), Location = new Point(20, 20), AutoSize = true };
            this.Controls.Add(lblTitle);

            Panel CreateCard(string title, int x, Color color) {
                Panel p = new Panel() { Location = new Point(x, 70), Size = new Size(200, 100), BackColor = Color.White };
                p.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, p.Width, p.Height, 20, 20));
                Label l = new Label() { Text = title, Font = new Font("Segoe UI", 11, FontStyle.Bold), Location = new Point(15, 15), AutoSize = true, ForeColor = Color.Gray };
                p.Controls.Add(l);
                return p;
            }

            var p1 = CreateCard("TỔNG THU", 20, Color.Green);
            var p2 = CreateCard("TỔNG CHI", 240, Color.Red);
            var p3 = CreateCard("CÒN LẠI", 460, Color.Blue);

            txtTongThu = new Label() { Location = new Point(15, 50), Size = new Size(170, 30), Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.Green, TextAlign = ContentAlignment.MiddleRight };
            txtTongChi = new Label() { Location = new Point(15, 50), Size = new Size(170, 30), Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.Red, TextAlign = ContentAlignment.MiddleRight };
            txtConLai = new Label() { Location = new Point(15, 50), Size = new Size(170, 30), Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.Blue, TextAlign = ContentAlignment.MiddleRight };

            p1.Controls.Add(txtTongThu);
            p2.Controls.Add(txtTongChi);
            p3.Controls.Add(txtConLai);

            this.Controls.Add(p1); this.Controls.Add(p2); this.Controls.Add(p3);

            // Right panel Nhin Lai
            pnlRight = new Panel() { Location = new Point(690, 20), Size = new Size(270, 600), BackColor = Color.FromArgb(41, 40, 104) };
            pnlRight.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom;
            pnlRight.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, pnlRight.Width, pnlRight.Height, 20, 20));
            Label lblNhinLai = new Label() { Text = "NHÌN LẠI", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.White, Location = new Point(90, 20), AutoSize = true };
            pnlRight.Controls.Add(lblNhinLai);
            this.Controls.Add(pnlRight);

            // Recent grid
            Label lblRecent = new Label() { Text = "Giao dịch gần đây", Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(20, 190), AutoSize = true };
            this.Controls.Add(lblRecent);

            dgvRecent = new DataGridView() { Location = new Point(20, 230), Size = new Size(640, 390), BackgroundColor = Color.White, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect };
            dgvRecent.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.Controls.Add(dgvRecent);
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        public void LoadData()
        {
            try
            {
                var transBLL = new quanlycitieu.BLL.TransactionBLL();
                decimal thu = transBLL.GetTotalIncome(AuthForm.CurrentUserId);
                decimal chi = transBLL.GetTotalExpense(AuthForm.CurrentUserId);
                decimal conLai = thu - chi;

                txtTongThu.Text = thu.ToString("N0") + " đ";
                txtTongChi.Text = chi.ToString("N0") + " đ";
                txtConLai.Text = conLai.ToString("N0") + " đ"; 

                for (int i = pnlRight.Controls.Count - 1; i >= 0; i--) {
                    if (pnlRight.Controls[i] is Label lbl && lbl.Text == "NHÌN LẠI") continue;
                    pnlRight.Controls.RemoveAt(i);
                }

                var dtCats = transBLL.GetExpensesByCategory(AuthForm.CurrentUserId);

                int y = 80;
                foreach (DataRow row in dtCats.Rows)
                {
                    string cat = row["Name"].ToString();
                    decimal total = Convert.ToDecimal(row["Total"]);

                    Panel p = new Panel() { Location = new Point(20, y), Size = new Size(200, 45), BackColor = Color.WhiteSmoke };
                    p.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, p.Width, p.Height, 10, 10));
                    Label l = new Label() { Text = cat, Location = new Point(10, 12), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
                    Label t = new Label() { Text = total.ToString("N0") + " đ", Location = new Point(90, 12), Size = new Size(100, 25), Font = new Font("Segoe UI", 10, FontStyle.Bold), TextAlign = ContentAlignment.MiddleRight, ForeColor = Color.Red };
                    
                    p.Controls.Add(l);
                    p.Controls.Add(t);
                    pnlRight.Controls.Add(p);
                    y += 55;
                }

                dgvRecent.DataSource = transBLL.GetRecentTransactions(AuthForm.CurrentUserId, 5);
                if (dgvRecent.Columns.Contains("Số Tiền"))
                {
                    dgvRecent.Columns["Số Tiền"].DefaultCellStyle.Format = "N0";
                }
                
                // Tô màu theo Loại
                foreach (DataGridViewRow row in dgvRecent.Rows)
                {
                    if (row.Cells["Loại"].Value?.ToString() == "Thu Nhập")
                        row.DefaultCellStyle.ForeColor = Color.Green;
                    else
                        row.DefaultCellStyle.ForeColor = Color.Red;
                }
            }
            catch { }
        }
    }
}
