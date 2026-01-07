namespace DeployedInAzure.EmbeddingsExamples.CosmosDbForNoSql
{
    public record CosmosDbForNoSqlDocumentModel
    {
        public required string id { get; init; }
        public required string Phrase { get; init; }
        public required List<string> Tags { get; init; } = [];
        public required float[] Vector { get; init; }
    }
}
