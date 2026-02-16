namespace DeployedInAzure.EmbeddingsExamples.AiSearchMultimodalVectorSearch
{
    public record AzureComputerVisionVectorizeResult
    {
        public float[] Vector { get; init; } = [];
        public string ModelVersion { get; init; } = "";
    }
}
