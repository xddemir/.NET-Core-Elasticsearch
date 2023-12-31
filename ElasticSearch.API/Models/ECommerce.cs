using System.Text.Json.Serialization;

namespace ElasticSearch.API.Models.ECommerceModel;

public class ECommerce
{
    [JsonPropertyName("_id")]
    public string Id { get; set; } = null;
    
    [JsonPropertyName("customer_first_name")]
    public string CustomerFirstName { get; set; }
    
    [JsonPropertyName("customer_last_name")]
    public string CustomerLastName { get; set; } = null;
    
    [JsonPropertyName("customer_full_name")]
    public string CustomerFullName { get; set; } = null;
    
    [JsonPropertyName("category")]
    public string[] Category { get; set; } = null;

    [JsonPropertyName("taxful_total_price")]
    public double TaxFulTotalPrice { get; set; }
    
    [JsonPropertyName("order_id")]
    public int OrderId { get; set; } 
    
    public DateTime OrderDate { get; set; }
    
    [JsonPropertyName("products")] 
    public Product[] Products { get; set; }

}

public class Product
{
    [JsonPropertyName("product_id")]
    public long productId { get; set; }
    [JsonPropertyName("product_name")]
    public string productName { get; set; }
}