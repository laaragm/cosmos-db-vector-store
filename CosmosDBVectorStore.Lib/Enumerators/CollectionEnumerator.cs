namespace CosmosDBVectorStore.Lib.Enumerators;

public class CollectionEnumerator
{
    public string Name { get; }
    
    public CollectionEnumerator(string name)
    {
        Name = name;
    }
    
    public static CollectionEnumerator Docs => new CollectionEnumerator("docs");
    public static CollectionEnumerator Vectors => new CollectionEnumerator("vectors");
    
    public static readonly IEnumerable<CollectionEnumerator> AllCollections = new List<CollectionEnumerator>
    {
        Docs,
        Vectors,
    };
}