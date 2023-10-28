using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSC.DigitalServer.Core.Interfaces
{
	public interface IRestClient
	{
		Task<T> GetAsync<T>(string url, string token = null);
		Task<T> PostAsync<T>(string url, object payload, string token = null);
		Task<T> PutAsync<T>(string url, object payload, string token = null);
		Task<T> DeleteAsync<T>(string url, string token = null);
	}
}
