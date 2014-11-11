using System.Windows;
using System.Windows.Controls;

namespace Fatigue_Calculator_Desktop
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow(Page startPage)
		{
			InitializeComponent();
			this.container.Navigate(startPage);
		}
	}
}