using System;
using System.Windows;
using System.Data;
using System.Xml;
using System.Configuration;
using ProcrastinHater.BLL;
using ProcrastinHater.ViewModels;

namespace ProcrastinHater.Views
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			
			Bridge bridge = new Bridge();
			
			
			MainWindowVM mainWinVm = new MainWindowVM(bridge.TasksMgr, bridge.GroupsMgr, bridge.CEOrganizer);
			MainView window = new MainView();
			
			mainWinVm.RequestClose += delegate { window.Close(); };
			window.DataContext = mainWinVm;
			
			
			window.Show();
		}		
	}
}