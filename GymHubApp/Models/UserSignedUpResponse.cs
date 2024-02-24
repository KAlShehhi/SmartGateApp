using System;
namespace GymHubApp.Models
{
	public class UserSignedUpResponse
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
        public string role { get; set; }
        public string token { get; set; }
    }
}

