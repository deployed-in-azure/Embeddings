namespace DeployedInAzure.EmbeddingsExamples.Attributes
{
    public class AttributesExample
    {
        private Dictionary<string, string[]> _items = new()
        {
            ["Mars"] =              ["Planet"],
            ["Apollo 11"] =         ["Mission", "Astronaut"],
            ["Neil Armstrong"] =    ["Mission", "Astronaut"],
            ["Curiosity Rover"] =   ["Planet", "Mission"]
        };

        private const int NUMBER_OF_ATTRIBUTES = 3;

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
            var searchKeywordAttributes = _items[searchKeyword];

            return _items
                .Where(kvp => !string.Equals(kvp.Key, searchKeyword, StringComparison.OrdinalIgnoreCase))
                .Select(kvp =>
                {
                    var otherKeywordAttributes = kvp.Value;
                    var intersectionCount = searchKeywordAttributes.Intersect(otherKeywordAttributes).Count();

                    var percentage = (int)Math.Round(intersectionCount * 100.0 / NUMBER_OF_ATTRIBUTES);

                    return (Keyword: kvp.Key, Similarity: percentage);
                })
                .OrderByDescending(x => x.Similarity)
                .ToList();
        }
    }
}
