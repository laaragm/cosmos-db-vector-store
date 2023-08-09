namespace CosmosDBVectorStore.Lib.Interfaces;

public interface IOpenAIService
{
    Task<float[]?> GetEmbeddings(string data);
}