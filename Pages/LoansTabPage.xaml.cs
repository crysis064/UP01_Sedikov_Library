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
    public partial class LoansTabPage : Page
    {
        public LoansTabPage()
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
                    DataGridLoans.ItemsSource = db.Loan.Include("Book").Include("User").ToList();
                }
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new AddLoanPage(null));

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var loansForRemoving = DataGridLoans.SelectedItems.Cast<Loan>().ToList();
            if (loansForRemoving.Count == 0) { MessageBox.Show("Выберите запись!"); return; }

            if (MessageBox.Show($"Удалить {loansForRemoving.Count} записей?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var db = new DB_Sedikov_LibraryEntities())
                {
                    try
                    {
                        foreach (var loan in loansForRemoving)
                        {
                            var book = db.Book.Find(loan.BookID);
                            if (book != null)
                            {
                                book.Penalty += 1;
                            }

                            db.Loan.Attach(loan);
                            db.Loan.Remove(loan);
                        }
                        db.SaveChanges();
                        DataGridLoans.ItemsSource = db.Loan.Include("Book").Include("User").ToList();
                    }
                    catch (System.Exception ex) { MessageBox.Show(ex.Message); }
                }
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedLoan = (sender as Button).DataContext as Loan;
            NavigationService?.Navigate(new AddLoanPage(selectedLoan));
        }
    }
}