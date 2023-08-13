// using Elasticsearch.Net;
// using Nest;

using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

namespace Elasticsearch.WEB.Extensions;

public static class ElasticSearch
{
    public static void AddElasticService(this IServiceCollection services, IConfiguration configuration)
    {
        var url = new Uri(configuration.GetSection("Elastic")["Url"]!);
        var userName = configuration.GetSection("Elastic")["Username"];
        var password = configuration.GetSection("Elastic")["Password"];

        var settings = new ElasticsearchClientSettings(url).Authentication(new BasicAuthentication(userName!, password!));
        // var pool = new SingleNodeConnectionPool(url);
        // var settings = new ConnectionSettings(pool);
        var client = new ElasticsearchClient(settings);
        services.AddSingleton(client); 
    }
}