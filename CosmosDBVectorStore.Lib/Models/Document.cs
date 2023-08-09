using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CosmosDBVectorStore.Lib.Models;

public class PageDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string DocumentName { get; set; }
    public string Content { get; set; }
    public float[]? Vector { get; set; }

    public PageDocument(string id, string documentName, string content, float[]? vector = null)
    {
        Id = id;
		DocumentName = documentName;
        Content = content;
        Vector = vector;
    }
}