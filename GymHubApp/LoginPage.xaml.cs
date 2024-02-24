using System.Text;
using System.Text.RegularExpressions;
using GymHubApp.Helpers;
using GymHubApp.Models;
using Mzsoft.BCrypt;
using Newtonsoft.Json;

namespace GymHubApp;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
        InitializeComponent();
	}


    protected override async void OnAppearing()
    {
        base.OnAppearing();
        bool isAuthed = await Task.Run(() => LocalDB.CheckUser());
        if (isAuthed)
            await Shell.Current.GoToAsync("//main");

    }

    async void registerBtn_Clicked(System.Object sender, System.EventArgs e)
    {
        try
        {
            await Shell.Current.GoToAsync($"//login/register");
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error navigating to SignUpPage: {ex.Message}");
        }

    }

    private bool IsEmailValid(string email)
    {
        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        if (Regex.IsMatch(email, pattern))
            return true;
        return false;
    }

    bool Validate()
    {
        if(String.IsNullOrEmpty(emailEntry.Text) || String.IsNullOrEmpty(passwordEntry.Text))
        {
            wanringFrame.IsVisible = true;
            warnText.Text = "• Please enter your email and password";
            return false;
        }
        if (!(IsEmailValid(emailEntry.Text)))
        {
            wanringFrame.IsVisible = true;
            warnText.Text = "• Please enter a valid email";
            return false;
        }
        return true;
    }


    void loginBtn_Clicked(System.Object sender, System.EventArgs e)
    {
        if (Validate())
        {

            Login();
          
        }

    }


    async void Login()
    {
        var email = new{ email = emailEntry.Text };
        var checkURL = Constants.SERVER_URL + "app/users/checkPassword";
        try
        {
            HttpClient httpClient = new HttpClient();
            var emailJSON = JsonConvert.SerializeObject(email);
            var checkBody = new StringContent(emailJSON, Encoding.UTF8, "application/json");
            var checkResponse = await httpClient.PostAsync(checkURL, checkBody);
            if (checkResponse.IsSuccessStatusCode)
            {
                var checkResponseBody = await checkResponse.Content.ReadAsStringAsync();
                var checkResponseJSON = JsonConvert.DeserializeObject<Password>(checkResponseBody);
                if(BCrypt.CheckPassword(passwordEntry.Text, checkResponseJSON.password))
                {
                    try
                    {
                        var loginURL = Constants.SERVER_URL + "app/users/login";
                        var user = new
                        {
                            email = emailEntry.Text,
                            checkResponseJSON.token,
                            isAuthed = true
                        };
                        var userJSON = JsonConvert.SerializeObject(user);
                        var loginBody = new StringContent(userJSON, Encoding.UTF8, "application/json");
                        var loginResponse = await httpClient.PostAsync(loginURL, loginBody);
                        if (loginResponse.IsSuccessStatusCode)
                        {
                            var loginResponseBody = await loginResponse.Content.ReadAsStringAsync();
                            var loginResponseJSON = JsonConvert.DeserializeObject<UserLoggedIn>(loginResponseBody);
                            string role = String.Empty;
                            if (loginResponseJSON.isAdmin)
                            {
                                role = "admin";
                            }
                            else if (loginResponseJSON.isGymOwner)
                            {
                                role = "gymOwner";
                            }
                            else
                            {
                                role = "regular";
                            }
                            User loggedUser = new User()
                            {
                                userID = loginResponseJSON.userID,
                                firstName = loginResponseJSON.firstName,
                                lastName = loginResponseJSON.lastName,
                                phoneNumber = loginResponseJSON.phoneNumber,
                                dateOfBirth = loginResponseJSON.dateOfBirth,
                                email = loginResponseJSON.email,
                                isMale = loginResponseJSON.isMale,
                                emirate = loginResponseJSON.emirate,
                                role = role,
                                token = loginResponseJSON.token
                            };

                            LocalDB.StoreUser(loggedUser);
                            var shell = Shell.Current;
                            var appShell = shell as AppShell;
                            appShell.changeTab(role);
                            httpClient.Dispose();
                            wanringFrame.IsVisible = false;
                            //passwordEntry.Text = "";
                            await Shell.Current.GoToAsync("//main");
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                    }
                }
                else
                {
                    wanringFrame.IsVisible = true;
                    warnText.Text = "Email or password are incorrect";
                }
            }
            else
            {
                wanringFrame.IsVisible = true;
                warnText.Text = "Email or password are incorrect";
            }
        }
        catch(Exception ex)
        {
            wanringFrame.IsVisible = false;
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }
}
