namespace DeployedInAzure.EmbeddingsExamples.BinaryVectors
{
    public class BinaryVectorsExample
    {
        // [Planet, Mission, Astronaut]
        private Dictionary<string, bool[]> _items = new()
        {
            ["Mars"] =              [true,  false, false],
            ["Apollo 11"] =         [false, true,  true ],
            ["Neil Armstrong"] =    [false, true,  true ],
            ["Curiosity Rover"] =   [true,  true,  false]
        };

        private const int NUMBER_OF_DIMENSIONS = 3;

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
                Console.WriteLine($"- {result.Keyword}: {result.Similarity}%");
            }
            Console.WriteLine();
        }

        private IReadOnlyCollection<(string Keyword, int Similarity)> GetTheMostSimilarItems(string searchKeyword)
        {
            var searchVector = _items[searchKeyword];

            return _items
                .Where(kvp => !string.Equals(kvp.Key, searchKeyword, StringComparison.OrdinalIgnoreCase))
                .Select(kvp =>
                {
                    var otherVector = kvp.Value;

                    var intersectionCount = 0;
                    for (var i = 0; i < NUMBER_OF_DIMENSIONS; i++)
                    {
                        if (searchVector[i] && otherVector[i])
                        {
                            intersectionCount++;
                        }
                    }

                    var percentage = (int)Math.Round(intersectionCount * 100.0 / NUMBER_OF_DIMENSIONS);

                    return (Keyword: kvp.Key, Similarity: percentage);
                })
                .OrderByDescending(x => x.Similarity)
                .ToList();
        }
    }
}
