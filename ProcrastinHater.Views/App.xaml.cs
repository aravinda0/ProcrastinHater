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
			
			TasksManager tm = new TasksManager();
			GroupsManager gm = new GroupsManager();
			ChecklistElementOrganizer cm = new ChecklistElementOrganizer();
			
			
			MainWindowVM mainWinVm = new MainWindowVM(tm, gm, cm);
			MainView window = new MainView();
			
			mainWinVm.RequestClose += delegate { window.Close(); };
			window.DataContext = mainWinVm;
			
			
			window.Show();
		}		
	}
}