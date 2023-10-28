using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using CSC.DigitalServer.Core.Interfaces;

namespace CSC.DigitalServer.Services.ExternalServices
{
	public class RestClient : IRestClient
	{
		private string _authToken = String.Empty;
		private readonly ILogger _logger;

		public RestClient(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger("RestClient");
		}

		public string GetResponse { get; set; }

		public async Task<T> GetAsync<T>(string url, string token = null)
		{
			HttpResponseMessage response = await RequestAsync(HttpMethod.Get, url, token).ConfigureAwait(false);
			string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			HandleIfErrorResponse(url, response, content);
			if (typeof(T) == typeof(string))
			{
				return GetValue<T>(content);
			}

			return JsonConvert.DeserializeObject<T>(content);
		}

		public async Task<T> PostAsync<T>(string url, object payload, string token = null)
		{
			HttpResponseMessage response = await RequestAsync(HttpMethod.Post, url, token, payload).ConfigureAwait(false);
			string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			HandleIfErrorResponse(url, response, content);
			if (typeof(T) == typeof(string))
			{
				return GetValue<T>(content);
			}
			return JsonConvert.DeserializeObject<T>(content);
		}

		public async Task<T> PutAsync<T>(string url, object payload, string token = null)
		{
			HttpResponseMessage response = await RequestAsync(HttpMethod.Put, url, token, payload).ConfigureAwait(false);
			string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			HandleIfErrorResponse(url, response, content);
			if (typeof(T) == typeof(string))
			{
				return GetValue<T>(content);
			}

			return JsonConvert.DeserializeObject<T>(content);
		}


		public async Task<T> DeleteAsync<T>(string url, string token = null)
		{
			HttpResponseMessage response = await RequestAsync(HttpMethod.Delete, url, token).ConfigureAwait(false);
			string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			HandleIfErrorResponse(url, response, content);
			if (typeof(T) == typeof(string))
			{
				return GetValue<T>(content);
			}

			return JsonConvert.DeserializeObject<T>(content);
		}

		private HttpClient GetClient(HttpClientHandler handler = null)
		{
			var client = handler == null ? new HttpClient() : new HttpClient(handler, true);
			client.Timeout = TimeSpan.FromSeconds(360);
			return client;
		}


		private Task<HttpResponseMessage> RequestAsync(HttpMethod method, string url, string token = null, object payload = null)
		{
			var request = PrepareRequest(method, url, payload);
			var localToken = token ?? _authToken;
			if (!string.IsNullOrEmpty(localToken))
			{
				var authHeader = new AuthenticationHeaderValue("Bearer", localToken);
				request.Headers.Authorization = authHeader;
			}
			return GetClient().SendAsync(request, HttpCompletionOption.ResponseContentRead);
		}

		private HttpRequestMessage PrepareRequest(HttpMethod method, string url, object payload)
		{
			var uri = PrepareUri(url);
			var request = new HttpRequestMessage(method, uri);
			if (payload != null)
			{
				var json = JsonConvert.SerializeObject(payload);
				request.Content = new StringContent(json, Encoding.UTF8, "application/json");
			}
			return request;
		}
		private Uri PrepareUri(string url)
		{
			return new Uri(url);
		}

		private void HandleIfErrorResponse(string url, HttpResponseMessage response, string content)
		{
			if (!response.IsSuccessStatusCode)
			{
				string message = $"Request url: {url}, statuCode: {response.StatusCode}, response: {content}";
				_logger.LogError(message);
				// we are not logging the whole url - since the url can contain the connection string or secure information.
				throw new RestException($"Error while getting data for url: {response.RequestMessage.RequestUri.GetLeftPart(UriPartial.Path)}, statusCode: {response.StatusCode}");
			}
		}

		private T GetValue<T>(String value)
		{
			return (T)Convert.ChangeType(value, typeof(T));
		}
	}
}
