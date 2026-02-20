using Azure.AI.OpenAI;
using Azure.Identity;
using DeployedInAzure.EmbeddingsExamples.CustomVectorDb;
using OpenAI.Chat;
using OpenAI.Embeddings;

namespace DeployedInAzure.EmbeddingsExamples.ImageVerbalizationViaLLMs
{
    public class ImageVerbalizationViaLLMsExample
    {
        private const int EMBEDDING_VECTOR_DIMENSION = 1536;

        private DeployedInAzureVectorDb _vectorDb = new(supportedVectorDimension: EMBEDDING_VECTOR_DIMENSION);

        private readonly EmbeddingClient _embeddingClient;
        private readonly ChatClient _chatClient;

        public ImageVerbalizationViaLLMsExample()
        {
            var openAiClient = new AzureOpenAIClient(
                new Uri(Environment.GetEnvironmentVariable("AZURE_OPEN_AI_CLIENT_URI")!),
                new DefaultAzureCredential());

            _embeddingClient = openAiClient.GetEmbeddingClient(Environment.GetEnvironmentVariable("AZURE_OPEN_AI_EMBEDDING_CLIENT_DEPLOYMENT_NAME")!);
            _chatClient = openAiClient.GetChatClient(Environment.GetEnvironmentVariable("AZURE_OPEN_AI_EMBEDDING_CHAT_CLIENT_DEPLOYMENT_NAME")!);
        }

        public async Task Run()
        {
            var astronautImageText = await VerbalizeImageAsync(GetFilePath("astronaut.jpg"));
            var coffeeImageText = await VerbalizeImageAsync(GetFilePath("coffee.jpg"));
            var marsImageText = await VerbalizeImageAsync(GetFilePath("mars.jpg"));
            var marsRoverImageText = await VerbalizeImageAsync(GetFilePath("mars_rover.jpg"));

            var verbalizedImageAstronaut = await GetTextEmbeddingAsync(astronautImageText);
            var verbalizedImageCoffee = await GetTextEmbeddingAsync(coffeeImageText);
            var verbalizedImageMars = await GetTextEmbeddingAsync(marsImageText);
            var verbalizedImageMarsRover = await GetTextEmbeddingAsync(marsRoverImageText);

            var verbalizedImages = new (string Label, float[] Vector)[]
            {
                ("(Verbalized) Astronaut image",  verbalizedImageAstronaut),
                ("(Verbalized) Coffee image",     verbalizedImageCoffee),
                ("(Verbalized) Mars image",       verbalizedImageMars),
                ("(Verbalized) Mars Rover image", verbalizedImageMarsRover),
            };

            // index verbalized images
            foreach (var (label, vector) in verbalizedImages)
            {
                _vectorDb.Index(VectorSearchRecord.Create(
                    id: Guid.NewGuid().ToString(),
                    vector: vector,
                    data: new Dictionary<string, string> { ["Label"] = label }
                ));
            }

            var astronautText = "astronaut on the moon";
            var coffeeText = "latte and cake outside";
            var marsText = "planet mars from space";
            var marsRoverText = "mars rover on rocky surface";

            var vectorizedTextAstronaut = await GetTextEmbeddingAsync(astronautText);
            var vectorizedTextCoffee = await GetTextEmbeddingAsync(coffeeText);
            var vectorizedTextMars = await GetTextEmbeddingAsync(marsText);
            var vectorizedTextMarsRover = await GetTextEmbeddingAsync(marsRoverText);

            var texts = new (string Label, float[] Vector)[]
            {
                ("(Text) Astronaut",  vectorizedTextAstronaut),
                ("(Text) Coffee",     vectorizedTextCoffee),
                ("(Text) Mars",       vectorizedTextMars),
                ("(Text) Mars Rover", vectorizedTextMarsRover),
            };

            foreach (var text in texts)
            {
                var textVsImages = _vectorDb.Search(text.Vector, topK: verbalizedImages.Length);

                Console.WriteLine($"{text.Label} vs images (image verbalization):");

                foreach (var r in textVsImages)
                {
                    var label = r.Data.TryGetValue("Label", out var l) ? l : r.Id;
                    Console.WriteLine($"- {text.Label} vs {label}: {r.Similarity:F2}");
                }

                Console.WriteLine();
            }
        }

        private async Task<string> VerbalizeImageAsync(string imagePath)
        {
            byte[] imageBytes = await File.ReadAllBytesAsync(imagePath);

            var messageParts = new List<ChatMessageContentPart>
            {
                ChatMessageContentPart.CreateTextPart("Describe this image."),
                ChatMessageContentPart.CreateImagePart(BinaryData.FromBytes(imageBytes), "image/jpeg")
            };

            var chatMessages = new List<ChatMessage>
            {
                new SystemChatMessage(GetSystemPromptText()),
                new UserChatMessage(messageParts)
            };

            ChatCompletion completion = await _chatClient.CompleteChatAsync(chatMessages);

            var text = completion.Content.FirstOrDefault()?.Text ?? string.Empty;
            Console.WriteLine($"Image verbalized ({Path.GetFileName(imagePath)}): {text}\n");

            return text;
        }

        private async Task<float[]> GetTextEmbeddingAsync(string text)
        {
            var response = await _embeddingClient.GenerateEmbeddingAsync(text);
            return response.Value.ToFloats().ToArray();
        }

        private static string GetFilePath(string fileName)
        {
            return Path.Combine(AppContext.BaseDirectory, "MultimodalVectorSearchWithAzureVision", "SamplePhotos", fileName);
        }

        private static string GetSystemPromptText()
        {
            return """
            You are an Image Verbalization assistant; your goal is to translate visual content into descriptive, searchable text.

            Instructions:
            - Identify the Subject: State clearly what is in the image.
            - Identify Context: Mention the setting or significant background elements.
            - Be Literal, Not Stylistic: Focus on 'what' is in the frame, not 'how' it looks. Ignore artistic style, lighting, or camera angles.
            - Search-Optimized: Use standard terminology that a user would likely type into a search engine.

            Output Format:
            - Provide a single, detailed paragraph (2-3 sentences) that captures the essence of the image for a vector search index.
            """;
        }
    }
}
