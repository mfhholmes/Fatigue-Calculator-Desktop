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
        private string _defaultSettingsPath = "%app_path%";
        private string _dataPath = "";
        private ICalculatorSettings _settings;

        public adminMenuPage()
        {
            InitializeComponent();
        }

        public adminMenuPage(ICalculatorSettings _newSettings)
        {
            InitializeComponent();
            _settings = _newSettings;
        }
        /// <summary>
        /// loads the default settings file, and pulls the data file path from it
        /// </summary>
        private void LoadDefaults()
        {
            CalculatorSettingsFile _filesettings = new CalculatorSettingsFile();
            _settings = _filesettings;
            // parse the path
            string parsedPath = Utilities.parsePath(_defaultSettingsPath);
            // load up the settings file
            _settings = new CalculatorSettingsFile();
            string checkedPath = _filesettings.checkPath(parsedPath);
            if(_filesettings.LoadFile(checkedPath))
            {
                // we've got a good file, let's get the data path
                _dataPath = _settings.GetSetting("LogServiceURL");
            }
        }
        private void btnUserList_Click(object sender, RoutedEventArgs e)
        {
            if(_settings == null ) LoadDefaults();
            this.NavigationService.Navigate(new adminIDListPage(_settings));
        }

        private void btnGraph_Click(object sender, RoutedEventArgs e)
        {
            if (_settings == null) LoadDefaults();
            this.NavigationService.Navigate(new adminGraphPage(_settings));
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            if (_settings == null) LoadDefaults();
            this.NavigationService.Navigate(new adminSettingsPage(_settings));
        }

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            if (_settings == null) LoadDefaults();
            this.NavigationService.Navigate(new adminAdvancedOptionsPage(_settings));
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}
