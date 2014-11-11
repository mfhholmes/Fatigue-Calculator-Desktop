using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace Fatigue_Calculator_Desktop
{
	/// <summary>
	/// Interaction logic for resultsPage.xaml
	/// </summary>
	public partial class resultsPage : Page
	{
		private calculation currentCalc;

		public resultsPage()
		{
			InitializeComponent();
		}

		public resultsPage(calculation passedCalc)
		{
			InitializeComponent();
			currentCalc = passedCalc;
			// show the results
			lblScore.Text = "Your Fatigue Score is " + currentCalc.currentOutputs.currentScore;
			lblLevel.Inlines.Clear();
			lblLevel.Inlines.Add(new Run("You are at "));
			Run levelRun = new Run(currentCalc.currentOutputs.currentLevel.ToString());
			levelRun.Foreground = currentCalc.getColourForLevel(currentCalc.currentOutputs.currentLevel);
			lblLevel.Inlines.Add(levelRun);
			lblLevel.Inlines.Add(" risk of Fatigue");
			if (currentCalc.currentOutputs.becomesModerate > new TimeSpan(0, 0, 0))
			{
				lblModerate.Text = "You will be at Moderate risk of Fatigue at " + (currentCalc.currentOutputs.calcDone + currentCalc.currentOutputs.becomesModerate).ToString("ddd dd MMM HH:mm");
				lblModerate.Visibility = System.Windows.Visibility.Visible;
				btnModerateRecommend.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{
				lblModerate.Visibility = System.Windows.Visibility.Hidden;
				btnModerateRecommend.Visibility = System.Windows.Visibility.Hidden;
				imgLow.Visibility = System.Windows.Visibility.Hidden;
				Grid.SetRow(imgModerate, 0);
				Grid.SetRow(lblHigh, 2);
				Grid.SetRow(btnHighRecommend, 2);
				Grid.SetRow(imgHigh, 2);
				Grid.SetRow(lblExtreme, 4);
				Grid.SetRow(btnExtremeRecommend, 4);
				Grid.SetRow(imgExtreme, 4);
			}
			if (currentCalc.currentOutputs.becomesHigh > new TimeSpan(0, 0, 0))
			{
				lblHigh.Text = "You will be at High risk of Fatigue at " + (currentCalc.currentOutputs.calcDone + currentCalc.currentOutputs.becomesHigh).ToString("ddd dd MMM HH:mm");
				lblHigh.Visibility = System.Windows.Visibility.Visible;
				btnHighRecommend.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{
				lblHigh.Visibility = System.Windows.Visibility.Hidden;
				btnHighRecommend.Visibility = System.Windows.Visibility.Hidden;
				imgModerate.Visibility = System.Windows.Visibility.Hidden;
				Grid.SetRow(imgHigh, 0);
				Grid.SetRow(lblExtreme, 2);
				Grid.SetRow(btnExtremeRecommend, 2);
				Grid.SetRow(imgExtreme, 2);
			}
			if (currentCalc.currentOutputs.becomesExtreme > new TimeSpan(0, 0, 0))
			{
				lblExtreme.Text = "You will be at Extreme risk of Fatigue at " + (currentCalc.currentOutputs.calcDone + currentCalc.currentOutputs.becomesExtreme).ToString("ddd dd MMM HH:mm");
				lblExtreme.Visibility = System.Windows.Visibility.Visible;
				btnExtremeRecommend.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{
				lblLevel.Inlines.Add(new Run(" from now until you next sleep"));
				lblExtreme.Visibility = System.Windows.Visibility.Hidden;
				btnExtremeRecommend.Visibility = System.Windows.Visibility.Hidden;
				imgHigh.Visibility = System.Windows.Visibility.Hidden;
				Grid.SetRow(imgExtreme, 0);
			}
		}

		private void btnBack_Click(object sender, RoutedEventArgs e)
		{
			this.NavigationService.GoBack();
		}

		private void btnGo_Click(object sender, RoutedEventArgs e)
		{
			recommendPage next = new recommendPage(currentCalc);
			this.NavigationService.Navigate(next);
		}

		private void btnGraph_Click(object sender, RoutedEventArgs e)
		{
			graphPage graph = new graphPage(currentCalc);
			NavigationService.Navigate(graph);
		}

		private void btnCurrentRecommend_Click(object sender, RoutedEventArgs e)
		{
			recommendPage next = new recommendPage(currentCalc);
			this.NavigationService.Navigate(next);
		}

		private void btnModerateRecommend_Click(object sender, RoutedEventArgs e)
		{
			recommendPage next = new recommendPage(currentCalc, calculation.fatigueLevels.Moderate);
			this.NavigationService.Navigate(next);
		}

		private void btnHighRecommend_Click(object sender, RoutedEventArgs e)
		{
			recommendPage next = new recommendPage(currentCalc, calculation.fatigueLevels.High);
			this.NavigationService.Navigate(next);
		}

		private void btnExtremeRecommend_Click(object sender, RoutedEventArgs e)
		{
			recommendPage next = new recommendPage(currentCalc, calculation.fatigueLevels.Extreme);
			this.NavigationService.Navigate(next);
		}

		private void btnPrint_Click(object sender, RoutedEventArgs e)
		{
			Print printit = new Print();
			//printit.printCalc(currentCalc);
			printit.printResultPage(currentCalc);
		}
	}
}