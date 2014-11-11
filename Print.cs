using System;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Fatigue_Calculator_Desktop
{
	internal class Print
	{
		public bool printCalc(calculation currentCalc)
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
			if (currentCalc.currentOutputs.becomesModerate > new TimeSpan(0, 0, 0)) addRun(text, "Risk becomes Moderate: " + (currentCalc.currentOutputs.calcDone + currentCalc.currentOutputs.becomesModerate).ToString("dd/MMM/yyyy h:mm tt"), true);
			if (currentCalc.currentOutputs.becomesHigh > new TimeSpan(0, 0, 0)) addRun(text, "Risk becomes High: " + (currentCalc.currentOutputs.calcDone + currentCalc.currentOutputs.becomesHigh).ToString("dd/MMM/yyyy h:mm tt"), true);
			if (currentCalc.currentOutputs.becomesExtreme > new TimeSpan(0, 0, 0)) addRun(text, "Risk becomes Extreme: " + (currentCalc.currentOutputs.calcDone + currentCalc.currentOutputs.becomesExtreme).ToString("dd/MMM/yyyy h:mm tt"), true);
			PrintUsingDocumentCondensed(text, "Fatigue Calculation Print");
			return true;
		}

		private void PrintUsingDocumentCondensed(Paragraph text, string printCaption)
		{
			//Create the document, passing a new paragraph and new run using text
			FlowDocument doc = new FlowDocument(text);
			PrintDialog diag = new PrintDialog();// used to perform printing
			//Send the document to the printer
			diag.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, printCaption);
		}

		private void addRun(Paragraph text, string toAdd, bool addBreak)
		{
			Run newText = new Run(toAdd);
			text.Inlines.Add(newText);
			if (addBreak) text.Inlines.Add(new LineBreak());
		}
	}
}