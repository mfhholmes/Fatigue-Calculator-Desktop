﻿using System.Windows;
using System.Windows.Controls;

namespace Fatigue_Calculator_Desktop
{
	/// <summary>
	/// Interaction logic for disclaimerPage.xaml
	/// </summary>
	public partial class disclaimerPage : Page
	{
		private calculation currentCalc;

		public disclaimerPage()
		{
			InitializeComponent();
		}

		public disclaimerPage(calculation passedCalc)
		{
			InitializeComponent();
			currentCalc = passedCalc;
		}

		private void btnGo_Click(object sender, RoutedEventArgs e)
		{
			if (Config.ConfigSettings.settings.researchPage == "shown")
			{
				researchPage nextPage = new researchPage(currentCalc);
				this.NavigationService.Navigate(nextPage);
			}
			else
			{
				shiftPage nextPage = new shiftPage(currentCalc);
				this.NavigationService.Navigate(nextPage);
			}
		}

		private void btnBack_Click(object sender, RoutedEventArgs e)
		{
			this.NavigationService.GoBack();
		}
	}
}