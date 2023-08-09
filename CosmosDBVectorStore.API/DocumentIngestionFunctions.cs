using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;
using CosmosDBVectorStore.Lib.Interfaces;

namespace CosmosDBVectorStore.API;

public class DocumentIngestionFunctions
{
	private readonly ILogger _logger;
	private readonly IPdfIngestorService _pdfIngestorService;
	private readonly IVectorsGeneratorService _vectorsGeneratorService;

	public DocumentIngestionFunctions(IPdfIngestorService pdfIngestorService, IVectorsGeneratorService vectorsGeneratorService, ILoggerFactory loggerFactory)
	{
		_pdfIngestorService = pdfIngestorService;
		_vectorsGeneratorService = vectorsGeneratorService;
		_logger = loggerFactory.CreateLogger<DocumentIngestionFunctions>();
	}

	[Function("Ingest")]
	public async Task Run([BlobTrigger("documents/{name}.pdf", Connection = "StorageAccountConnectionString")] byte[] pdfData, string name, FunctionContext context)
	{
		try
		{
			_logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name}");

			await _pdfIngestorService.Ingest(pdfData, name);
			await _vectorsGeneratorService.VectorizeDocuments();
			
			_logger.LogInformation("Current blob has been vectorized successfully");
		}
		catch (Exception exception)
		{
			_logger.LogError("{Error}", exception.Message);
			throw;
		}
	}
}