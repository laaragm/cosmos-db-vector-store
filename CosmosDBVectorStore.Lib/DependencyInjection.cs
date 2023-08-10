using CosmosDBVectorStore.Lib.Services;
using CosmosDBVectorStore.Lib.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CosmosDBVectorStore.Lib;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IMongoDbService, MongoDbService>();
        services.AddScoped<IOpenAIService, OpenAIService>();
        services.AddScoped<IPdfIngestorService, PdfIngestorService>();
        services.AddScoped<IVectorsGeneratorService, VectorsGeneratorService>();
        services.AddScoped<IAzureOpenAIClientHandler, AzureOpenAIClientHandler>();
        services.AddScoped<IOpenAIClientHandler, OpenAIClientHandler>();
        services.AddScoped<IOpenAIClientFactory, OpenAIClientFactory>();

        return services;
    }
}