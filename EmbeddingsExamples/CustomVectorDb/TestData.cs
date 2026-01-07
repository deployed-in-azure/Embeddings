namespace DeployedInAzure.EmbeddingsExamples.CustomVectorDb
{
    public class TestData
    {
        public static IEnumerable<(string Tag, string Phrase)> GetAllTestData()
        {
            return MarsCluster()
                .Concat(Apollo11Cluster())
                .Concat(NeilArmstrongCluster())
                .Concat(CuriosityRoverCluster())
                .Concat(UnrelatedCluster());
        }

        public static IEnumerable<(string Tag, string Phrase)> MarsCluster()
        {
            yield return ("Mars", "Red Planet");
            yield return ("Mars", "Martian surface");
            yield return ("Mars", "Olympus Mons");
            yield return ("Mars", "Mars atmosphere");
            yield return ("Mars", "Mars exploration");
        }

        public static IEnumerable<(string Tag, string Phrase)> Apollo11Cluster()
        {
            yield return ("Apollo 11", "Moon landing mission");
            yield return ("Apollo 11", "Lunar module");
            yield return ("Apollo 11", "Saturn V rocket");
            yield return ("Apollo 11", "Sea of Tranquility");
            yield return ("Apollo 11", "NASA 1969");
        }

        public static IEnumerable<(string Tag, string Phrase)> NeilArmstrongCluster()
        {
            yield return ("Neil Armstrong", "First man on the Moon");
            yield return ("Neil Armstrong", "Astronaut");
            yield return ("Neil Armstrong", "Buzz Aldrin");
            yield return ("Neil Armstrong", "NASA astronaut corps");
            yield return ("Neil Armstrong", "Space suit");
        }

        public static IEnumerable<(string Tag, string Phrase)> CuriosityRoverCluster()
        {
            yield return ("Curiosity Rover", "Mars rover");
            yield return ("Curiosity Rover", "Gale Crater");
            yield return ("Curiosity Rover", "Mars Science Laboratory");
            yield return ("Curiosity Rover", "Rover landing");
            yield return ("Curiosity Rover", "Martian soil analysis");
        }

        public static IEnumerable<(string Tag, string Phrase)> UnrelatedCluster()
        {
            yield return ("Random", "Banana");
            yield return ("Random", "Coffee mug");
            yield return ("Random", "Jazz music");
            yield return ("Random", "Laptop charger");
            yield return ("Random", "Soccer stadium");
            yield return ("Random", "Rainforest");
            yield return ("Random", "Chocolate cake");
            yield return ("Random", "Mountain bike");
            yield return ("Random", "Smartphone app");
            yield return ("Random", "Piano lesson");
            yield return ("Random", "Traffic jam");
            yield return ("Random", "Cooking recipe");
            yield return ("Random", "Beach vacation");
            yield return ("Random", "Electric guitar");
            yield return ("Random", "Car insurance");
            yield return ("Random", "Dog training");
            yield return ("Random", "City subway");
            yield return ("Random", "Winter jacket");
            yield return ("Random", "Garden hose");
            yield return ("Random", "Yoga mat");
        }
    }
}
