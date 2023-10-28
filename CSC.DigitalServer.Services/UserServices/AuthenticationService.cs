using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSC.DigitalServer.Core.Interfaces;
using CSC.DigitalServer.Core.Models;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CSC.DigitalServer.Services.UserServices
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly ICosmosDbService _cosmosDbService;
		//private readonly IServiceBusToSignalHubProcessor _serviceBusToSignalHubProcessor;
		//private readonly TelemetryClient _telemetryClient;
		private readonly ILogger _logger;

		public AuthenticationService(
			//ICosmosDbService cosmosDbService, 
			IConfiguration configuration,
		   //IServiceBusToSignalHubProcessor serviceBusToSignalHubProcessor,
		   ILoggerFactory factory)
		{
			//_cosmosDbService = cosmosDbService;
			//_serviceBusToSignalHubProcessor = serviceBusToSignalHubProcessor;
			var telConfig = TelemetryConfiguration.CreateDefault();
			//telConfig.ConnectionString = configuration.GetSection("ConnectionStrings").GetSection("TelemetryConnectionString").Value;
			//_telemetryClient = new TelemetryClient(telConfig);
			_logger = factory.CreateLogger("AuthenticationService");
		}

		public async Task<LoginResponse> Authenticate(string userName, string password, string deviceId)
		{
			var response = new LoginResponse();
			//var user = await _cosmosDbService.GetAsync<UserModel>(userName, userName, Constants.CosmosDBContainer.UsersData);
			var user = new { BadgeId = userName, Password = password, IsAdmin = true, Role = "dev" };
			if (user == null || user.Password != password)
			{
				return response;
			}
			response.Username = user.BadgeId;
			response.Role = user.Role.ToString();
			response.IsAdmin = user.IsAdmin;
			response.IsLoggedIn = true;
			/*var isLoggedInSomewhere = await GetUserLoggedInStatus(userName, deviceId);
			var updatequery = new QueryDefinition("SELECT* FROM c where c.UserName = @userName");
			updatequery.WithParameter("@userName", userName);
			var itemTobeUpdated = new
			{
				BadgeId = userName,
				id = userName,
				IsLoggedIn = true,
				DeviceId = deviceId
			};
			if (isLoggedInSomewhere)
			{

				await _cosmosDbService.UpdateAsyncv2(userName, itemTobeUpdated, Constants.CosmosDBContainer.UserSession, updatequery);
			}
			else
			{
				await _cosmosDbService.AddAsync(userName, itemTobeUpdated, Constants.CosmosDBContainer.UserSession);
			}

			IDictionary<string, string> parameters = new Dictionary<string, string>
			{
				{ "DeviceId", deviceId },
				{ "UserId", userName },
				{ "IsLoggedInSomewhere", isLoggedInSomewhere.ToString() }
			};
			//_telemetryClient.TrackEvent("Login", parameters);
			_logger.LogInformation($"Login, {JsonConvert.SerializeObject(parameters)}");
			*/
			return response;
		}

		public async Task<bool> Logout(string userName)
		{
			/*
			var query = new QueryDefinition("SELECT* FROM c where c.UserName = @UserName");
			query.WithParameter("@userName", userName);
			var result = await _cosmosDbService.GetMultipleAsync<dynamic>(query, Constants.CosmosDBContainer.UserSession);
			var output = false;
			if (result.Any())
			{
				var tasksList = new List<Task>();
				foreach (var item in result)
				{
					tasksList.Add(_cosmosDbService.DeleteAsync(item.id.ToString(), item.BadgeId.ToString(), Constants.CosmosDBContainer.UserSession));
				}
				await Task.WhenAll(tasksList);
				output = true;
			}
			IDictionary<string, string> parameters = new Dictionary<string, string>
			{
				{ "UserId", userName }
			};
			//_telemetryClient.TrackEvent("Logout", parameters);
			_logger.LogInformation($"Logout, {JsonConvert.SerializeObject(parameters)}");
			return output;
			*/
			return true;
		}

		/*
		private async Task<bool> GetUserLoggedInStatus(string userName, string deviceId)
		{
			var query = new QueryDefinition("SELECT* FROM c where c.UserName = @userName");
			query.WithParameter("@userName", userName);
			var result = await _cosmosDbService.GetMultipleAsync<dynamic>(query, Constants.CosmosDBContainer.UserSession);

			if (result.Any() && result.FirstOrDefault().IsLoggedIn == true)
			{
				IDictionary<string, string> parameters = new Dictionary<string, string>
				{
					{ "MethodName", "DuplicateLogin" },
					{ "UserId", userName }
				};
				//_telemetryClient.TrackEvent("DuplicateLogin", parameters);
				_logger.LogInformation($"DuplicateLogin, {JsonConvert.SerializeObject(parameters)}");
				//await _serviceBusToSignalHubProcessor.SendMessageToSignalR(JsonConvert.SerializeObject(new { userName, deviceId }), Constants.DuplicateLogin);
				return true;
			}
			else
			{
				return false;
			}

		}*/
	}
}
