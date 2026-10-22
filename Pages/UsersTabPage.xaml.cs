using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class UsersTabPage : Page
    {
        public UsersTabPage()
        {
            InitializeComponent();
            this.IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                using (var db = new DB_Sedikov_LibraryEntities())
                {
                    DataGridUser.ItemsSource = db.User.ToList();
                }
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddUserPage(null));
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var usersForRemoving = DataGridUser.SelectedItems.Cast<User>().ToList();
            if (usersForRemoving.Count == 0)
            {
                MessageBox.Show("Выберите пользователя для удаления!");
                return;
            }

            if (MessageBox.Show($"Удалить {usersForRemoving.Count} записей?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var db = new DB_Sedikov_LibraryEntities())
                {
                    try
                    {
                        foreach (var user in usersForRemoving)
                        {
                            db.User.Attach(user);
                            db.User.Remove(user);
                        }
                        db.SaveChanges();
                        MessageBox.Show("Данные удалены!");
                        DataGridUser.ItemsSource = db.User.ToList();
                    }
                    catch (System.Exception ex) { MessageBox.Show(ex.Message); }
                }
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = (sender as Button).DataContext as User;
            NavigationService?.Navigate(new AddUserPage(selectedUser));
        }
    }
}