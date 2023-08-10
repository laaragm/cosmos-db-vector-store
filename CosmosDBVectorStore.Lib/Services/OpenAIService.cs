using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;
using CosmosDBVectorStore.Lib.Interfaces;

namespace CosmosDBVectorStore.Lib.Services;

public class OpenAIService : IOpenAIService
{
    private readonly string _openAIEmbeddings;
    private readonly int _openAIMaxTokens;
    private readonly ILogger<OpenAIService> _logger;
    private readonly OpenAIClient? _client;

    public OpenAIService(IAppSettings appSettings, ILogger<OpenAIService> logger, IOpenAIClientFactory clientFactory)
    {
        _openAIEmbeddings = appSettings.OpenAIEmbeddingsDeployment;
        _openAIMaxTokens = int.TryParse(appSettings.OpenAIMaxTokens, out _openAIMaxTokens) ? _openAIMaxTokens : 4096;
        _logger = logger;
        _client = clientFactory.CreateClient(appSettings.OpenAIEndpoint, appSettings.OpenAIKey);
    }

    public async Task<float[]?> GetEmbeddings(string data)
    {
        if (_client == null)
        {
            _logger.LogError("OpenAI client has not been initialized.");
            return null;
        }
        
        try
        {
            EmbeddingsOptions options = new EmbeddingsOptions(data);
            var response = await _client.GetEmbeddingsAsync(_openAIEmbeddings, options);
            Embeddings embeddings = response.Value;
            float[] result = embeddings.Data[0].Embedding.ToArray();

            return result;
        }
        catch (Exception exception)
        {
            _logger.LogError($"Failed to get embeddings: {exception.Message}");
            return null;
        }
    }
}