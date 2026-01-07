namespace DeployedInAzure.EmbeddingsExamples.SoftLabelEncoding
{
    public class SoftLabelEncodingExample
    {
        // [Planet, Mission, Astronaut]
        private Dictionary<string, double[]> _items = new()
        {
            ["Mars"] =              [1.0, 0.2, 0.0],   // Mostly a planet, slightly related to missions
            ["Apollo 11"] =         [0.0, 1.0, 0.9],   // A mission, strongly tied to astronauts
            ["Neil Armstrong"] =    [0.0, 0.8, 1.0],   // An astronaut, strongly tied to missions
            ["Curiosity Rover"] =   [0.9, 1.0, 0.0]    // A rover on a planet, fully a mission asset
        };

        public void Run()
        {
            DisplaySimilarItems("Mars");
            DisplaySimilarItems("Apollo 11");
            DisplaySimilarItems("Neil Armstrong");
            DisplaySimilarItems("Curiosity Rover");
        }

        private void DisplaySimilarItems(string keyword)
        {
            var results = GetTheMostSimilarItems(keyword);

            Console.WriteLine($"Similar items to \"{keyword}\":");
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

                    var dotProduct = 0.0;
                    var sumSquaresA = 0.0;
                    var sumSquaresB = 0.0;

                    for (var i = 0; i < searchVector.Length; i++)
                    {
                        var a = searchVector[i];
                        var b = otherVector[i];
                        dotProduct += a * b;
                        sumSquaresA += a * a;
                        sumSquaresB += b * b;
                    }

                    var vectorALength = Math.Sqrt(sumSquaresA);
                    var vectorBLength = Math.Sqrt(sumSquaresB);
                    var denominator = vectorALength * vectorBLength;
                    var cosine = Math.Round(denominator == 0.0 ? 0.0 : dotProduct / denominator, 2);

                    return (Keyword: kvp.Key, Similarity: cosine);
                })
                .OrderByDescending(x => x.Similarity)
                .ToList();
        }
    }
}
