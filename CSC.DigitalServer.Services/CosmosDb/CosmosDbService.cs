using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSC.DigitalServer.Core.Interfaces;
using Microsoft.Azure.Cosmos;

namespace CSC.DigitalServer.Services.CosmosDb
{
	public class CosmosDbService : ICosmosDbService
	{
		private Container _container;
		private string _dataBaseName;
		private CosmosClient _cosmosDbClient;
		public CosmosDbService(string databaseName, CosmosClient cosmosDbClient)
		{
			_dataBaseName = databaseName;
			_cosmosDbClient = cosmosDbClient;
		}

		public async Task AddAsync<T>(string partitionKey, T item, string containerName)
		{
			_container = _cosmosDbClient.GetContainer(_dataBaseName, containerName);
			await _container.CreateItemAsync(
				item: item,
				partitionKey: new PartitionKey(partitionKey)
			);
		}

		public async Task DeleteAsync(string id, string partitionKey, string containerName)
		{
			try
			{
				_container = _cosmosDbClient.GetContainer(_dataBaseName, containerName);
				await _container.DeleteItemAsync<dynamic>(id, new PartitionKey(partitionKey));
			}
			catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound) { }
		}

		public async Task<T> GetAsync<T>(string id, string partitionKey, string containerName)
		{
			T response = default;
			try
			{
				_container = _cosmosDbClient.GetContainer(_dataBaseName, containerName);
				response = await _container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
			}
			catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound) { }
			return response;
		}

		public async Task<List<T>> GetMultipleAsync<T>(QueryDefinition queryDefinition, string containerName)
		{
			_container = _cosmosDbClient.GetContainer(_dataBaseName, containerName);
			var results = new List<T>();
			using var query = _container.GetItemQueryIterator<T>(queryDefinition);
			while (query.HasMoreResults)
			{
				var response = await query.ReadNextAsync();
				results.AddRange(response.ToList());
			}
			return results.ToList();
		}

		public async Task UpdateAsync<T>(string partitionKey, T item, string containerName)
		{
			_container = _cosmosDbClient.GetContainer(_dataBaseName, containerName);
			await _container.UpsertItemAsync(item, new PartitionKey(partitionKey));
		}

		public async Task UpdateAsyncv2(string partitionKey, dynamic item, string containerName, QueryDefinition queryDefinition)
		{
			var results = await GetMultipleAsync<dynamic>(queryDefinition, containerName);
			if (results.Any())
			{
				var idfordel = results.FirstOrDefault().id.ToString();
				await _container.DeleteItemAsync<dynamic>(idfordel, new PartitionKey(partitionKey));
				await _container.CreateItemAsync(item, new PartitionKey(partitionKey));
			}
			else
			{
				await _container.CreateItemAsync(item, new PartitionKey(partitionKey));
			}
		}
	}
}
