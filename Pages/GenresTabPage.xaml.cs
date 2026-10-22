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
using System.Data.Entity;

namespace UP01_Sedikov_Library.Pages
{
    public partial class GenresTabPage : Page
    {
        public GenresTabPage()
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
                    DataGridGenres.ItemsSource = db.Genres.ToList();
                }
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AddGenresPage(null));

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var genresForRemoving = DataGridGenres.SelectedItems.Cast<Genres>().ToList();

            if (genresForRemoving.Count == 0)
            {
                MessageBox.Show("Выберите жанр!");
                return;
            }

            if (MessageBox.Show($"Удалить {genresForRemoving.Count} записей?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var db = new DB_Sedikov_LibraryEntities())
                {
                    try
                    {
                        foreach (var genre in genresForRemoving)
                        {
                            db.Genres.Attach(genre);
                            db.Genres.Remove(genre);
                        }
                        db.SaveChanges();
                        DataGridGenres.ItemsSource = db.Genres.ToList();
                    }
                    catch (System.Exception ex) { MessageBox.Show(ex.Message); }
                }
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedGenre = (sender as Button).DataContext as Genres;
            NavigationService?.Navigate(new AddGenresPage(selectedGenre));
        }
    }
}