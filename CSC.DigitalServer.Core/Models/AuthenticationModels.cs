using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CSC.DigitalServer.Core.Models
{
	public class LoginRequest
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public string DeviceId { get; set; }

	}
	public class LoginResponse
	{
		[JsonProperty("Username")]
		public string Username { get; set; }
		[JsonProperty("Role")]
		public string Role { get; set; }
		public string ErrorMessage { get; set; }
		public bool IsLoggedIn { get; set; }
		public bool IsAdmin { get; set; }
		public string Token { get; set; }

	}

	public class LogoutRequest
	{
		public string Username { get; set; }
		public string MouldId { get; set; }
	}
}
