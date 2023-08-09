using Newtonsoft.Json;
using UglyToad.PdfPig;
using Microsoft.Extensions.Logging;
using CosmosDBVectorStore.Lib.Interfaces;

namespace CosmosDBVectorStore.Lib.Services;

public class PdfIngestorService : IPdfIngestorService
{
	private readonly ILogger<PdfIngestorService> _logger;
	private readonly IMongoDbService _mongoDbService;
	
	public PdfIngestorService(IMongoDbService mongoDbService, ILogger<PdfIngestorService> logger)
	{
		_mongoDbService = mongoDbService;
		_logger = logger;
	}
	
	public async Task Ingest(byte[] pdfData, string blobName)
	{
		try
		{
			_logger.LogInformation("Ingesting document.");

			// PDF text extraction
			var pagesList = new List<Dictionary<string, string>>();
			using (PdfDocument document = PdfDocument.Open(pdfData))
			{
				for (int i = 0; i < document.NumberOfPages; i++)
				{
					pagesList.Add(new Dictionary<string, string>
					{
						{ "DocumentName", blobName },
						{ "Content", document.GetPage(i + 1).Text }
					});
				}
			}

			// Convert to JSON string
			var jsonString = JsonConvert.SerializeObject(pagesList);
			await _mongoDbService.ImportJson("docs", jsonString);

			_logger.LogInformation($"Data ingestion has been completed for blob: {blobName}");
		}
		catch (Exception ex)
		{
			_logger.LogError($"Could not ingest data: {ex.Message}");
			throw;
		}
	}
}