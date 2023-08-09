using Azure;
using Azure.Core;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;
using CosmosDBVectorStore.Lib.Interfaces;

namespace CosmosDBVectorStore.Lib.Services;

public class OpenAIService : IOpenAIService
{
    private readonly string _openAIEndpoint;
    private readonly string _openAIKey;
    private readonly string _openAIEmbeddings;
    private readonly int _openAIMaxTokens;
    private readonly ILogger<OpenAIService> _logger;
    private readonly OpenAIClient? _client;

    public OpenAIService(IAppSettings appSettings, ILogger<OpenAIService> logger)
    {
        _openAIEndpoint = appSettings.OpenAIEndpoint;
        _openAIKey = appSettings.OpenAIKey;
        _openAIEmbeddings = appSettings.OpenAIEmbeddingsDeployment;
        _openAIMaxTokens = int.TryParse(appSettings.OpenAIMaxTokens, out _openAIMaxTokens) ? _openAIMaxTokens : 4096;
        _logger = logger;
        _client = InitializeOpenAIClient();
    }

    private OpenAIClient InitializeOpenAIClient()
    {
        OpenAIClientOptions clientOptions = new OpenAIClientOptions()
        {
            Retry = { Delay = TimeSpan.FromSeconds(2), MaxRetries = 10, Mode = RetryMode.Exponential }
        };

        OpenAIClient client = null!;
        try
        {
            // Use this as endpoint in configuration to use non-Azure Open AI endpoint and OpenAI model names
            if (_openAIEndpoint.Contains("api.openai.com"))
                client = new OpenAIClient(_openAIKey, clientOptions);
            client = new(new Uri(_openAIEndpoint), new AzureKeyCredential(_openAIKey), clientOptions);
        }
        catch (Exception exception)
        {
            _logger.LogError($"Could not instantiate Open AI client: {exception.Message}");
        }

        return client;
    }

    public async Task<float[]?> GetEmbeddings(string data)
    {
        try
        {
            EmbeddingsOptions options = new EmbeddingsOptions(data);
            var response = await _client!.GetEmbeddingsAsync(_openAIEmbeddings, options);
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