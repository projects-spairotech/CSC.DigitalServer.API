using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static CSC.DigitalServer.Core.Constants;

namespace CSC.DigitalServer.Core.Models.DBModels
{
	public record UserModel
	{
		[Newtonsoft.Json.JsonProperty("id")]
		[JsonPropertyName("id")]
		public string Id { get; set; }
		public string BadgeId { get; set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public UserRoles Role { get; set; }
		public string Password { get; set; }
		public bool IsAdmin
		{
			get
			{
				return Role == UserRoles.Admin || Role == UserRoles.Dev;
			}
		}

		public bool IsTesting
		{
			get
			{
				return Role == UserRoles.Dev;
			}
		}
	}
}
