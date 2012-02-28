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
using Microsoft.Win32;
using System.IO;
using System.Configuration;

namespace Fatigue_Calculator_Desktop
{
    /// <summary>
    /// Interaction logic for StatusPage.xaml
    /// </summary>
    public partial class adminAdvancedOptionsPage : Page
    {
        private string _dataPath = "";
        private string _filter = "";

        public adminAdvancedOptionsPage()
        {
            InitializeComponent();
            showSettings();
        }

       
        private void btnChangeSettings_Click(object sender, RoutedEventArgs e)
        {
            LabelKey.Text = "Settings File";
            txtFilePath.Text = txtSettings.Text;
            LabelError.Text = "";
            _filter = "Config Files|*.exe.config";
            showChanges();
        }

        private void btnChangeData_Click(object sender, RoutedEventArgs e)
        {
            LabelKey.Text = "Data File";
            txtFilePath.Text = txtData.Text;
            LabelError.Text = "";
            _filter = "Log files|*.csv";
            showChanges();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog browser = new OpenFileDialog();
            browser.Filter = _filter;
            browser.Title = "Choose file";
            browser.Multiselect = false;
            if (browser.ShowDialog() == true)
            {
                txtFilePath.Text = browser.FileName;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            // disable for now until I can work out how the advanced options page will work to the future   
            //if (LabelKey.Text == "Settings File")
            //{
            //    // parse the path
            //    string parsedPath = Utilities.parsePath(txtFilePath.Text);
            //    // load up the settings file
            //    _settings = new CalculatorSettingsFile();
            //    CalculatorSettingsFile settingsFile = new CalculatorSettingsFile();
            //    string checkedPath = settingsFile.checkPath(parsedPath);
            //    if (settingsFile.LoadFile(checkedPath))
            //    {
            //        // all good
            //        txtSettings.Text = txtFilePath.Text;
            //        _settings = settingsFile;
            //        _dataPath = _settings.GetSetting("LogServiceURL");
            //        showSettings();
            //    }
            //    else
            //    {
            //        //failed to load
            //        LabelError.Text = "Invalid File" + System.Environment.NewLine + _settings.lastValidationError;
            //    }
            //}
            //else
            //{
            //    txtData.Text = txtFilePath.Text;
            //    _dataPath = txtFilePath.Text;
            //    showSettings();
            //}
        }

        private void showSettings()
        {
            bdrSettings.Visibility = System.Windows.Visibility.Visible;
            bdrChanges.Visibility = System.Windows.Visibility.Hidden;
        }
        private void showChanges()
        {
            bdrSettings.Visibility = System.Windows.Visibility.Hidden;
            bdrChanges.Visibility = System.Windows.Visibility.Visible;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // just hide this and don't change anything
            showSettings();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new adminMenuPage());
        }
    }
}
