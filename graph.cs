using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Fatigue_Calculator_Desktop
{
	internal class graph
	{
		// just Draw Shift: if set, only draws the hours involved in the user's shift pattern
		private bool _justDrawShift = false;

		public bool JustDrawShift
		{
			get { return _justDrawShift; }
			set { _justDrawShift = value; }
		}

		// max Hours: if >0 limits the number of hours drawn, otherwise draws however many is in the rosterLength in the calculation object
		private int _maxTicks = 0;

		public int MaxHours
		{
			get { return _maxTicks; }
			set { _maxTicks = value; }
		}

		//draw a graph on a canvas passed to it
		public bool drawGraph(Canvas context, calculation calc)
		{
			try
			{
				//assume the canvas is the correct scale
				// clear the canvas and set up the various constants
				context.Children.Clear();
				double x1 = context.ActualWidth / 10.0;
				double x2 = x1 * 9;
				double y1 = (context.ActualHeight / 10.0) * 2.0;
				double y2 = (context.ActualHeight / 10.0) * 6.0;

				//no vertical scale, just the horizontal one
				int hourStart, hourStop;
				DateTime timeStart, timeStop;
				// check if we're just displaying the shift graph, or the whole graph
				if (_justDrawShift)
				{
					// just the shift
					hourStart = calc.currentOutputs.shiftStart.Hours;
					hourStop = calc.currentOutputs.shiftEnd.Hours;
					timeStart = calc.currentOutputs.calcDone + calc.currentOutputs.shiftStart;
					timeStop = calc.currentOutputs.calcDone + calc.currentOutputs.shiftEnd;
				}
				else
				{
					// the whole pattern
					hourStart = 0;
					hourStop = calc.currentOutputs.rosterLength - 1;//zero-bounded
					if (hourStop > _maxTicks) hourStop = _maxTicks;
					timeStart = calc.currentOutputs.calcDone;
					timeStop = calc.currentOutputs.calcDone + new TimeSpan(hourStop, 0, 0);
				}
				// set up the ticks
				double tickHeight = y2 - y1;
				double tickWidth = (x2 - x1) / (double)(hourStop - hourStart);

				// YAxis
				drawLine(context, x1, y1, x1, y2, Brushes.White, 3, 1);
				// XAxis
				drawLine(context, x1, y2, x2, y2, Brushes.White, 3, 1);
				// Y-Axis ticks
				drawTicks(context, x1, y1, x1, y2, 5, -10);
				// X-Axis ticks
				drawTicks(context, x1, y2, x2, y2, (hourStop - hourStart), 10);
				// add in the times
				drawDayTicks(context, x1, y2, x2, y2, hourStart, hourStop, calc, 30, 20, Brushes.White, tickWidth);
				// bar segments and labels
				double lastSeg = 0, nextSeg = 0;
				// shift start and finish
				drawShiftMarker(context, x1, y1, x2, tickWidth, Brushes.White, 40, 40, (calc.currentInputs.shiftStart - timeStart), "shift start " + calc.currentInputs.shiftStart.ToString("ddd HH:mm"));
				drawShiftMarker(context, x1, y1, x2, tickWidth, Brushes.White, 20, 40, (calc.currentInputs.shiftEnd - timeStart), "shift end " + calc.currentInputs.shiftEnd.ToString("ddd HH:mm"));

				// so the graph is limited by hourStart and hourStop, and so the graph might start on a non-zero hour in the array
				// and may well start on an intermediate fatigue state

				// first find out what colour we start on
				calculation.fatigueLevels startLevel = calc.currentOutputs.roster[hourStart].level;
				// and find out what colour we stop on
				calculation.fatigueLevels stopLevel = calc.currentOutputs.roster[hourStop].level;
				if (startLevel == stopLevel)
				{
					drawBarSegment(context, x1, y1, x2, y2, calc.getColourForLevel(startLevel), 1);
					drawSegmentLabel(context, x1, y1, x2, y2, calc.getColourForLevel(startLevel), 1, startLevel.ToString() + " Fatigue Risk");
				}
				else
				{
					// more than one level, so iterate them
					int nextTransition = 0, lastTransition = hourStart, transHeight = 40;
					string transitionLabel = "";
					for (int i = calc.levelToNumber(startLevel); i <= calc.levelToNumber(stopLevel); i++)
					{
						//so work out which level we're dealing with and therefore where the next transition is
						if (i == 1)
						{
							nextTransition = (int)calc.currentOutputs.becomesModerate.TotalHours;
							transitionLabel = (calc.currentOutputs.calcDone).ToString("ddd HH:mm");
							transHeight = 40;
						}
						if (i == 2)
						{
							nextTransition = (int)calc.currentOutputs.becomesHigh.TotalHours;
							transitionLabel = (calc.currentOutputs.calcDone + calc.currentOutputs.becomesModerate).ToString("ddd HH:mm");
							transHeight = 80;
						}
						if (i == 3)
						{
							nextTransition = (int)calc.currentOutputs.becomesExtreme.TotalHours;
							transitionLabel = (calc.currentOutputs.calcDone + calc.currentOutputs.becomesHigh).ToString("ddd HH:mm");
							transHeight = 40;
						}
						if (i == 4)
						{
							nextTransition = hourStop;
							transitionLabel = (calc.currentOutputs.calcDone + calc.currentOutputs.becomesExtreme).ToString("ddd HH:mm");
							transHeight = 80;
						}
						// then work out where the lines are
						lastSeg = x1 + ((lastTransition - hourStart) * tickWidth);
						nextSeg = x1 + ((nextTransition - hourStart) * tickWidth);
						//then plot the segment
						drawBarSegment(context, lastSeg, y1, nextSeg, y2, calc.getColourForLevel(calc.levelFromNumber(i)), 1);
						drawSegmentLabel(context, lastSeg, y1, nextSeg, y2, Brushes.White, 1, calc.levelFromNumber(i).ToString() + " Fatigue Risk");
						drawLabel(context, lastSeg, y2, transHeight, 40, transitionLabel);
						lastTransition = nextTransition;
					}
				}
				drawShiftSegment(context, x1, y1, y2, Brushes.WhiteSmoke, 0.2, (int)Math.Round(calc.currentOutputs.shiftStart.TotalHours, 0), (int)Math.Round(calc.currentOutputs.shiftEnd.TotalHours, 0), tickWidth, _maxTicks);
			}// try block
			catch (Exception e)
			{
				MessageBox.Show("drawGraph: " + e.Message);
				return false;
			}
			return true;
		}

		private void drawLabel(Canvas context, double x, double y, double tickHeight, double textHeight, string label)
		{
			try
			{
				Line longTick = new Line();
				longTick.X1 = x;
				longTick.X2 = x;
				longTick.Y1 = y;
				longTick.Y2 = y + tickHeight;
				longTick.Stroke = Brushes.White;
				longTick.Opacity = 0.5;
				longTick.StrokeThickness = 2;
				Canvas.SetTop(longTick, 0);
				Canvas.SetLeft(longTick, 0);
				context.Children.Add(longTick);

				Label lbl = new Label();
				lbl.Margin = new System.Windows.Thickness(0);
				lbl.Padding = new System.Windows.Thickness(0);
				lbl.Content = label;
				lbl.Height = textHeight;
				lbl.Foreground = Brushes.White;
				lbl.FontSize = textHeight / 2; // apparently font sizes set in pixels, so let's see if this works ;)
				context.Children.Add(lbl);
				lbl.Measure(context.RenderSize);
				Canvas.SetLeft(lbl, (x - (lbl.DesiredSize.Width / 2)));
				Canvas.SetTop(lbl, y + tickHeight);
			}
			catch (Exception e)
			{
				MessageBox.Show("drawLabel: " + e.Message);
			}
		}

		private void drawLine(Canvas context, double x1, double y1, double x2, double y2, Brush colour, int thickness, double opacity)
		{
			try
			{
				Line newLine = new Line();
				newLine.X1 = x1;
				newLine.X2 = x2;
				newLine.Y1 = y1;
				newLine.Y2 = y2;
				newLine.Stroke = colour;
				newLine.StrokeThickness = thickness;
				newLine.Opacity = opacity;
				Canvas.SetLeft(newLine, 0);
				Canvas.SetTop(newLine, 0);
				context.Children.Add(newLine);
				newLine.Visibility = System.Windows.Visibility.Visible;
			}
			catch (Exception e)
			{
				MessageBox.Show("drawLine: " + e.Message);
			}
		}

		private void drawTicks(Canvas context, double x1, double y1, double x2, double y2, int numTicks, double tickSize)
		{
			try
			{
				double tickXoffset = tickSize;
				double tickYoffset = tickSize;
				if (x1 == x2) tickYoffset = 0; // vertical line, so no need for up-down component
				if (y1 == y2) tickXoffset = 0; // horizontal line
				double xx1, yy1; // calculated x and y positions for the ticks
				double xgap = x2 - x1; // distance between the two ends of the x-line
				double xtick = xgap / numTicks; // distance between each x-tick
				double ygap = y2 - y1; // same for y-line
				double ytick = ygap / numTicks;
				Line tickMark;
				for (int i = 0; i <= numTicks; i++)
				{
					xx1 = x1 + (xtick * i); // calculate x and y co-ords of the next tick
					yy1 = y1 + (ytick * i);
					tickMark = new Line(); // create a new line and set it up
					tickMark.StrokeThickness = 1;
					tickMark.Stroke = Brushes.White;
					Canvas.SetLeft(tickMark, 0);
					Canvas.SetTop(tickMark, 0);
					tickMark.X1 = xx1;
					tickMark.X2 = xx1 + tickXoffset;
					tickMark.Y1 = yy1;
					tickMark.Y2 = yy1 + tickYoffset;
					context.Children.Add(tickMark);
				}
			}
			catch (Exception e)
			{
				MessageBox.Show("drawTicks: " + e.Message);
			}
		}

		private void drawDayTicks(Canvas context, double x1, double y1, double x2, double y2, int hourStart, int hourStop, calculation calc, int tickHeight, int textHeight, Brush colour, double tickWidth)
		{
			try
			{
				// work out if there are any day boundaries between hourStart and hourStop
				DateTime timeStart, timeStop, timeTick;
				int numTicks;
				Line tickMark;
				timeStart = calc.currentOutputs.calcDone + new TimeSpan(hourStart, 0, 0);
				timeStop = calc.currentOutputs.calcDone + new TimeSpan(hourStop, 0, 0);
				// draw in the hour labels quickly
				string hourLabel = "";
				double tickPos = x1;
				for (DateTime hourTick = timeStart; hourTick < timeStop; hourTick += new TimeSpan(1, 0, 0))
				{
					hourLabel = hourTick.ToString("HH:mm");
					Label lbl = new Label();
					Viewbox vbox = new Viewbox();
					lbl.Content = hourLabel;
					lbl.Foreground = Brushes.White;
					lbl.LayoutTransform = new RotateTransform(270);
					vbox.Child = lbl;
					vbox.Height = tickHeight;
					vbox.Width = tickWidth;
					Canvas.SetTop(vbox, y2);
					Canvas.SetLeft(vbox, tickPos);
					context.Children.Add(vbox);
					tickPos = tickPos + tickWidth;
				}

				while (timeStart.Day != timeStop.Day)
				{
					//work out how many ticks between the last
					timeTick = timeStart + new TimeSpan(1, 0, 0, 0, 0);
					timeTick = new DateTime(timeTick.Year, timeTick.Month, timeTick.Day, 0, 0, 0);
					numTicks = (int)(timeTick - calc.currentOutputs.calcDone).TotalHours + 1;
					numTicks = numTicks - hourStart;
					// and draw the line
					tickMark = new Line(); // create a new line and set it up
					tickMark.StrokeThickness = 1;
					tickMark.Stroke = colour;
					Canvas.SetLeft(tickMark, 0);
					Canvas.SetTop(tickMark, 0);
					tickMark.X1 = x1 + (numTicks * tickWidth);
					tickMark.X2 = tickMark.X1;
					tickMark.Y1 = y1;
					tickMark.Y2 = y2;
					context.Children.Add(tickMark);
					drawLabel(context, tickMark.X1, tickMark.Y1, tickHeight, textHeight, timeTick.ToString("ddd dd/MM/yyyy"));
					// move the start time on a day
					timeStart = timeStart + new TimeSpan(1, 0, 0, 0, 0);
				}
			}
			catch (Exception e)
			{
				MessageBox.Show("drawDayTicks: " + e.Message);
			}
		}

		private void drawShiftMarker(Canvas context, double x1, double y1, double x2, double tickWidth, Brush colour, double tickHeight, double textHeight, TimeSpan shift, string label)
		{
			try
			{
				// shift markers are above the graph
				Label marker;
				// get the number of hours between origin and shift start
				int hours = (int)Math.Round(shift.TotalHours, 0);
				if (hours < 0) // always possible that they're doing a calculation mid-shift
				{
					hours = 0;
				}
				double pos = x1 + (hours * tickWidth);
				if (pos > x2) pos = x2;
				drawLine(context, pos, y1 - tickHeight, pos, y1, colour, 2, 1.0);
				marker = new Label();
				marker.Foreground = colour;
				marker.Height = textHeight;
				marker.Content = label;
				marker.FontSize = textHeight / 2;
				context.Children.Add(marker);
				marker.Measure(context.RenderSize);
				Canvas.SetTop(marker, (y1 - tickHeight) - textHeight);
				Canvas.SetLeft(marker, pos - (marker.DesiredSize.Width / 2));
			}
			catch (Exception e)
			{
				MessageBox.Show("drawShiftMarker: " + e.Message);
			}
		}

		private void drawBarSegment(Canvas context, double x1, double y1, double x2, double y2, Brush colour, double opacity)
		{
			try
			{
				Rectangle bar = new Rectangle();
				bar.Height = (y2 - y1);
				bar.Width = (x2 - x1);
				bar.Fill = colour;
				bar.Opacity = opacity;
				Canvas.SetTop(bar, y1);
				Canvas.SetLeft(bar, x1);
				context.Children.Add(bar);
			}
			catch (Exception e)
			{
				MessageBox.Show("drawBarSegment: " + e.Message);
			}
		}

		private void drawShiftSegment(Canvas context, double x1, double y1, double y2, Brush colour, double opacity, int shiftStart, int shiftEnd, double tickWidth, int maxTicks)
		{
			try
			{
				if (shiftEnd < shiftStart) return;
				if (shiftStart < 0) shiftStart = 0;
				if (shiftEnd < shiftStart) shiftEnd = shiftStart;
				if (shiftEnd > maxTicks) shiftEnd = maxTicks;
				double startX, endX;
				startX = x1 + (shiftStart * tickWidth);
				endX = x1 + (shiftEnd * tickWidth);
				Rectangle bar = new Rectangle();
				bar.Height = (y2 - y1);
				bar.Width = (endX - startX);
				bar.Fill = colour;
				bar.Opacity = opacity;
				Canvas.SetTop(bar, y1);
				Canvas.SetLeft(bar, startX);
				context.Children.Add(bar);
			}
			catch (Exception e)
			{
				MessageBox.Show("drawShiftSegment: " + e.Message);
			}
		}

		private void drawSegmentLabel(Canvas context, double x1, double y1, double x2, double y2, Brush colour, double opacity, string label)
		{
			try
			{
				Label lbl = new Label();
				Viewbox vbox = new Viewbox();
				lbl.Content = label;
				lbl.Foreground = colour;
				lbl.LayoutTransform = new RotateTransform(270);
				lbl.Opacity = opacity;
				vbox.Child = lbl;
				vbox.Height = y2 - y1;
				vbox.Width = x2 - x1;
				Canvas.SetTop(vbox, y1);
				Canvas.SetLeft(vbox, x1);
				context.Children.Add(vbox);
			}
			catch (Exception e)
			{
				MessageBox.Show("drawSegmentLine: " + e.Message);
			}
		}
	}
}