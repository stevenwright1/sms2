/*
 * Created by SharpDevelop.
 * User: GPB
 * Date: 01/07/2014
 * Time: 23:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using Caliburn.Micro;
using Microsoft.Win32;

namespace AuthGateway.Tester
{
	public class ResultsPageModel : PropertyChangedBase
	{
		public ResultsPageModel(List<string> lines) {
			if (lines == null) 
				lines = new List<string>();
			this.LogLines = lines;
		}
		
		public List<string> LogLines { get; set; }
		
		private int _polled;
		public int polled { 
			private get { return _polled; }
			set {
				_polled = value; NotifyOfPropertyChange(() => UsersPolledFromAd);
			} 
		}

		private int _polledProcessed;
		public int polledProcessed
		{
			private get { return _polledProcessed; }
			set
			{
				_polledProcessed = value; NotifyOfPropertyChange(() => UsersPolledFromAdProcessed);
			}
		}
		
		private int _polledActive;
		public int polledActive { 
			private get { return _polledActive; }
			set {
				_polledActive = value; NotifyOfPropertyChange(() => UsersPolledFromAdActive);
			} 
		}

		private int _polledInactive;
		public int polledInactive
		{
			private get { return _polledInactive; }
			set
			{
				_polledInactive = value; NotifyOfPropertyChange(() => UsersPolledFromAdInactive);
			}
		}

		private int _polledSkipped;
		public int polledSkipped
		{
			private get { return _polledSkipped; }
			set
			{
				_polledSkipped = value; NotifyOfPropertyChange(() => UsersPolledFromAdSkipped);
			}
		}

		private int _polledInDb;
		public int polledInDb { 
			private get { return _polledInDb; }
			set {
				_polledInDb = value; NotifyOfPropertyChange(() => UsersPolledFromAdAlreadyInDatabase);
			} 
		}
		
		private int _dbUsers;
		public int dbUsers { 
			private get { return _dbUsers; }
			set {
				_dbUsers = value; NotifyOfPropertyChange(() => UsersInDatabase);
			} 
		}
		
		private int _dbUsersActive;
		public int dbUsersActive { 
			private get { return _dbUsersActive; }
			set {
				_dbUsersActive = value; NotifyOfPropertyChange(() => UsersInDatabaseActive);
			} 
		}

		public string UsersPolledFromAd { get { return string.Format("AD total users: {0}", polled); } }
		public string UsersPolledFromAdSkipped { get { return string.Format("AD failed users: {0}", polledSkipped); } }
		public string UsersPolledFromAdProcessed { get { return string.Format("AD processed users: {0}", polledProcessed); } }
		public string UsersPolledFromAdActive { get { return string.Format("AD active users: {0}", polledActive); } }
		public string UsersPolledFromAdInactive { get { return string.Format("AD inactive accounts: {0}", polledInactive); } }
		public string UsersPolledFromAdAlreadyInDatabase  { get { return string.Format("AD users already in database: {0}", polledInDb); } } //Users polled from AD existing in the database
		public string UsersInDatabase  { get { return string.Format("Total users in database: {0}", dbUsers); } }
		public string UsersInDatabaseActive  { get { return string.Format("Total active users in database: {0}", dbUsersActive); } }
	}
	
	/// <summary>
	/// Interaction logic for ResultsPage.xaml
	/// </summary>
	public partial class ResultsPage : Page
	{
		private ResultsPageModel vmodel;
		
		public ResultsPage() : this(new ResultsPageModel(null))
		{
			
		}
		
		public ResultsPage(ResultsPageModel vmodel)
		{
			InitializeComponent();
			
			this.vmodel = vmodel;
			this.DataContext = vmodel;
			this.Loaded += new RoutedEventHandler(ResultsPage_Loaded);
		}

		void ResultsPage_Loaded(object sender, RoutedEventArgs e)
		{
			this.NavigationService.RemoveBackEntry();
		}
		
		private void Save_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new SaveFileDialog();
			dlg.DefaultExt = ".txt";
			dlg.Filter = "Text documents (.txt)|*.txt";
			
			var dlgResult = dlg.ShowDialog();
			if (dlgResult == null || dlgResult == false)
				return;
			
			try
			{
				using (var textWriter = new StreamWriter(dlg.OpenFile())) {
					textWriter.WriteLine(string.Format("Log date: {0}", DateTime.Now.ToString("yyyy-MM-dd HH':'mm':'ss")));
					textWriter.WriteLine();
					textWriter.WriteLine(this.vmodel.UsersPolledFromAd);
					textWriter.WriteLine(this.vmodel.UsersPolledFromAdSkipped);
					textWriter.WriteLine(this.vmodel.UsersPolledFromAdProcessed);
					textWriter.WriteLine(this.vmodel.UsersPolledFromAdActive);
					textWriter.WriteLine(this.vmodel.UsersPolledFromAdInactive);
					textWriter.WriteLine(this.vmodel.UsersPolledFromAdAlreadyInDatabase);
					textWriter.WriteLine();
					textWriter.WriteLine(this.vmodel.UsersInDatabase);
					textWriter.WriteLine(this.vmodel.UsersInDatabaseActive);
					textWriter.WriteLine();
					foreach(var msg in this.vmodel.LogLines)
						textWriter.WriteLine(msg);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error: Could not read export log to disk. Original error: " + ex.Message,
				                "AuthGateway.Tester", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}