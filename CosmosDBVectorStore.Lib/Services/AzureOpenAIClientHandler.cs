using Azure;
using Azure.Core;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;
using CosmosDBVectorStore.Lib.Interfaces;

namespace CosmosDBVectorStore.Lib.Services;

public class AzureOpenAIClientHandler : IAzureOpenAIClientHandler
{
    private readonly string _openAIEndpoint;
    private readonly string _openAIKey;
    private readonly ILogger<AzureOpenAIClientHandler> _logger;

    public AzureOpenAIClientHandler(string openAIEndpoint, string openAIKey, ILogger<AzureOpenAIClientHandler> logger)
    {
        _openAIEndpoint = openAIEndpoint;
        _openAIKey = openAIKey;
        _logger = logger;
    }

    public OpenAIClient? CreateAzureClient()
    {
        try
        {
            OpenAIClientOptions clientOptions = new OpenAIClientOptions()
            {
                Retry = { Delay = TimeSpan.FromSeconds(2), MaxRetries = 10, Mode = RetryMode.Exponential }
            };
            return new OpenAIClient(new Uri(_openAIEndpoint), new AzureKeyCredential(_openAIKey), clientOptions);
        }
        catch (Exception exception)
        {
            _logger.LogError($"Could not instantiate Azure client: {exception.Message}");
            return null;
        }
    }
}