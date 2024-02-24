using System.Text.RegularExpressions;
using Mzsoft.BCrypt;
using GymHubApp.Models;
using GymHubApp.Helpers;
using System.Text;
using Newtonsoft.Json;
using System.Data;

namespace GymHubApp;

public partial class SignUpPage : ContentPage
{
    public SignUpPage()
	{
		InitializeComponent();

	}

    async void loginButton_Clicked(System.Object sender, System.EventArgs e)
    {
        LocalDB.dropTable();
        await Navigation.PopAsync();
    }


    private bool HasCapital(string password)
    {
        var hasUpperChar = new Regex(@"[A-Z]+");
        if (hasUpperChar.IsMatch(password))
            return true;
        return false;
    }

    private bool HasNumber(string password)
    {
        var hasNumber = new Regex(@"[0-9]+");
        if (hasNumber.IsMatch(password))
            return true;
        return false;
    }

    private bool HasSpecial(string password)
    {
        var hasSpecial = @"[@!_#]";
        if (Regex.IsMatch(password, hasSpecial))
            return true;
        return false;
    }

    private bool LongerThan8(string password)
    {
        var hasMinimum8Chars = new Regex(@".{8,}");
        if (hasMinimum8Chars.IsMatch(password))
            return true;
        return false;
    }

    private bool IsEmailValid(string email)
    {
        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        if (Regex.IsMatch(email, pattern))
            return true;
        return false;
    }

    private bool IsPhoneNumberValid(string phoneNumber)
    {
        phoneNumber = phoneNumber.Replace("-", "");
        string pattern = @"^5\d{8}$";
        if (Regex.IsMatch(phoneNumber, pattern))
            return true;
        return false;
    }

    private bool IsAbove18(int yearOfBirth, int monthOfBirth, int dayOfBirth)
    {
        DateTime birthDate = new DateTime(yearOfBirth, monthOfBirth, dayOfBirth);
        DateTime currentDate = DateTime.Now;
        int age = currentDate.Year - birthDate.Year;
        if (currentDate.Month < birthDate.Month || (currentDate.Month == birthDate.Month && currentDate.Day < birthDate.Day))
        {
            age--;
        }

        if (age <= 17)
        {
            return false;
        }

        return true;
    }

    bool Validate()
    {
        int count = 0;
        string message = String.Empty;
        DateTime date = dateEntry.Date;
        if (String.IsNullOrEmpty(firstNameEntry.Text))
        {
            count++;
            message = addNewLine(message, "Please enter your first name", count);
        }
        if (String.IsNullOrEmpty(lastNameEntry.Text))
        {
            count++;
            message = addNewLine(message, "Please enter your last name", count);
        }
        if (String.IsNullOrEmpty(emailEntry.Text))
        {
            count++;
            message = addNewLine(message, "Please add an email", count);
        }
        if (!String.IsNullOrEmpty(emailEntry.Text) && IsEmailValid(emailEntry.Text) != true)
        {
            count++;
            message = addNewLine(message, "Please enter a valid email", count);
        }
        if (String.IsNullOrEmpty(phoneEntry.Text))
        {
            count++;
            message = addNewLine(message, "Please add a phone number", count);
        }
        if (!String.IsNullOrEmpty(phoneEntry.Text) && IsPhoneNumberValid(phoneEntry.Text) != true)
        {
            count++;
            message = addNewLine(message, "Please enter a valid phone number", count);
        }

        if (!maleBtn.IsChecked && !femaleBtn.IsChecked)
        {
            count++;
            message = addNewLine(message, "Please select a gender", count);
        }

        if (!IsAbove18(date.Year, date.Month, date.Day))
        {
            count++;
            message = addNewLine(message, "You must be over 18 to register", count);
        }

        if (String.IsNullOrEmpty(passwordEntry.Text))
        {
            count++;
            message = addNewLine(message, "Please enter a password", count);
        }
        if (String.IsNullOrEmpty(ConfirmPasswordEntry.Text))
        {
            count++;
            message = addNewLine(message, "Please enter re-enter your password", count);
        }
        if (!String.IsNullOrEmpty(passwordEntry.Text) && !String.IsNullOrEmpty(ConfirmPasswordEntry.Text))
        {
            if (passwordEntry.Text.Equals(ConfirmPasswordEntry.Text))
            {
                if (!HasCapital(passwordEntry.Text))
                {
                    count++;
                    message = addNewLine(message, "Your password must contain a capital letter", count);
                }
                if (!HasNumber(passwordEntry.Text))
                {
                    count++;
                    message = addNewLine(message, "Your password must contain a number", count);
                }
                if (!HasSpecial(passwordEntry.Text))
                {
                    count++;
                    message = addNewLine(message, "Your password must contain a special character", count);
                }
                if (!LongerThan8(passwordEntry.Text))
                {
                    count++;
                    message = addNewLine(message, "Your password must be longer than 8 characters", count);
                }
            }
            else
            {
                count++;
                message = addNewLine(message, "Your password and confirm password don't match", count);
            }
        }

        if (emirateEntry.SelectedIndex == -1)
        {
            count++;
            message = addNewLine(message, "Please choose an emirate", count);
        }

        if (count == 0)
        {
            wanringFrame.IsVisible = false;
            return true;
        }
        wanringFrame.IsVisible = true;
        warnText.Text = message;
        return false;
    }

    string addNewLine(string message, string newMessage, int count)
    {
        if (count == 1)
        {
            return "• " + newMessage;
        }
        else
        {
            return message + System.Environment.NewLine + "• " + newMessage;
        }
    }

    async void SignUp()
    {
        try { 
            string firstName = firstNameEntry.Text;
            string lastName = lastNameEntry.Text;
            string email = emailEntry.Text.ToLower();
            string phoneNumber = phoneEntry.Text;
            string emirate = emirateEntry.Items[emirateEntry.SelectedIndex];

            DateTime date = dateEntry.Date;
            int day = date.Day;
            int month = date.Month;
            int year = date.Year;
            bool isMale = false;
            if (maleBtn.IsChecked)
                isMale = true;
            string password = BCrypt.HashPassword(passwordEntry.Text, BCrypt.GenerateSalt(10));
            var newUser = new
            {
                firstName,
                lastName,
                email,
                phoneNumber,
                isMale,
                day,
                month,
                year,
                password,
                emirate
            };
            var url = Constants.SERVER_URL + "app/users/registerUser";
            HttpClient httpClient = new HttpClient();
            string newUserJSON = JsonConvert.SerializeObject(newUser);
            var reqbody = new StringContent(newUserJSON, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, reqbody);
            if (response.IsSuccessStatusCode)
            {
                var resBody = await response.Content.ReadAsStringAsync();
                var resBodyJSON = JsonConvert.DeserializeObject<UserSignedUpResponse>(resBody);

                User user = new User()
                {
                    userID = resBodyJSON.userID,
                    firstName = resBodyJSON.firstName,
                    lastName = resBodyJSON.lastName,
                    phoneNumber = resBodyJSON.phoneNumber,
                    email = resBodyJSON.email,
                    dateOfBirth = resBodyJSON.dateOfBirth,
                    emirate = resBodyJSON.emirate,
                    isMale = resBodyJSON.isMale,
                    role = resBodyJSON.role,
                    token = resBodyJSON.token,
                };
                Console.Write("123442 " + user.phoneNumber);
                LocalDB.StoreUser(user);
                httpClient.Dispose();
                var shell = Shell.Current;
                var appShell = shell as AppShell;
                appShell.changeTab(user.role);
                await Shell.Current.GoToAsync("//main");
            }
            else if((int)response.StatusCode == 409)
            {
                wanringFrame.IsVisible = true;
                warnText.Text = "Email is used in another account";
            }
            else
            {
                await DisplayAlert("Error", "There has been an error, try again later", "Ok");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Server might be offline, try agian later", "OK");

        }
    }

    async void signUpButton_Clicked(System.Object sender, System.EventArgs e)
    {

        if (Validate())
        {
            SignUp();
        }
        else
        {
            await scrollView.ScrollToAsync(0, 250, true);
        }

    }

}