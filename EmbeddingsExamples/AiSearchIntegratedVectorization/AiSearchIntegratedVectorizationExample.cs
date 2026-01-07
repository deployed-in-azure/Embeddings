using Azure.Identity;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using DeployedInAzure.EmbeddingsExamples.AiSearchVectorSearch;

namespace DeployedInAzure.EmbeddingsExamples.AiSearchIntegratedVectorization
{
    public class AiSearchIntegratedVectorizationExample
    {
        private readonly SearchClient _searchClient = new SearchClient(
            new Uri(Environment.GetEnvironmentVariable("AZURE_AI_SEARCH_URI")!),
            indexName: "vector-search-index-with-vectorizer",
            new DefaultAzureCredential());

        private readonly List<string> _keywords = ["Mars", "Apollo 11", "Neil Armstrong", "Curiosity Rover"];

        public async Task Run()
        {
            foreach (var keyword in _keywords)
            {
                await DisplaySimilarItemsAsync(keyword, topK: 5);
            }
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

        private async Task<IReadOnlyCollection<AiSearchVectorSearchResult>> FindSimilarItemsAsync(string keyword, int topK)
        {
            var searchOptions = new SearchOptions
            {
                VectorSearch = new VectorSearchOptions
                {
                    Queries =
                    {
                        new VectorizableTextQuery(keyword)
                        {
                            KNearestNeighborsCount = topK,
                            Fields = { nameof(AiSearchVectorSearchDocumentModel.Vector) }
                        }
                    },
                },
                Select =
                {
                    nameof(AiSearchVectorSearchDocumentModel.id),
                    nameof(AiSearchVectorSearchDocumentModel.Phrase),
                    nameof(AiSearchVectorSearchDocumentModel.Tags)
                },
                Size = topK
            };

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
