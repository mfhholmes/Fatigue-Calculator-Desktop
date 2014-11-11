using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace Fatigue_Calculator_Desktop
{
	/// <summary>
	/// Interaction logic for StatusPage.xaml
	/// </summary>
	public partial class adminAdvancedOptionsPage : Page
	{
		//private string _dataPath = "";
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
			try
			{
				Config.ConfigSettings.settings.logServiceUrl = txtFilePath.Text;
				Config.ConfigSettings.saveSettings();
				showSettings();
			}
			catch (Exception err)
			{
				LabelError.Text = err.Message;
			}
		}

		private void showSettings()
		{
			bdrSettings.Visibility = System.Windows.Visibility.Visible;
			bdrChanges.Visibility = System.Windows.Visibility.Hidden;
			txtData.Text = Config.ConfigSettings.settings.logServiceUrl;
			txtSettings.Text = Config.ConfigSettings.source.URL;
			btnChangeSettings.Visibility = System.Windows.Visibility.Hidden;
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