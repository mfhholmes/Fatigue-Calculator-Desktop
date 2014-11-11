using System.Windows;
using System.Windows.Controls;

namespace Fatigue_Calculator_Desktop
{
	/// <summary>
	/// Interaction logic for researchPage.xaml
	/// </summary>
	public partial class researchPage : Page
	{
		private calculation currentCalc;

		public researchPage()
		{
			InitializeComponent();
		}

		public researchPage(calculation passedCalc)
		{
			InitializeComponent();
			currentCalc = passedCalc;
		}

		private void btnGo_Click(object sender, RoutedEventArgs e)
		{
			currentCalc.currentInputs.identity.ResearchApproved = identity.researchStates.research_approved;
			saveIdentity();
			shiftPage nextPage = new shiftPage(currentCalc);
			this.NavigationService.Navigate(nextPage);
		}

		private void btnBack_Click(object sender, RoutedEventArgs e)
		{
			currentCalc.currentInputs.identity.ResearchApproved = identity.researchStates.research_denied;
			saveIdentity();
			shiftPage nextPage = new shiftPage(currentCalc);
			this.NavigationService.Navigate(nextPage);
		}

		private void saveIdentity()
		{
			IIdentityService idfile = new identityFile();
			idfile.SetIdentityListSource(Config.ConfigSettings.settings.IDLookupFile);
			// the identity search only uses name (or id if the names don't find a match), so we can use the current calc identity for both search value and identity value.
			idfile.ChangeIdentity(currentCalc.currentInputs.identity, currentCalc.currentInputs.identity);
		}
	}
}