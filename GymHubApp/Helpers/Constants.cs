using System;
namespace GymHubApp.Helpers
{
	public static class Constants
	{
		public static string SERVER_URL = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:3000/api/" : "http://localhost:3000/api/";
    }
}

