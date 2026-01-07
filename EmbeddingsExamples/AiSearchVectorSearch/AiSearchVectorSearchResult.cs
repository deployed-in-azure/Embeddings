namespace DeployedInAzure.EmbeddingsExamples.AiSearchVectorSearch
{
    public record AiSearchVectorSearchResult
    {
        public string id { get; init; } = "";
        public string Phrase { get; init; } = "";
        public List<string> Tags { get; init; } = [];
        public double SimilarityScore { get; init; }
    }
}
