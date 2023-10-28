using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CSC.DigitalServer.API.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using CSC.DigitalServer.Core.Models;
using IAuthenticationService = CSC.DigitalServer.Core.Interfaces.IAuthenticationService;

namespace CSC.DigitalServer.API.Controllers.User
{
	[ApiController]
	[Authorize]
	[Route("api/[controller]")]
	[Consumes("application/json")]
	[Produces("application/json")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthenticationService _authenticationService;
		private readonly AppSettings _appSettings;
		private readonly ILogger _logger;

		public AuthController(IAuthenticationService authenticationService, IOptions<AppSettings> appSettings, ILoggerFactory factory)
		{
			_authenticationService = authenticationService;
			_appSettings = appSettings.Value;
			_logger = factory.CreateLogger("AuthController");
		}

		/// <summary>
		/// Login
		/// </summary>
		/// <param name="request"></param>
		/// <returns>Successful login and returns the authentication token</returns>
		/// <remarks>
		/// Sample request:
		///
		///     POST /Login
		///     {
		///        "username": "",
		///        "password": "",
		///        "deviceId": ""
		///     }
		///
		/// </remarks>
		[AllowAnonymous]
		[HttpPost("Login")]
		[SwaggerResponse(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
		[SwaggerResponse(StatusCodes.Status400BadRequest)]
		[SwaggerResponse(StatusCodes.Status409Conflict)]
		[SwaggerResponse(StatusCodes.Status500InternalServerError)]
		[SwaggerResponse(StatusCodes.Status503ServiceUnavailable)]
		public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
		{
			try
			{
				var user = await _authenticationService.Authenticate(request.Username, request.Password, request.DeviceId);

				if (string.IsNullOrEmpty(user.Username))
					return BadRequest(new LoginResponse { ErrorMessage = "UserId or Password is incorrect" });
				_logger.LogInformation($"user {user.Username} logged in");

				var tokenHandler = new JwtSecurityTokenHandler();
				var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
				var tokenDescriptor = new SecurityTokenDescriptor
				{
					Subject = new ClaimsIdentity(new Claim[]
					{
					new Claim(ClaimTypes.Name, user.Role)
					}),
					Expires = DateTime.UtcNow.AddDays(3),
					SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
				};
				var token = tokenHandler.CreateToken(tokenDescriptor);
				var tokenString = tokenHandler.WriteToken(token);

				// return basic user info and authentication token
				return Ok(new
				{
					user.Username,
					user.Role,
					user.IsLoggedIn,
					user.IsAdmin,
					Token = tokenString
				});
			}
			catch (Exception ex)
			{
				// return error message if there was an exception
				_logger.LogError(ex, message: ex.Message);
				return BadRequest(new LoginResponse { ErrorMessage = ex.Message });
			}
		}

		/// <summary>
		/// Logout
		/// </summary>
		/// <param name="request"></param>
		/// <returns>Successful logout from web portal and deallocates token</returns>
		/// <remarks>
		/// Sample request:
		///
		///     POST /Logout
		///     {
		///        "username": ""
		///     }
		///
		/// </remarks>
		[Authorize]
		[HttpPost("Logout")]
		[SwaggerResponse(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
		[SwaggerResponse(StatusCodes.Status400BadRequest)]
		[SwaggerResponse(StatusCodes.Status409Conflict)]
		[SwaggerResponse(StatusCodes.Status500InternalServerError)]
		[SwaggerResponse(StatusCodes.Status503ServiceUnavailable)]
		public async Task<IActionResult> Logout(LogoutRequest request)
		{
			try
			{
				var user = await _authenticationService.Logout(request.Username);
				_logger.LogInformation($"user {request.Username} logged out");
				return Ok(new LoginResponse
				{
					IsLoggedIn = false,
					ErrorMessage = String.Empty,
					Username = request.Username
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, message: ex.Message);
				// return error message if there was an exception
				return BadRequest(new LoginResponse { ErrorMessage = ex.Message });
			}
		}

		[AllowAnonymous]
		[HttpPost("testmethod")]
		public async Task<IActionResult> testmethod([FromBody] LogoutRequest request)
		{
			return Ok();
		}
	}
}
