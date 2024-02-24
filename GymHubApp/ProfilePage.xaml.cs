using System.Text;
using GymHubApp.Helpers;
using GymHubApp.Models;
using Newtonsoft.Json;

namespace GymHubApp;

public partial class ProfilePage : ContentPage
{
    string applyStat;
    public ProfilePage()
	{
		InitializeComponent();
        updateApplyBtn();
        getUserDate();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        updateApplyBtn();
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
        phoneLabel.Text = "+971" +user.phoneNumber;
        if (user.isMale)
        {
            genderLabel.Text = "Male";
        }
        else
        {
            genderLabel.Text = "Female";
        }
    }

    async void updateApplyBtn()
    {
        User user = LocalDB.GetUser();
        using (HttpClient httpClient = new HttpClient())
        {
            var url = Constants.SERVER_URL + "app/users/getApplyStatus";
            var userID = new { user.userID };
            var idJSON = JsonConvert.SerializeObject(userID);
            var body = new StringContent(idJSON, Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(url, body).Result;
            if (response.IsSuccessStatusCode)
            {
                var resBody = response.Content.ReadAsStringAsync().Result;
                var resJSON = JsonConvert.DeserializeObject<ApplyStatus>(resBody);
                applyStat = resJSON.applyStatus;
                switch (applyStat)
                {
                    case "0":
                        ApplyBtn.IsEnabled = true;
                        ApplyBtn.Text = "Apply";
                        break;
                    case "1":
                        ApplyBtn.Text = "Waiting for approval";
                        ApplyBtn.IsEnabled = false;
                        break;
                    case "2":
                        ApplyBtn.Text = "You have been rejected";
                        ApplyBtn.IsEnabled = false;
                        break;
                    case "3":
                        ApplyBtn.Text = "You have been approved";
                        ApplyBtn.IsEnabled = false;
                        await DisplayAlert("You are now a gym owner!", "You are now a gym owner, please log in again for the changes to take affect, Thanks!", "log out");
                        LocalDB.dropTable();
                        await Shell.Current.GoToAsync("//login");
                        break;
                }
                httpClient.Dispose();
            }
            else
            {
                warnApply.IsVisible = true;
                httpClient.Dispose();
            }
        }
    }

    async void Button_Clicked(System.Object sender, System.EventArgs e)
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

    void GymOwnerBtn_Clicked(System.Object sender, System.EventArgs e)
    {
        if (applyStat.Equals("0"))
        {
            using (HttpClient httpClient = new HttpClient())
            {
                User user = LocalDB.GetUser();
                var url = Constants.SERVER_URL + "app/users/applyGymOwner";
                var userID = new { user.userID };
                var idJSON = JsonConvert.SerializeObject(userID);
                var body = new StringContent(idJSON, Encoding.UTF8, "application/json");
                var response = httpClient.PostAsync(url, body).Result;
                if (response.IsSuccessStatusCode)
                {
                    updateApplyBtn();
                    httpClient.Dispose();
                    return;
                }
                else
                {
                    warnApply.IsVisible = true;
                    httpClient.Dispose();
                }
            }
        }
    }
}
