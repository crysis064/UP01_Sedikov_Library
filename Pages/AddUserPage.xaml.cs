using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
    public partial class AddUserPage : Page
    {
        private readonly User _currentUser = new User();

        public AddUserPage(User selectedUser)
        {
            InitializeComponent();
            if (selectedUser != null)
            {
                _currentUser = selectedUser;
                TitleText.Text = "Редактирование пользователя";
                TBPass.Text = "";
            }
            DataContext = _currentUser;
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
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentUser.Login)) errors.AppendLine("Укажите логин!");
            if (string.IsNullOrWhiteSpace(_currentUser.FIO)) errors.AppendLine("Укажите ФИО!");
            if (string.IsNullOrWhiteSpace(_currentUser.Role)) errors.AppendLine("Выберите роль!");

            if (_currentUser.ID == 0 && string.IsNullOrWhiteSpace(TBPass.Text))
                errors.AppendLine("Введите пароль!");

            if (errors.Length > 0) { MessageBox.Show(errors.ToString()); return; }

            if (!string.IsNullOrWhiteSpace(TBPass.Text))
                _currentUser.Password = GetHash(TBPass.Text);

            using (var db = new DB_Sedikov_LibraryEntities())
            {
                try
                {
                    if (_currentUser.ID == 0)
                    {
                        if (db.User.Any(u => u.Login == _currentUser.Login))
                        {
                            MessageBox.Show("Пользователь с таким логином уже существует!");
                            return;
                        }
                        db.User.Add(_currentUser);
                    }
                    else
                    {
                        db.User.Attach(_currentUser);
                        db.Entry(_currentUser).State = EntityState.Modified;
                    }

                    db.SaveChanges();
                    MessageBox.Show("Данные сохранены!");
                    NavigationService?.Navigate(new UsersTabPage());
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new UsersTabPage());
    }
}