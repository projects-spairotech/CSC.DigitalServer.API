using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSC.DigitalServer.Core.Models;

namespace CSC.DigitalServer.Core.Interfaces
{
	public interface IAuthenticationService
	{
		Task<LoginResponse> Authenticate(string userName, string password, string deviceId = null);
		Task<bool> Logout(string userName);
	}
}
