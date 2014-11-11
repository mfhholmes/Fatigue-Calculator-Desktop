using System;
using System.Windows;
using System.Windows.Controls;
using Fatigue_Calculator_Desktop.Config;

namespace Fatigue_Calculator_Desktop
{
	/// <summary>
	/// Interaction logic for SettingsPage.xaml
	/// </summary>
	public partial class adminSettingsPage : Page
	{
		//ICalculatorSettings _settings;

		public adminSettingsPage()
		{
			InitializeComponent();
			LoadSettings();
		}

		/// <summary>
		/// Loads the settings from a settings file to the screen
		/// </summary>
		public void LoadSettings()
		{
			// grab some of the common controls first
			Grid grdSettings = (Grid)this.FindName("grdSettings");
			grdSettings.Children.Clear();
			grdSettings.RowDefinitions.RemoveRange(0, grdSettings.RowDefinitions.Count);
			RowDefinition rowSetting;
			TextBlock nameSetting;
			TextBlock valueSetting;
			Button changeSetting;
			int i = 0;
			foreach (System.Collections.Generic.KeyValuePair<string, configItem> setting in ConfigSettings.settings.itemList())
			{
				// add a row
				if (i == grdSettings.RowDefinitions.Count)
				{
					rowSetting = new RowDefinition();
					rowSetting.MaxHeight = 50;
					grdSettings.RowDefinitions.Add(rowSetting);
				}
				//name
				nameSetting = new TextBlock();
				Grid.SetRow(nameSetting, grdSettings.RowDefinitions.Count - 1);
				Grid.SetColumn(nameSetting, 0);
				nameSetting.Style = (Style)grdSettings.FindResource("styleFCSettingBlue");
				nameSetting.Name = "Setting" + i.ToString() + "Name";
				nameSetting.Text = setting.Value.name;
				grdSettings.Children.Add(nameSetting);
				//value
				valueSetting = new TextBlock();
				Grid.SetRow(valueSetting, grdSettings.RowDefinitions.Count - 1);
				Grid.SetColumn(valueSetting, 1);
				valueSetting.Style = (Style)grdSettings.FindResource("styleFCSettingBlue");
				valueSetting.Name = "Setting" + i.ToString() + "Value";
				valueSetting.Text = setting.Value.strValue;
				grdSettings.Children.Add(valueSetting);
				// change
				changeSetting = new Button();
				Grid.SetRow(changeSetting, grdSettings.RowDefinitions.Count - 1);
				Grid.SetColumn(changeSetting, 2);
				changeSetting.Template = (ControlTemplate)grdSettings.FindResource("controlFCButton");
				changeSetting.Name = "Setting" + i.ToString() + "Change";
				changeSetting.Content = "Change";
				changeSetting.Click += new RoutedEventHandler(btnChangeClick);
				changeSetting.Tag = setting.Value.key;
				grdSettings.Children.Add(changeSetting);
				// increment the setting counter
				i++;
			}
			// set the visibility
			bdrSettings.Visibility = System.Windows.Visibility.Visible;
			bdrChanges.Visibility = System.Windows.Visibility.Hidden;
		}

		private void btnChangeClick(object sender, RoutedEventArgs e)
		{
			// find the grid row the button is on
			Button source = (Button)sender;
			int row = Grid.GetRow(source);
			// set the key name and value
			string keyName = "Setting" + row.ToString() + "Name";
			string settingKey = (string)((Button)sender).Tag;
			configItem setting = ConfigSettings.settings.item(settingKey);
			LabelKey.Text = setting.key;
			txtValue.Text = setting.strValue;
			LabelDescription.Text = setting.description;
			// Reset the error text
			LabelError.Text = "";
			// set the grid visibilities
			bdrSettings.Visibility = System.Windows.Visibility.Hidden;
			bdrChanges.Visibility = System.Windows.Visibility.Visible;
		}

		private void btnOK_Click(object sender, RoutedEventArgs e)
		{
			//Validate the new value
			try
			{
				ConfigSettings.settings.item(LabelKey.Text).strValue = txtValue.Text;
			}
			catch (Exception err)
			{
				LabelError.Text = err.Message;
				return;
			}
			// we're good so write the changes to the file
			if (!ConfigSettings.saveSettings())
			{
				LabelError.Text = "Settings didn't save correctly";
				return;
			}
			if (ConfigSettings.settings.item(LabelKey.Text).strValue == txtValue.Text)
			{
				LoadSettings();
				//and reset the visibilities
				bdrSettings.Visibility = System.Windows.Visibility.Visible;
				bdrChanges.Visibility = System.Windows.Visibility.Hidden;
			}
			else
			{
				LabelError.Text = ConfigSettings.settings.item(LabelKey.Text).lastValidationError;
			}
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			//ignore everything and just switch the visibilities back
			bdrSettings.Visibility = System.Windows.Visibility.Visible;
			bdrChanges.Visibility = System.Windows.Visibility.Hidden;
		}

		private void btnBack_Click(object sender, RoutedEventArgs e)
		{
			this.NavigationService.GoBack();
		}
	}
}