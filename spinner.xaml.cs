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
using System.Windows.Threading;

namespace Fatigue_Calculator_Desktop
{
    /// <summary>
    /// Interaction logic for spinner.xaml
    /// </summary>
    public partial class spinner : UserControl
    {
#region "properties"
        private DispatcherTimer UpSpinTimer;
        private DispatcherTimer DownSpinTimer;
        private double Velocity = 1000;

        private bool UpMouseDown = false;
        private bool DownMouseDown = false;

        private double _velocityShift = 0.5;

        private int currentValue= 0;

        private int _step = 1;
        public int Step
        {
            get {return _step;}
            set {_step = value;}
        }
        public int Value
        {
            get { return currentValue; }
            set { currentValue = value; adjustValue(0); }
        }
        private int _maxValue;
        public int maxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; if (currentValue > value) { currentValue = value; adjustValue(0); } }
        }
        private int _minValue;
        public int minValue
        {
            get { return _minValue; }
            set { _minValue = value; if (currentValue < value) { currentValue = value; adjustValue(0); } }
        }
        public double velocityShift
        {
            get { return _velocityShift; }
            set { _velocityShift = value; }
        }
        private int _numDigits = 2;
        private string _formatDigits = "D2";
        public int numDigits
        {
            get { return _numDigits; }
            set { _numDigits = value; _formatDigits = "D" + _numDigits.ToString(); }
        }
        private bool _showMax = true;
        public bool ShowMax
        {
            get { return _showMax; }
            set { _showMax = value; }

        }
#endregion
#region "constructor"
        public spinner()
        {
            InitializeComponent();
            UpSpinTimer = new DispatcherTimer();
            UpSpinTimer.Tick += UpTick;
            DownSpinTimer = new DispatcherTimer();
            DownSpinTimer.Tick += DownTick;
        }
#endregion
#region "Events"
        public static readonly RoutedEvent ValueChangedEvent = EventManager.RegisterRoutedEvent("ValueChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(spinner));
        public event RoutedEventHandler ValueChanged
        {
            add { AddHandler(ValueChangedEvent, value); }
            remove { RemoveHandler(ValueChangedEvent, value); }
        }
        public static readonly RoutedEvent UnderflowEvent = EventManager.RegisterRoutedEvent("Underflow", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(spinner));
        public event RoutedEventHandler Underflow
        {
            add { AddHandler(UnderflowEvent, value); }
            remove { RemoveHandler(UnderflowEvent, value); }
        }
        public static readonly RoutedEvent OverflowEvent = EventManager.RegisterRoutedEvent("Overflow", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(spinner));
        public event RoutedEventHandler Overflow
        {
            add { AddHandler(OverflowEvent, value); }
            remove { RemoveHandler(OverflowEvent, value); }
        }

#endregion
#region "methods"
        private void spinDown_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // decrement the hour immediately on every mousedown
            adjustValue(_step * -1);
            // set the timer up to call the increment again in a bit if they hold the mouse down
            DownMouseDown = true;
            Velocity = 1000;
            DownSpinTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)Velocity);
            DownSpinTimer.Start();
        }

        private void spinDown_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DownMouseDown = false;
        }

        private void spinUp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // increment the hour immediately on every mousedown
            adjustValue(_step);
            // set the timer up to call the increment again in a bit if they hold the mouse down
            UpMouseDown = true;
            Velocity = 1000;
            UpSpinTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)Velocity);
            UpSpinTimer.Start();
        }

        private void spinUp_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            UpMouseDown = false;
        }

        private void changeValue(string text, int position)
        {
            // change the value by the amount input
            string value = currentValue.ToString(_formatDigits);
            value = value.Substring(0, position) + text + value.Substring(position);
            if (checkInput(value)) currentValue = Convert.ToInt16(value);
            adjustValue(0);
        }
        private void adjustValue(int amount)
        {
            //adjust the hour by the amount
            currentValue = currentValue + amount;
            //bounds check - if we can't show the maximum then we have to exceed bounds at the max instead of over it
            RoutedEventArgs overunderflow = new RoutedEventArgs();
            bool overunderflown = false;
            if (_showMax)
            {
                if (currentValue > _maxValue)
                {
                    currentValue =_minValue;
                    overunderflow = new RoutedEventArgs(OverflowEvent, this);
                    overunderflown = true;
                }
            }
            else
                if (currentValue >= _maxValue)
                {
                    currentValue =_minValue;
                    overunderflow = new RoutedEventArgs(OverflowEvent, this);
                    overunderflown = true;
                }

            if (currentValue < _minValue)
            {
                if (_showMax) currentValue = _maxValue;
                else currentValue = _maxValue - _step;
                overunderflow = new RoutedEventArgs(UnderflowEvent, this);
                overunderflown = true;
            }
            // write the value
            txtValue.Content = currentValue.ToString(_formatDigits );
            // if we triggered an over or underflow, raise it now
            if (overunderflown) RaiseEvent(overunderflow);
            // always raise a Changed event
            RoutedEventArgs changedEvent = new RoutedEventArgs(ValueChangedEvent, this);
            RaiseEvent(changedEvent);
        }
        protected void DownTick(object sender, EventArgs e)
        {
            // check they're still holding the mouse down
            if (DownMouseDown)
            {
                // yep, so decrement the number
                adjustValue(-1 * _step);
                //reduce the velocity
                Velocity = Velocity * _velocityShift;
                //bounds check
                if (Velocity < 100) Velocity = 100;
                DownSpinTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)Velocity);
            }
            else
            {
                // nope, so stop the timer
                DownSpinTimer.Stop();
            }
        }
        protected void UpTick(object sender, EventArgs e)
        {
            // check they're still holding the mouse down
            if (UpMouseDown)
            {
                // yep, so decrement the number
                adjustValue(_step);
                //reduce the velocity
                Velocity = Velocity * _velocityShift;
                //bounds check
                if (Velocity < 100) Velocity = 100;
                UpSpinTimer.Interval = new TimeSpan(0, 0, 0, 0, (int)Velocity);
            }
            else
            {
                // nope, so stop the timer
                UpSpinTimer.Stop();
            }
        }
        

        private void spinUp_MouseLeave(object sender, MouseEventArgs e)
        {
            UpMouseDown = false;
        }

        private void spinDown_MouseLeave(object sender, MouseEventArgs e)
        {
            DownMouseDown = false;
        }

        private void txtValue_Click(object sender, RoutedEventArgs e)
        {
            // same as click up, but no mouse held down thing
            adjustValue(_step);
        }

        private bool checkInput(string textToCheck)
        {
            // check each character in the new text.
            foreach (char c in textToCheck.ToCharArray())
            {
                // if it's not a valid number, kill the entry
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }


#endregion


            
    }
}
