namespace DeployedInAzure.EmbeddingsExamples.CosmosDbForNoSql
{
    public class CosmosDbForNoSqlVectorSearchResult
    {
        public required string id { get; init; }
        public required string Phrase { get; init; }
        public required List<string> Tags { get; init; } = [];
        public required double SimilarityScore { get; init; }
    }
}
