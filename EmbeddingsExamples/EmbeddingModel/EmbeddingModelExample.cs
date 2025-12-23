using Azure.AI.OpenAI;
using Azure.Identity;
using OpenAI.Embeddings;
using System.Numerics.Tensors;

namespace DeployedInAzure.EmbeddingsExamples.EmbeddingModel
{
    public class EmbeddingModelExample
    {
        private Dictionary<string, float[]> _items = new()
        {
            ["Mars"] = [],
            ["Apollo 11"] = [], 
            ["Neil Armstrong"] = [],
            ["Curiosity Rover"] = []
        };

        private readonly EmbeddingClient _embeddingClient = new AzureOpenAIClient(
            new Uri("replaceItWithUriToYourMicrosoftFoundryService"), 
            new DefaultAzureCredential())
            .GetEmbeddingClient(deploymentName: "replaceItWithASpecificDeploymentName");

        public async Task Run()
        {
            foreach (var keyword in _items.Keys)
            {
                // if you use Visual Studio Authentication and 401 is returned even if you have 'Azure AI User' RBAC role assigned
                // make sure to set the environment variable `AZURE_TENANT_ID` to your Entra tenant ID where the Microsoft Foundry resource is deployed
                var response = await _embeddingClient.GenerateEmbeddingAsync(keyword);
                _items[keyword] = response.Value.ToFloats().ToArray();
            }

            DisplaySimilarItems("Mars");
            DisplaySimilarItems("Apollo 11");
            DisplaySimilarItems("Neil Armstrong");
            DisplaySimilarItems("Curiosity Rover");

            await DisplayOrderedKeywordsForSentenceAsync("the planet next to Earth but not Venus");
            await DisplayOrderedKeywordsForSentenceAsync("the first mission that carried humans to the Moon");
            await DisplayOrderedKeywordsForSentenceAsync("the first person to step onto the lunar surface");
            await DisplayOrderedKeywordsForSentenceAsync("the NASA robot exploring Mars since 2012");
        }

        private void DisplaySimilarItems(string keyword)
        {
            var results = GetTheMostSimilarItems(keyword);

            Console.WriteLine($"Similar items to \"{keyword}\" (vector length: {_items[keyword].Length}):");
            foreach (var result in results)
            {
                Console.WriteLine($"- {result.Keyword}: {result.Similarity:F2}");
            }
            Console.WriteLine();
        }

        private IReadOnlyCollection<(string Keyword, double Similarity)> GetTheMostSimilarItems(string searchKeyword)
        {
            var searchVector = _items[searchKeyword];

            return _items
                .Where(kvp => !string.Equals(kvp.Key, searchKeyword, StringComparison.OrdinalIgnoreCase))
                .Select(kvp =>
                {
                    var otherVector = kvp.Value;
                    var cosine = TensorPrimitives.CosineSimilarity(searchVector.AsSpan(), otherVector.AsSpan());

                    return (Keyword: kvp.Key, Similarity: Math.Round(cosine, 2));
                })
                .OrderByDescending(x => x.Similarity)
                .ToList();
        }

        private async Task DisplayOrderedKeywordsForSentenceAsync(string sentence)
        {
            var response = await _embeddingClient.GenerateEmbeddingAsync(sentence);
            var sentenceVector = response.Value.ToFloats().ToArray();

            var ordered = _items
                .Select(kvp =>
                {
                    var otherVector = kvp.Value;
                    var cosine = TensorPrimitives.CosineSimilarity(sentenceVector.AsSpan(), otherVector.AsSpan());
                    return (Keyword: kvp.Key, Similarity: Math.Round(cosine, 2));
                })
                .OrderByDescending(x => x.Similarity)
                .ToList();

            Console.WriteLine($"Sentence: \"{sentence}\"");
            foreach (var item in ordered)
            {
                Console.WriteLine($"- {item.Keyword}: {item.Similarity:F2}");
            }
            Console.WriteLine();
        }
    }
}
