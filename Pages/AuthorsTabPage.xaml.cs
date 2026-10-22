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
    public partial class AuthorsTabPage : Page
    {
        
        public AuthorsTabPage()
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
                    DataGridAuthors.ItemsSource = db.Author.ToList();
                }
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AddAuthorPage(null));

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var authorsForRemoving = DataGridAuthors.SelectedItems.Cast<Author>().ToList();
            if (authorsForRemoving.Count == 0) { MessageBox.Show("Выберите автора!"); return; }

            if (MessageBox.Show($"Удалить {authorsForRemoving.Count} записей?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var db = new DB_Sedikov_LibraryEntities())
                {
                    try
                    {
                        foreach (var author in authorsForRemoving)
                        {
                            db.Author.Attach(author);
                            db.Author.Remove(author);
                        }
                        db.SaveChanges();
                        DataGridAuthors.ItemsSource = db.Author.ToList();
                    }
                    catch (System.Exception ex) { MessageBox.Show(ex.Message); }
                }
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedAuthor = (sender as Button).DataContext as Author;
            NavigationService?.Navigate(new AddAuthorPage(selectedAuthor));
        }
    }
}