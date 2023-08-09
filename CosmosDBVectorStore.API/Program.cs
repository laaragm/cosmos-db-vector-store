using CosmosDBVectorStore.Lib;
using CosmosDBVectorStore.Lib.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CosmosDBVectorStore.Lib.Interfaces;

IAppSettings appSettings = null!;
var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration(builder =>
    {
        builder
            .AddJsonFile("local.settings.json", true, true)
            .AddEnvironmentVariables();
        var config = builder.Build();
        appSettings = new AppSettings(
            config["StorageAccountConnectionString"], 
            config["OpenAIEndpoint"], 
            config["OpenAIKey"],
            config["OpenAIEmbeddingsDeployment"], 
            config["OpenAIMaxTokens"], 
            config["DbConnectionString"],
            config["DbName"], 
            config["DbCollectionNames"]);
    })
    .ConfigureServices(services =>
    {
        services.AddLogging();
        services.AddScoped(x => appSettings);
        services.AddApplication();
    })
    .Build();

host.Run();