using Azure.AI.OpenAI;
using Azure.Identity;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using DeployedInAzure.EmbeddingsExamples.CustomVectorDb;
using OpenAI.Embeddings;

namespace DeployedInAzure.EmbeddingsExamples.AiSearchVectorSearch
{
    public class AiSearchVectorSearchExample
    {
        private readonly EmbeddingClient _embeddingClient = new AzureOpenAIClient(
            new Uri(Environment.GetEnvironmentVariable("AZURE_OPEN_AI_CLIENT_URI")!),
            new DefaultAzureCredential())
            .GetEmbeddingClient(deploymentName: Environment.GetEnvironmentVariable("AZURE_OPEN_AI_CLIENT_DEPLOYMENT_NAME")!);

        private readonly SearchClient _searchClient = new SearchClient(
            new Uri(Environment.GetEnvironmentVariable("AZURE_AI_SEARCH_URI")!),       
            indexName: "vector-search-index",
            new DefaultAzureCredential());

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
            var documentsToBeIndexed = new List<AiSearchVectorSearchDocumentModel>();

            foreach (var item in TestData.GetAllTestData().Select((phraseAndTagPair, index) => (phraseAndTagPair, Index: index + 1)))
            {
                // this could be run in parallel if needed too using Task.WhenAll
                var response = await _embeddingClient.GenerateEmbeddingAsync(item.phraseAndTagPair.Phrase);

                var document = new AiSearchVectorSearchDocumentModel
                {
                    id = item.Index.ToString(),
                    Phrase = item.phraseAndTagPair.Phrase,
                    Vector = response.Value.ToFloats().ToArray(),
                    Tags = [item.phraseAndTagPair.Tag]
                };

                documentsToBeIndexed.Add(document);
            }

            var batch = IndexDocumentsBatch.Upload(documentsToBeIndexed);

            // if you use Visual Studio Authentication and 401 or 403 is returned even if you have 'Search Index Data Contributor' RBAC role assigned
            // make sure to set the environment variable `AZURE_TENANT_ID` to your Entra tenant ID where the Microsoft Foundry resource is deployed
            var indexDocumentsResult = await _searchClient.IndexDocumentsAsync(batch);

            Console.WriteLine($"`{indexDocumentsResult.Value.Results.Where(x => x.Succeeded).Count()}` documents were uploaded to Azure AI Search successfully!");
        }

        private async Task DisplaySimilarItemsAsync(string keyword, int topK)
        {
            var results = await FindSimilarItemsAsync(keyword, topK);

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
            var results = await FindSimilarItemsAsync(keyword, topK, tag);

            Console.WriteLine($"Top {topK} similar items to \"{keyword}\":");
            foreach (var result in results)
            {
                var tags = result.Tags is { Count: > 0 } ? string.Join(",", result.Tags) : "N/A";
                Console.WriteLine($"- [Tag:{tags}] {result.Phrase}: {result.SimilarityScore:F2}");
            }
            Console.WriteLine();
        }

        private async Task<IReadOnlyCollection<AiSearchVectorSearchResult>> FindSimilarItemsAsync(string keyword, int topK, string? tag = null)
        {
            var queryVector = (await _embeddingClient.GenerateEmbeddingAsync(keyword)).Value.ToFloats();

            var searchOptions = new SearchOptions
            {
                VectorSearch = new VectorSearchOptions
                {
                    Queries =
                    {
                        new VectorizedQuery(queryVector)
                        {
                            KNearestNeighborsCount = topK,
                            Fields = { nameof(AiSearchVectorSearchDocumentModel.Vector) }
                        },
                    },
                    //FilterMode = VectorFilterMode.PostFilter
                },
                Select =
                {
                    nameof(AiSearchVectorSearchDocumentModel.id),
                    nameof(AiSearchVectorSearchDocumentModel.Phrase),
                    nameof(AiSearchVectorSearchDocumentModel.Tags)
                }
            };

            if (!string.IsNullOrWhiteSpace(tag))
            {
                searchOptions.Filter = $"Tags/any(t: t eq '{tag}')";
            }

            var response = await _searchClient.SearchAsync<AiSearchVectorSearchDocumentModel>(searchText: null, searchOptions);

            var results = new List<AiSearchVectorSearchResult>(capacity: topK);
            await foreach (var searchResult in response.Value.GetResultsAsync())
            {
                results.Add(new AiSearchVectorSearchResult
                {
                    id = searchResult.Document.id,
                    Phrase = searchResult.Document.Phrase,
                    Tags = searchResult.Document.Tags,
                    SimilarityScore = searchResult.Score.GetValueOrDefault()
                });
            }

            return results;
        }
    }
}
