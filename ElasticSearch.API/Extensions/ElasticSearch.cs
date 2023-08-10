using Elasticsearch.Net;
using Nest;

namespace ElasticSearch.API.Extensions;

public static class ElasticSearch
{
    public static void AddElasticService(this IServiceCollection services, IConfiguration configuration)
    {
        var url = new Uri(configuration.GetSection("Elastic")["Url"]!);
        var pool = new SingleNodeConnectionPool(url);
        var settings = new ConnectionSettings(pool);
        var client = new ElasticClient(settings);
        services.AddSingleton(client); 
    }
}