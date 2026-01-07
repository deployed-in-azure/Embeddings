# Embeddings and Vector Search in Azure

This repository contains a collection of C# examples ranging from the absolute basics of generating embeddings, through building a custom vector database, using the FAISS library, integrating Cosmos DB as a vector store, and exploring Azure AI Search with its vector-related capabilities.

## Blog Posts

### (1) Introduction to Embeddings: Capture the Meaning of Data

Learn how embeddings capture meaning in high-dimensional space, from simple vectors to real semantic search using Microsoft Foundry and C#.

To run the examples in this repository, set the following environment variables:
- `AZURE_OPEN_AI_CLIENT_URI`: Your Microsoft Foundry endpoint URL.
- `AZURE_OPEN_AI_CLIENT_DEPLOYMENT_NAME`: Your deployment name for the embedding model (`text-embedding-3-small` for instance).

__Ensure that your identity has the `Azure AI User` RBAC role assigned to access the Foundry resource__

[Read the blog post to find more details](https://deployedinazure.com/introduction-to-embeddings/)


