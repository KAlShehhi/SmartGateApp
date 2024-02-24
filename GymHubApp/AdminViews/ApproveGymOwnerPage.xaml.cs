using System.Collections.ObjectModel;
using System.Text;
using GymHubApp.Helpers;
using GymHubApp.Models;
using Newtonsoft.Json;

namespace GymHubApp.AdminViews;

public partial class ApproveGymOwnerPage : ContentPage
{

    public ApproveGymOwnerPage()
	{
		InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        getUser();
    }

    public async void getUser()
    {
        using (HttpClient httpClient = new HttpClient())
        {
            var url = Constants.SERVER_URL + "admin/getUserRequests";
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var userResponse = JsonConvert.DeserializeObject<UserRequestResponse>(responseBody);
                var users = userResponse.Users;
                listView.ItemsSource = new ObservableCollection<UserRequestGymOwner>(users);
            }
            else
            {
                Console.WriteLine("Failed to retrieve user requests: " + response.StatusCode);
            }
        }
    }




    void ApproveBtn_Clicked(System.Object sender, System.EventArgs e)
    {
        Button button = (Button)sender;
        UserRequestGymOwner user = (UserRequestGymOwner)button.BindingContext;
        var users = (ObservableCollection<UserRequestGymOwner>)listView.ItemsSource;
        using (HttpClient httpClient = new HttpClient())
        {
            string token = LocalDB.GetUser().token;
            string adminID = LocalDB.GetUser().userID;
            var url = Constants.SERVER_URL + "admin/makeUserAGymOwner";
            var input = new
            {
                token,
                user.userID,
                adminID,
                approval = "1"
            };
            string inputJSON = JsonConvert.SerializeObject(input);
            var body = new StringContent(inputJSON, Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(url, body).Result;
            if (response.IsSuccessStatusCode)
            {
                users.Remove(user);
            }
            else
            {
                warnText.Text = "An Error occurred";
                wanringFrame.IsVisible = true;
                return;
            }
        }
    }

    void DisapproveBtn_Clicked(System.Object sender, System.EventArgs e)
    {
        Button button = (Button)sender;
        UserRequestGymOwner user = (UserRequestGymOwner)button.BindingContext;
        var users = (ObservableCollection<UserRequestGymOwner>)listView.ItemsSource;
        using (HttpClient httpClient = new HttpClient())
        {
            string token = LocalDB.GetUser().token;
            string adminID = LocalDB.GetUser().userID;
            var url = Constants.SERVER_URL + "admin/makeUserAGymOwner";
            var input = new
            {
                token,
                user.userID,
                adminID,
                approval = "2"
            };
            string inputJSON = JsonConvert.SerializeObject(input);
            var body = new StringContent(inputJSON, Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(url, body).Result;
            if (response.IsSuccessStatusCode)
            {
                users.Remove(user);
            }
            else
            {
                warnText.Text = "An Error occurred";
                wanringFrame.IsVisible = true;
                return;
            }
        }
    }

}
