namespace GymHubApp.AdminViews;

public partial class AdminProfilePage : ContentPage
{
	public AdminProfilePage()
	{
		InitializeComponent();
	}

    async void Logout_Clicked(System.Object sender, System.EventArgs e)
    {
        try
        {
            LocalDB.dropTable();
            await Shell.Current.GoToAsync("//login");
        }
        catch (NullReferenceException ex)
        {
            Console.WriteLine(ex.StackTrace);
        }
    }
}
