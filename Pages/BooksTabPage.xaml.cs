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
    public partial class BooksTabPage : Page
    {
        public BooksTabPage()
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
                    DataGridBooks.ItemsSource = db.Book
                        .Include("Genres")
                        .Include("Author")
                        .ToList();
                }
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AddBookPage(null));

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var booksForRemoving = DataGridBooks.SelectedItems.Cast<Book>().ToList();
            if (booksForRemoving.Count == 0) { MessageBox.Show("Выберите книгу!"); return; }

            if (MessageBox.Show($"Удалить {booksForRemoving.Count} записей?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var db = new DB_Sedikov_LibraryEntities())
                {
                    try
                    {
                        foreach (var book in booksForRemoving)
                        {
                            db.Book.Attach(book);
                            db.Book.Remove(book);
                        }
                        db.SaveChanges();
                        DataGridBooks.ItemsSource = db.Book.Include("Genres").ToList();
                    }
                    catch (System.Exception ex) { MessageBox.Show(ex.Message); }
                }
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedBook = (sender as Button).DataContext as Book;
            NavigationService?.Navigate(new AddBookPage(selectedBook));
        }
    }
}