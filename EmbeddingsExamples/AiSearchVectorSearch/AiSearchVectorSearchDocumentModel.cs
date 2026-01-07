namespace DeployedInAzure.EmbeddingsExamples.AiSearchVectorSearch
{
    public record AiSearchVectorSearchDocumentModel
    {
        public required string id { get; init; }
        public required string Phrase { get; init; }
        public List<string> Tags { get; init; } = [];
        public float[] Vector { get; init; } = [];
    }
}
