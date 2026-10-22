using System;
using System.Collections.Generic;
using System.Data;
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
    public partial class AddLoanPage : Page
    {
        private Loan _currentLoan = new Loan();

        public AddLoanPage(Loan selectedLoan)
        {
            InitializeComponent();

            if (selectedLoan != null)
            {
                _currentLoan = selectedLoan;
                TitleText.Text = "Редактирование выдачи";
                DPIssueDate.SelectedDate = _currentLoan.LoanDate;
                DPReturnDate.SelectedDate = _currentLoan.ReturnDate;
            }
            else
            {
                _currentLoan = new Loan();
                DPIssueDate.SelectedDate = DateTime.Today;
                DPReturnDate.SelectedDate = DateTime.Today.AddDays(14);
            }

            using (var db = new DB_Sedikov_LibraryEntities())
            {
                CmbBook.ItemsSource = db.Book.ToList();
                CmbUser.ItemsSource = db.User.ToList();

                if (selectedLoan != null)
                {
                    CmbBook.SelectedValue = _currentLoan.BookID;
                    CmbBook.SelectedValuePath = "ID";
                    CmbUser.SelectedValue = _currentLoan.UserID;
                    CmbUser.SelectedValuePath = "ID";
                }
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (CmbBook.SelectedItem == null)
            {
                MessageBox.Show("Выберите книгу!");
                return;
            }
            if (CmbUser.SelectedItem == null)
            {
                MessageBox.Show("Выберите пользователя!");
                return;
            }
            if (DPIssueDate.SelectedDate == null || DPReturnDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите обе даты!");
                return;
            }
            if (DPReturnDate.SelectedDate < DPIssueDate.SelectedDate)
            {
                MessageBox.Show("Дата возврата не может быть раньше даты выдачи!");
                return;
            }

            var selectedBook = CmbBook.SelectedItem as Book;
            var selectedUser = CmbUser.SelectedItem as User;

            _currentLoan.BookID = selectedBook.ID;
            _currentLoan.UserID = selectedUser.ID;
            _currentLoan.LoanDate = DPIssueDate.SelectedDate.Value;
            _currentLoan.ReturnDate = DPReturnDate.SelectedDate.Value;

            using (var db = new DB_Sedikov_LibraryEntities())
            {
                try
                {
                    if (_currentLoan.ID == 0)
                    {
                        var bookToUpdate = db.Book.Find(_currentLoan.BookID);

                        if (bookToUpdate == null || bookToUpdate.Penalty <= 0)
                        {
                            MessageBox.Show("Этой книги нет в наличии!");
                            return;
                        }

                        bookToUpdate.Penalty -= 1;

                        db.Loan.Add(_currentLoan);
                    }
                    else
                    {
                        var loanToUpdate = db.Loan.Find(_currentLoan.ID);
                        if (loanToUpdate != null)
                        {
                            loanToUpdate.UserID = _currentLoan.UserID;
                            loanToUpdate.LoanDate = _currentLoan.LoanDate;
                            loanToUpdate.ReturnDate = _currentLoan.ReturnDate;
                        }
                    }

                    db.SaveChanges();
                    MessageBox.Show("Данные сохранены!");
                    NavigationService?.Navigate(new LoansTabPage());
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + (ex.InnerException?.InnerException?.Message ?? ex.Message));
                }
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e) => NavigationService?.Navigate(new LoansTabPage());
    }
}