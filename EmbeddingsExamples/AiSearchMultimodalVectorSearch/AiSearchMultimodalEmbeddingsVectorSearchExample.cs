using Azure.Identity;
using DeployedInAzure.EmbeddingsExamples.CustomVectorDb;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace DeployedInAzure.EmbeddingsExamples.AiSearchMultimodalVectorSearch
{
    public class AiSearchMultimodalEmbeddingsVectorSearchExample
    {
        private DeployedInAzureVectorDb _vectorDb = new(supportedVectorDimension: 1024);

        private readonly HttpClient httpClient = new()
        {
            BaseAddress = new Uri(Environment.GetEnvironmentVariable("AZURE_COMPUTER_VISION_URI")!)
        };

        private const string ENDPOINT_VECTORIZE_TEXT = "/computervision/retrieval:vectorizeText?api-version=2024-02-01&model-version=2023-04-15";
        private const string ENDPOINT_VECTORIZE_IMAGE = "/computervision/retrieval:vectorizeImage?api-version=2024-02-01&model-version=2023-04-15";

        public async Task Run()
        {
            var token = await new DefaultAzureCredential().GetTokenAsync(new Azure.Core.TokenRequestContext(["https://cognitiveservices.azure.com/.default"]));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

            var vectorizedImageAstronaut = await VectorizeImageAsync(GetFilePath("astronaut.jpg"));
            var vectorizedImageCoffee = await VectorizeImageAsync(GetFilePath("coffee.jpg"));
            var vectorizedImageMars = await VectorizeImageAsync(GetFilePath("mars.jpg"));
            var vectorizedImageMarsRover = await VectorizeImageAsync(GetFilePath("mars_rover.jpg"));

            var vectorizedTextAstronaut = await VectorizeTextAsync("An astronaut stands on the Moon. The visor reflects the lunar module. The scene captures early space exploration.");
            var vectorizedTextCoffee = await VectorizeTextAsync("A cup of latte with leaf art and a chocolate cake on a glass table. The cake is on a blue plate with nuts on top.");
            var vectorizedTextMars = await VectorizeTextAsync("Mars appears red against dark space. Craters and valleys are visible. The scene suggests planetary science.");
            var vectorizedTextMarsRover = await VectorizeTextAsync("A Mars rover sits on rocky terrain. Its wheels and arm are visible. The scene suggests exploration.");

            // Phase 1: Mars text vs each of the images
            var imageCandidates = new (string Label, float[] Vector)[]
            {
                ("Astronaut image",  vectorizedImageAstronaut),
                ("Coffee image",     vectorizedImageCoffee),
                ("Mars image",       vectorizedImageMars),
                ("Mars Rover image", vectorizedImageMarsRover),
            };

            foreach (var (label, vector) in imageCandidates)
            {
                _vectorDb.Index(VectorSearchRecord.Create(
                    id: Guid.NewGuid().ToString(),
                    vector: vector,
                    data: new Dictionary<string, string> { ["Label"] = label }
                ));
            }

            var marsTextVsImages = _vectorDb.Search(vectorizedTextMars, topK: imageCandidates.Length);

            Console.WriteLine("Mars text vs images:");
            foreach (var r in marsTextVsImages)
            {
                var label = r.Data.TryGetValue("Label", out var l) ? l : r.Id;
                Console.WriteLine($"- Mars text vs {label}: {r.Similarity:F2}");
            }
            Console.WriteLine();

            // Phase 2: Index text embeddings and compare each against Mars image
            _vectorDb = new DeployedInAzureVectorDb(supportedVectorDimension: 1024);

            var textCandidates = new (string Label, float[] Vector)[]
            {
                ("Astronaut text", vectorizedTextAstronaut),
                ("Coffee text",    vectorizedTextCoffee),
                ("Mars text",      vectorizedTextMars),
                ("Mars Rover text",vectorizedTextMarsRover),
            };

            foreach (var (label, vector) in textCandidates)
            {
                _vectorDb.Index(VectorSearchRecord.Create(
                    id: Guid.NewGuid().ToString(),
                    vector: vector,
                    data: new Dictionary<string, string> { ["Label"] = label }
                ));
            }

            var textsVsMarsImage = _vectorDb.Search(vectorizedImageMars, topK: textCandidates.Length);

            Console.WriteLine("Mars image vs texts:");
            foreach (var r in textsVsMarsImage)
            {
                var label = r.Data.TryGetValue("Label", out var l) ? l : r.Id;
                Console.WriteLine($"- Mars image vs {label}: {r.Similarity:F2}");
            }
            Console.WriteLine();

            // Phase 3: Mars text vs other texts
            _vectorDb = new DeployedInAzureVectorDb(supportedVectorDimension: 1024);

            var otherTextCandidates = new (string Label, float[] Vector)[]
            {
                ("Astronaut text", vectorizedTextAstronaut),
                ("Coffee text",    vectorizedTextCoffee),
                ("Mars Rover text",vectorizedTextMarsRover),
            };

            foreach (var (label, vector) in otherTextCandidates)
            {
                _vectorDb.Index(VectorSearchRecord.Create(
                    id: Guid.NewGuid().ToString(),
                    vector: vector,
                    data: new Dictionary<string, string> { ["Label"] = label }
                ));
            }

            var marsTextVsOtherTexts = _vectorDb.Search(vectorizedTextMars, topK: otherTextCandidates.Length);

            Console.WriteLine("Mars text vs other texts:");
            foreach (var r in marsTextVsOtherTexts)
            {
                var label = r.Data.TryGetValue("Label", out var l) ? l : r.Id;
                Console.WriteLine($"- Mars text vs {label}: {r.Similarity:F2}");
            }
            Console.WriteLine();

            // Phase 4: Mars image vs other images
            _vectorDb = new DeployedInAzureVectorDb(supportedVectorDimension: 1024);

            var otherImageCandidates = new (string Label, float[] Vector)[]
            {
                ("Astronaut image",  vectorizedImageAstronaut),
                ("Coffee image",     vectorizedImageCoffee),
                ("Mars Rover image", vectorizedImageMarsRover),
            };

            foreach (var (label, vector) in otherImageCandidates)
            {
                _vectorDb.Index(VectorSearchRecord.Create(
                    id: Guid.NewGuid().ToString(),
                    vector: vector,
                    data: new Dictionary<string, string> { ["Label"] = label }
                ));
            }

            var marsImageVsOtherImages = _vectorDb.Search(vectorizedImageMars, topK: otherImageCandidates.Length);

            Console.WriteLine("Mars image vs other images:");
            foreach (var r in marsImageVsOtherImages)
            {
                var label = r.Data.TryGetValue("Label", out var l) ? l : r.Id;
                Console.WriteLine($"- Mars image vs {label}: {r.Similarity:F2}");
            }
            Console.WriteLine();
        }

        private async Task<float[]> VectorizeTextAsync(string text)
        {
            var payload = new
            {
                text = text
            };

            var response = await httpClient.PostAsJsonAsync(ENDPOINT_VECTORIZE_TEXT, payload);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AzureComputerVisionVectorizeResult>();
            return result?.Vector ?? throw new Exception("Something went wrong");
        }

        private async Task<float[]> VectorizeImageAsync(string imagePath)
        {
            var content = new ByteArrayContent(File.ReadAllBytes(imagePath));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var response = await httpClient.PostAsync(ENDPOINT_VECTORIZE_IMAGE, content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<AzureComputerVisionVectorizeResult>();
            return result?.Vector ?? throw new Exception("Something went wrong");
        }

        private static string GetFilePath(string fileName)
        {
            return Path.Combine(AppContext.BaseDirectory, "AiSearchMultimodalVectorSearch", "SamplePhotos", fileName);
        }
    }
}
