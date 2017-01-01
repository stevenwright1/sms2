/*
 * Created by SharpDevelop.
 * User: GPB
 * Date: 02/07/2014
 * Time: 22:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

using AuthGateway.Shared;
using Caliburn.Micro;

namespace AuthGateway.Tester
{
	public class SelectConfigModel : PropertyChangedBase
	{
		private string fname;
		public string Filename { 
			get { return fname; } 
			set { 
				fname = value; 
				NotifyOfPropertyChange(() => Filename); 
			}
		}
	}
	/// <summary>
	/// Interaction logic for SelectConfig.xaml
	/// </summary>
	public partial class SelectConfig : Page
	{
		
		private SelectConfigModel vmodel;
		
		public SelectConfig(SelectConfigModel vmodel)
		{
			InitializeComponent();
			
			this.vmodel = vmodel;
			this.DataContext = vmodel;
		}
		
		private void Open_Click(object sender, RoutedEventArgs e)
		{
			try {
				var sc = new SystemConfiguration();
				if (!string.IsNullOrWhiteSpace(vmodel.Filename)) {
					sc.LoadSettingsFromFile(vmodel.Filename, false);
				} else
					vmodel.Filename = "";

				var nextPage = new ConfigPage(new ConfigPageModel(sc));
				this.NavigationService.Navigate(nextPage);
			} catch(Exception ex) {
				MessageBox.Show(string.Format("Error: {0}", ex.Message),
				                "AuthGateway.Tester", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		
		void Browse_Click(object sender, RoutedEventArgs e)
		{
			var dlg = new Microsoft.Win32.OpenFileDialog();

			dlg.Multiselect = false;
			//openFileDialog.InitialDirectory = "c:\\";
			dlg.Filter = "XML Files (*.xml)|*.xml|All files (*.*)|*.*";
			
			var dlgResult = dlg.ShowDialog();
			
			if (dlgResult == null || dlgResult == false)
				return;
			
			try
			{
				if (!dlg.CheckFileExists)
					throw new Exception("File does not exist.");
				
				using (var checkStream = dlg.OpenFile()) {
					vmodel.Filename = dlg.FileName;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message,
				                "AuthGateway.Tester", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}