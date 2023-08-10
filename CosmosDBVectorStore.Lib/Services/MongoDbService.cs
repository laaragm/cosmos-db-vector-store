using CosmosDBVectorStore.Lib.Enumerators;
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
    
    private const string VectorIndexName = "vectorSearchIndex";

    public MongoDbService(IAppSettings appSettings, ILogger<MongoDbService> logger)
    {
        _logger = logger;
        _collections = new Dictionary<string, IMongoCollection<BsonDocument>>();

        try
        {
            _database = SetupDatabase(appSettings);
            InitializeCollections();
            EnsureVectorIndexIsPresent(_collections[CollectionEnumerator.Vectors.Name]);
        }
        catch (Exception exception)
        {
            _logger.LogError("Could not initialize MongoDB: " + exception.Message);
        }
    }

    private IMongoDatabase SetupDatabase(IAppSettings appSettings)
    {
        var client = new MongoClient(appSettings.DbConnectionString);
        return client.GetDatabase(appSettings.DbName);
    }

    private void InitializeCollections()
    {
        var collections = CollectionEnumerator.AllCollections.ToList();
        foreach (var item in collections)
        {
            var collectionName = item.Name;
            IMongoCollection<BsonDocument>? collection = _database?.GetCollection<BsonDocument>(collectionName.Trim()) ?? throw new ArgumentException("Unable to connect to database.");
            _collections.Add(collectionName, collection);
        }
    }

    private void EnsureVectorIndexIsPresent(IMongoCollection<BsonDocument> vectorCollection)
    {
        using (IAsyncCursor<BsonDocument> indexCursor = vectorCollection.Indexes.List())
        {
            bool vectorIndexExists = indexCursor.ToList().Any(x => x["name"] == VectorIndexName);
            if (!vectorIndexExists)
                CreateIndexForVectors();
        }
    }

    private void CreateIndexForVectors()
    {
        var documentCount = GetDocumentCount(CollectionEnumerator.Vectors.Name);
        var numLists = documentCount / 1000;
        BsonDocumentCommand<BsonDocument> command = new BsonDocumentCommand<BsonDocument>(
            BsonDocument.Parse($@"
                {{ 
                    createIndexes: '{CollectionEnumerator.Vectors.Name}',
                    indexes: [{{
                        name: '{VectorIndexName}', 
                        key: {{ vector: 'cosmosSearch' }}, 
                        cosmosSearchOptions: {{ kind: 'vector-ivf', numLists: {numLists}, similarity: 'COS', dimensions: 1536 }} 
                    }}] 
                }}"
            ));

        if (_database is null) return;

        BsonDocument result = _database.RunCommand(command);
        if (result["ok"] != 1)
            _logger.LogError("Could not create vector index: " + result.ToJson());
    }
    
    private long GetDocumentCount(string collectionName)
    {
        if (!_collections.ContainsKey(collectionName))
        {
            _logger.LogError($"No collection found with the name: {collectionName}.");
            throw new ArgumentException($"Collection {collectionName} does not exist.");
        }

        try
        {
            return _collections[collectionName].CountDocuments(new BsonDocument());
        }
        catch (MongoException exception)
        {
            _logger.LogError($"Could not get document count for {collectionName}: {exception.Message}");
            throw;
        }
    }


    public IDictionary<string, IMongoCollection<BsonDocument>> GetCollections() => _collections;

    public async Task ImportJson(string collectionName, string json)
    {
        try
        {
            IMongoCollection<BsonDocument> collection = _collections[collectionName];
            var documents = BsonSerializer.Deserialize<IEnumerable<BsonDocument>>(json);
            await InsertDocumentsAsync(collection, documents);
        }
        catch (MongoException exception)
        {
            _logger.LogError($"Could not import document: {exception.Message}");
            throw;
        }
    }

    private async Task InsertDocumentsAsync(IMongoCollection<BsonDocument> collection, IEnumerable<BsonDocument> documents)
    {
        await collection.InsertManyAsync(documents);
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
            await UpsertDocumentInVectorsCollection(document);
        }
        catch (Exception exception)
        {
            _logger.LogError($"Could not upsert vector: {exception.Message}");
            throw;
        }
    }

    private async Task UpsertDocumentInVectorsCollection(BsonDocument document)
    {
        var filter = Builders<BsonDocument>.Filter.Eq("_id", document["_id"]);
        var options = new ReplaceOptions { IsUpsert = true };
        await _collections[CollectionEnumerator.Vectors.Name].ReplaceOneAsync(filter, document, options);
    }
}