using DeployedInAzure.EmbeddingsExamples.CustomVectorDb;

namespace DeployedInAzure.EmbeddingsExamples.CustomVectorDbFaiss
{
    public class DeployedInAzureVectorDbFaiss
    {
        private  FaissNet.Index _index;
        private Dictionary<string, VectorSearchRecord> _storage = new();

        private const int SUPPORTED_VECTOR_DIMENSION = 1536;
        
        public DeployedInAzureVectorDbFaiss()
        {
            _index = FaissNet.Index.Create(
                SUPPORTED_VECTOR_DIMENSION, 
                "IDMap2,Flat", 
                FaissNet.MetricType.METRIC_INNER_PRODUCT);
        }

        public void Index(IEnumerable<VectorSearchRecord> vectorDocuments)
        {
            _storage = vectorDocuments.ToDictionary(vd => vd.Id, vd => vd);

            _index.AddWithIds([.. vectorDocuments.Select(vd => vd.Vector)], [.. vectorDocuments.Select(vd => long.Parse(vd.Id))]);
        }

        public IReadOnlyCollection<VectorSearchResult> Search(float[] queryVector, int topK)
        {
            var result = _index.SearchFlat(1, queryVector, topK);

            var distance = result.Item1;
            var ids = result.Item2;

            return ids.Zip(distance)
                .Select(kvp => new VectorSearchResult
                {
                    Id = kvp.First.ToString(),
                    Similarity = kvp.Second,
                    Data = _storage[kvp.First.ToString()].Data
                })
                .ToList();
        }
    }
}
