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
            _logger.LogInformation("Generating vectors.");
            
            var filter = new BsonDocument();
            var collections = _mongoDbService.GetCollections();
            using (var cursor = await collections["docs"].Find(filter).ToCursorAsync())
            {
                while (await cursor.MoveNextAsync())
                {
                    var batch = cursor.Current;
                    foreach (var document in batch)
                    { 
                        var pageDocument = BsonSerializer.Deserialize<PageDocument>(document);
                        pageDocument.Vector = await _openAiService.GetEmbeddings(document.ToString());
                        await _mongoDbService.UpsertVector(pageDocument.ToBsonDocument());
                    }
                }
            }

            _logger.LogInformation($"Document vector generation has been successfully finished.");
        } 
        catch(MongoException exception)
        {
            _logger.LogError($"Could not generate vectors: {exception.Message}");
            throw;
        }
    }
}