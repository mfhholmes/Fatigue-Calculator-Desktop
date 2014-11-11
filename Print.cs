using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;

namespace Fatigue_Calculator_Desktop
{
	internal class Print
	{
		public bool printCalc(calculation currentCalc)
		{
			FlowDocument doc = new FlowDocument();
			PrintDialog diag = new PrintDialog();
			bool? result = diag.ShowDialog();
			if (result == true)
			{
				doc.PageWidth = diag.PrintableAreaWidth;
				doc.PageHeight = diag.PrintableAreaHeight;
				addText(doc, currentCalc);
				addGraph(doc, currentCalc);
				diag.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Fatigue Calculation Result");
			}
			return true;
		}
		public void addText(FlowDocument doc, calculation currentCalc)
		{
			Paragraph text = new Paragraph();
			addRun(text, "Fatigue Calculation", true);
			addRun(text, "", true);
			addRun(text, "Date: " + currentCalc.currentOutputs.calcDone.ToString("dd/MMM/yyyy"), true);
			addRun(text, "Time: " + currentCalc.currentOutputs.calcDone.ToString("h:mm tt"), true);
			addRun(text, "Operator: " + currentCalc.currentInputs.identity.ToString(), true);
			addRun(text, "", true);
			addRun(text, "Calculation Details", true);
			addRun(text, "Shift Start: " + currentCalc.currentInputs.shiftStart.ToString("dd/MMM/yyyy h:mm tt"), true);
			addRun(text, "Shift End: " + currentCalc.currentInputs.shiftEnd.ToString("dd/MMM/yyyy h:mm tt"), true);
			addRun(text, "Sleep in 24 hours: " + currentCalc.currentInputs.sleep24.ToString(), true);
			addRun(text, "Sleep in 48 hours: " + currentCalc.currentInputs.sleep48.ToString(), true);
			addRun(text, "Hours awake: " + currentCalc.currentInputs.hoursAwake.ToString(), true);
			if (!currentCalc.logged) addRun(text, "This calculation was not logged", true);
			addRun(text, "", true);
			addRun(text, "Calculation Results", true);
			addRun(text, "Current Score:" + currentCalc.currentOutputs.currentScore.ToString(), true);
			addRun(text, "Current Risk Level:" + currentCalc.currentOutputs.currentLevel.ToString(), true);
			if (currentCalc.currentOutputs.becomesModerate > new TimeSpan(0, 0, 0))
				{ addRun(text, "Risk becomes Moderate: " + (currentCalc.currentOutputs.calcDone + currentCalc.currentOutputs.becomesModerate).ToString("dd/MMM/yyyy h:mm tt"), true); }
			if (currentCalc.currentOutputs.becomesHigh > new TimeSpan(0, 0, 0))
				{ addRun(text, "Risk becomes High: " + (currentCalc.currentOutputs.calcDone + currentCalc.currentOutputs.becomesHigh).ToString("dd/MMM/yyyy h:mm tt"), true); }
			if (currentCalc.currentOutputs.becomesExtreme > new TimeSpan(0, 0, 0))
				{ addRun(text, "Risk becomes Extreme: " + (currentCalc.currentOutputs.calcDone + currentCalc.currentOutputs.becomesExtreme).ToString("dd/MMM/yyyy h:mm tt"), true); }
			text.Margin = new Thickness(0, 50, 0, 50);
			doc.Blocks.Add(text);
		}
		private void addGraph(FlowDocument doc, calculation currentCalc){
			//print the graph below the text
			Canvas cnv = new Canvas();
			cnv.Width = doc.PageWidth -50.0;
			cnv.Height = doc.PageWidth/4;
			cnv.Margin = new Thickness(0, 50, 50, 50);
			graph printgraph = new graph();
			printgraph.JustDrawShift = false;
			printgraph.MaxHours = 24;
			graphTheme theme = new graphTheme();
			theme.LineColour = Brushes.Black;
			theme.TextColour = Brushes.Black;
			theme.TickColour = Brushes.Black;
			printgraph.drawGraph(cnv, currentCalc, theme);
			Paragraph graphpara = new Paragraph();
			graphpara.Padding = new Thickness(0);
			graphpara.Margin = new Thickness(0);
			graphpara.Inlines.Add(cnv);
			doc.Blocks.Add(graphpara);
		}

		private void addRun(Paragraph text, string toAdd, bool addBreak)
		{
			Run newText = new Run(toAdd);
			text.Inlines.Add(newText);
			if (addBreak) text.Inlines.Add(new LineBreak());
		}
	}
}