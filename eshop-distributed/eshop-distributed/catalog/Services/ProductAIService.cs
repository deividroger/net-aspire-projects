using Catalog.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;

namespace catalog.Services;

public class ProductAIService(ProductDbContext dbContext,
    IChatClient chatClient,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    IVectorStoreRecordCollection<int, ProductVector> productVectorCollection)
{
    public async Task<string> SupportAsync(string query)
    {
        var systemPrompt = """
            You are a useful assistant.
            You always reply with a short ans funny message.
            If you don't know an answer, you say 'I don't know that.'
            Your only answer question related to outdoor camping products.
            For any other type of questions,  explain to the user that you answer outdoor camping products related questions only.
            At the end, Offer one of our products: Hiking Backpack, Camping Tent, Sleeping Bag, Camping Stove, or Outdoor Lantern.
            Do not store memory of the chat conversation.
            """;

        var chatHistory = new List<ChatMessage>
        {
            new ChatMessage(ChatRole.System, systemPrompt),
            new ChatMessage(ChatRole.User, query),

        };

        var result = "";


        await foreach (var chunk in chatClient.GetStreamingResponseAsync(chatHistory))
        {
            result += chunk.Text;
        }
        return string.Join("", result);


    }
    public async Task<IEnumerable<Product>> SearchProductsAsync(string query)
    {
        if (!await productVectorCollection.CollectionExistsAsync())
        {
            await InitEmbeddingsAsync();
        }

        var queryEmbedding = await embeddingGenerator.GenerateVectorAsync(query);

        var vectorSearchOptions = new VectorSearchOptions
        {
            Top = 1,
            VectorPropertyName = "Vector"
        };

        var results =
            await productVectorCollection.VectorizedSearchAsync(queryEmbedding, vectorSearchOptions);

        List<Product> products = [];
        await foreach (var resultItem in results.Results)
        {
            products.Add(new Product
            {
                Id = resultItem.Record.Id,
                Name = resultItem.Record.Name,
                Description = resultItem.Record.Description,
                Price = resultItem.Record.Price,
                ImageUrl = resultItem.Record.ImageUrl
            });
        }

        return products;
    }

    private async Task InitEmbeddingsAsync()
    {
        await productVectorCollection.CreateCollectionIfNotExistsAsync();

        var products = await dbContext.Products.ToListAsync();
        foreach (var product in products)
        {
            var productInfo = $"[{product.Name}] is a product that costs [{product.Price}] and is described as [{product.Description}]";

            var productVector = new ProductVector
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Vector = await embeddingGenerator.GenerateVectorAsync(productInfo)
            };

            await productVectorCollection.UpsertAsync(productVector);
        }
    }

}
