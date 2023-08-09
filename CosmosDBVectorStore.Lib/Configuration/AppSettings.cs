using CosmosDBVectorStore.Lib.Interfaces;

namespace CosmosDBVectorStore.Lib.Configuration;

public class AppSettings : IAppSettings
{
    public string StorageAccountConnectionString { get; }
    public string OpenAIEndpoint { get; }
    public string OpenAIKey { get; }
    public string OpenAIEmbeddingsDeployment { get; }
    public string OpenAIMaxTokens { get; }
    public string DbConnectionString { get; }
    public string DbName { get; }
    public string DbCollectionNames { get; }

    public AppSettings(string storageAccountConnectionString, string openAIEndpoint, string openAIKey, string openAIEmbeddingsDeployment, string openAIMaxTokens, string dbConnectionString, string dbName, string dbCollectionNames)
    {
        StorageAccountConnectionString = storageAccountConnectionString;
        OpenAIEndpoint = openAIEndpoint;
        OpenAIKey = openAIKey;
        OpenAIEmbeddingsDeployment = openAIEmbeddingsDeployment;
        OpenAIMaxTokens = openAIMaxTokens;
        DbConnectionString = dbConnectionString;
        DbName = dbName;
        DbCollectionNames = dbCollectionNames;
    }
}