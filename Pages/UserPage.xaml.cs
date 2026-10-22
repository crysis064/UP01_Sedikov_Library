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
    public partial class UserPage : Page
    {
        public UserPage()
        {
            InitializeComponent();
            LoadCategories();
            UpdateBooks();
        }

        private void LoadCategories()
        {
            using (var db = new DB_Sedikov_LibraryEntities())
            {
                var categories = db.Genres.ToList();
                categories.Insert(0, new Genres { ID = 0, Name = "Все жанры" });
                CategoryComboBox.ItemsSource = categories;
                CategoryComboBox.SelectedIndex = 0;
            }
        }

        private void UpdateBooks()
        {
            if (!IsInitialized) return;

            using (var db = new DB_Sedikov_LibraryEntities())
            {
                var books = db.Book.Include("Genres").Include("Author").ToList();

                if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
                {
                    books = books.Where(b => b.Title.ToLower().Contains(SearchTextBox.Text.ToLower())).ToList();
                }

                if (CategoryComboBox.SelectedItem is Genres selectedGenre && selectedGenre.ID != 0)
                {
                    books = books.Where(b => b.GenresID == selectedGenre.ID).ToList();
                }
                if (SortComboBox.SelectedIndex == 0)
                    books = books.OrderBy(b => b.Title).ToList();
                else
                    books = books.OrderByDescending(b => b.Title).ToList();

                BooksListView.ItemsSource = books;
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e) => UpdateBooks();
        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateBooks();
        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateBooks();

        private void ClearFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";
            CategoryComboBox.SelectedIndex = 0;
            SortComboBox.SelectedIndex = 0;
            UpdateBooks();
        }
    }
}