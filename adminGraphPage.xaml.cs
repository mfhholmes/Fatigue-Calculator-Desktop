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
using Fatigue_Calculator_Desktop;

namespace Fatigue_Calculator_Desktop
{
    /// <summary>
    /// Interaction logic for graphPage.xaml
    /// </summary>
    public partial class adminGraphPage : Page
    {
        private ICalculatorSettings _settings;
        private string _dataFilePath;
        private DateTime _firstResult = DateTime.Now;
        private DateTime _lastResult = DateTime.Now;

        private class graphEntry
        {
            public Brush[] hourColour = new Brush[48];
            public logEntry entry;
            public graphEntry(logEntry values)
            {
                entry = values;
                calcHourValues();
            }
            private void calcHourValues(bool justShift = true)
            {
                if (entry.isValid == false)
                {
                    for (int i = 0; i < 48; i++) hourColour[i] = Brushes.Gray;
                    return;
                }
                int done = entry.dateTimeDone.Hour;
                //int mod = ((DateTime)entry.becomesModerate).Hour;
                //int high = ((DateTime)entry.becomesHigh).Hour;
                //int ext = ((DateTime)entry.becomesExtreme).Hour;
                //int start = ((DateTime)entry.shiftStart).Hour;
                //int end = ((DateTime)entry.shiftEnd).Hour;
                int mod = ((DateTime)entry.becomesModerate - entry.dateTimeDone).Hours + done;
                int high = ((DateTime)entry.becomesHigh - entry.dateTimeDone).Hours + done;
                int ext = ((DateTime)entry.becomesExtreme - entry.dateTimeDone).Hours + done;
                int start = ((DateTime)entry.shiftStart - entry.dateTimeDone).Hours + done;
                int end = ((DateTime)entry.shiftEnd - entry.dateTimeDone).Hours + done;

                // check for extreme cases
                //if (((TimeSpan)((DateTime)entry.becomesExtreme - entry.dateTimeDone)).Hours > 48) ext = 48;
                //if (((TimeSpan)((DateTime)entry.becomesHigh - entry.dateTimeDone)).Hours > 48) high = 48;
                //if (((TimeSpan)((DateTime)entry.becomesModerate - entry.dateTimeDone)).Hours > 48) mod = 48;
                if (mod > 48) mod = 48;
                if (high > 48) high = 48;
                if (ext > 48) ext = 48;
                if (start < 0) start = 0;
                if (end < 0) end = 0;
                if (start > 48) start = 48;
                if (end > 48) end = 48;

                for (int i = 0; i < 48; i++)
                {
                    //disregard shift until the end, just work out which colour this should be
                    if (i < done)
                        hourColour[i] = Brushes.Transparent;
                    else if (i >= done && i < mod)
                        hourColour[i] = Brushes.Green;
                    else if (i >= mod && i < high)
                        hourColour[i] = Brushes.Orange;
                    else if (i >= high && i < ext)
                        hourColour[i] = Brushes.Red;
                    else if (i >= ext)
                        hourColour[i] = Brushes.Black;

                    // now work out shift
                    if (justShift)
                    {
                        if (i < start || i > end)
                            hourColour[i] = Brushes.Transparent;
                    }

                }


            }

        }

        private enum graphType
        {
            byDate = 0,
            byID = 1
        }
        private graphType _graphType = graphType.byDate;
        private ILogService _log;

        public adminGraphPage()
        {
            InitializeComponent();
        }
        public adminGraphPage(ICalculatorSettings settings)
        {
            InitializeComponent();
            _settings = settings;
            setUpPage(_settings.GetSetting("LogServiceURL"));
        }
        /// <summary>
        /// sets up the graph page with the data pulled from the data file
        /// </summary>
        /// <param name="dataFilePath">the path to the current data file</param>
        private void setUpPage(string dataFilePath)
        {
            _dataFilePath = Utilities.checkFileName(dataFilePath);
            if (_dataFilePath.Length == 0)
            {
                // invalid data path sent...so tell them
                TextBlock error = new TextBlock();
                error.Inlines.Add(new Run("A data file cannot be found at the location specified"));
                error.Inlines.Add(new LineBreak());
                error.Inlines.Add(new Run("Please check your settings"));
                error.FontFamily = lblTitle.FontFamily;
                error.FontSize = 36;
                error.Foreground = Brushes.Red;
            }
            else
            {
                LoadStartup();
                //start in date mode with the last date possible
                dtpDate.SelectedDate = dtpDate.DisplayDateEnd;
                setType(graphType.byDate);
            }
        }

        /// <summary>
        /// Loads the startup summary data from the data file
        /// </summary>
        private void LoadStartup()
        {
            // open the file and parse the data in
            _log = new logFile();
            _log.setLogURL(_dataFilePath);
            // grab the earliest and latest dates from the file
            _firstResult = _log.logEntries.Min(entry => entry.dateTimeDone);
            _lastResult = _log.logEntries.Max(entry => entry.dateTimeDone);
            // set the datepickers to have those dates as limits
            dtpFirst.DisplayDateStart = _firstResult;
            dtpFirst.DisplayDateEnd = _lastResult;
            dtpLast.DisplayDateStart = _firstResult;
            dtpLast.DisplayDateEnd = _lastResult;
            dtpDate.DisplayDateStart = _firstResult;
            dtpDate.DisplayDateEnd = _lastResult;

            // load up the combo for ids
            cboIds.Items.Clear();
            foreach (string ID in _log.logEntries.Select(entry => entry.Identity).Distinct<string>()) cboIds.Items.Add(ID);

        }

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnSwitch_Click(object sender, RoutedEventArgs e)
        {
            if (_graphType == graphType.byDate)
            {
                setType(graphType.byID);
            }
            else
            {
                setType(graphType.byDate);
            }
        }
        /// <summary>
        /// sets the graph screen up to use the specified type of graph
        /// </summary>
        /// <param name="newType">the type of graph to switch to</param>
        private void setType(graphType newType)
        {
            if (newType == graphType.byDate)
            {
                // switching to byDate
                _graphType = graphType.byDate;
                btnSwitch.Content = "Graph By ID";
                grdByDate.Visibility = System.Windows.Visibility.Visible;
                grdByID.Visibility = System.Windows.Visibility.Hidden;
                if(dtpDate.SelectedDate.HasValue)
                    drawDateGraph(dtpDate.SelectedDate.Value);
            }
            else
            {
                // switching to byID
                _graphType = graphType.byID;
                btnSwitch.Content = "Graph By Date";
                grdByDate.Visibility = System.Windows.Visibility.Hidden;
                grdByID.Visibility = System.Windows.Visibility.Visible;
                if(dtpFirst.SelectedDate.HasValue && dtpLast.SelectedDate.HasValue && cboIds.SelectedIndex > -1)
                    drawIDGraph(cboIds.SelectedItem.ToString(), dtpFirst.SelectedDate.Value, dtpLast.SelectedDate.Value);
            }
        }

        /// <summary>
        /// draws a date graph, which shows all log entries for a given date
        /// </summary>
        /// <param name="dateToDraw">the date to draw the graph for</param>
        private void drawDateGraph(DateTime dateToDraw)
        {
            // set the start date as the beginning of the day specified, and the end as the finish
            DateTime start = new DateTime(dateToDraw.Year, dateToDraw.Month, dateToDraw.Day, 0, 0, 0);
            DateTime end = new DateTime(dateToDraw.Year, dateToDraw.Month, dateToDraw.Day, 23, 59, 59);
            var result = from logEntry entry in _log.logEntries
                         where (entry.dateTimeDone > start && entry.dateTimeDone < end)
                         select entry;


            setUpGrid("Identity");
            //this graph adds a line for each entry in the results
            long rows = result.LongCount<logEntry>();
            for (int i = 1; i <= rows; i++)
            {
                RowDefinition newRow = new RowDefinition();
                newRow.Height = new GridLength(30);
                grdResults.RowDefinitions.Add(newRow);
            }
            // now iterate through the results and add the appropriate names and colour segments
            graphEntry[] rowvalues = new graphEntry[rows];
            int rowCounter = 1;
            foreach (logEntry entry in result)
            {
                rowvalues[rowCounter - 1] = new graphEntry(entry);
                addText(entry.Identity, rowCounter, 0, 1,22);
                for (int i = 0; i < 24; i++)
                {
                    addRect(rowvalues[rowCounter-1].hourColour[i], rowCounter, i+1);
                }
                rowCounter++;
            }
        }

        /// <summary>
        /// draws an ID graph, which shows all the log entries for a given identity between the dates specified
        /// </summary>
        /// <param name="ID">the identity to draw for</param>
        /// <param name="fromDate">the earliest date to draw from</param>
        /// <param name="toDate">the latest date to draw to</param>
        private void drawIDGraph(string ID, DateTime fromDate, DateTime toDate)
        {
            if (toDate < fromDate)
            {
                // bloody users
                dtpLast.SelectedDate = dtpFirst.SelectedDate;
                // that should trigger a redraw so we quit here
                return;
            }
            // set the start date as the beginning of the day specified, and the end as the finish
            DateTime start = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0, 0, 0);
            DateTime end = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);
            var result = from logEntry entry in _log.logEntries 
                         where (entry.dateTimeDone > start && entry.dateTimeDone < end && entry.Identity==ID)
                         select entry;

            // we have one row for each day between the start and finish dates
            setUpGrid("Date");
            int rows = (toDate - fromDate).Days+1;
            for (int i = 1; i <= rows; i++)
            {
                RowDefinition newRow = new RowDefinition();
                newRow.Height = new GridLength(30);
                grdResults.RowDefinitions.Add(newRow);
                addText((fromDate + new TimeSpan(i-1, 0, 0, 0)).ToString("dd MMM yyyy"), i, 0, 1, 22);
            }

            int row = 1;
            // cull the data set of extraneous entries, only keep the last entry done on each date
            List<logEntry> culledResults = new List<logEntry>();
            DateTime[] doneTimes = new DateTime[rows];
            foreach (logEntry entry in result)
            { 
                row = (entry.dateTimeDone - start).Days;
                if(entry.dateTimeDone > doneTimes[row])
                {
                    doneTimes[row] = entry.dateTimeDone;
                    culledResults.Add(entry);
                }
            }

            graphEntry[] rowvalues = new graphEntry[rows];
            foreach (logEntry entry in culledResults)
            {
                
                //work out which row this lives in (row is always the date the calc was done in, and we always start in row 1)
                row = (entry.dateTimeDone - start).Days +1;
                rowvalues[row-1] = new graphEntry(entry);

                //work out which hour we start in (hours < start hour are plotted on the following day)
                int hour = entry.dateTimeDone.Hour;
                // plot from hour to 24 but only if there's something to plot
                for (int i = hour; i < 24;i++ )
                    if (rowvalues[row - 1].hourColour[i] != Brushes.Transparent)
                        addRect(rowvalues[row-1].hourColour[i], row, i + 1);
                // then plot the next day if it's less than the rows we're plotting
                if(row < rows)
                    for(int i=24;i<48;i++)
                        if (rowvalues[row - 1].hourColour[i] != Brushes.Transparent)
                            addRect(rowvalues[row-1].hourColour[i], row+1, i - 23);
            }
            
        }
        private void setUpGrid(string header)
        {
            // clear down the old
            grdResults.Children.Clear();
            if(grdResults.RowDefinitions.Count>1) grdResults.RowDefinitions.RemoveRange(1, grdResults.RowDefinitions.Count - 1);
            // header
            addText("header", 0, 0, 1, 22);
            // times
            addText("00:00", 0, 1, 4, 22);
            addText("04:00", 0, 5, 4, 22);
            addText("08:00", 0, 9, 4, 22);
            addText("12:00", 0, 13, 4, 22);
            addText("16:00", 0, 17, 4, 22);
            addText("20:00", 0, 21, 4, 22);
        }

        private TextBlock addText(string text,int row, int column, int cspan, int fontSize)
        {
            TextBlock newText = new TextBlock();
            newText.Text = text;
            newText.FontSize = fontSize;
            Grid.SetRow(newText, row);
            Grid.SetColumn(newText, column);
            Grid.SetColumnSpan(newText, cspan);
            grdResults.Children.Add(newText);
            return newText;
        }
        private Rectangle addRect(Brush colour, int row, int col)
        {
            Rectangle result = new Rectangle();
            result.Fill = colour;
            Grid.SetRow(result,row);
            Grid.SetColumn(result,col);
            result.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            result.VerticalAlignment= System.Windows.VerticalAlignment.Stretch;
            grdResults.Children.Add(result);
            return result;
        }

        private void checkValidIDGraphParams()
        {
            if ((dtpFirst.SelectedDate.HasValue) && (dtpLast.SelectedDate.HasValue) && (cboIds.SelectedIndex > -1))
                drawIDGraph(cboIds.SelectedItem.ToString(), dtpFirst.SelectedDate.Value, dtpLast.SelectedDate.Value);
        }

        private void checkValidDateGraphParams()
        {
            if (dtpDate.SelectedDate.HasValue)
                drawDateGraph(dtpDate.SelectedDate.Value);
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void dtp_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_graphType == graphType.byID) checkValidIDGraphParams();
        }

        private void dtpDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_graphType == graphType.byDate) checkValidDateGraphParams();
        }

        private void cboIds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_graphType == graphType.byID) checkValidIDGraphParams();
        }


    }
}
