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
            
            var auth = new QuanLyChiTieu.AuthForm();
            if (auth.ShowDialog() == DialogResult.OK)
            {
                Application.Run(new Form1());
            }
        }
    }
}