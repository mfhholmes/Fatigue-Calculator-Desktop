using System.Windows;
using System.Windows.Controls;

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
			//check for remote log service
			checkLogService(Config.ConfigSettings.settings.logServiceUrl);
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

		private void checkLogService(string url)
		{
			// check for a remote log
			ILogService log = LogFactory.createLog(url);
			if (log.thisLogType != LogType.local)
			{
				GraphLabel.Text = "See Fatigue Manager for calculation log information";
				btnGraph.IsEnabled = false;
				btnGraph.Opacity = 0.5;
			}
			else
			{
				GraphLabel.Text = "Display the data collected by the Fatigue Calculator in Graph Format";
				btnGraph.IsEnabled = true;
				btnGraph.Opacity = 1;
			}
		}
	}
}