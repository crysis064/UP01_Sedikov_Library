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
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
        }

        private void BtnUsers_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new UsersTabPage());
        private void BtnCategories_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new GenresTabPage());
        private void BtnBooks_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new BooksTabPage());
        private void BtnLoans_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new LoansTabPage());
        private void BtnDiagrams_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new DiagrammPage());
        private void BtnLogout_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AuthPage());
        private void BtnAuthors_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AuthorsTabPage());
    }
}