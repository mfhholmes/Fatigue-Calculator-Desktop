using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Fatigue_Calculator_Desktop
{
	/// <summary>
	/// Interaction logic for recommendPage.xaml
	/// </summary>
	public partial class recommendPage : Page
	{
		private calculation currentCalc;

		public recommendPage()
		{
			InitializeComponent();
		}

		public recommendPage(calculation passedCalc)
		{
			InitializeComponent();
			currentCalc = passedCalc;
			writeRecommendations(currentCalc.currentOutputs.currentLevel);
		}

		public recommendPage(calculation passedCalc, calculation.fatigueLevels adviceLevel)
		{
			InitializeComponent();
			currentCalc = passedCalc;
			writeRecommendations(adviceLevel);
		}

		private void btnBack_Click(object sender, RoutedEventArgs e)
		{
			this.NavigationService.GoBack();
		}

		private void btnFinish_Click(object sender, RoutedEventArgs e)
		{
			// clear any nav history so someone else can't end up navigating back through this calculation
			while (this.NavigationService.CanGoBack)
			{
				this.NavigationService.RemoveBackEntry();
			}

			welcomePage next = new welcomePage();
			this.NavigationService.Navigate(next);
		}

		private void writeRecommendations(calculation.fatigueLevels adviceLevel)
		{
			Advice.Visibility = System.Windows.Visibility.Visible;
			controls.Visibility = System.Windows.Visibility.Hidden;
			symptoms.Visibility = System.Windows.Visibility.Hidden;
			title.Text = "Fatigue Recommendations";

			// reset the advice levels and get the basic recommendations for the fatigue risk level on the screen
			DateTime start, finish;
			Run level;
			Advice.Inlines.Clear();
			switch (adviceLevel)
			{
				case calculation.fatigueLevels.Low:
					{
						//keep calm and carry on
						if (currentCalc.currentOutputs.currentLevel == adviceLevel)
						{
							addRun("You have been identified as a ");
							level = new Run("LOW");
							level.Foreground = Brushes.Green;
							level.FontWeight = FontWeights.Bold;
							Advice.Inlines.Add(level);
							addRun(" Fatigue Likelihood", 2);
						}
						else
						{
							addRun("Should you become identified as a ");
							level = new Run("LOW");
							level.Foreground = Brushes.Green;
							level.FontWeight = FontWeights.Bold;
							Advice.Inlines.Add(level);
							addRun(" Fatigue Likelihood", 2);
						}
						addRun("Please continue your duties as normal, and ensure you continue to monitor your symptoms of fatigue.", 2);
						addRun("Click the 'Fatigue Symptoms' button below for a list of fatigue symptoms.", 2);
						addRun("If you notice ");
						addRun("THREE", 0, true);
						addRun(" of these symptoms within ");
						addRun("FIFTEEN", 0, true);
						addRun(" minutes you may need to implement further controls. ", 2);
						addRun("The list of controls can be found if you click the 'Fatigue Controls' button below.", 1);
						break;
					}
				case calculation.fatigueLevels.Moderate:
					{
						// mild panic
						if (currentCalc.currentOutputs.currentLevel == adviceLevel)
						{
							addRun("You have been identified as a ");
							level = new Run("MODERATE");
							level.Foreground = Brushes.Orange;
							level.FontWeight = FontWeights.Bold;
							Advice.Inlines.Add(level);
							addRun(" Fatigue Likelihood.", 2);
						}
						else
						{
							if (currentCalc.currentOutputs.becomesModerate > new TimeSpan(0, 0, 0))
							{
								start = currentCalc.currentOutputs.calcDone + currentCalc.currentOutputs.becomesModerate;
								finish = currentCalc.currentOutputs.calcDone + currentCalc.currentOutputs.becomesHigh;
								addRun("You have been identified as a ");
								level = new Run("MODERATE");
								level.Foreground = Brushes.Orange;
								level.FontWeight = FontWeights.Bold;
								Advice.Inlines.Add(level);
								addRun(" Fatigue Likelihood from ");
								addRun(start.ToString("dddd HH:mm"));
								addRun(" to ");
								addRun(finish.ToString("dddd HH:mm"), 2);
							}
							else
							{
								addRun("Should you become identified as a ");
								level = new Run("MODERATE");
								level.Foreground = Brushes.Orange;
								level.FontWeight = FontWeights.Bold;
								Advice.Inlines.Add(level);
								addRun(" Fatigue Likelihood.", 2);
							}
						}
						addRun("Please continue your duties as normal, and implement relevant individual controls.", 2);
						addRun("Ensure you continue to monitor your symptoms of fatigue.", 1);
						addRun("If you notice ");
						addRun("THREE", 0, true);
						addRun(" fatigue symptoms in ");
						addRun("FIFTEEN", 0, true);
						addRun(" minutes, and the individual controls are not assisting in keeping you alert, you may need to report to your supervisor.", 1);
						break;
					}
				case calculation.fatigueLevels.High:
					{
						string startText = "";
						// normal levels of service will be resumed shortly
						if (currentCalc.currentOutputs.currentLevel == adviceLevel)
						{
							addRun("You have been identified as a ");
							level = new Run("HIGH");
							level.Foreground = Brushes.Red;
							level.FontWeight = FontWeights.Bold;
							Advice.Inlines.Add(level);
							addRun(" Fatigue Likelihood", 2);

							startText = "Notify your supervisor that you are already a HIGH Fatigue Risk";
						}
						else
						{
							if (currentCalc.currentOutputs.becomesHigh > new TimeSpan(0, 0, 0))
							{
								start = currentCalc.currentOutputs.calcDone + currentCalc.currentOutputs.becomesHigh;
								finish = currentCalc.currentOutputs.calcDone + currentCalc.currentOutputs.becomesExtreme;
								addRun("You have been identified as a ");
								level = new Run("HIGH");
								level.Foreground = Brushes.Red;
								level.FontWeight = FontWeights.Bold;
								Advice.Inlines.Add(level);
								addRun(" Fatigue Likelihood from ");
								addRun(start.ToString("dddd HH:mm"));
								addRun(" to ");
								addRun(finish.ToString("dddd HH:mm"), 2);
								if (start.Date == DateTime.Now.Date)
									startText = "Notify your supervisor that you will become a HIGH Fatigue Risk at " + start.ToString("HH:mm");
								else
									startText = "Notify your supervisor that you will become a HIGH Fatigue Risk on " + start.ToString("dddd HH:mm");
							}
							else
							{
								addRun("Should you become identified as a ");
								level = new Run("HIGH");
								level.Foreground = Brushes.Red;
								level.FontWeight = FontWeights.Bold;
								Advice.Inlines.Add(level);
								addRun(" Fatigue Likelihood", 2);

								startText = "You will need to notify your supervisor when you are identified as becoming a HIGH Fatigue risk";
							}
						}

						addRun("Please report to your supervisor before commencing duties to determine what Team Based Controls you will need to implement.", 2);
						addRun(startText, 1);
						addRun("Ensure you continue to monitor your symptoms of fatigue. ");
						addRun("THREE", 0, true);
						addRun(" fatigue symptoms in ");
						addRun("FIFTEEN", 0, true);
						addRun(" minutes could indicate fatigue. ", 1);
						break;
					}
				case calculation.fatigueLevels.Extreme:
					{
						// go straight home. Do not pass Go
						if (currentCalc.currentOutputs.currentLevel == adviceLevel)
						{
							addRun("You have been identified as an ");
							level = new Run("EXTREME");
							level.Foreground = Brushes.Black;
							level.FontWeight = FontWeights.Bold;
							Advice.Inlines.Add(level);
							addRun(" Fatigue Likelihood.", 2);
						}
						else
						{
							if (currentCalc.currentOutputs.becomesExtreme > new TimeSpan(0, 0, 0))
							{
								start = currentCalc.currentOutputs.calcDone + currentCalc.currentOutputs.becomesExtreme;
								addRun("You have been identified as an ");
								level = new Run("EXTREME");
								level.Foreground = Brushes.Black;
								level.FontWeight = FontWeights.Bold;
								Advice.Inlines.Add(level);
								addRun(" Fatigue Likelihood from ");
								addRun(start.ToString("dddd HH:mm"));
								addRun(" until you next sleep.", 2);
								addRun("At this time:", 1);
							}
							else
							{
								addRun("Should you become identified as an ");
								level = new Run("EXTREME");
								level.Foreground = Brushes.Black;
								level.FontWeight = FontWeights.Bold;
								Advice.Inlines.Add(level);
								addRun(" Fatigue Likelihood", 2);
							}
						}
						addRun("You are NOT to commence duties.", 2, true);
						addRun("You are to report to your supervisor.", 2, true);
						addRun("It is highly recommended that you obtain a safe means of transportation to your final destination.", 1, false);
						break;
					}
				default:
					{
						// a previously unknown level of fatigue!
						addRun("You have been misidentified as an UNKNOWN Fatigue Likelihood", 2);
						addRun("Please alert your supervisor immediately as this software is experiencing technical difficulties.", 2);
						break;
					}
			}
		}

		private void addRun(string text, int numLineBreaks = 0, bool isBold = false)
		{
			Run newRun = new Run(text);
			if (isBold) newRun.FontWeight = FontWeights.Bold;
			Advice.Inlines.Add(newRun);
			while (numLineBreaks > 0)
			{
				Advice.Inlines.Add(new LineBreak());
				numLineBreaks--;
			}
		}

		private void writeSymptoms()
		{
			Advice.Visibility = System.Windows.Visibility.Hidden;
			controls.Visibility = System.Windows.Visibility.Hidden;
			symptoms.Visibility = System.Windows.Visibility.Visible;
			title.Text = "Fatigue Symptoms";
		}

		private void writeControls()
		{
			Advice.Visibility = System.Windows.Visibility.Hidden;
			controls.Visibility = System.Windows.Visibility.Visible;
			symptoms.Visibility = System.Windows.Visibility.Hidden;
			title.Text = "Individual Fatigue Controls";
		}

		private void btnControls_Click(object sender, RoutedEventArgs e)
		{
			writeControls();
		}

		private void btnSymptoms_Click(object sender, RoutedEventArgs e)
		{
			writeSymptoms();
		}

		private void btnRecommend_Click(object sender, RoutedEventArgs e)
		{
			writeRecommendations(currentCalc.currentOutputs.currentLevel);
		}
	}
}