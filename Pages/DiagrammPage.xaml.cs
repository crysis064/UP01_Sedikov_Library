using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;

namespace UP01_Sedikov_Library.Pages
{
    public partial class DiagrammPage : Page
    {
        private readonly Chart chart;

        public DiagrammPage()
        {
            InitializeComponent();

            WindowsFormsHost host = new WindowsFormsHost();

            chart = new Chart();
            chart.ChartAreas.Add(new ChartArea("MainArea"));
            chart.Legends.Add(new Legend("MainLegend") { Docking = Docking.Top });

            host.Child = chart;
            ChartContainer.Children.Add(host);

            LoadData();
        }

        private void LoadData()
        {
            using (var db = new DB_Sedikov_LibraryEntities())
            {
                var users = db.User.ToList();
                users.Insert(0, new User { ID = 0, FIO = "Все пользователи" });
                CmbUser.ItemsSource = users;
                CmbUser.SelectedIndex = 0;

                CmbDiagram.ItemsSource = Enum.GetValues(typeof(SeriesChartType));
                CmbDiagram.SelectedItem = SeriesChartType.Column;
            }
        }

        private void UpdateChart(object sender, SelectionChangedEventArgs e)
        {
            if (CmbUser.SelectedItem == null || CmbDiagram.SelectedItem == null) return;

            var selectedUser = CmbUser.SelectedItem as User;
            var selectedType = (SeriesChartType)CmbDiagram.SelectedItem;

            chart.Series.Clear();

            var series = new Series("Выдачи")
            {
                ChartType = selectedType,
                IsValueShownAsLabel = true
            };
            using (var db = new DB_Sedikov_LibraryEntities())
            {
                var loans = db.Loan.Include("Book.Genres").AsQueryable();

                if (selectedUser.ID != 0)
                {
                    loans = loans.Where(l => l.UserID == selectedUser.ID);
                }

                var data = loans.ToList()
                    .GroupBy(l => l.Book.Genres.Name)
                    .Select(g => new { Genre = g.Key, Count = g.Count() })
                    .ToList();

                foreach (var item in data)
                {
                    series.Points.AddXY(item.Genre, item.Count);
                }
            }

            if (series.Points.Count == 0)
            {
                series.Points.AddXY("Нет данных", 0);
            }

            chart.Series.Add(series);
        }

        private void ExportExcel_Click(object sender, RoutedEventArgs e)
        {
            var application = new Excel.Application();
            Excel.Workbook workbook = null;

            try
            {
                using (var db = new DB_Sedikov_LibraryEntities())
                {
                    var selectedUser = CmbUser.SelectedItem as User;
                    var allUsers = db.User.ToList().OrderBy(u => u.FIO).ToList();

                    if (selectedUser != null && selectedUser.ID != 0)
                    {
                        allUsers = new System.Collections.Generic.List<User> { selectedUser };
                    }

                    application.SheetsInNewWorkbook = allUsers.Count;
                    workbook = application.Workbooks.Add(Type.Missing);

                    for (int i = 0; i < allUsers.Count; i++)
                    {
                        int startRowIndex = 1;
                        Excel.Worksheet worksheet = application.Worksheets.Item[i + 1];

                        string sheetName = allUsers[i].FIO;
                        if (sheetName.Length > 31) sheetName = sheetName.Substring(0, 31);
                        worksheet.Name = sheetName;

                        worksheet.Cells[1][startRowIndex] = "Книга";
                        worksheet.Cells[2][startRowIndex] = "Автор";
                        worksheet.Cells[3][startRowIndex] = "Дата выдачи";
                        worksheet.Cells[4][startRowIndex] = "Дата возврата";

                        Excel.Range columnHeaderRange = worksheet.Range[worksheet.Cells[1][1], worksheet.Cells[4][1]];
                        columnHeaderRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        columnHeaderRange.Font.Bold = true;
                        startRowIndex++;

                        int currentUserId = allUsers[i].ID;
                        var loans = db.Loan.Where(l => l.UserID == currentUserId).ToList();

                        if (loans.Count > 0)
                        {
                            foreach (var loan in loans)
                            {
                                worksheet.Cells[1][startRowIndex] = loan.Book.Title;
                                worksheet.Cells[2][startRowIndex] = loan.Book.Author.FIO;
                                worksheet.Cells[3][startRowIndex] = loan.LoanDate.ToString("dd.MM.yyyy");
                                worksheet.Cells[4][startRowIndex] = loan.ReturnDate.ToString("dd.MM.yyyy");
                                startRowIndex++;
                            }
                        }
                        else
                        {
                            worksheet.Cells[1][startRowIndex] = "Нет выданных книг";
                        }

                        Excel.Range rangeBorders = worksheet.Range[worksheet.Cells[1][1], worksheet.Cells[4][startRowIndex - 1]];
                        rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle =
                        rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle =
                        rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle =
                        rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle =
                        rangeBorders.Borders[Excel.XlBordersIndex.xlInsideHorizontal].LineStyle =
                        rangeBorders.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;

                        worksheet.Columns.AutoFit();
                    }
                }

                application.Visible = true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка при экспорте в Excel: " + ex.Message);
                if (workbook != null) workbook.Close(false, Type.Missing, Type.Missing);
                application.Quit();
            }
        }

        private void ExportWord_Click(object sender, RoutedEventArgs e)
        {
            var application = new Word.Application();
            Word.Document document = application.Documents.Add();

            try
            {
                using (var db = new DB_Sedikov_LibraryEntities())
                {
                    var selectedUser = CmbUser.SelectedItem as User;
                    var allUsers = db.User.ToList().OrderBy(u => u.FIO).ToList();

                    if (selectedUser != null && selectedUser.ID != 0)
                    {
                        allUsers = new System.Collections.Generic.List<User> { selectedUser };
                    }

                    foreach (var user in allUsers)
                    {
                        Word.Paragraph userParagraph = document.Paragraphs.Add();
                        Word.Range userRange = userParagraph.Range;
                        userRange.Text = user.FIO;
                        userParagraph.set_Style("Заголовок 1");
                        userRange.InsertParagraphAfter();

                        var loans = db.Loan.Where(l => l.UserID == user.ID).ToList();

                        if (loans.Count > 0)
                        {
                            Word.Paragraph tableParagraph = document.Paragraphs.Add();
                            Word.Range tableRange = tableParagraph.Range;
                            Word.Table loansTable = document.Tables.Add(tableRange, loans.Count + 1, 4);
                            loansTable.Borders.InsideLineStyle = loansTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;

                            loansTable.Cell(1, 1).Range.Text = "Книга";
                            loansTable.Cell(1, 2).Range.Text = "Автор";
                            loansTable.Cell(1, 3).Range.Text = "Дата выдачи";
                            loansTable.Cell(1, 4).Range.Text = "Дата возврата";

                            loansTable.Rows[1].Range.Font.Name = "Times New Roman";
                            loansTable.Rows[1].Range.Font.Size = 12;
                            loansTable.Rows[1].Range.Bold = 1;
                            loansTable.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                            for (int i = 0; i < loans.Count; i++)
                            {
                                var loan = loans[i];
                                loansTable.Cell(i + 2, 1).Range.Text = loan.Book.Title;

                                loansTable.Cell(i + 2, 2).Range.Text = loan.Book.Author.FIO;

                                loansTable.Cell(i + 2, 3).Range.Text = loan.LoanDate.ToString("dd.MM.yyyy");
                                loansTable.Cell(i + 2, 4).Range.Text = loan.ReturnDate.ToString("dd.MM.yyyy");

                                loansTable.Rows[i + 2].Range.Font.Name = "Times New Roman";
                                loansTable.Rows[i + 2].Range.Font.Size = 12;
                            }
                        }
                        else
                        {
                            Word.Paragraph emptyParagraph = document.Paragraphs.Add();
                            emptyParagraph.Range.Text = "Нет выданных книг.";
                        }

                        if (user != allUsers.LastOrDefault())
                        {
                            document.Words.Last.InsertBreak(Word.WdBreakType.wdPageBreak);
                        }
                    }
                }

                application.Visible = true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка при экспорте в Word: " + ex.Message);
                application.Quit();
            }
        }
    }
}