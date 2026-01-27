# Embeddings and Vector Search in Azure

This repository contains a collection of C# examples ranging from the absolute basics of generating embeddings, through building a custom vector database, using the FAISS library, integrating Cosmos DB as a vector store, and exploring Azure AI Search with its vector-related capabilities.

## Blog Posts

### Introduction to Embeddings: Capture the Meaning of Data

Learn how embeddings capture meaning in high-dimensional space, from simple vectors to real semantic search using Microsoft Foundry and C#.

To run the examples, set the following environment variables:
- `AZURE_OPEN_AI_CLIENT_URI`: Your Microsoft Foundry endpoint URL.
- `AZURE_OPEN_AI_CLIENT_DEPLOYMENT_NAME`: Your deployment name for the embedding model (`text-embedding-3-small` for instance).

__Ensure that your identity has the `Azure AI User` RBAC role assigned to access the Foundry resource__

[Read the blog post to find more details](https://deployedinazure.com/introduction-to-embeddings/)

### Vector Databases in Azure: Powering AI Apps at Scale

A practical intro to Vector Databases in Azure, how they work, key search algorithms, and how to choose the right Azure service for AI apps.

To run the examples, set the following environment variables:
- `AZURE_OPEN_AI_CLIENT_URI`: Your Microsoft Foundry endpoint URL.
- `AZURE_OPEN_AI_CLIENT_DEPLOYMENT_NAME`: Your deployment name for the embedding model (`text-embedding-3-small` for instance).

__Ensure that your identity has the `Azure AI User` RBAC role assigned to access the Foundry resource__

[Read the blog post to find more details](https://deployedinazure.com/vector-databases-in-azure/)

### Vector Search in Azure Cosmos DB for NoSQL: A Practical Guide

Hands-on guide to Vector Search in Azure Cosmos DB for NoSQL with practical examples in C# using embeddings and vector indexes.

To run the examples, set the following environment variables:
- `AZURE_OPEN_AI_CLIENT_URI`: Your Microsoft Foundry endpoint URL.
- `AZURE_OPEN_AI_CLIENT_DEPLOYMENT_NAME`: Your deployment name for the embedding model (`text-embedding-3-small` for instance).
- `AZURE_COSMOS_DB_URI`: Your Azure Cosmos DB account URI.
- `AZURE_COSMOS_DB_DATABASE`: Your Azure Cosmos DB database name.
- `AZURE_COSMOS_DB_CONTAINER`: Your Azure Cosmos DB container name.

Ensure that your identity has:
- the `Azure AI User` RBAC role assigned to access the Foundry resource
- the `Cosmos DB Built-in Data Contributor` RBAC role assigned to access the Azure Cosmos DB resource

[Read the blog post to find more details](https://deployedinazure.com/vector-search-in-azure-cosmos-db-for-nosql/)

### Vector Search in Azure AI Search: A Practical Guide

Vector Search in Azure AI Search explained with filters, role-based filtering and C# examples for building fast, intelligent vector search features.

To run the examples, set the following environment variables:
- `AZURE_OPEN_AI_CLIENT_URI`: Your Microsoft Foundry endpoint URL.
- `AZURE_OPEN_AI_CLIENT_DEPLOYMENT_NAME`: Your deployment name for the embedding model (`text-embedding-3-small` for instance).
- `AZURE_AI_SEARCH_URI`: Your Azure AI Search service endpoint URL.
- `AZURE_AI_SEARCH_INDEX`: Your Azure AI Search index name.

Ensure that your identity has:
- the `Azure AI User` RBAC role assigned to access the Foundry resource
- the `Search Index Data Contributor` RBAC role assigned to access the Azure AI Search resource

[Read the blog post to find more details](https://deployedinazure.com/vector-search-in-azure-ai-search/)

### Vectorizers in Azure AI Search: 5 Key Insights You Must Know

Learn how Vectorizers in Azure AI Search simplify embedding generation, streamline C# code, and improve vector search workflows.

To run the examples, set the following environment variables:
- `AZURE_OPEN_AI_CLIENT_URI`: Your Microsoft Foundry endpoint URL.
- `AZURE_OPEN_AI_CLIENT_DEPLOYMENT_NAME`: Your deployment name for the embedding model (`text-embedding-3-small` for instance).
- `AZURE_AI_SEARCH_URI`: Your Azure AI Search service endpoint URL.
- `AZURE_AI_SEARCH_INDEX`: Your Azure AI Search index name.

Ensure that your identity has:
- the `Azure AI User` RBAC role assigned to access the Foundry resource
- the `Search Index Data Contributor` RBAC role assigned to access the Azure AI Search resource

Ensure that the Azure AI Search identity has:
- the `Azure AI User` RBAC role assigned to access the Foundry resource

[Read the blog post to find more details](https://deployedinazure.com/vectorizers-in-azure-ai-search/)

### Integrated Vectorization in Azure AI Search: How to Automate Embeddings

A complete guide to integrated vectorization in Azure AI Search, showing how to automate embeddings and streamline your vector search pipeline.

To run the examples, set the following environment variables:
- `AZURE_AI_SEARCH_URI`: Your Azure AI Search service endpoint URL.
- `AZURE_AI_SEARCH_INDEX`: Your Azure AI Search index name.

Ensure that your identity has:
- the `Search Index Data Reader` RBAC role assigned to access the Azure AI Search resource

Ensure that the Azure AI Search identity has:
- the `Cosmos DB Account Reader` (control-plane) and `Cosmos DB Built-in Data Reader` (data-plane) RBAC roles assigned to access the Azure Cosmos DB resource
- the `Azure AI User` RBAC role assigned to access the Foundry resource

[Read the blog post to find more details](https://deployedinazure.com/integrated-vectorization-in-azure-ai-search/)



