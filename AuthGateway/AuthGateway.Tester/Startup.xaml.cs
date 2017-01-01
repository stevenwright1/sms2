/*
 * Created by SharpDevelop.
 * User: GPB
 * Date: 03/07/2014
 * Time: 05:42
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

namespace AuthGateway.Tester
{
	/// <summary>
	/// Interaction logic for Startup.xaml
	/// </summary>
	public partial class Startup : Page
	{
		public Startup()
		{
			InitializeComponent();
		}
		
		private void Btn_Click(object sender, RoutedEventArgs e)
		{
			var scm = new SelectConfigModel();
			var nextPage = new SelectConfig(scm);
			this.NavigationService.Navigate(nextPage);	
		}
	}
}