namespace DeployedInAzure.EmbeddingsExamples.CustomVectorDb
{
    public record VectorSearchRecord
    {
        public required string Id { get; init; }
        public required IReadOnlyDictionary<string, string> Data { get; init; }
        public required float[] Vector { get; init; }

        public static VectorSearchRecord Create(string id, float[] vector, IReadOnlyDictionary<string, string> data)
        {
            return new VectorSearchRecord
            {
                Id = id,
                Vector = vector,
                Data = data
            };
        }
    }
}
