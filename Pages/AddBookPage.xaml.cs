using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
using UP01_Sedikov_Library.Pages;

namespace UP01_Sedikov_Library
{
    public partial class AddBookPage : Page
    {
        private void TBPenalty_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !char.IsDigit(e.Text, 0);
        }

        private void TBPenalty_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private bool IsTextAllowed(string text)
        {
            return text.All(char.IsDigit);
        }

        private readonly Book _currentBook = new Book();

        public AddBookPage(Book selectedBook)
        {
            InitializeComponent();

            if (selectedBook != null)
            {
                _currentBook = selectedBook;
                TitleText.Text = "Редактирование книги";

                TBTitle.Text = _currentBook.Title;
                TBPenalty.Text = _currentBook.Penalty.ToString("0");
            }

            using (var db = new DB_Sedikov_LibraryEntities())
            {
                CmbGenre.ItemsSource = db.Genres.ToList();
                CmbAuthor.ItemsSource = db.Author.ToList();

                if (selectedBook != null)
                {
                    CmbGenre.SelectedValue = _currentBook.GenresID;
                    CmbAuthor.SelectedValue = _currentBook.AuthorID;
                }
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TBTitle.Text))
            {
                MessageBox.Show("Введите название книги!");
                return;
            }

            if (CmbAuthor.SelectedItem == null)
            {
                MessageBox.Show("Выберите автора!");
                return;
            }

            if (CmbGenre.SelectedItem == null)
            {
                MessageBox.Show("Выберите жанр!");
                return;
            }

            if (!int.TryParse(TBPenalty.Text, out int qty) || qty < 0)
            {
                MessageBox.Show("Введите корректное количество (целое число >= 0)!");
                return;
            }
            _currentBook.Title = TBTitle.Text;
            _currentBook.Penalty = qty;

            var selectedAuthor = CmbAuthor.SelectedItem as Author;
            var selectedGenre = CmbGenre.SelectedItem as Genres;

            _currentBook.AuthorID = selectedAuthor.ID;
            _currentBook.GenresID = selectedGenre.ID;

            using (var db = new DB_Sedikov_LibraryEntities())
            {
                try
                {
                    if (_currentBook.ID == 0)
                        db.Book.Add(_currentBook);
                    else
                    {
                        var bookToUpdate = db.Book.Find(_currentBook.ID);
                        if (bookToUpdate != null)
                        {
                            bookToUpdate.Title = _currentBook.Title;
                            bookToUpdate.Penalty = _currentBook.Penalty;
                            bookToUpdate.AuthorID = _currentBook.AuthorID;
                            bookToUpdate.GenresID = _currentBook.GenresID;
                        }
                    }

                    db.SaveChanges();
                    MessageBox.Show("Данные сохранены!");
                    NavigationService?.Navigate(new BooksTabPage());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка БД: " + (ex.InnerException?.InnerException?.Message ?? ex.Message));
                }
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new BooksTabPage());
    }
}