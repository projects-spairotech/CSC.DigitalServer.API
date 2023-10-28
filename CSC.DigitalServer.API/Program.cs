using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CSC.DigitalServer.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Build().Run();
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((hostingContext, config) =>
				{
#if DEBUG
					config.AddJsonFile("appsettings.Development.json",
					optional: true,
					reloadOnChange: true);
#elif STAGING
                    config.AddJsonFile("appsettings.quality.json",
                        optional: true,
                        reloadOnChange: true);
#elif RELEASE_LEH
                    config.AddJsonFile("appsettings.leh.json",
                        optional: true,
                        reloadOnChange: true);
#else
                    config.AddJsonFile("appsettings.json",
                                            optional: true,
                                            reloadOnChange: true);
#endif
					var builtConfig = config.Build();
					var vaultName = builtConfig.GetSection("ConnectionStrings").GetSection("VaultName").Value;
					//var keyvaultClient = new KeyVaultClient(async (authority, resource, scope) =>
					//{
					//	var credential = new DefaultAzureCredential(false);
					//	var token = credential.GetToken(new Azure.Core.TokenRequestContext(new[] { "https://vault.azure.net/.default" }));
					//	return token.Token;
					//});
					//config.AddAzureKeyVault(vaultName, keyvaultClient, new DefaultKeyVaultSecretManager());
				})
			.ConfigureLogging(logging =>
			{
				logging.ClearProviders();
				// We have to be precise on the logging levels
				logging.AddConsole();
				logging.AddDebug();
				//logging.AddAzureWebAppDiagnostics();
			}).UseStartup<Startup>();

	}
}
