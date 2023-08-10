using Azure.AI.OpenAI;

namespace CosmosDBVectorStore.Lib.Interfaces;

public interface IOpenAIClientHandler
{
    OpenAIClient? CreateOpenAIClient();
}