using System;
using System.Windows.Forms;

namespace quanlycitieu
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            while (true)
            {
                var auth = new QuanLyChiTieu.AuthForm();
                if (auth.ShowDialog() == DialogResult.OK)
                {
                    var mainForm = new Form1();
                    Application.Run(mainForm);
                    
                    // Nếu người dùng chọn Đăng xuất, vòng lặp quay lại AuthForm
                    if (!mainForm.IsLogout)
                    {
                        break;
                    }
                }
                else
                {
                    // Người dùng đóng cửa sổ AuthForm -> thoát ứng dụng
                    break;
                }
            }
        }
    }
}