using MongoDB.Bson;
using MongoDB.Driver;

namespace CosmosDBVectorStore.Lib.Interfaces;

public interface IMongoDbService
{
	Task UpsertVector(BsonDocument document);
	Task ImportJson(string collectionName, string json);
	IDictionary<string, IMongoCollection<BsonDocument>> GetCollections();
}