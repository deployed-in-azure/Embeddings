using Azure.AI.OpenAI;
using Azure.Identity;
using DeployedInAzure.EmbeddingsExamples.CustomVectorDb;
using OpenAI.Embeddings;

namespace DeployedInAzure.EmbeddingsExamples.CustomVectorDbFaiss
{
    public class CustomVectorDbFaissExample
    {
        private readonly DeployedInAzureVectorDbFaiss _vectorDb = new();

        private readonly EmbeddingClient _embeddingClient = new AzureOpenAIClient(
            new Uri(Environment.GetEnvironmentVariable("AZURE_OPEN_AI_CLIENT_URI")!),
            new DefaultAzureCredential())
            .GetEmbeddingClient(deploymentName: Environment.GetEnvironmentVariable("AZURE_OPEN_AI_CLIENT_DEPLOYMENT_NAME")!);

        private List<string> _keywords = ["Mars", "Apollo 11", "Neil Armstrong", "Curiosity Rover"];

        public async Task Run()
        {
            var recordsToBeIndexed = new List<VectorSearchRecord>();

            foreach (var item in TestData.GetAllTestData().Select((phraseAndTagPair, index) => (phraseAndTagPair, Index: index + 1)))
            {
                var response = await _embeddingClient.GenerateEmbeddingAsync(item.phraseAndTagPair.Phrase);

                var vectorSearchRecord = new VectorSearchRecord()
                {
                    Id = item.Index.ToString(),
                    Vector = response.Value.ToFloats().ToArray(),
                    Data = new Dictionary<string, string>()
                    {
                        { "Tag", item.phraseAndTagPair.Tag },
                        { "Phrase", item.phraseAndTagPair.Phrase }
                    }
                };

                recordsToBeIndexed.Add(vectorSearchRecord);
            }

            _vectorDb.Index(recordsToBeIndexed);

            foreach (var keyword in _keywords)
            {
                await DisplaySimilarItemsAsync(keyword, topK: 5);
            }
        }

        private async Task DisplaySimilarItemsAsync(string keyword, int topK)
        {
            var response = await _embeddingClient.GenerateEmbeddingAsync(keyword);
            var queryVector = response.Value.ToFloats().ToArray();

            var results = _vectorDb.Search(queryVector, topK);

            Console.WriteLine($"Top {topK} similar items to \"{keyword}\":");
            foreach (var result in results)
            {
                var tag = result.Data.TryGetValue("Tag", out var t) ? t : "N/A";
                var itemPhrase = result.Data.TryGetValue("Phrase", out var k) ? k : result.Id;
                Console.WriteLine($"- [Tag:{tag}] {itemPhrase}: {result.Similarity:F2}");
            }
            Console.WriteLine();
        }
    }
}
