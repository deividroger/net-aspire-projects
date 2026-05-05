using catalog.Models;

namespace WebApp.ApiClients;

public class CatalogApiClient(HttpClient httpClient)
{
    public async Task<List<Product>> GetProducts()
    {
        var response = await httpClient.GetFromJsonAsync<List<Product>>($"/products");
        return response!; 
    }

    public async Task<Product> GetProductById(int id)
    {
        var response = await httpClient.GetFromJsonAsync<Product>($"/products/{id}");
        return response!;
    }

    public async Task<string> SupportWithProducts(string query)
    {
        var response = await httpClient.GetFromJsonAsync<string>($"/products/support/{query}");
        return response!;
    }

    public async Task<List<Product>> SearchProducts(string query,bool aiSearch)
    {
        if (aiSearch)
        {
            var response = await httpClient.GetFromJsonAsync<List<Product>>($"/products/search/ai/{query}");
            return response!;
        }
        else
        {
            var response = await httpClient.GetFromJsonAsync<List<Product>>($"/products/search/{query}");
            return response!;
        }
    }
}