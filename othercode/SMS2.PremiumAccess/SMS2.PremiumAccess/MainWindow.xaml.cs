using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Media.Animation;
using System.IO;

namespace SMS2.PremiumAccess
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		protected System.Windows.Forms.NotifyIcon ni = null;

		private string tboxConfigOldText = string.Empty;

		public MainWindow()
		{

			InitializeComponent();
			ni = new System.Windows.Forms.NotifyIcon();

			var uri = new Uri("pack://application:,,,/" 
				+ "SMS2.PremiumAccess" 
				+";component/" 
				+ "sms.ico", UriKind.RelativeOrAbsolute);
			using (var iconStream = Application.GetResourceStream(uri).Stream)
			{
				ni.Icon = new System.Drawing.Icon(
					iconStream
				);
				ni.Visible = true;
				ni.DoubleClick +=
						delegate(object sender, EventArgs args)
						{
							if (this.WindowState != System.Windows.WindowState.Minimized)
							{
								this.Hide();
								this.WindowState = WindowState.Minimized;
							}
							else
							{
								this.Show();
								this.WindowState = WindowState.Normal;
							}
						};
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			if (ni != null)
				ni.Dispose();
			base.OnClosed(e);
		}

		/// <summary>
		/// TitleBar_MouseDown - Drag if single-click, resize if double-click
		/// </summary>
		private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				if (e.ClickCount == 2)
				{
					AdjustWindowSize();
				}
				else
				{
					if (e.LeftButton == MouseButtonState.Pressed)
						Application.Current.MainWindow.DragMove();
				}
		}

		private Client GetCurrentClient()
		{
			return cboxClientID.SelectedValue as Client;
		}

		private void RemoveButton_Click(object sender, RoutedEventArgs e)
		{
			var client = GetCurrentClient();
			if (client == null)
				return;
			if (MessageBox.Show(
				string.Format("Are you sure you want to remove '{0}'?", client.ID),
				"SMS2 - PremiumAccess",
				MessageBoxButton.YesNo,
				MessageBoxImage.Question
				) == MessageBoxResult.Yes)
			{
				var vm = (ClientViewModel)DataContext;
				vm.Remove(client);
			}
		}

		private void RefreshButton_Click(object sender, RoutedEventArgs e)
		{
			var client = GetCurrentClient();
			if (client == null) return;
			var actioner = ClientActionerFactory.Get(client);
			actioner.Refresh(client);
		}

		/// <summary>
		/// CloseButton_Clicked
		/// </summary>
		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			var vm = (ClientViewModel)DataContext;

			var clients = new SMS2.PremiumAccess.PA.ClientsList();

			var client = GetCurrentClient();
			if (client != null)
			{
				//clients.SelectedIndex = cboxClientID.SelectedIndex;
				clients.SelectedIndex = vm.ListOfClients.IndexOf(client);
			}
			else
			{
				clients.SelectedIndex = 0;
			}
			foreach (var c in vm.ListOfClients)
			{
				clients.Items.Add(c);
			}

			var pa = new PA();
			pa.SaveClients(clients);

			Application.Current.Shutdown();
		}

		/// <summary>
		/// MaximizedButton_Clicked
		/// </summary>
		private void MaximizeButton_Click(object sender, RoutedEventArgs e)
		{
			AdjustWindowSize();
		}

		/// <summary>
		/// Minimized Button_Clicked
		/// </summary>
		private void MinimizeButton_Click(object sender, RoutedEventArgs e)
		{
			this.Hide();
			this.WindowState = WindowState.Minimized;
		}

		/// <summary>
		/// Adjusts the WindowSize to correct parameters when Maximize button is clicked
		/// </summary>
		private void AdjustWindowSize()
		{
			//if (this.WindowState == WindowState.Maximized)
			//{
			//  this.WindowState = WindowState.Normal;
			//  MaxButton.Content = "1";
			//}
			//else
			//{
			//  this.WindowState = WindowState.Maximized;
			//  MaxButton.Content = "2";
			//}
		}

		private void tboxSecurityCode_TextChanged(object sender, TextChangedEventArgs e)
		{
			return;

			var client = GetCurrentClient();
			if (client == null) return;

			if (string.IsNullOrEmpty(tboxSecurityCode.Text))
			{
			  RefreshButton_Click(sender, e);
			}
		}

		private void btnEditClient_Click(object sender, RoutedEventArgs e)
		{
			this.Dispatcher.BeginInvoke((Action)(() =>
			{
				DoubleAnimation animation = new DoubleAnimation();
				animation.From = this.ActualHeight;
				double width = 400;
				animation.To = width;
				animation.Duration = TimeSpan.FromSeconds(5.0);
				this.BeginAnimation(Window.WidthProperty, animation);

			}), null);

			showEdit(e);
		}

		private void btnAddClient_Click(object sender, RoutedEventArgs e)
		{
			var cvm = (ClientViewModel)DataContext;

			cvm.ListOfClients.Add(new Client() { ID = "New", Type = OAuthType.HOTP, Digits = 6, Config = "1", SharedSecret = string.Empty });
			cboxClientID.SelectedIndex = cboxClientID.Items.Count - 1;

			showEdit(e);
		}

		private void showEdit(RoutedEventArgs e)
		{
			tboxConfigOldText = tboxConfig.Text;
			tboxSharedSecret.Password = string.Empty;
			DetailGrid.Visibility = Visibility.Visible;
			e.Handled = true;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			var pa = new PA();
			var clients = pa.LoadClients();

			ClientViewModel vm = new ClientViewModel(clients);
			DataContext = vm;

			if (clients.Items.Count > 0)
				cboxClientID.SelectedValue = clients.Items[0];
			if (clients.SelectedIndex <= (cboxClientID.Items.Count - 1))
				cboxClientID.SelectedValue = clients.Items[clients.SelectedIndex];
		}

		private void MainGrid_Click(object sender, RoutedEventArgs e)
		{
			if (DetailGrid.Visibility == Visibility.Visible)
			{
				var client = GetCurrentClient();
				if (client != null)
				{
					var password = tboxSharedSecret.Password;
					if (!string.IsNullOrWhiteSpace(password))
						client.SharedSecret = password;
				}
				DetailGrid.Visibility = Visibility.Collapsed;
			}
		}

		private void tboxConfig_TextChanged(object sender, TextChangedEventArgs e)
		{
			var client = GetCurrentClient();
			if (client == null) 
				return;

			var config = tboxConfig.Text;
			if (string.IsNullOrEmpty(config)) 
				return;

			var actioner = ClientActionerFactory.Get(client);
			try
			{
				actioner.ValidateConfig(config);
			}
			catch (ArgumentException ex)
			{
				ShowError(ex);
				tboxConfig.Text = tboxConfigOldText;
			}
		}

		private void ShowError(string message)
		{
			MessageBox.Show(message, "SMS2 PremiumAccess - Error", MessageBoxButton.OK, MessageBoxImage.Error);
		}

		private void ShowError(Exception ex)
		{
			ShowError(ex.Message);
		}
	}
}
