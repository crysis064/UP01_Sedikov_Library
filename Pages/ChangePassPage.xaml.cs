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
    public partial class ChangePassPage : Page
    {
        public ChangePassPage()
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

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TbLogin.Text) ||
                string.IsNullOrEmpty(CurrentPasswordBox.Password) ||
                string.IsNullOrEmpty(NewPasswordBox.Password) ||
                string.IsNullOrEmpty(ConfirmPasswordBox.Password))
            {
                MessageBox.Show("Все поля обязательны к заполнению!");
                return;
            }

            using (var db = new DB_Sedikov_LibraryEntities())
            {
                string hashedCurrentPass = GetHash(CurrentPasswordBox.Password);
                var user = db.User.FirstOrDefault(u => u.Login == TbLogin.Text && u.Password == hashedCurrentPass);

                if (user == null)
                {
                    MessageBox.Show("Текущий логин или пароль введены неверно!");
                    return;
                }

                if (NewPasswordBox.Password.Length < 6)
                {
                    MessageBox.Show("Новый пароль должен содержать 6 или более символов!");
                    return;
                }

                bool en = true;
                bool number = false;

                for (int i = 0; i < NewPasswordBox.Password.Length; i++)
                {
                    if (NewPasswordBox.Password[i] >= '0' && NewPasswordBox.Password[i] <= '9')
                        number = true;
                    else if (!((NewPasswordBox.Password[i] >= 'A' && NewPasswordBox.Password[i] <= 'Z') ||
                               (NewPasswordBox.Password[i] >= 'a' && NewPasswordBox.Password[i] <= 'z')))
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

                if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
                {
                    MessageBox.Show("Новые пароли не совпадают!");
                    return;
                }

                try
                {
                    user.Password = GetHash(NewPasswordBox.Password);
                    db.SaveChanges();
                    MessageBox.Show("Пароль успешно изменен!");
                    NavigationService?.Navigate(new AuthPage());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранении: " + ex.Message);
                }
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AuthPage());
        }
    }
}