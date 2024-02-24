using System.Text;
using GymHubApp.Helpers;
using GymHubApp.Models;
using Newtonsoft.Json;

namespace GymHubApp;

public partial class AppShell : Shell
{
    bool hasUserCreatedAGym;

    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("register", typeof(SignUpPage));
        changeTab();
    }


    public AppShell(string role)
	{
		InitializeComponent();
		Routing.RegisterRoute("register", typeof(SignUpPage));
        changeTab(role);
    }


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        changeTab();
        hasUserCreatedAGym = await GymCreated();
        Console.WriteLine(hasUserCreatedAGym + "123");
    }


    public void changeTab(string role)
    {
        if (role.Equals("gymOwner"))
        {
            Console.WriteLine("Gym Onwer");
            subsTab.IsVisible = false;
            approveGymOwner.IsVisible = false;
            approvedUsers.IsVisible = false;
            disapprovedUsers.IsVisible = false;
            adminProfile.IsVisible = false;
            createGym.IsVisible = false;
            gymstab.IsVisible = false;
            subsTab.IsVisible = false;
            statsTab.IsVisible = false;
            profileTab.IsVisible = false;
            
            if (hasUserCreatedAGym)
            {
                createGym.IsVisible = false;
                membersTab.IsVisible = true;
                gymOwnerProfileTab.IsVisible = true;
            }
            else
            {
                createGym.IsVisible = true;
                membersTab.IsVisible = false;
                gymOwnerProfileTab.IsVisible = true;
            }
        }
        else if (role.Equals("admin"))
        {
            Console.WriteLine("Admin");
            gymstab.IsVisible = false;
            subsTab.IsVisible = false;
            statsTab.IsVisible = false;
            profileTab.IsVisible = false;
            createGym.IsVisible = false;
            gymOwnerProfileTab.IsVisible = false;
            membersTab.IsVisible = false;

            approveGymOwner.IsVisible = true;
            approvedUsers.IsVisible = true;
            disapprovedUsers.IsVisible = true;
            adminProfile.IsVisible = true;

        }
        else
        {
            Console.WriteLine("Regular user");
            approveGymOwner.IsVisible = false;
            approvedUsers.IsVisible = false;
            disapprovedUsers.IsVisible = false;
            adminProfile.IsVisible = false;
            createGym.IsVisible = false;
            gymOwnerProfileTab.IsVisible = false;
            membersTab.IsVisible = false;

            gymstab.IsVisible = true;
            subsTab.IsVisible = true;
            statsTab.IsVisible = true;
            profileTab.IsVisible = true;
        }

    }



    public void changeTab()
	{
        bool isGymOwner = CheckGymOwner();
        bool isAdmin = CheckAdmin();
        if (isGymOwner)
        {
            Console.WriteLine("Gym Onwer");
            subsTab.IsVisible = false;
            approveGymOwner.IsVisible = false;
            approvedUsers.IsVisible = false;
            disapprovedUsers.IsVisible = false;
            adminProfile.IsVisible = false;
            createGym.IsVisible = false;
            gymstab.IsVisible = false;
            subsTab.IsVisible = false;
            statsTab.IsVisible = false;
            profileTab.IsVisible = false;

            if (hasUserCreatedAGym)
            {
                createGym.IsVisible = false;
                membersTab.IsVisible = true;
                gymOwnerProfileTab.IsVisible = true;
            }
            else
            {
                createGym.IsVisible = true;
                membersTab.IsVisible = false;
                gymOwnerProfileTab.IsVisible = true;
            }


        }
        else if (isAdmin)
        {
            Console.WriteLine("Admin");
            gymstab.IsVisible = false;
            subsTab.IsVisible = false;
            statsTab.IsVisible = false;
            profileTab.IsVisible = false;
            createGym.IsVisible = false;
            gymOwnerProfileTab.IsVisible = false;
            membersTab.IsVisible = false;

            approveGymOwner.IsVisible = true;
            approvedUsers.IsVisible = true;
            disapprovedUsers.IsVisible = true;
            adminProfile.IsVisible = true;
        }
        else
        {
            Console.WriteLine("Regular user");
            approveGymOwner.IsVisible = false;
            approvedUsers.IsVisible = false;
            disapprovedUsers.IsVisible = false;
            adminProfile.IsVisible = false;
            createGym.IsVisible = false;
            gymOwnerProfileTab.IsVisible = false;
            membersTab.IsVisible = false;

            gymstab.IsVisible = true;
            subsTab.IsVisible = true;
            statsTab.IsVisible = true;
            profileTab.IsVisible = true;
        }

    }


	public  bool CheckGymOwner()
	{
		try
		{
            using (HttpClient httpClient = new HttpClient())
            {
                User user = LocalDB.GetUser();
                var url = Constants.SERVER_URL + "app/users/checkGymOwner";
				var id = new { user.userID };
				string userIDJSON = JsonConvert.SerializeObject(id);
				var body = new StringContent(userIDJSON, Encoding.UTF8, "application/json");
				var response =  httpClient.PostAsync(url, body).Result;
				if (response.IsSuccessStatusCode)
				{
					return true;
				}
            }

			return false;
        }
        catch(Exception ex)
		{
			return false;
		}

    }


    public  bool CheckAdmin()
    {
        try
        {
            using (HttpClient httpClient = new HttpClient())
            {
                User user = LocalDB.GetUser();
                var url = Constants.SERVER_URL + "app/users/checkAdmin";
                var id = new { user.userID };
                string userIDJSON = JsonConvert.SerializeObject(id);
                var body = new StringContent(userIDJSON, Encoding.UTF8, "application/json");
                var response = httpClient.PostAsync(url, body).Result;
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            return false;
        }

    }


    public async Task<bool> GymCreated()
    {
        try
        {
            using(HttpClient httpClient = new HttpClient())
            {
                User user = LocalDB.GetUser();
                var url = Constants.SERVER_URL + "app/users/gymCreated/" + user.userID;
                var response = httpClient.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var responseJSON = JsonConvert.DeserializeObject<GymIDResponse>(responseBody);
                    if (responseJSON.gymID.Equals("0")){
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        
        }
        catch
        {
            return false;
        }
    }

}
