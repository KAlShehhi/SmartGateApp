using GymHubApp.Models;

namespace GymHubApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
		try
		{
			User user = LocalDB.GetUser();
            MainPage = new AppShell(user.role);
        }
		catch 
		{
            MainPage = new AppShell();
        }
	
    }
}
