namespace CosmosDBVectorStore.Lib.Interfaces;

public interface IVectorsGeneratorService
{
    Task VectorizeDocuments();
}