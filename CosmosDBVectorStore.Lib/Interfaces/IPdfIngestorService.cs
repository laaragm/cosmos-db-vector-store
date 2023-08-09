namespace CosmosDBVectorStore.Lib.Interfaces;

public interface IPdfIngestorService
{
    Task Ingest(byte[] pdfData, string blobName);
}