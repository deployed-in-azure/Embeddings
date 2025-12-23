using DeployedInAzure.EmbeddingsExamples.Attributes;
using DeployedInAzure.EmbeddingsExamples.BinaryVectors;
using DeployedInAzure.EmbeddingsExamples.EmbeddingModel;
using DeployedInAzure.EmbeddingsExamples.SoftLabelEncoding;

namespace DeployedInAzure.EmbeddingsExamples
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Introduction to embeddings: Capture the meaning of data
            // ---
            new AttributesExample().Run();
            //new BinaryVectorsExample().Run();
            //new SoftLabelEncodingExample().Run();
            //await new EmbeddingModelExample().Run();
            // ---

            Console.ReadKey();
        }
    }
}
