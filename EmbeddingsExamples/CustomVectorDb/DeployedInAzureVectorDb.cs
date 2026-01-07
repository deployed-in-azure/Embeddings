using System.Numerics.Tensors;

namespace DeployedInAzure.EmbeddingsExamples.CustomVectorDb
{
    public class DeployedInAzureVectorDb
    {
        private readonly Dictionary<string, VectorSearchRecord> _vectors = new();

        private const int SUPPORTED_VECTOR_DIMENSION = 1536;

        public void Index(VectorSearchRecord? vectorDocument)
        {
            ArgumentNullException.ThrowIfNull(vectorDocument);

            if (vectorDocument.Vector.Length != SUPPORTED_VECTOR_DIMENSION)
            {
                throw new InvalidOperationException($"Invalid vector dimension. The only supported dimension is {SUPPORTED_VECTOR_DIMENSION}.");
            }

            if (_vectors.ContainsKey(vectorDocument.Id))
            {
                throw new InvalidOperationException($"A document with ID '{vectorDocument.Id}' already exists.");
            }

            _vectors[vectorDocument.Id] = vectorDocument;
        }

        public IReadOnlyCollection<VectorSearchResult> Search(float[] queryVector, int topK)
        {
            if (queryVector.Length != SUPPORTED_VECTOR_DIMENSION)
            {
                throw new InvalidOperationException($"Invalid vector dimension. The only supported dimension is {SUPPORTED_VECTOR_DIMENSION}.");
            }

            if (topK <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(topK), "topK must be greater than zero.");
            }

            return _vectors.Values
                .Select(record => new
                {
                    Document = record,
                    Similarity = TensorPrimitives.CosineSimilarity(queryVector, record.Vector)
                })
                .OrderByDescending(x => x.Similarity)
                .Take(topK)
                .Select(x => new VectorSearchResult
                {
                    Id = x.Document.Id,
                    Similarity = x.Similarity,
                    Data = x.Document.Data
                })
                .ToList();
        }
    }
}
