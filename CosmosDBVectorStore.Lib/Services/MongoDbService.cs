using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using Microsoft.Extensions.Logging;
using CosmosDBVectorStore.Lib.Interfaces;

namespace CosmosDBVectorStore.Lib.Services;

public class MongoDbService : IMongoDbService
{
    private readonly ILogger<MongoDbService> _logger;
    private readonly IMongoDatabase? _database;
    private readonly Dictionary<string, IMongoCollection<BsonDocument>> _collections;
    
    public MongoDbService(IAppSettings appSettings, ILogger<MongoDbService> logger)
	{
		_logger = logger;
		_collections = new Dictionary<string, IMongoCollection<BsonDocument>>();
		try
		{
			var client = new MongoClient(appSettings.DbConnectionString);
			_database = client.GetDatabase(appSettings.DbName);
			InitializeCollections(appSettings.DbCollectionNames);
			CreateVectorIndexIfNotExists(_collections["vectors"]);
		}
		catch (Exception exception)
		{
			_logger.LogError("Could not initialize MongoDB: " + exception.Message);
		}
	}

    public IDictionary<string, IMongoCollection<BsonDocument>> GetCollections() => _collections;

    private void InitializeCollections(string collectionNames)
    {
	    var collections = collectionNames.Split(',').ToList();
	    foreach (string collectionName in collections)
	    {
		    IMongoCollection<BsonDocument>? collection = _database?.GetCollection<BsonDocument>(collectionName.Trim()) ?? throw new ArgumentException("Unable to connect to existing Azure Cosmos DB for MongoDB vCore collection or database.");
		    _collections.Add(collectionName, collection);
	    }
    }

	private void CreateVectorIndexIfNotExists(IMongoCollection<BsonDocument> vectorCollection)
	{
		try
		{
			string vectorIndexName = "vectorSearchIndex";
			using (IAsyncCursor<BsonDocument> indexCursor = vectorCollection.Indexes.List())
			{
				bool vectorIndexExists = indexCursor.ToList().Any(x => x["name"] == vectorIndexName);
				if (!vectorIndexExists)
				{
					BsonDocumentCommand<BsonDocument> command = new BsonDocumentCommand<BsonDocument>(
					BsonDocument.Parse(@"
						{ 
							createIndexes: 'vectors', 
							indexes: [{ 
								name: 'vectorSearchIndex', 
								key: { vector: 'cosmosSearch' }, 
								cosmosSearchOptions: { kind: 'vector-ivf', numLists: 5, similarity: 'COS', dimensions: 1536 } 
							}] 
						}"
					));

					if (_database is null) return;

					BsonDocument result = _database.RunCommand(command);
					if (result["ok"] != 1)
						_logger.LogError("Could not create vector index: " + result.ToJson());
				}
			}
		}
		catch (Exception exception)
		{
			_logger.LogError("Could not create vector index: " + exception.Message);
		}
	}
	
	public async Task ImportJson(string collectionName, string json)
	{
		try
		{
			IMongoCollection<BsonDocument> collection = _collections[collectionName];
			var documents = BsonSerializer.Deserialize<IEnumerable<BsonDocument>>(json);
			await collection.InsertManyAsync(documents);
		}
		catch (MongoException exception)
		{
			_logger.LogError($"Could not import document: {exception.Message}");
			throw;
		}
	}
	
	public async Task UpsertVector(BsonDocument document)
	{
		if (!document.Contains("_id"))
		{
			_logger.LogError("Document does not contain _id.");
			throw new ArgumentException("Document does not contain _id.");
		}

		try
		{
			var filter = Builders<BsonDocument>.Filter.Eq("_id", document["_id"]);
			var options = new ReplaceOptions { IsUpsert = true };
			await _collections["vectors"].ReplaceOneAsync(filter, document, options);
		}
		catch (Exception exception)
		{
			_logger.LogError($"Could not upsert vector: {exception.Message}");
			throw;
		}
	}
}