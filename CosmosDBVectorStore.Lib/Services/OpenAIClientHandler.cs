using Azure.Core;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;
using CosmosDBVectorStore.Lib.Interfaces;

namespace CosmosDBVectorStore.Lib.Services;

public class OpenAIClientHandler : IOpenAIClientHandler
{
    private readonly string _openAIKey;
    private readonly ILogger<OpenAIClientHandler> _logger;

    public OpenAIClientHandler(string openAIKey, ILogger<OpenAIClientHandler> logger)
    {
        _openAIKey = openAIKey;
        _logger = logger;
    }

    public OpenAIClient? CreateOpenAIClient()
    {
        try
        {
            OpenAIClientOptions clientOptions = new OpenAIClientOptions()
            {
                Retry = { Delay = TimeSpan.FromSeconds(2), MaxRetries = 10, Mode = RetryMode.Exponential }
            };
            return new OpenAIClient(_openAIKey, clientOptions);
        }
        catch (Exception exception)
        {
            _logger.LogError($"Could not instantiate Open AI client: {exception.Message}");
            return null;
        }
    }
}