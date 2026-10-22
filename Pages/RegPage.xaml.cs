using System;
using System.Collections.Generic;
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

namespace UP01_Sedikov_Library.Pages
{
    public partial class RegPage : Page
    {
        public RegPage()
        {
            InitializeComponent();
        }
        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }

        private void regButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtbxLog.Text) || string.IsNullOrEmpty(txtbxFIO.Text) ||
                string.IsNullOrEmpty(passBxFrst.Password) || string.IsNullOrEmpty(passBxScnd.Password))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            using (var db = new DB_Sedikov_LibraryEntities())
            {
                var existingUser = db.User.AsNoTracking().FirstOrDefault(u => u.Login == txtbxLog.Text);
                if (existingUser != null)
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!");
                    return;
                }

                if (passBxFrst.Password.Length < 6)
                {
                    MessageBox.Show("Пароль должен содержать 6 или более символов!");
                    return;
                }

                bool en = true;
                bool number = false;

                for (int i = 0; i < passBxFrst.Password.Length; i++)
                {
                    if (passBxFrst.Password[i] >= '0' && passBxFrst.Password[i] <= '9')
                        number = true;
                    else if (!((passBxFrst.Password[i] >= 'A' && passBxFrst.Password[i] <= 'Z') ||
                               (passBxFrst.Password[i] >= 'a' && passBxFrst.Password[i] <= 'z')))
                        en = false;
                }

                if (!en)
                {
                    MessageBox.Show("Допускается только английская раскладка!");
                    return;
                }
                if (!number)
                {
                    MessageBox.Show("Пароль должен содержать хотя бы одну цифру!");
                    return;
                }

                if (passBxFrst.Password != passBxScnd.Password)
                {
                    MessageBox.Show("Пароли не совпадают!");
                    return;
                }

                if (txtbxFIO.Text.Split(' ').Length < 2 || txtbxFIO.Text.Any(char.IsDigit))
                {
                    MessageBox.Show("Введите корректное ФИО (минимум имя и фамилия, без цифр)!");
                    return;
                }

                User userObject = new User
                {
                    FIO = txtbxFIO.Text,
                    Login = txtbxLog.Text,
                    Password = GetHash(passBxFrst.Password),
                    Role = comboBox.Text
                };

                try
                {
                    db.User.Add(userObject);
                    db.SaveChanges();
                    MessageBox.Show("Пользователь успешно зарегистрирован!");

                    NavigationService?.Navigate(new AuthPage());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранении: " + ex.Message);
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AuthPage());
        }

        private void lblLogHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => txtbxLog.Focus();
        private void lblPassHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => passBxFrst.Focus();
        private void lblPassSecHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => passBxScnd.Focus();
        private void lblFioHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => txtbxFIO.Focus();

        private void txtbxLog_TextChanged(object sender, TextChangedEventArgs e) => lblLogHint.Visibility = string.IsNullOrEmpty(txtbxLog.Text) ? Visibility.Visible : Visibility.Hidden;
        private void passBxFrst_PasswordChanged(object sender, RoutedEventArgs e) => lblPassHint.Visibility = string.IsNullOrEmpty(passBxFrst.Password) ? Visibility.Visible : Visibility.Hidden;
        private void passBxScnd_PasswordChanged(object sender, RoutedEventArgs e) => lblPassSecHint.Visibility = string.IsNullOrEmpty(passBxScnd.Password) ? Visibility.Visible : Visibility.Hidden;
        private void txtbxFIO_TextChanged(object sender, TextChangedEventArgs e) => lblFioHint.Visibility = string.IsNullOrEmpty(txtbxFIO.Text) ? Visibility.Visible : Visibility.Hidden;
    }
}