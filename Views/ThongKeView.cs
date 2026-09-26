using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Data;
using QuanLyChiTieu;
using quanlycitieu.BLL;

namespace quanlycitieu.Views
{
    public class ThongKeView : UserControl
    {
        private StatisticBLL statisticBLL = new StatisticBLL();
        private DashboardBLL dashboardBLL = new DashboardBLL();

        // Top Filter Bar
        private Panel pnlTop;
        private ComboBox cbChartType;
        private Label lblMonth;
        private ComboBox cbMonth;
        private Label lblYear;
        private NumericUpDown numYear;
        private Button btnRefresh;
        private Button btnExport;

        // Chart
        private Chart chart;

        public ThongKeView()
        {
            this.Size = new Size(980, 700);
            this.BackColor = Color.FromArgb(209, 233, 255);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // 1. THANH CÔNG CỤ BỘ LỌC PHÍA TRÊN
            pnlTop = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = Color.FromArgb(209, 233, 255)
            };

            // Nút Xuất Báo Cáo
            btnExport = new Button()
            {
                Text = "Xuất Báo Cáo",
                Location = new Point(20, 14),
                Size = new Size(125, 36),
                BackColor = Color.FromArgb(41, 40, 104),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnExport.FlatAppearance.BorderSize = 0;
            btnExport.Click += BtnExport_Click;
            pnlTop.Controls.Add(btnExport);

            // Nhãn & ComboBox Loại Biểu Đồ
            Label lblType = new Label()
            {
                Text = "Loại biểu đồ:",
                Location = new Point(160, 22),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30)
            };
            pnlTop.Controls.Add(lblType);

            cbChartType = new ComboBox()
            {
                Location = new Point(255, 18),
                Size = new Size(295, 30),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbChartType.Items.AddRange(new object[]
            {
                "Biểu đồ cột: So sánh ngày trong tháng",
                "Biểu đồ cột: Thu/Chi 12 tháng trong năm",
                "Biểu đồ tròn: Tỷ lệ chi tiêu theo danh mục"
            });
            cbChartType.SelectedIndex = 0;
            cbChartType.SelectedIndexChanged += (s, e) =>
            {
                // Nếu là biểu đồ 12 tháng thì ẩn hoặc disable chọn Tháng
                bool isYearOnly = cbChartType.SelectedIndex == 1;
                lblMonth.Enabled = !isYearOnly;
                cbMonth.Enabled = !isYearOnly;
                LoadData();
            };
            pnlTop.Controls.Add(cbChartType);

            // Nhãn & ComboBox Tháng
            lblMonth = new Label()
            {
                Text = "Tháng:",
                Location = new Point(565, 22),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30)
            };
            pnlTop.Controls.Add(lblMonth);

            cbMonth = new ComboBox()
            {
                Location = new Point(620, 18),
                Size = new Size(65, 30),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            for (int i = 1; i <= 12; i++) cbMonth.Items.Add(i);
            cbMonth.SelectedItem = DateTime.Now.Month;
            cbMonth.SelectedIndexChanged += (s, e) => LoadData();
            pnlTop.Controls.Add(cbMonth);

            // Nhãn & NumericUpDown Năm
            lblYear = new Label()
            {
                Text = "Năm:",
                Location = new Point(695, 22),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30)
            };
            pnlTop.Controls.Add(lblYear);

            numYear = new NumericUpDown()
            {
                Location = new Point(740, 18),
                Size = new Size(75, 30),
                Font = new Font("Segoe UI", 10),
                Minimum = 2000,
                Maximum = 2100,
                Value = DateTime.Now.Year
            };
            numYear.ValueChanged += (s, e) => LoadData();
            pnlTop.Controls.Add(numYear);

            // Nút Làm Mới
            btnRefresh = new Button()
            {
                Text = "LÀM MỚI",
                Location = new Point(830, 14),
                Size = new Size(95, 36),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(41, 40, 104),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            btnRefresh.Click += (s, e) => LoadData();
            pnlTop.Controls.Add(btnRefresh);

            this.Controls.Add(pnlTop);

            // 2. CHART CONTROL
            chart = new Chart()
            {
                Location = new Point(20, 75),
                Size = new Size(940, 605),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.White
            };
            this.Controls.Add(chart);
        }

        public void LoadData()
        {
            chart.Series.Clear();
            chart.ChartAreas.Clear();
            chart.Titles.Clear();
            chart.Legends.Clear();

            try
            {
                int userId = AuthForm.CurrentUserId;
                int month = cbMonth.SelectedItem != null ? Convert.ToInt32(cbMonth.SelectedItem) : DateTime.Now.Month;
                int year = Convert.ToInt32(numYear.Value);
                int chartMode = cbChartType.SelectedIndex;

                var area = new ChartArea("MainArea")
                {
                    BackColor = Color.White
                };
                area.AxisX.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
                area.AxisY.MajorGrid.LineColor = Color.FromArgb(240, 240, 240);
                area.AxisX.LineColor = Color.FromArgb(160, 160, 160);
                area.AxisY.LineColor = Color.FromArgb(160, 160, 160);
                area.AxisX.LabelStyle.Font = new Font("Segoe UI", 8.5f);
                area.AxisY.LabelStyle.Font = new Font("Segoe UI", 8.5f);
                area.AxisY.LabelStyle.Format = "N0";

                chart.ChartAreas.Add(area);

                if (chartMode == 0)
                {
                    // ============================================================
                    // CHẾ ĐỘ 1: BIỂU ĐỒ CỘT - SO SÁNH NGÀY TRONG THÁNG
                    // ============================================================
                    var title = chart.Titles.Add($"BÁO CÁO THU CHI CÁC NGÀY TRONG THÁNG {month:D2}/{year}");
                    title.Font = new Font("Segoe UI", 12.5f, FontStyle.Bold);
                    title.ForeColor = Color.FromArgb(41, 40, 104);

                    area.AxisX.Title = "Ngày trong tháng";
                    area.AxisX.TitleFont = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                    area.AxisY.Title = "Số tiền (VNĐ)";
                    area.AxisY.TitleFont = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                    area.AxisX.Interval = 1;

                    var legend = new Legend("Legend1")
                    {
                        Docking = Docking.Top,
                        Alignment = StringAlignment.Center,
                        Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
                    };
                    chart.Legends.Add(legend);

                    Series sThu = new Series("Thu Nhập")
                    {
                        ChartType = SeriesChartType.Column,
                        Color = Color.FromArgb(34, 197, 94), // Xanh lá
                        IsValueShownAsLabel = false
                    };
                    sThu.SmartLabelStyle.Enabled = false;

                    Series sChi = new Series("Chi Tiêu")
                    {
                        ChartType = SeriesChartType.Column,
                        Color = Color.FromArgb(239, 68, 68), // Đỏ
                        IsValueShownAsLabel = false
                    };
                    sChi.SmartLabelStyle.Enabled = false;

                    DataTable dtDaily = statisticBLL.GetDailyStatistics(userId, month, year);

                    foreach (DataRow r in dtDaily.Rows)
                    {
                        int day = Convert.ToInt32(r["Day"]);
                        string dayLabel = day.ToString();
                        decimal inc = Convert.ToDecimal(r["Income"]);
                        decimal exp = Convert.ToDecimal(r["Expense"]);

                        int idxThu = sThu.Points.AddXY(dayLabel, (double)inc);
                        if (inc > 0)
                        {
                            sThu.Points[idxThu].IsValueShownAsLabel = true;
                            sThu.Points[idxThu].Label = inc.ToString("N0");
                            sThu.Points[idxThu].Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
                            sThu.Points[idxThu].ToolTip = $"Ngày {day:D2}/{month:D2}/{year}\nThu Nhập: {inc:N0} đ";
                        }
                        else
                        {
                            sThu.Points[idxThu].IsValueShownAsLabel = false;
                            sThu.Points[idxThu].Label = "";
                            sThu.Points[idxThu].IsEmpty = true;
                        }

                        int idxChi = sChi.Points.AddXY(dayLabel, (double)exp);
                        if (exp > 0)
                        {
                            sChi.Points[idxChi].IsValueShownAsLabel = true;
                            sChi.Points[idxChi].Label = exp.ToString("N0");
                            sChi.Points[idxChi].Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
                            sChi.Points[idxChi].ToolTip = $"Ngày {day:D2}/{month:D2}/{year}\nChi Tiêu: {exp:N0} đ";
                        }
                        else
                        {
                            sChi.Points[idxChi].IsValueShownAsLabel = false;
                            sChi.Points[idxChi].Label = "";
                            sChi.Points[idxChi].IsEmpty = true;
                        }
                    }

                    chart.Series.Add(sThu);
                    chart.Series.Add(sChi);
                }
                else if (chartMode == 1)
                {
                    // ============================================================
                    // CHẾ ĐỘ 2: BIỂU ĐỒ CỘT - THU/CHI 12 THÁNG TRONG NĂM
                    // ============================================================
                    var title = chart.Titles.Add($"XU HƯỚNG THU CHI 12 THÁNG TRONG NĂM {year}");
                    title.Font = new Font("Segoe UI", 12.5f, FontStyle.Bold);
                    title.ForeColor = Color.FromArgb(41, 40, 104);

                    area.AxisX.Title = "12 Tháng trong năm";
                    area.AxisX.TitleFont = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                    area.AxisY.Title = "Số tiền (VNĐ)";
                    area.AxisY.TitleFont = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                    area.AxisX.Interval = 1;

                    var legend = new Legend("Legend1")
                    {
                        Docking = Docking.Top,
                        Alignment = StringAlignment.Center,
                        Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
                    };
                    chart.Legends.Add(legend);

                    Series sThu = new Series("Thu Nhập")
                    {
                        ChartType = SeriesChartType.Column,
                        Color = Color.FromArgb(34, 197, 94), // Xanh lá
                        IsValueShownAsLabel = false
                    };
                    sThu.SmartLabelStyle.Enabled = false;

                    Series sChi = new Series("Chi Tiêu")
                    {
                        ChartType = SeriesChartType.Column,
                        Color = Color.FromArgb(239, 68, 68), // Đỏ
                        IsValueShownAsLabel = false
                    };
                    sChi.SmartLabelStyle.Enabled = false;

                    DataTable dtYearly = statisticBLL.GetYearlyStatistics(userId, year);

                    foreach (DataRow r in dtYearly.Rows)
                    {
                        string mLabel = r["MonthLabel"]?.ToString() ?? "";
                        decimal inc = Convert.ToDecimal(r["Income"]);
                        decimal exp = Convert.ToDecimal(r["Expense"]);

                        int idxThu = sThu.Points.AddXY(mLabel, (double)inc);
                        if (inc > 0)
                        {
                            sThu.Points[idxThu].IsValueShownAsLabel = true;
                            sThu.Points[idxThu].Label = inc.ToString("N0");
                            sThu.Points[idxThu].Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
                            sThu.Points[idxThu].ToolTip = $"{mLabel}/{year}\nThu Nhập: {inc:N0} đ";
                        }
                        else
                        {
                            sThu.Points[idxThu].IsValueShownAsLabel = false;
                            sThu.Points[idxThu].Label = "";
                            sThu.Points[idxThu].IsEmpty = true;
                        }

                        int idxChi = sChi.Points.AddXY(mLabel, (double)exp);
                        if (exp > 0)
                        {
                            sChi.Points[idxChi].IsValueShownAsLabel = true;
                            sChi.Points[idxChi].Label = exp.ToString("N0");
                            sChi.Points[idxChi].Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
                            sChi.Points[idxChi].ToolTip = $"{mLabel}/{year}\nChi Tiêu: {exp:N0} đ";
                        }
                        else
                        {
                            sChi.Points[idxChi].IsValueShownAsLabel = false;
                            sChi.Points[idxChi].Label = "";
                            sChi.Points[idxChi].IsEmpty = true;
                        }
                    }

                    chart.Series.Add(sThu);
                    chart.Series.Add(sChi);
                }
                else
                {
                    // ============================================================
                    // CHẾ ĐỘ 3: BIỂU ĐỒ TRÒN (PIE CHART) - TỶ LỆ CHI TIÊU THEO DANH MỤC
                    // ============================================================
                    var title = chart.Titles.Add($"TỶ LỆ CHI TIÊU THEO DANH MỤC THÁNG {month:D2}/{year}");
                    title.Font = new Font("Segoe UI", 12.5f, FontStyle.Bold);
                    title.ForeColor = Color.FromArgb(41, 40, 104);

                    DataTable dtShare = statisticBLL.GetCategoryExpenseShare(userId, month, year);

                    if (dtShare.Rows.Count == 0)
                    {
                        var sub = chart.Titles.Add($"\n\n(Không có khoản chi tiêu nào phát sinh trong tháng {month:D2}/{year})");
                        sub.Font = new Font("Segoe UI", 11f, FontStyle.Italic);
                        sub.ForeColor = Color.FromArgb(120, 120, 120);
                    }
                    else
                    {
                        var legend = new Legend("Legend1")
                        {
                            Docking = Docking.Right,
                            Alignment = StringAlignment.Center,
                            Font = new Font("Segoe UI", 9.5f)
                        };
                        chart.Legends.Add(legend);

                        Series sPie = new Series("Tỷ Lệ Chi Tiêu")
                        {
                            ChartType = SeriesChartType.Pie,
                            Font = new Font("Segoe UI", 9f, FontStyle.Bold)
                        };

                        sPie["PieLabelStyle"] = "Outside";
                        sPie["PieLineColor"] = "Gray";

                        Color[] palette = new Color[]
                        {
                            Color.FromArgb(239, 68, 68),   // Red
                            Color.FromArgb(59, 130, 246),  // Blue
                            Color.FromArgb(245, 158, 11),  // Amber
                            Color.FromArgb(16, 185, 129),  // Emerald
                            Color.FromArgb(139, 92, 246),  // Purple
                            Color.FromArgb(236, 72, 153),  // Pink
                            Color.FromArgb(20, 184, 166),  // Teal
                            Color.FromArgb(249, 115, 22),  // Orange
                            Color.FromArgb(99, 102, 241),  // Indigo
                            Color.FromArgb(107, 114, 128)  // Gray
                        };

                        int colorIdx = 0;
                        foreach (DataRow r in dtShare.Rows)
                        {
                            string cat = r["CategoryName"]?.ToString() ?? "Khác";
                            decimal amt = Convert.ToDecimal(r["Amount"]);
                            double pct = Convert.ToDouble(r["Percentage"]);

                            int ptIdx = sPie.Points.AddXY(cat, (double)amt);
                            var pt = sPie.Points[ptIdx];
                            pt.Color = palette[colorIdx % palette.Length];
                            colorIdx++;

                            pt.Label = $"{cat}\n{pct:F1}%";
                            pt.LegendText = $"{cat}: {amt:N0} đ ({pct:F1}%)";
                            pt.ToolTip = $"Danh mục: {cat}\nSố tiền: {amt:N0} đ\nTỷ lệ: {pct:F1}%";
                        }

                        chart.Series.Add(sPie);
                    }
                }
            }
            catch { }
        }

        private void BtnExport_Click(object? sender, EventArgs e)
        {
            try
            {
                int month = cbMonth.SelectedItem != null ? Convert.ToInt32(cbMonth.SelectedItem) : DateTime.Now.Month;
                int year = Convert.ToInt32(numYear.Value);

                var sfd = new SaveFileDialog()
                {
                    Filter = "CSV file (*.csv)|*.csv",
                    FileName = $"BaoCaoChiTieu_T{month:D2}_{year}.csv"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var dt = dashboardBLL.GetMonthlyTransactions(AuthForm.CurrentUserId, month, year);
                    using (var sw = new System.IO.StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
                    {
                        sw.WriteLine("Mã,Loại,Danh Mục,Số Tiền,Thời Gian,Ghi Chú");
                        foreach (DataRow row in dt.Rows)
                        {
                            string dateStr = Convert.ToDateTime(row["Thời Gian"]).ToString("dd/MM/yyyy HH:mm");
                            sw.WriteLine($"{row["Mã"]},{row["Loại"]},\"{row["Danh Mục"]}\",{row["Số Tiền"]},\"{dateStr}\",\"{row["Ghi Chú"]}\"");
                        }
                    }
                    MessageBox.Show(this, "Xuất báo cáo thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
