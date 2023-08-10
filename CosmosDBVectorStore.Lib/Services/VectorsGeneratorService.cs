using CosmosDBVectorStore.Lib.Enumerators;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using Microsoft.Extensions.Logging;
using CosmosDBVectorStore.Lib.Models;
using CosmosDBVectorStore.Lib.Interfaces;

namespace CosmosDBVectorStore.Lib.Services;

public class VectorsGeneratorService : IVectorsGeneratorService
{
    private readonly ILogger<VectorsGeneratorService> _logger;
    private readonly IMongoDbService _mongoDbService;
    private readonly IOpenAIService _openAiService;
    
    public VectorsGeneratorService(IMongoDbService mongoDbService, IOpenAIService openAiService, ILogger<VectorsGeneratorService> logger)
    {
        _mongoDbService = mongoDbService;
        _openAiService = openAiService;
        _logger = logger;
    }

    public async Task VectorizeDocuments()
    {
        try
        {
            _logger.LogInformation("Starting vector generation.");

            var documents = await RetrieveAllDocuments();
            foreach (var document in documents)
            {
                var vector = await ConvertDocumentToVector(document);
                await StoreVectorInDatabase(document, vector);
            }

            _logger.LogInformation("Vector generation completed successfully.");
        }
        catch (MongoException exception)
        {
            _logger.LogError($"Error during vector generation: {exception.Message}");
            throw;
        }
    }

    private async Task<IEnumerable<BsonDocument>> RetrieveAllDocuments()
    {
        var filter = new BsonDocument();
        var collections = _mongoDbService.GetCollections();
        var documents = new List<BsonDocument>();

        using var cursor = await collections[CollectionEnumerator.Docs.Name].Find(filter).ToCursorAsync();
        while (await cursor.MoveNextAsync())
            documents.AddRange(cursor.Current);

        return documents;
    }

    private async Task<float[]?> ConvertDocumentToVector(BsonDocument document)
    {
        return await _openAiService.GetEmbeddings(document.ToString());
    }

    private async Task StoreVectorInDatabase(BsonDocument document, float[]? vector)
    {
        var pageDocument = BsonSerializer.Deserialize<PageDocument>(document);
        pageDocument.Vector = vector;
        await _mongoDbService.UpsertVector(pageDocument.ToBsonDocument());
    }
}