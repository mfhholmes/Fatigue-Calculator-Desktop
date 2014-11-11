using System.Windows;
using System.Windows.Controls;

namespace Fatigue_Calculator_Desktop
{
	/// <summary>
	/// Interaction logic for welcomePage.xaml
	/// </summary>
	public partial class welcomePage : Page
	{
		public welcomePage()
		{
			InitializeComponent();
		}

		private void go_Click(object sender, RoutedEventArgs e)
		{
			// need to create a new calculation object every time we leave the welcome page
			calculation newCalc = new calculation();
			// next page is the identity page, depending on licence type

			identityPage next = new identityPage(newCalc);
			if (next.skipPage)
				this.NavigationService.Navigate(new shiftPage(newCalc));
			else
				this.NavigationService.Navigate(next);
		}
	}
}