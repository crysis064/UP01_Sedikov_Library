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

namespace UP01_Sedikov_Library.Pages
{
    public partial class AddGenresPage : Page
    {
        private Genres _currentGenre = new Genres();

        public AddGenresPage(Genres selectedGenre)
        {
            InitializeComponent();
            if (selectedGenre != null)
            {
                _currentGenre = selectedGenre;
                TitleText.Text = "Редактирование жанра";
            }
            DataContext = _currentGenre;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_currentGenre.Name))
            {
                MessageBox.Show("Введите название жанра!");
                return;
            }

            using (var db = new DB_Sedikov_LibraryEntities())
            {
                try
                {
                    if (_currentGenre.ID == 0)
                        db.Genres.Add(_currentGenre);
                    else
                        db.Entry(_currentGenre).State = EntityState.Modified;

                    db.SaveChanges();
                    MessageBox.Show("Данные сохранены!");
                    NavigationService?.Navigate(new GenresTabPage());
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new GenresTabPage());
    }
}