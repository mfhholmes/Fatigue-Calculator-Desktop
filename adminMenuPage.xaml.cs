using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fatigue_Calculator_Desktop
{
    /// <summary>
    /// Interaction logic for MenuPage.xaml
    /// </summary>
    public partial class adminMenuPage : Page
    {
        //private string _dataPath = "";

        public adminMenuPage()
        {
            InitializeComponent();
        }

        private void btnUserList_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new adminIDListPage());
        }

        private void btnGraph_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new adminGraphPage());
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new adminSettingsPage());
        }

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new adminAdvancedOptionsPage());
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}
