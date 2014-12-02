using System;
using System.IO.Ports;
using System.Net.Mime;
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
				addText(doc, currentCalc);
				diag.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Fatigue Calculation Result");
			}
			return true;
		}

		
		//Standard simple text for the non-pretty output required for some devices
		private void addText(FlowDocument doc, calculation currentCalc)
		{
			Paragraph text = new Paragraph();
			MakeRun(text, "Fatigue Calculation", true);
			MakeRun(text, "", true);
			MakeRun(text, "Date: " + currentCalc.currentOutputs.calcDone.ToString("dd/MMM/yyyy"), true);
			MakeRun(text, "Time: " + currentCalc.currentOutputs.calcDone.ToString("h:mm tt"), true);
			MakeRun(text, "Operator: " + currentCalc.currentInputs.identity.ToString(), true);
			MakeRun(text, "", true);
			MakeRun(text, "Calculation Details", true);
			MakeRun(text, "Shift Start: " + currentCalc.currentInputs.shiftStart.ToString("dd/MMM/yyyy h:mm tt"), true);
			MakeRun(text, "Shift End: " + currentCalc.currentInputs.shiftEnd.ToString("dd/MMM/yyyy h:mm tt"), true);
			MakeRun(text, "Sleep in 24 hours: " + currentCalc.currentInputs.sleep24.ToString(), true);
			MakeRun(text, "Sleep in 48 hours: " + currentCalc.currentInputs.sleep48.ToString(), true);
			MakeRun(text, "Hours awake: " + currentCalc.currentInputs.hoursAwake.ToString(), true);
			if (!currentCalc.logged) MakeRun(text, "This calculation was not logged", true);
			MakeRun(text, "", true);
			MakeRun(text, "Calculation Results", true);
			MakeRun(text, "Current Score:" + currentCalc.currentOutputs.currentScore.ToString(), true);
			MakeRun(text, "Current Risk Level:" + currentCalc.currentOutputs.currentLevel.ToString(), true);
			if (currentCalc.currentOutputs.becomesModerate > new TimeSpan(0, 0, 0))
				{ MakeRun(text, "Risk becomes Moderate: " + (currentCalc.currentOutputs.calcDone + currentCalc.currentOutputs.becomesModerate).ToString("dd/MMM/yyyy h:mm tt"), true); }
			if (currentCalc.currentOutputs.becomesHigh > new TimeSpan(0, 0, 0))
				{ MakeRun(text, "Risk becomes High: " + (currentCalc.currentOutputs.calcDone + currentCalc.currentOutputs.becomesHigh).ToString("dd/MMM/yyyy h:mm tt"), true); }
			if (currentCalc.currentOutputs.becomesExtreme > new TimeSpan(0, 0, 0))
				{ MakeRun(text, "Risk becomes Extreme: " + (currentCalc.currentOutputs.calcDone + currentCalc.currentOutputs.becomesExtreme).ToString("dd/MMM/yyyy h:mm tt"), true); }
			text.Margin = new Thickness(0, 50, 0, 50);
			doc.Blocks.Add(text);
		}
		
		public bool printResultPage(calculation currentCalc)
		{
			var doc = new FlowDocument();
			doc.FontFamily = new FontFamily("#Sans Serif");
			var diag = new PrintDialog();
			bool? result = diag.ShowDialog();
			if (result == true)
			{
				// printable area width doesn't include the unprintable gutter around the edge... you'd think it would huh?
				double pageWidth = diag.PrintableAreaWidth;
				double pageHeight = diag.PrintableAreaHeight;
				
				doc.PageWidth = pageWidth;
				doc.PageHeight = pageHeight;
				doc.ColumnWidth = pageWidth;

				var sz = new Size(pageWidth, pageHeight/4);

				doc.Blocks.Add(MakeLogoHeader(sz));
				doc.Blocks.Add(MakeNiceText(sz, currentCalc));
				var outline = new Section {Margin = new Thickness(0), Padding = new Thickness(0)};
				outline.Blocks.Add(MakeResults(new Size(sz.Width-20,sz.Height), currentCalc));
				outline.Blocks.Add(MakeGraph(new Size(sz.Width*0.9,sz.Height), currentCalc));
				outline.BorderBrush = Brushes.Black;
				outline.BorderThickness = new Thickness(2);
				doc.Blocks.Add(outline);
	
				diag.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Fatigue Calculation Result");
			}
			return true;
		}

		private Block MakeGraph(Size space, calculation currentCalc){
			//print the graph below the text
			var cnv = new Canvas {Width = space.Width, Height = space.Height};
			var printgraph = new graph {JustDrawShift = false, MaxHours = 24};
			var theme = new graphTheme {LineColour = Brushes.Black, TextColour = Brushes.Black, TickColour = Brushes.Black};
			printgraph.drawGraph(cnv, currentCalc, theme);

			var graphpara = new Paragraph {Padding = new Thickness(0), Margin = new Thickness(0)};
			graphpara.Inlines.Add(cnv);
			return graphpara;
		}


		private Block MakeNiceText(Size space, calculation currentCalc)
		{
			var text= new Section();
			var inputs = new Paragraph{Margin=new Thickness(0,10,0,10), Padding = new Thickness(0)};
			MakeRun(inputs, "Date & Time: " + currentCalc.currentOutputs.calcDone.ToString("dd/MMM/yyyy hh:mm"), true);
			MakeRun(inputs, "Operator: " + currentCalc.currentInputs.identity.ToSmallString(), true);
			text.Blocks.Add(inputs);

			var outputs = new Paragraph{Margin=new Thickness(0,20,0,0),Padding = new Thickness(0)};
			addSubTitle(outputs, "Calculation Details");
			MakeRun(outputs, "Shift Start: " + currentCalc.currentInputs.shiftStart.ToString("dd/MMM/yyyy hh:mm"), true);
			MakeRun(outputs, "Shift End: " + currentCalc.currentInputs.shiftEnd.ToString("dd/MMM/yyyy hh:mm"), true);
			MakeRun(outputs, "Sleep in 24 hours: " + currentCalc.currentInputs.sleep24.ToString(), true);
			MakeRun(outputs, "Sleep in 48 hours: " + currentCalc.currentInputs.sleep48.ToString(), true);
			MakeRun(outputs, "Hours awake: " + currentCalc.currentInputs.hoursAwake.ToString(), true);
			if (!currentCalc.logged) MakeRun(outputs, "This calculation was not logged", true);
			text.Blocks.Add(outputs);

			var results = new Paragraph { Margin = new Thickness(0), Padding = new Thickness(0) };
			addSubTitle(results, "Calculation Results");
			text.Blocks.Add(results);
		
			text.Margin = new Thickness(0);
			return text;
		}
		
		private Block MakeLogoHeader(Size space){
			var photoFile = new BitmapImage();
			photoFile.BeginInit();
			photoFile.UriSource = new Uri("pack://application:,,,/Fatigue Calculator Desktop;component/Images/newcrest.png");
			photoFile.EndInit();

			var image = new Image{Source = photoFile};
			var lh = new InlineUIContainer(image);
			var ret = new Paragraph(lh);
			return ret;
		}

		private static Block MakeResults(Size space, calculation currentCalc){
			// build a table for the results
			var tb = new Table();
			var tc1 = new TableColumn{Width = new GridLength(space.Width-80)};
			tb.Columns.Add(tc1);
			var tc2 = new TableColumn { Width = new GridLength(50) };
			tb.Columns.Add(tc2);
			
			var trg = new TableRowGroup();
			tb.RowGroups.Add(trg);

			var ff = new FontFamily(new Uri("pack://application:,,,/Fatigue Calculator Desktop;component/Fonts/PFHandbookPro-Thin.otf"),"#PFHPThin");
			//current fatigue level
			var text = new Run("Your Fatigue Score is " + currentCalc.currentOutputs.currentScore)
			{
				Foreground = currentCalc.getColourForLevel(currentCalc.currentOutputs.currentLevel),
				FontFamily = ff
			};
			var icon = calculation.GetIconNameForLevel(currentCalc.currentOutputs.currentLevel);
			
			var tr = MakeRow(text, icon);
			trg.Rows.Add(tr);

			//check for moderate fatigue
			if (currentCalc.currentOutputs.becomesModerate > new TimeSpan(0, 0, 0))
			{
				text = new Run("You will be at Moderate risk of Fatigue at " + (currentCalc.currentOutputs.calcDone + currentCalc.currentOutputs.becomesModerate).ToString("ddd dd MMM HH:mm"));
				text.Foreground = currentCalc.getColourForLevel(calculation.fatigueLevels.Moderate);
				text.FontFamily = ff;
				trg.Rows.Add(MakeRow(text, calculation.GetIconNameForLevel(calculation.fatigueLevels.Moderate)));
			}
			//high fatigue
			if (currentCalc.currentOutputs.becomesHigh > new TimeSpan(0, 0, 0))
			{
				text = new Run("You will be at High risk of Fatigue at " + (currentCalc.currentOutputs.calcDone + currentCalc.currentOutputs.becomesHigh).ToString("ddd dd MMM HH:mm"));
				text.Foreground = currentCalc.getColourForLevel(calculation.fatigueLevels.High);
				text.FontFamily = ff;
				trg.Rows.Add(MakeRow(text, calculation.GetIconNameForLevel(calculation.fatigueLevels.High)));
			}
			//extreme fatigue
			if (currentCalc.currentOutputs.becomesExtreme > new TimeSpan(0, 0, 0))
			{
				text = new Run("You will be at Extreme risk of Fatigue at " + (currentCalc.currentOutputs.calcDone + currentCalc.currentOutputs.becomesExtreme).ToString("ddd dd MMM HH:mm"));
				text.Foreground = currentCalc.getColourForLevel(calculation.fatigueLevels.Extreme);
				text.FontFamily = ff;
				trg.Rows.Add(MakeRow(text, calculation.GetIconNameForLevel(calculation.fatigueLevels.Extreme)));
			}
			return tb;
		}

		private static TableRow MakeRow(Inline text, String imagename)
		{
			var tr = new TableRow();
			var para = new Paragraph(text);
			para.LineHeight = 50; // hard-coded value to fit the image size

			tr.Cells.Add(new TableCell(para));
			var image = new BitmapImage();
			image.BeginInit();
			image.UriSource = new Uri("pack://application:,,,/Fatigue Calculator Desktop;component/Images/"+imagename);
			image.EndInit();
			tr.Cells.Add(new TableCell(new BlockUIContainer(new Image{Source = image})));
			return tr;
		}
		private static Run MakeRun(Paragraph text, string toAdd, bool addBreak)
		{
			var newText = new Run(toAdd);
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