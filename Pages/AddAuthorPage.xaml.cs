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
    public partial class AddAuthorPage : Page
    {
        private readonly Author _currentAuthor = new Author();

        public AddAuthorPage(Author selectedAuthor)
        {
            InitializeComponent();
            if (selectedAuthor != null)
            {
                _currentAuthor = selectedAuthor;
                TitleText.Text = "Редактирование автора";
            }
            DataContext = _currentAuthor;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_currentAuthor.FIO))
            {
                MessageBox.Show("Введите ФИО автора!");
                return;
            }

            using (var db = new DB_Sedikov_LibraryEntities())
            {
                try
                {
                    if (_currentAuthor.ID == 0) db.Author.Add(_currentAuthor);
                    else
                    {
                        var authorToUpdate = db.Author.Find(_currentAuthor.ID);
                        if (authorToUpdate != null)
                        {
                            authorToUpdate.FIO = _currentAuthor.FIO;
                        }
                    }

                    db.SaveChanges();
                    MessageBox.Show("Данные сохранены!");
                    NavigationService?.Navigate(new AuthorsTabPage());
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AuthorsTabPage());
    }
}