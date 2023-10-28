using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CSC.DigitalServer.API.Helpers;
using CSC.DigitalServer.Core;
using CSC.DigitalServer.Core.Interfaces;
using CSC.DigitalServer.Services;
using CSC.DigitalServer.Services.CosmosDb;
using CSC.DigitalServer.Services.UserServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace CSC.DigitalServer.API
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{

			services.AddControllers();
			services.AddScoped<IAuthenticationService, AuthenticationService>();
			//services.AddScoped<IAzureIotHubService, AzureIotHubService>();
			//services.AddScoped<IPrismaServices, PrismaServices>();
			//services.AddScoped<IMultiServerDeploymentService, MultiServerDeploymentService>();
			//services.AddScoped<ISchedularService, SchedularService>();
			//services.AddSingleton<IServiceBusToSignalHubProcessor, ServiceBusToSignalHubProcessor>();
			//services.AddSingleton<IBladeProductionApi, BladeServices>();
			//services.AddSingleton<IStoreService, StoreService>();
			//services.AddSingleton<ICosmosService, CosmosService>();
			//services.AddSingleton<IServiceBusConsumer, ServiceBusConsumer>();
			services.AddSingleton<IMemoryCacheService, MemoryCacheService>();
			//services.AddSingleton<IRestClient, RestClient>();
			services.AddSingleton<ICosmosDbService>(InitializeCosmosClientInstanceAsync().GetAwaiter().GetResult());
			services.AddSingleton<ILoggerFactory, LoggerFactory>();
			//services.AddSingleton<IBlobServices, BlobServices>();
			//services.AddSingleton<IPalletStore, PalletStoreDb>();
			//services.AddHostedService<BackgroundSchedularService>();

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "CSC.DigitalServer.API", Version = "v1" });
				var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
				c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
				{
					Name = "Authorization",
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header,
					Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
				});
				c.AddSecurityRequirement(new OpenApiSecurityRequirement {
					{
						new OpenApiSecurityScheme {
							Reference = new OpenApiReference {
								Type = ReferenceType.SecurityScheme,
									Id = "Bearer"
							}
						},
						Array.Empty<string>()
					}
				});
			});

			var appSettingsSection = Configuration.GetSection("AppSettings");
			services.Configure<AppSettings>(appSettingsSection);

			// configure jwt authentication
			var appSettings = appSettingsSection.Get<AppSettings>();
			var key = Encoding.ASCII.GetBytes(appSettings.Secret);
			services.AddAuthentication(x =>
			{
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(x =>
			{
				x.RequireHttpsMetadata = false;
				x.SaveToken = true;
				x.TokenValidationParameters = new TokenValidationParameters
				{
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = false,
					ValidateAudience = false,
					RequireExpirationTime = true,
				};
			});
			services.AddMemoryCache();
			//services.AddSignalR().AddAzureSignalR(Configuration.GetSection("ConnectionStrings").GetSection("SignalREndpoint").Value);
			//services.Configure<AzureFileLoggerOptions>(options =>
			//{
			//	options.FileName = "OBLogger";
			//	options.FileSizeLimit = 50 * 1024;
			//	options.RetainedFileCountLimit = 20;
			//});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseAuthentication();
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CSC.DigitalServer.API v1"));
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				//endpoints.MapHub<SignalHubService>("/api/signalhubservice");
			});
		}

		private async Task<CosmosDbService> InitializeCosmosClientInstanceAsync()
		{
			var account = Configuration.GetSection("ConnectionStrings").GetSection("CosmosDbEndpoint").Value;
			var key = Configuration.GetSection("ConnectionStrings").GetSection("CosmosKey").Value;
			var client = new Microsoft.Azure.Cosmos.CosmosClient(account, key, new CosmosClientOptions { ConnectionMode = ConnectionMode.Direct });
			var database = await client.CreateDatabaseIfNotExistsAsync(Constants.CosmosDBContainer.CosmosDbName);
			await database.Database.CreateContainerIfNotExistsAsync(Constants.CosmosDBContainer.UsersData, "/BadgeId");
			await database.Database.CreateContainerIfNotExistsAsync(Constants.CosmosDBContainer.UserSession, "/BadgeId");
			await database.Database.CreateContainerIfNotExistsAsync(Constants.CosmosDBContainer.ProsoftMapping, "/Mouldid");
			await database.Database.CreateContainerIfNotExistsAsync(Constants.CosmosDBContainer.EdgeServerHostnameMapping, "/HostName");
			await database.Database.CreateContainerIfNotExistsAsync(Constants.CosmosDBContainer.DeploymentPlan, "/Mouldid");
			await database.Database.CreateContainerIfNotExistsAsync(Constants.CosmosDBContainer.PalletStore, "/MouldId");
			await database.Database.CreateContainerIfNotExistsAsync(Constants.CosmosDBContainer.CameraMapping, "/MouldId");
			await database.Database.CreateContainerIfNotExistsAsync(Constants.CosmosDBContainer.CameraCalibrationState, "/MouldId");
			var cosmosDbService = new CosmosDbService(Constants.CosmosDBContainer.CosmosDbName, client);
			return cosmosDbService;
		}
	}
}
