using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Data;
using QuanLyChiTieu;

namespace quanlycitieu.Views
{
    public class ThongKeView : UserControl
    {
        private Panel pnlTop;
        private Chart chart;
        private Button btnShow, btnExit;
        private ComboBox cbChartType;

        public ThongKeView()
        {
            this.Size = new Size(980, 700);
            this.BackColor = Color.FromArgb(209, 233, 255);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            pnlTop = new Panel() { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(209, 233, 255) };
            
            Button CreateTopBtn(string text, int x, int width) {
                var b = new Button() { Text = text, Location = new Point(x, 10), Size = new Size(width, 40), BackColor = Color.FromArgb(41, 40, 104), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
                b.FlatAppearance.BorderSize = 0;
                return b;
            }

            var btnExport = CreateTopBtn("Xuất Báo Cáo", 20, 150);
            btnExport.Click += BtnExport_Click;
            pnlTop.Controls.Add(btnExport);
            
            cbChartType = new ComboBox() { Location = new Point(190, 18), Size = new Size(180, 30), Font = new Font("Segoe UI", 11), DropDownStyle = ComboBoxStyle.DropDownList };
            cbChartType.Items.AddRange(new object[] { "Biểu đồ Cột (Ngày)", "Biểu đồ Tròn (Loại)" });
            cbChartType.SelectedIndex = 0;
            cbChartType.SelectedIndexChanged += (s, e) => LoadData();
            pnlTop.Controls.Add(cbChartType);

            this.Controls.Add(pnlTop);

            chart = new Chart() { Location = new Point(20, 80), Size = new Size(800, 550) };
            chart.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            var area = new ChartArea("MainArea");
            area.AxisX.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.MajorGrid.LineColor = Color.LightGray;
            area.AxisY.Title = "Số tiền (VNĐ)";
            area.AxisY.TitleFont = new Font("Segoe UI", 10, FontStyle.Bold);
            area.AxisX.Title = "Thời gian / Danh mục";
            area.AxisX.TitleFont = new Font("Segoe UI", 10, FontStyle.Bold);
            chart.ChartAreas.Add(area);
            chart.Legends.Add(new Legend("Legend1") { Docking = Docking.Right });

            this.Controls.Add(chart);

            btnShow = new Button() { Text = "LÀM MỚI", Location = new Point(840, 100), Size = new Size(100, 40), BackColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnShow.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnShow.Click += (s, e) => LoadData();
            
            btnExit = new Button() { Text = "THOÁT", Location = new Point(840, 160), Size = new Size(100, 40), BackColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnExit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExit.Click += (s, e) => Application.Exit();

            this.Controls.Add(btnShow);
            this.Controls.Add(btnExit);
        }

        public void LoadData()
        {
            chart.Series.Clear();
            try {
                var bll = new quanlycitieu.BLL.TransactionBLL();
                if (cbChartType.SelectedIndex == 0)
                {
                    chart.ChartAreas[0].AxisX.Title = "Ngày";
                    var dt = bll.GetDailyTransactions(AuthForm.CurrentUserId);
                    
                    Series sThuNhap = new Series("Thu Nhập") { ChartType = SeriesChartType.Column, IsValueShownAsLabel = true, LabelFormat = "{0:N0}", Color = Color.Green };
                    Series sChiTieu = new Series("Chi Tiêu") { ChartType = SeriesChartType.Column, IsValueShownAsLabel = true, LabelFormat = "{0:N0}", Color = Color.Red };
                    
                    // Lấy danh sách các ngày duy nhất
                    var dates = new System.Collections.Generic.List<string>();
                    foreach (DataRow row in dt.Rows) {
                        string d = Convert.ToDateTime(row["Date"]).ToString("dd/MM");
                        if (!dates.Contains(d)) dates.Add(d);
                    }

                    // Map dữ liệu cho từng ngày
                    foreach (string d in dates)
                    {
                        double thu = 0, chi = 0;
                        foreach (DataRow row in dt.Rows) {
                            if (Convert.ToDateTime(row["Date"]).ToString("dd/MM") == d) {
                                if (row["Type"].ToString() == "Thu Nhập") thu = Convert.ToDouble(row["Amount"]);
                                else chi = Convert.ToDouble(row["Amount"]);
                            }
                        }
                        
                        // Luôn add cả 2 series để chúng cùng cột mốc X
                        sThuNhap.Points.AddXY(d, thu);
                        sChiTieu.Points.AddXY(d, chi);
                        
                        // Hide 0 values for cleaner UI
                        if (thu == 0) sThuNhap.Points[sThuNhap.Points.Count - 1].IsValueShownAsLabel = false;
                        if (chi == 0) sChiTieu.Points[sChiTieu.Points.Count - 1].IsValueShownAsLabel = false;
                    }
                    
                    chart.Series.Add(sThuNhap);
                    chart.Series.Add(sChiTieu);
                }
                else
                {
                    chart.ChartAreas[0].AxisX.Title = "Danh Mục";
                    var dt = bll.GetExpensesByCategory(AuthForm.CurrentUserId);
                    
                    Series s = new Series("Danh Mục");
                    s.ChartType = SeriesChartType.Doughnut;
                    s.IsValueShownAsLabel = true;
                    s.LabelFormat = "{0:N0}";
                    foreach (DataRow row in dt.Rows)
                    {
                        string label = $"{row["Name"]} ({row["Type"]})";
                        s.Points.AddXY(label, Convert.ToDouble(row["Total"]));
                    }
                    if (s.Points.Count > 0) chart.Series.Add(s);
                }
            } catch { }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try {
                var sfd = new SaveFileDialog() { Filter = "CSV file (*.csv)|*.csv", FileName = "BaoCaoChiTieu.csv" };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var bll = new quanlycitieu.BLL.TransactionBLL();
                    var dt = bll.GetTransactionsByFilter(AuthForm.CurrentUserId, 0, ""); // Get all
                    using (var sw = new System.IO.StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
                    {
                        sw.WriteLine("Mã,Loại,Thời Gian,Ghi Chú,Số Tiền,Danh Mục");
                        foreach (DataRow row in dt.Rows)
                        {
                            sw.WriteLine($"{row["Mã"]},{row["Loại"]},\"{Convert.ToDateTime(row["Thời Gian"]).ToString("dd/MM/yyyy")}\",\"{row["GhiChu"]}\",{row["SoTien"]},\"{row["DanhMuc"]}\"");
                        }
                    }
                    MessageBox.Show("Xuất báo cáo thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex) {
                MessageBox.Show("Lỗi xuất file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
