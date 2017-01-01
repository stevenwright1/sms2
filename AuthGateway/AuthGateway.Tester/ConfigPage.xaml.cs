using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using AuthGateway.Shared;
using Caliburn.Micro;

namespace AuthGateway.Tester
{
	public class ConfigPageModel : PropertyChangedBase
	{
		public SystemConfiguration sc { get; private set; }
		
		public ConfigPageModel() : this(new SystemConfiguration())
		{
			
		}
		
		public ConfigPageModel(SystemConfiguration sc)
		{
			this.sc = sc;
		}
		
		public string AdServer {
			get { return this.sc.ADServerAddress; }
			set { this.sc.ADServerAddress = value; NotifyOfPropertyChange(() => AdServer ); }
		}
		public string AdUsername {
			get { return sc.ADUsername; }
			set { sc.ADUsername = value; NotifyOfPropertyChange(() => AdUsername ); }
		}
		public string AdPassword {
			get { return sc.ADPassword; }
			set { sc.ADPassword = value; NotifyOfPropertyChange(() => AdPassword ); }
		}

		public string AdBaseDN {
			get { return sc.ADBaseDN; }
			set { sc.ADBaseDN = value; NotifyOfPropertyChange(() => AdBaseDN ); }
		}
		public string AdFilter {
			get { return sc.ADFilter; }
			set { sc.ADFilter = value; NotifyOfPropertyChange(() => AdFilter ); }
		}
		public string AdContainer {
			get { return sc.ADContainer; }
			set { sc.ADContainer = value; NotifyOfPropertyChange(() => AdContainer ); }
		}
	}
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class ConfigPage : Page
	{
		private ConfigPageModel vmodel;
        private NavigationService _navsvc;
		
		public ConfigPage(ConfigPageModel vmodel)
		{
			
			InitializeComponent();
			
			this.vmodel = vmodel;
			this.DataContext = vmodel;
			
			this.Loaded += Page1_Loaded;
            this.Unloaded += Page1_Unloaded;
        }
 
 
        void Page1_Loaded(object sender, RoutedEventArgs e)
        {
            this._navsvc = this.NavigationService;
            this._navsvc.Navigating += NavigationService_Navigating;
        }
 
        void Page1_Unloaded(object sender, RoutedEventArgs e)
        {
            this._navsvc.Navigating -= NavigationService_Navigating;
            this._navsvc = null;
        }
 
        void NavigationService_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            e.Cancel = ! this.btnRunTest.IsEnabled;
        }
		
		private void DoTest_Click(object sender, RoutedEventArgs e)
		{
			try {
				//this.btnRunTest.IsEnabled = false;
				
				var nextPage = new ProgressPage(new ProgressPageModel(this.vmodel));
				this.NavigationService.Navigate(nextPage);
			} finally {
				//this.btnRunTest.IsEnabled = true;
			}
		}
	}
}