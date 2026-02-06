using Azure.AI.OpenAI;
using Azure.Identity;
using DeployedInAzure.EmbeddingsExamples.CustomVectorDb;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using OpenAI.Embeddings;

namespace DeployedInAzure.EmbeddingsExamples.CosmosDbForNoSql
{
    public class CosmosDbForNoSqlExample
    {
        private readonly EmbeddingClient _embeddingClient = new AzureOpenAIClient(
            new Uri(Environment.GetEnvironmentVariable("AZURE_OPEN_AI_CLIENT_URI")!),
            new DefaultAzureCredential())
            .GetEmbeddingClient(deploymentName: Environment.GetEnvironmentVariable("AZURE_OPEN_AI_CLIENT_DEPLOYMENT_NAME")!);

        private readonly CosmosClient _cosmosClient = new(Environment.GetEnvironmentVariable("AZURE_COSMOS_DB_URI")!, new DefaultAzureCredential());

        private Container VectorSearchContainer => _cosmosClient
            .GetDatabase(Environment.GetEnvironmentVariable("AZURE_COSMOS_DB_DATABASE")!)
            .GetContainer(Environment.GetEnvironmentVariable("AZURE_COSMOS_DB_CONTAINER")!);

        private readonly List<string> _keywords = ["Mars", "Apollo 11", "Neil Armstrong", "Curiosity Rover"];

        public async Task Run()
        {
            await UpsertSampleDocumentsAsync();

            foreach (var keyword in _keywords)
            {
                await DisplaySimilarItemsAsync(keyword, topK: 5);
                await DisplaySimilarItemsAsync(keyword, topK: 5, tag: keyword);
            }
        }

        private async Task UpsertSampleDocumentsAsync()
        {
            var documentsToBeIndexed = new List<CosmosDbForNoSqlDocumentModel>();

            foreach (var item in TestData.GetAllTestData().Select((phraseAndTagPair, index) => (phraseAndTagPair, Index: index + 1)))
            {
                // this could be run in parallel if needed too using Task.WhenAll
                var response = await _embeddingClient.GenerateEmbeddingAsync(item.phraseAndTagPair.Phrase);

                var document = new CosmosDbForNoSqlDocumentModel()
                {
                    id = item.Index.ToString(),
                    Phrase = item.phraseAndTagPair.Phrase,
                    Vector = response.Value.ToFloats().ToArray(),
                    ShardKey = item.phraseAndTagPair.Tag,
                    Tags = [item.phraseAndTagPair.Tag]
                };

                documentsToBeIndexed.Add(document);
            }

            var container = VectorSearchContainer;

            // if you use Visual Studio Authentication and 401 or 403 is returned even if you have 'Cosmos DB Built‑in Data Contributor' RBAC role assigned
            // make sure to set the environment variable `AZURE_TENANT_ID` to your Entra tenant ID where the Microsoft Foundry resource is deployed
            await Task.WhenAll(documentsToBeIndexed.Select(document => container.UpsertItemAsync(document, new PartitionKey(document.id))));

            Console.WriteLine($"`{documentsToBeIndexed.Count}` documents were usperted to Cosmos DB successfully!");
        }

        private async Task DisplaySimilarItemsAsync(string keyword, int topK)
        {
            var response = await _embeddingClient.GenerateEmbeddingAsync(keyword);
            var queryVector = response.Value.ToFloats().ToArray();

            var results = await SearchAsync(queryVector, topK);

            Console.WriteLine($"Top {topK} similar items to \"{keyword}\":");
            foreach (var result in results)
            {
                var tags = result.Tags is { Count: > 0 } ? string.Join(",", result.Tags) : "N/A";
                Console.WriteLine($"- [Tag:{tags}] {result.Phrase}: {result.SimilarityScore:F2}");
            }
            Console.WriteLine();
        }

        private async Task DisplaySimilarItemsAsync(string keyword, int topK, string tag)
        {
            var response = await _embeddingClient.GenerateEmbeddingAsync(keyword);
            var queryVector = response.Value.ToFloats().ToArray();

            var results = await SearchAsync(queryVector, topK, tag);

            Console.WriteLine($"Top {topK} similar items to \"{keyword}\" with the Tags filter applied:");
            foreach (var result in results)
            {
                var tags = result.Tags is { Count: > 0 } ? string.Join(",", result.Tags) : "N/A";
                Console.WriteLine($"- [Tag:{tags}] {result.Phrase}: {result.SimilarityScore:F2}");
            }
            Console.WriteLine();
        }

        public async Task<IReadOnlyCollection<CosmosDbForNoSqlVectorSearchResult>> SearchAsync(float[] queryVector, int topK)
        {
            var queryable = VectorSearchContainer.GetItemLinqQueryable<CosmosDbForNoSqlDocumentModel>()
                .OrderByRank(x => CosmosLinqExtensions.VectorDistance(x.Vector, queryVector, false, options: null))
                .Select(x => new CosmosDbForNoSqlVectorSearchResult()
                {
                    id = x.id,
                    Phrase = x.Phrase,
                    Tags = x.Tags,
                    SimilarityScore = CosmosLinqExtensions.VectorDistance(x.Vector, queryVector, false, options: null)
                })
                .Take(topK);

            var results = new List<CosmosDbForNoSqlVectorSearchResult>();
            using var iterator = queryable.ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                var partialResponse = await iterator.ReadNextAsync();
                results.AddRange(partialResponse);
            }
    
            return results;
        }

        public async Task<IReadOnlyCollection<CosmosDbForNoSqlVectorSearchResult>> SearchAsync(float[] queryVector, int topK, string tag)
        {
            var sql = """
                SELECT TOP @topK
                    c.id,
                    c.Phrase,
                    c.Tags,
                    VectorDistance(c.Vector, @queryVector) AS SimilarityScore
                FROM c
                WHERE ARRAY_CONTAINS(c.Tags, @tag)
                ORDER BY VectorDistance(c.Vector, @queryVector)
                """;

            var query = new QueryDefinition(sql)
                .WithParameter("@topK", topK)
                .WithParameter("@queryVector", queryVector)
                .WithParameter("@tag", tag);

            var results = new List<CosmosDbForNoSqlVectorSearchResult>();

            using var iterator = VectorSearchContainer.GetItemQueryIterator<CosmosDbForNoSqlVectorSearchResult>(query);
            while (iterator.HasMoreResults)
            {
                var partialResponse = await iterator.ReadNextAsync();
                results.AddRange(partialResponse);
            }

            return results;
        }
    }
}
