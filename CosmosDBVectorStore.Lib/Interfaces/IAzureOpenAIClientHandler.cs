using Azure.AI.OpenAI;

namespace CosmosDBVectorStore.Lib.Interfaces;

public interface IAzureOpenAIClientHandler
{
    OpenAIClient? CreateAzureClient();
}