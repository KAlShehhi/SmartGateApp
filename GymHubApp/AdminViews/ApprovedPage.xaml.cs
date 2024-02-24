using System.Collections.ObjectModel;
using GymHubApp.Helpers;
using GymHubApp.Models;
using Newtonsoft.Json;

namespace GymHubApp.AdminViews;

public partial class ApprovedPage : ContentPage
{
	public ApprovedPage()
	{
		InitializeComponent();
        getUser();
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
            var url = Constants.SERVER_URL + "admin/getAllGymOwners";
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var userResponse = JsonConvert.DeserializeObject<ApprovedUserResponse>(responseBody);
                var users = userResponse.Users;
                listView.ItemsSource = new ObservableCollection<ApprovedGymOwners>(users);
            }
            else
            {
                Console.WriteLine("Failed to retrieve user requests: " + response.StatusCode);
            }
        }
    }


}
