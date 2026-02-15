using DeployedInAzure.EmbeddingsExamples.AiSearchIntegratedVectorization;
using DeployedInAzure.EmbeddingsExamples.AiSearchMultimodalVectorSearch;
using DeployedInAzure.EmbeddingsExamples.AiSearchVectorSearch;
using DeployedInAzure.EmbeddingsExamples.AiSearchVectorSearchUsingVectorizer;
using DeployedInAzure.EmbeddingsExamples.Attributes;
using DeployedInAzure.EmbeddingsExamples.BinaryVectors;
using DeployedInAzure.EmbeddingsExamples.CosmosDbForNoSql;
using DeployedInAzure.EmbeddingsExamples.CustomVectorDb;
using DeployedInAzure.EmbeddingsExamples.CustomVectorDbFaiss;
using DeployedInAzure.EmbeddingsExamples.EmbeddingModel;
using DeployedInAzure.EmbeddingsExamples.SoftLabelEncoding;

namespace DeployedInAzure.EmbeddingsExamples
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // --- Introduction to embeddings: Capture the meaning of data ---
            //new AttributesExample().Run();
            //new BinaryVectorsExample().Run();
            //new SoftLabelEncodingExample().Run();
            //await new EmbeddingModelExample().Run();
            // ---


            // --- Vector Databases Explained: Powering AI Applications at Scale ---
            //await new CustomVectorDbExample().Run();
            //await new CustomVectorDbFaissExample().Run();
            // ---


            // --- Vector Search in Azure Cosmos DB for NoSQL: A Practical Guide ---
            //await new CosmosDbForNoSqlExample().Run();
            // ---


            // --- Vector Search in Azure AI Search: A Practical Guide ---
            //await new AiSearchVectorSearchExample().Run();
            // ---


            // --- Vectorizers in Azure AI Search: 5 Key Insights You Must Know ---
            //await new AiSearchVectorSearchUsingVectorizerExample().Run();
            // ---


            // --- Integrated Vectorization in Azure AI Search: How to Automate Embeddings ---
            //await new AiSearchIntegratedVectorizationExample().Run();
            // ---

            // --- Multimodal Vector Search in Azure AI Search: Combining Text and Images ---
            //await new AiSearchMultimodalEmbeddingsVectorSearchExample().Run();

            Console.ReadKey();
        }
    }
}
