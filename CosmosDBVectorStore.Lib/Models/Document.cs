using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CosmosDBVectorStore.Lib.Models;

public class PageDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string id { get; set; }
    public string documentName { get; set; }
    public string content { get; set; }
    public float[]? vector { get; set; }

    public PageDocument(string id, string documentName, string content, float[]? vector = null)
    {
        this.id = id;
		this.documentName = documentName;
        this.content = content;
		this.vector = vector;
	}
}