using Azure.AI.OpenAI;

namespace CosmosDBVectorStore.Lib.Interfaces;

public interface IOpenAIClientFactory
{
    OpenAIClient? CreateClient(string endpoint, string apiKey);
}