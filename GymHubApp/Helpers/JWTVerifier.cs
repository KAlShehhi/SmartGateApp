using System;
using System.Text;
using GymHubApp.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;

namespace GymHubApp.Helpers
{
	public class JWTVerifier
	{
        public async static Task<bool> IsJwtExpired(string token)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(10);
            var url = Constants.SERVER_URL + "app/users/validate";
            var tokenObj = new { token };
            string tokenJSON = JsonConvert.SerializeObject(tokenObj);
            var reqbody = new StringContent(tokenJSON, Encoding.UTF8, "application/json");
            try
            {
                var response = await httpClient.PostAsync(url, reqbody);
                Console.WriteLine(response.StatusCode);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine("Request timed out: " + ex.Message);
                return false;
            }
        }
    }
}

