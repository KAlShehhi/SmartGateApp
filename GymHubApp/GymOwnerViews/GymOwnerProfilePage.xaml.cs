using GymHubApp.Models;

namespace GymHubApp.GymOwnerViews;

public partial class GymOwnerProfilePage : ContentPage
{
	public GymOwnerProfilePage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        getUserDate();
    }

    private void getUserDate()
    {
        User user = LocalDB.GetUser();
        DateTime dateTime = DateTime.Parse(user.dateOfBirth);
        emailLabel.Text = user.email;
        dateLabel.Text = dateTime.ToString("dd/MM/yyyy");
        emirateLabel.Text = user.emirate;
        fname.Text = user.firstName;
        lname.Text = user.lastName;
        phoneLabel.Text = "+971" + user.phoneNumber;
        if (user.isMale)
        {
            genderLabel.Text = "Male";
        }
        else
        {
            genderLabel.Text = "Female";
        }
    }


    async void LogoutButton_Clicked(System.Object sender, System.EventArgs e)
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
