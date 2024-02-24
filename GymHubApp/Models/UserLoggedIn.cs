using System;
namespace GymHubApp.Models
{
	public class UserLoggedIn
	{
	

        public int Id { get; set; }
        public string userID { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string dateOfBirth { get; set; }
        public string emirate { get; set; }
        public bool isMale { get; set; }
        public bool isAdmin { get; set; }
        public bool isGymOwner { get; set; }
        public string token { get; set; }
        
	}
}

