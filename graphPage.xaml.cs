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
    /// Interaction logic for graphPage.xaml
    /// </summary>
    public partial class graphPage : Page
    {
        private calculation currentCalc;
        private int numHours = 24;
        public graphPage()
        {
            InitializeComponent();
        }
        public graphPage(calculation passedCalc)
        {
            InitializeComponent();
            currentCalc = passedCalc;
        }
        
        public void drawGraph()
        {
            graph newGraph = new graph();
            newGraph.MaxHours = numHours;
            newGraph.drawGraph(context, currentCalc);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void btnRecommend_Click(object sender, RoutedEventArgs e)
        {
            recommendPage newPage = new recommendPage(currentCalc);
            this.NavigationService.Navigate(newPage);
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            drawGraph();
        }

        private void btnGraph24_Click(object sender, RoutedEventArgs e)
        {
            numHours = 24;
            drawGraph();
        }

        private void btnGraph48_Click(object sender, RoutedEventArgs e)
        {
            numHours = 48;
            drawGraph();
        }

        private void btnGraph72_Click(object sender, RoutedEventArgs e)
        {
            numHours = 72;
            drawGraph();
        }
    }
}
