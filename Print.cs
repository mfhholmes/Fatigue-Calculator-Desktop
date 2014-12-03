using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Fatigue_Calculator_Desktop
{
	internal class Print
	{
		private calculation _calc;
		private const string dateTimeFormat = "dddd dd/MMM/yyyy hh:mm";

		public Print(calculation thiscalc)
		{
			this._calc = thiscalc;
		}
		public bool PrintSimpleResults()
		{
			var doc = new FlowDocument();
			var diag = new PrintDialog();
			var result = diag.ShowDialog();
			if (result != true) return false;
			doc.PageWidth = diag.PrintableAreaWidth;
			doc.PageHeight = diag.PrintableAreaHeight;
			doc.Blocks.Add(MakeSimpleText());
			diag.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Fatigue Calculation Result");
			return true;
		}

		//Standard simple text for the non-pretty output required for some devices
		private Block MakeSimpleText()
		{
			var text = new Paragraph();
			AddRun(text, "Fatigue Calculation", true);
			AddRun(text, "", true);
			AddRun(text, "Date: " + this._calc.currentOutputs.calcDone.ToString("dd/MMM/yyyy"), true);
			AddRun(text, "Time: " + this._calc.currentOutputs.calcDone.ToString("h:mm tt"), true);
			AddRun(text, "Operator: " + this._calc.currentInputs.identity.ToString(), true);
			AddRun(text, "", true);
			AddRun(text, "Calculation Details", true);
			AddRun(text, "Shift Start: " + this._calc.currentInputs.shiftStart.ToString("dd/MMM/yyyy h:mm tt"), true);
			AddRun(text, "Shift End: " + this._calc.currentInputs.shiftEnd.ToString("dd/MMM/yyyy h:mm tt"), true);
			AddRun(text, "Sleep in 24 hours: " + this._calc.currentInputs.sleep24.ToString(), true);
			AddRun(text, "Sleep in 48 hours: " + this._calc.currentInputs.sleep48.ToString(), true);
			AddRun(text, "Hours awake: " + this._calc.currentInputs.hoursAwake.ToString(), true);
			if (!this._calc.logged) AddRun(text, "This calculation was not logged", true);
			AddRun(text, "", true);
			AddRun(text, "Calculation Results", true);
			AddRun(text, "Current Score:" + this._calc.currentOutputs.currentScore.ToString(), true);
			AddRun(text, "Current Risk Level:" + this._calc.currentOutputs.currentLevel.ToString(), true);
			if (this._calc.currentOutputs.becomesModerate > new TimeSpan(0, 0, 0))
			{ AddRun(text, "Risk becomes Moderate: " + (this._calc.currentOutputs.calcDone + this._calc.currentOutputs.becomesModerate).ToString("dd/MMM/yyyy h:mm tt"), true); }
			if (this._calc.currentOutputs.becomesHigh > new TimeSpan(0, 0, 0))
			{ AddRun(text, "Risk becomes High: " + (this._calc.currentOutputs.calcDone + this._calc.currentOutputs.becomesHigh).ToString("dd/MMM/yyyy h:mm tt"), true); }
			if (this._calc.currentOutputs.becomesExtreme > new TimeSpan(0, 0, 0))
			{ AddRun(text, "Risk becomes Extreme: " + (this._calc.currentOutputs.calcDone + this._calc.currentOutputs.becomesExtreme).ToString("dd/MMM/yyyy h:mm tt"), true); }
			text.Margin = new Thickness(0, 50, 0, 50);
			return text;
		}

		public bool PrintResultPage()
		{
			var doc = new FlowDocument { FontFamily = new FontFamily("#Sans Serif") };
			var diag = new PrintDialog();
			var result = diag.ShowDialog();
			if (result != true) return false;
			var pageWidth = diag.PrintableAreaWidth;
			var pageHeight = diag.PrintableAreaHeight;
			doc.PageWidth = pageWidth;
			doc.PageHeight = pageHeight;
			doc.ColumnWidth = pageWidth;
			//size of each section is the pagewidth and 1/4 of the height max
			var sz = new Size(pageWidth, pageHeight / 4);

			//TODO: add this into settings! - logo header image or title
			//doc.Blocks.Add(MakeLogoHeader(sz));
			doc.Blocks.Add(MakeTitle("Fatigue Assessment"));

			doc.Blocks.Add(MakeNiceText(sz));
			var outline = new Section { Margin = new Thickness(0), Padding = new Thickness(0), BorderBrush = Brushes.Black, BorderThickness = new Thickness(2) };
			outline.Blocks.Add(MakeResults(new Size(sz.Width - 20, sz.Height)));
			outline.Blocks.Add(MakeGraph(new Size(sz.Width * 0.9, sz.Height)));

			doc.Blocks.Add(outline);

			diag.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Fatigue Calculation Result");
			return true;
		}

		private Block MakeGraph(Size space)
		{
			//print the graph below the text
			var cnv = new Canvas { Width = space.Width, Height = space.Height };
			var printgraph = new graph { JustDrawShift = false, MaxHours = 24 };
			var theme = new graphTheme { LineColour = Brushes.Black, TextColour = Brushes.Black, TickColour = Brushes.Black };
			printgraph.drawGraph(cnv, this._calc, theme);

			var graphpara = new Paragraph { Padding = new Thickness(0), Margin = new Thickness(0) };
			graphpara.Inlines.Add(cnv);
			return graphpara;
		}

		private Block MakeNiceText(Size space)
		{
			var text = new Section();
			var inputs = new Paragraph { Margin = new Thickness(0, 10, 0, 10), Padding = new Thickness(0) };
			AddRun(inputs, "Date & Time: " + this._calc.currentOutputs.calcDone.ToString(dateTimeFormat), true);
			AddRun(inputs, "Operator: " + this._calc.currentInputs.identity.ToSmallString(), true);
			text.Blocks.Add(inputs);

			var outputs = new Paragraph { Margin = new Thickness(0, 20, 0, 0), Padding = new Thickness(0) };
			AddSubTitle(outputs, "Calculation Details");
			AddRun(outputs, "Shift Start: " + this._calc.currentInputs.shiftStart.ToString(dateTimeFormat), true);
			AddRun(outputs, "Shift End: " + this._calc.currentInputs.shiftEnd.ToString(dateTimeFormat), true);
			AddRun(outputs, "Sleep in 24 hours: " + this._calc.currentInputs.sleep24.ToString(), true);
			AddRun(outputs, "Sleep in 48 hours: " + this._calc.currentInputs.sleep48.ToString(), true);
			AddRun(outputs, "Hours awake: " + this._calc.currentInputs.hoursAwake.ToString(), true);
			if (!this._calc.logged) AddRun(outputs, "This calculation was not logged", true);
			text.Blocks.Add(outputs);

			var results = new Paragraph { Margin = new Thickness(0), Padding = new Thickness(0) };
			AddSubTitle(results, "Calculation Results");
			text.Blocks.Add(results);

			text.Margin = new Thickness(0);
			return text;
		}

		private static Block MakeLogoHeader(Size space)
		{
			var photoFile = new BitmapImage();
			photoFile.BeginInit();
			photoFile.UriSource = new Uri("pack://application:,,,/Fatigue Calculator Desktop;component/Images/header.png");
			photoFile.EndInit();

			var image = new Image { Source = photoFile };
			var lh = new InlineUIContainer(image);
			var ret = new Paragraph(lh);
			return ret;
		}

		private Block MakeResults(Size space)
		{
			// build a table for the results
			var tb = new Table();
			var tc1 = new TableColumn { Width = new GridLength(space.Width - 80) };
			tb.Columns.Add(tc1);
			var tc2 = new TableColumn { Width = new GridLength(50) };
			tb.Columns.Add(tc2);

			var trg = new TableRowGroup();
			tb.RowGroups.Add(trg);

			var ff = new FontFamily(new Uri("pack://application:,,,/Fatigue Calculator Desktop;component/Fonts/PFHandbookPro-Thin.otf"), "#PFHPThin");
			//current fatigue level
			var text = new Run("Your Fatigue Score is " + this._calc.currentOutputs.currentScore)
			{
				Foreground = this._calc.getColourForLevel(this._calc.currentOutputs.currentLevel),
				FontFamily = ff
			};
			var icon = calculation.GetIconNameForLevel(this._calc.currentOutputs.currentLevel);

			var tr = MakeRow(text, icon);
			trg.Rows.Add(tr);

			//check for moderate fatigue
			if (this._calc.currentOutputs.becomesModerate > new TimeSpan(0, 0, 0))
			{
				text = new Run("You will be at Moderate risk of Fatigue at " + (this._calc.currentOutputs.calcDone + this._calc.currentOutputs.becomesModerate).ToString(dateTimeFormat))
				{
					Foreground = this._calc.getColourForLevel(calculation.fatigueLevels.Moderate),
					FontFamily = ff
				};
				trg.Rows.Add(MakeRow(text, calculation.GetIconNameForLevel(calculation.fatigueLevels.Moderate)));
			}
			//high fatigue
			if (this._calc.currentOutputs.becomesHigh > new TimeSpan(0, 0, 0))
			{
				text = new Run("You will be at High risk of Fatigue at " + (this._calc.currentOutputs.calcDone + this._calc.currentOutputs.becomesHigh).ToString(dateTimeFormat))
				{
					Foreground = this._calc.getColourForLevel(calculation.fatigueLevels.High),
					FontFamily = ff
				};
				trg.Rows.Add(MakeRow(text, calculation.GetIconNameForLevel(calculation.fatigueLevels.High)));
			}
			//extreme fatigue
			if (this._calc.currentOutputs.becomesExtreme > new TimeSpan(0, 0, 0))
			{
				text = new Run("You will be at Extreme risk of Fatigue at " + (this._calc.currentOutputs.calcDone + this._calc.currentOutputs.becomesExtreme).ToString(dateTimeFormat))
				{
					Foreground = this._calc.getColourForLevel(calculation.fatigueLevels.Extreme),
					FontFamily = ff
				};
				trg.Rows.Add(MakeRow(text, calculation.GetIconNameForLevel(calculation.fatigueLevels.Extreme)));
			}
			return tb;
		}

		private static TableRow MakeRow(Inline text, String imagename)
		{
			var tr = new TableRow();
			var para = new Paragraph(text) { LineHeight = 50 };

			tr.Cells.Add(new TableCell(para));
			var image = new BitmapImage();
			image.BeginInit();
			image.UriSource = new Uri("pack://application:,,,/Fatigue Calculator Desktop;component/Images/" + imagename);
			image.EndInit();
			tr.Cells.Add(new TableCell(new BlockUIContainer(new Image { Source = image })));
			return tr;
		}

		private static void AddRun(Paragraph text, string toAdd, bool addBreak)
		{
			var newText = new Run(toAdd);
			text.Inlines.Add(newText);
			if (addBreak) text.Inlines.Add(new LineBreak());
		}

		private static Paragraph MakeTitle(string text)
		{
			var title = new Paragraph { TextAlignment = TextAlignment.Center };
			var txt = new Run(text);
			txt.FontSize *= 2.5;
			txt.FontWeight = FontWeights.Bold;
			txt.TextDecorations = TextDecorations.Underline;
			title.Inlines.Add(txt);
			title.Inlines.Add(new LineBreak());
			return title;
		}

		private static void AddSubTitle(Paragraph text, string toAdd)
		{
			var txt = new Run(toAdd);
			txt.FontSize *= 1.25;
			txt.FontWeight = FontWeights.Bold;
			txt.TextDecorations = TextDecorations.Underline;
			text.Inlines.Add(txt);
			text.Inlines.Add(new LineBreak());
		}
	}
}