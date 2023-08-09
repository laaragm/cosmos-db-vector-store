# cosmos-db-vector-store
Integrate AI-powered vector search in Cosmos DB. Store, index &amp; query high-dimensional vector data in Azure Cosmos DB for MongoDB vCore.

`CosmosDBVectorStore.API/local.settings.json`
```json
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
        "StorageAccountConnectionString": "",
        "OpenAIEndpoint": "",
        "OpenAIKey": "",
        "OpenAIEmbeddingsDeployment": "",
        "OpenAIMaxTokens": "",
        "DbConnectionString": "",
        "DbName": "",
        "DbCollectionNames": ""
    }
}
```
