using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

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
				addNiceText(doc, currentCalc);
				addGraph(doc, currentCalc);
				diag.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Fatigue Calculation Result");
			}
			return true;
		}

		public bool printResultPage(calculation currentCalc)
		{
			FlowDocument doc = new FlowDocument();
			PrintDialog diag = new PrintDialog();
			bool? result = diag.ShowDialog();
			if (result == true)
			{
				doc.PageWidth = diag.PrintableAreaWidth;
				doc.PageHeight = diag.PrintableAreaHeight;
				addLogoHeader(doc);
				addNiceText(doc, currentCalc);
				addGraph(doc, currentCalc);
				diag.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Fatigue Calculation Result");
			}
			return true;
		}
		private void addText(FlowDocument doc, calculation currentCalc)
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

		private void addNiceText(FlowDocument doc, calculation currentCalc)
		{
			Section text= new Section();
			text.Blocks.Add(makeTitle("Fatigue Calculation Result"));
			Paragraph inputs = new Paragraph();
			addRun(inputs, "Date & Time: " + currentCalc.currentOutputs.calcDone.ToString("dd/MMM/yyyy hh:mm"), true);
			addRun(inputs, "Operator: " + currentCalc.currentInputs.identity.ToSmallString(), true);
			text.Blocks.Add(inputs);

			Paragraph outputs = new Paragraph();
			addSubTitle(outputs, "Calculation Details");
			addRun(outputs, "Shift Start: " + currentCalc.currentInputs.shiftStart.ToString("dd/MMM/yyyy hh:mm"), true);
			addRun(outputs, "Shift End: " + currentCalc.currentInputs.shiftEnd.ToString("dd/MMM/yyyy hh:mm"), true);
			addRun(outputs, "Sleep in 24 hours: " + currentCalc.currentInputs.sleep24.ToString(), true);
			addRun(outputs, "Sleep in 48 hours: " + currentCalc.currentInputs.sleep48.ToString(), true);
			addRun(outputs, "Hours awake: " + currentCalc.currentInputs.hoursAwake.ToString(), true);
			if (!currentCalc.logged) addRun(outputs, "This calculation was not logged", true);
			text.Blocks.Add(outputs);

			Paragraph results = new Paragraph();
			addSubTitle(results, "Calculation Results");
			addRun(results, "Current Score:" + currentCalc.currentOutputs.currentScore.ToString(), true);
			addRun(results, "Current Risk Level:" + currentCalc.currentOutputs.currentLevel.ToString(), true);
			if (currentCalc.currentOutputs.becomesModerate > new TimeSpan(0, 0, 0))
				{ addRun(results, "Risk becomes Moderate: " + (currentCalc.currentOutputs.calcDone + currentCalc.currentOutputs.becomesModerate).ToString("dd/MMM/yyyy h:mm tt"), true); }
			if (currentCalc.currentOutputs.becomesHigh > new TimeSpan(0, 0, 0))
				{ addRun(results, "Risk becomes High: " + (currentCalc.currentOutputs.calcDone + currentCalc.currentOutputs.becomesHigh).ToString("dd/MMM/yyyy h:mm tt"), true); }
			if (currentCalc.currentOutputs.becomesExtreme > new TimeSpan(0, 0, 0))
				{ addRun(results, "Risk becomes Extreme: " + (currentCalc.currentOutputs.calcDone + currentCalc.currentOutputs.becomesExtreme).ToString("dd/MMM/yyyy h:mm tt"), true); }
			text.Blocks.Add(results);
		
			text.Margin = new Thickness(0, 50, 0, 50);
			doc.Blocks.Add(text);
		}
		
		private void addLogoHeader(FlowDocument doc){
			double width = doc.PageWidth;
			double height = doc.PageHeight/4;
			
			Section headerSect = new Section();
			doc.Blocks.Add(headerSect);
			Paragraph header = new Paragraph();
			headerSect.Blocks.Add(header);
			Brush lc = Brushes.Black;
			double boxw = width / 4;
			
			//top header
			Figure th = new Figure();
			Paragraph tp = new Paragraph();
			tp.Inlines.Add(new Run("Cadia Valley Operations"));
			tp.Inlines.Add(new LineBreak());
			tp.Inlines.Add(new Run("Emergency Response, Access Control & Health"));
			tp.TextAlignment = TextAlignment.Center;
			tp.FontSize *= 1.25;
			tp.FontWeight = FontWeights.Bold;
			th.HorizontalAnchor = FigureHorizontalAnchor.PageLeft;
			th.HorizontalOffset = boxw;
			th.VerticalAnchor = FigureVerticalAnchor.PageTop;
			th.VerticalOffset = 0;
			th.Width = new FigureLength(boxw * 2);
			th.Height = new FigureLength(boxw);
			th.Padding = new Thickness(0);
			th.Margin = new Thickness(0);
			th.BorderBrush = lc;
			th.BorderThickness = new Thickness(1,2,1,1);
			th.Blocks.Add(tp);
			header.Inlines.Add(th);

			// bottom header
			Figure bh = new Figure();
			Paragraph bp = new Paragraph();
			bp.Inlines.Add(new Run("Fatigue Calculation"));
			bh.TextAlignment = TextAlignment.Center;
			bh.HorizontalAnchor = FigureHorizontalAnchor.PageLeft;
			bh.VerticalAnchor = FigureVerticalAnchor.PageTop;
			bh.VerticalOffset = boxw;
			bh.Width = new FigureLength(width);
			bh.Height = new FigureLength(boxw);
			bh.Padding = new Thickness(0);
			bh.Margin = new Thickness(0);
			bh.BorderBrush = lc;
			bh.BorderThickness = new Thickness(2,1,2,2);
			bh.Blocks.Add(bp);
			header.Inlines.Add(bh);

			//left image
			Figure li = new Figure();
			Image leftimage = new Image();
			li.HorizontalAnchor = FigureHorizontalAnchor.PageLeft;
			li.VerticalAnchor = FigureVerticalAnchor.PageTop;
			li.Width = new FigureLength(boxw);
			li.Height = new FigureLength(boxw);

			BitmapImage photoFile = new BitmapImage();
			photoFile.BeginInit();
			photoFile.UriSource = new Uri("Images/logo1.png", UriKind.Relative);
			photoFile.EndInit();
			leftimage.Source= photoFile;
			leftimage.Width = boxw;
			leftimage.Height = boxw;
			BlockUIContainer lp = new BlockUIContainer(leftimage);
			li.Blocks.Add(lp);
			header.Inlines.Add(li);

			//right image
			Figure ri = new Figure();
			Image rightimage = new Image();
			ri.HorizontalAnchor = FigureHorizontalAnchor.PageRight;
			ri.VerticalAnchor = FigureVerticalAnchor.PageTop;
			ri.Width = new FigureLength(boxw);
			ri.Height = new FigureLength(boxw);

			BitmapImage photoFile2 = new BitmapImage();
			photoFile2.BeginInit();
			photoFile2.UriSource = new Uri("Images/logo2.png", UriKind.Relative);
			photoFile2.EndInit();
			rightimage.Source = photoFile2;
			rightimage.Width = boxw;
			rightimage.Height = boxw;
			BlockUIContainer rp = new BlockUIContainer(rightimage);
			ri.Blocks.Add(rp);
			header.Inlines.Add(ri);

		}
		private Run addRun(Paragraph text, string toAdd, bool addBreak)
		{
			Run newText = new Run(toAdd);
			text.Inlines.Add(newText);
			if (addBreak) text.Inlines.Add(new LineBreak());
			return newText;
		}
		private Paragraph makeTitle(string toAdd)
		{
			Paragraph title = new Paragraph();
			Run txt = new Run(toAdd);
			txt.FontSize *= 1.75;
			txt.FontWeight = FontWeights.Bold;
			txt.TextDecorations = TextDecorations.Underline;
			title.Inlines.Add(txt);
			title.Inlines.Add(new LineBreak());
			return title;
		}
		private void addSubTitle(Paragraph text, string toAdd)
		{
			Run txt = new Run(toAdd);
			txt.FontSize *= 1.25;
			txt.FontWeight = FontWeights.Bold;
			txt.TextDecorations = TextDecorations.Underline;
			text.Inlines.Add(txt);
			text.Inlines.Add(new LineBreak());
		}
	}
}