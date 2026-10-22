using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace UP01_Sedikov_Library
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            MainFrame.Navigated += MainFrame_Navigated;
        }

        private void MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (MainFrame.Content is Page page)
            {
                this.Title = page.Title;
            }
        } 
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DateTimeNow.Foreground = new SolidColorBrush(Color.FromRgb(35, 62, 118));
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.IsEnabled = true;
            timer.Tick += (o, t) => {
                DateTimeNow.Text = DateTime.Now.ToString();
            };
            timer.Start();
            MainFrame.Navigate(new Pages.AuthPage());
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
            {
                MainFrame.GoBack();
            }
        }
        void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите закрыть окно?", "Message",
            MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                e.Cancel = true;
            else

                e.Cancel = false;
        }
        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            var dict = new ResourceDictionary();
            if (Properties.Settings.Default.Theme == "Light")
            {
                dict.Source = new Uri("DarkDictionary.xaml", UriKind.Relative);
                Properties.Settings.Default.Theme = "Dark";
            }
            else
            {
                dict.Source = new Uri("Dictionary.xaml", UriKind.Relative);
                Properties.Settings.Default.Theme = "Light";
            }
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);
            Properties.Settings.Default.Save();
        }
    }
}
