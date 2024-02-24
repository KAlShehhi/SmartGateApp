using System;
using SQLite;

namespace GymHubApp.Models
{
	[Table("user")]
	public class User
	{
		[PrimaryKey]
		[Column("id")]
		public int Id { get; set; }
        [Column("userID")]
        public string userID { get; set; }
        [Column("firstName")]
		public string firstName { get; set; }
        [Column("lastName")]
        public string lastName { get; set; }
        [Column("phoneNumber")]
        public string phoneNumber { get; set; }
        [Column("email")]
        public string email { get; set; }
        [Column("dateOfBirth")]
        public string dateOfBirth { get; set; }
        [Column("emirate")]
        public string emirate { get; set; }
        [Column("isMale")]
        public bool isMale { get; set; }
        [Column("role")]
        public string role { get; set; }
        [Column("token")]
        public string token { get; set; }
    }
}

