using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Fatigue_Calculator_Desktop
{
	/// <summary>
	/// Interaction logic for IDListPage.xaml
	/// </summary>
	public partial class adminIDListPage : Page
	{
		private identityFile _idList = new identityFile();

		//state machine settings for the page
		private enum currentAction
		{
			action_list,
			action_new,
			action_edit,
		}

		private currentAction _currentAction = currentAction.action_list;

		private identity _currentIdent;

		public adminIDListPage()
		{
			InitializeComponent();
			setState(currentAction.action_list);
		}

		/// <summary>
		/// sets the current state of the page. which can be List, New or Edit
		/// </summary>
		/// <param name="newAction">the new action that is desired</param>
		/// <returns>true if the action was successfully changed</returns>
		private bool setState(currentAction newAction)
		{
			switch (newAction)
			{
				case currentAction.action_list:
					{
						_currentAction = currentAction.action_list;
						loadList();
						showList();
						break;
					}
				case currentAction.action_edit:
					{
						// check we've got an item to edit
						if (_currentIdent == null)
							return false;
						_currentAction = currentAction.action_edit;
						showEdit();
						break;
					}
				case currentAction.action_new:
					{
						_currentAction = currentAction.action_new;
						showNew();
						break;
					}
				default:
					{
						break;
					}
			}
			return true;
		}

		/// <summary>
		/// displays the edit screen, loading the identity data from the _currentIdentity
		/// </summary>
		private void showEdit()
		{
			bdrIdentity.Visibility = System.Windows.Visibility.Visible;
			bdrList.Visibility = System.Windows.Visibility.Hidden;
			txtIdentity.Text = _currentIdent.Id;
			txtName.Text = _currentIdent.Name;
			ErrorMessage.Visibility = System.Windows.Visibility.Hidden;
		}

		/// <summary>
		/// displays the New Identity screen
		/// </summary>
		private void showNew()
		{
			bdrIdentity.Visibility = System.Windows.Visibility.Visible;
			bdrList.Visibility = System.Windows.Visibility.Hidden;
			txtIdentity.Text = "";
			txtName.Text = "";
			ErrorMessage.Visibility = System.Windows.Visibility.Hidden;
		}

		/// <summary>
		/// displays the list screen
		/// </summary>
		private void showList()
		{
			bdrIdentity.Visibility = System.Windows.Visibility.Hidden;
			bdrList.Visibility = System.Windows.Visibility.Visible;
			ErrorMessage.Visibility = System.Windows.Visibility.Hidden;
		}

		/// <summary>
		/// loads the list with the data in the file
		/// </summary>
		private void loadList()
		{
			// get the filename from the settings
			string filename = Config.ConfigSettings.settings.IDLookupFile;
			// set the list up
			_idList.SetIdentityListSource(filename);
			// load the identities into the interface
			grdContent.Children.Clear();
			// cache the id list
			List<identity> idCache = _idList.IdentityList(noduplicates: true);
			foreach (identity ident in idCache)
			{
				listIdentity(ident);
			}
			// check we've got some identities
			if (idCache.Count == 0)
			{
				// valid but empty id file
				TextBlock name = new TextBlock();
				name.VerticalAlignment = System.Windows.VerticalAlignment.Center;
				name.Name = "name0";
				name.Text = "No identities in this file yet";
				name.Style = styleSample.Style;
				Grid.SetRow(name, 0);
				Grid.SetColumn(name, 1);
				Grid.SetColumnSpan(name, 3);
				grdContent.Children.Add(name);
			}
		}

		private void listIdentity(identity ident)
		{
			// new row to hold the new stuff
			grdContent.RowDefinitions.Add(new RowDefinition());
			int rownum = (grdContent.RowDefinitions.Count - 1);

			// create new checkbox, text blocks for the data and buttons for the actions
			// checkbox
			CheckBox chk = new CheckBox();
			chk.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
			chk.VerticalAlignment = System.Windows.VerticalAlignment.Center;
			chk.Name = "chk" + rownum.ToString();
			Grid.SetColumn(chk, 0);
			Grid.SetRow(chk, rownum);
			grdContent.Children.Add(chk);

			// name
			TextBlock name = new TextBlock();
			name.VerticalAlignment = System.Windows.VerticalAlignment.Center;
			name.Name = "name" + rownum.ToString();
			name.Text = ident.Name;
			name.Style = styleSample.Style;
			Grid.SetRow(name, rownum);
			Grid.SetColumn(name, 1);
			grdContent.Children.Add(name);

			// id
			TextBlock id = new TextBlock();
			id.VerticalAlignment = System.Windows.VerticalAlignment.Center;
			id.Name = "id" + rownum.ToString();
			id.Text = ident.Id;
			id.Style = styleSample.Style;
			Grid.SetRow(id, rownum);
			Grid.SetColumn(id, 3);
			grdContent.Children.Add(id);

			//delete
			Button del = new Button();
			del.Template = buttonSample.Template;
			del.Height = 32;
			del.Content = "Delete";
			del.Name = "btnDelete" + rownum.ToString();
			del.Click += new RoutedEventHandler(del_Click);
			Grid.SetRow(del, rownum);
			Grid.SetColumn(del, 4);
			grdContent.Children.Add(del);

			//edit
			Button edit = new Button();
			edit.Template = buttonSample.Template;
			edit.Height = 32;
			edit.Content = "Edit";
			edit.Name = "btnEdit" + rownum.ToString();
			edit.Click += new RoutedEventHandler(edit_Click);
			edit.Tag = ident;
			Grid.SetRow(edit, rownum);
			Grid.SetColumn(edit, 5);
			grdContent.Children.Add(edit);
		}

		private void btnBack_Click(object sender, RoutedEventArgs e)
		{
			this.NavigationService.GoBack();
		}

		private void btnAdd_Click(object sender, RoutedEventArgs e)
		{
			// show the edit sub-page
			setState(currentAction.action_new);
		}

		private void edit_Click(object sender, RoutedEventArgs e)
		{
			// throw the identity to the edit sub-page
			// grab the identity from the sender's tag
			Button _sender = (Button)sender;
			_currentIdent = (identity)_sender.Tag;
			setState(currentAction.action_edit);
		}

		private void del_Click(object sender, RoutedEventArgs e)
		{
			// confirm and delete
			// get the identity details from the edit button with the same name
			Button _del = (Button)sender;
			string name = "btnEdit" + _del.Name.Substring("btnDelete".Length);
			Button _edit = (Button)this.FindName(name);
			if (_edit == null)
			{
				foreach (Button possible in this.grdContent.Children.OfType<Button>())
				{
					if (possible.Name == name)
					{
						_edit = possible;
						break;
					}
				}
			}
			identity toDelete = (identity)_edit.Tag;
			_idList.deleteIdentity(toDelete);
			setState(currentAction.action_list);
		}

		private void btnOK_Click(object sender, RoutedEventArgs e)
		{
			// check we've got a name and do the appropriate thing
			switch (_currentAction)
			{
				case currentAction.action_edit:
					{
						identity newIdent = new identity(txtName.Text, txtIdentity.Text, identity.researchStates.research_unasked);
						_idList.ChangeIdentity(_currentIdent, newIdent);
						_currentIdent = newIdent;
						setState(currentAction.action_list);
						break;
					}
				case currentAction.action_new:
					{
						identity newIdent = new identity(txtName.Text, txtIdentity.Text, identity.researchStates.research_unasked);
						if (!_idList.AddNewIdentity(newIdent))
						{
							// didn't add to the file, let's tell the user
							ErrorMessage.Visibility = System.Windows.Visibility.Visible;
							ErrorMessage.Text = "A problem occurred adding the file. Please try again later or contact support";
						}
						_currentIdent = newIdent;
						// get it to reload the list
						_idList = new identityFile();
						setState(currentAction.action_list);
						break;
					}
				case currentAction.action_list:
					{
						//invalid state...weird
						setState(_currentAction);
						break;
					}
				default:
					{
						// reserved for future expansion...hah
						setState(currentAction.action_list);
						break;
					}
			}
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			// cancel whatever the action is
			setState(currentAction.action_list);
			ErrorMessage.Visibility = System.Windows.Visibility.Hidden;
		}
	}
}