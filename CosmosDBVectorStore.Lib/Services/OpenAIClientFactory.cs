using Azure;
using Azure.AI.OpenAI;
using Azure.Core;
using CosmosDBVectorStore.Lib.Interfaces;

namespace CosmosDBVectorStore.Lib.Services;

public class OpenAIClientFactory : IOpenAIClientFactory
{
    public OpenAIClient CreateClient(string endpoint, string apiKey)
    {
        OpenAIClientOptions clientOptions = new OpenAIClientOptions
        {
            Retry = 
            { 
                Delay = TimeSpan.FromSeconds(2), 
                MaxRetries = 10, 
                Mode = RetryMode.Exponential 
            }
        };

        if (endpoint.Contains("api.openai.com"))
        {
            return new OpenAIClient(apiKey, clientOptions);
        }

        return new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey), clientOptions);
    }
}