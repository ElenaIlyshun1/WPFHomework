using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EncryptPasswords
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //encrypt password
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            txtLastPasswd.Text = GetMD5(txtPasswd.Text);
        }
        public string GetMD5(string text)
        {
            MD5CryptoServiceProvider mD5 = new MD5CryptoServiceProvider();
            mD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] result = mD5.Hash;
            StringBuilder str = new StringBuilder();
            for (int i = 1; i < result.Length; i++)
            {
                str.Append(result[i].ToString("x2"));
            }
            return str.ToString();
        }

        private void BtnSaveByFile_Click(object sender, RoutedEventArgs e)
        {
            string encryptPasswd = GetMD5(txtPasswd.Text);
            using (StreamWriter sw = new StreamWriter(@"D:\ШАГГГГГ\WPF\EncryptPasswords\EncryptPasswords\passwd.txt", true, System.Text.Encoding.Default))
            {
                sw.WriteLine(txtEmail.Text + "-"+ encryptPasswd);
            }
        }

        private void BtnSaveBySQL_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string dbName = "encryptPassword.sqlite";
                SQLiteConnection con = new SQLiteConnection($"Data Source={dbName}");
                con.Open();
                string password = HashPassword(txtPasswd.Text);

                MessageBox.Show(password);

                string query = $"INSERT INTO MYSTUDENTS(NAME,PASSWORD)VALUES('{txtName.Text}','{password}')";
                //TODO: Insert by SQLite
                SQLiteCommand cmd = new SQLiteCommand(query, con);
                cmd.ExecuteNonQuery();
                con.Close();
              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                // throw;
            }

        }
        public static string HashPassword(string password)
        {
            byte[] salt;
            byte[] buffer2;
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
            {
                salt = bytes.Salt;
                buffer2 = bytes.GetBytes(0x20);
            }
            byte[] dst = new byte[0x31];
            Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
            Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
            return Convert.ToBase64String(dst);
        }
    }
}
