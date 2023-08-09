namespace CosmosDBVectorStore.Lib.Interfaces;

public interface IAppSettings
{
    string StorageAccountConnectionString { get; }
    string OpenAIEndpoint { get; }
    string OpenAIKey { get; }
    string OpenAIEmbeddingsDeployment { get; }
    string OpenAIMaxTokens { get; }
    string DbConnectionString { get; }
    string DbName { get; }
    string DbCollectionNames { get; }
}