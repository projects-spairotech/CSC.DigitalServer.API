using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace CSC.DigitalServer.Core.Interfaces
{
	public interface ICosmosDbService
	{
		Task<List<T>> GetMultipleAsync<T>(QueryDefinition queryDefinition, string containerName);

		Task<T> GetAsync<T>(string id, string partitionKey, string containerName);
		Task AddAsync<T>(string partitionKey, T item, string containerName);
		Task UpdateAsync<T>(string partitionKey, T item, string containerName);
		Task DeleteAsync(string id, string partitionKey, string containerName);
		Task UpdateAsyncv2(string partitionKey, dynamic item, string containerName, QueryDefinition queryDefinition);

	}
}
